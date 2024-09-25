using System.IO.Compression;
using Hangfire;
using HangfireHost.Models.Execute.PostModel;
using HangfireHost.Utils;
using Microsoft.AspNetCore.Mvc;

namespace HangfireHost.Controllers
{
    /// <summary>
    /// Manage recurring jobs to execute the executable file
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExecuteController : ControllerBase
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly string _executeDir = Path.Combine(Directory.GetCurrentDirectory(), "ExecuteLibrary");

        /// <summary>
        /// recurringJobManager can be used to create, update, delete recurring jobs.
        /// </summary>
        /// <param name="recurringJobManager"></param>
        public ExecuteController(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
            if (!Directory.Exists(_executeDir))
                Directory.CreateDirectory(_executeDir);
        }

        /// <summary>
        /// Create a recurring job to execute the mainFileName every intervalMinutes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> CreateNew([FromForm] CreateModel model)
        {
            // 1. the model contains [file(zip type), mainFileName, intervalMinutes]
            // 2. read the zip file to stream and unzip it in to exportDir
            // 3. create a recurring job to execute the mainFileName every intervalMinutes
            // 4. return Ok 
            if (!model.MainFileName.ToLower().EndsWith(".exe"))
                return BadRequest("Main file must be an executable file (.exe).");

            string id = model.MainFileName.Replace(".exe", "") + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string exportDir = Path.Combine(_executeDir, id);
            string mainFile = Path.Combine(exportDir, model.MainFileName);
            Directory.CreateDirectory(exportDir);

            await using (var stream = model.File.OpenReadStream())
            {
                using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
                archive.ExtractToDirectory(exportDir);
            }

            if (!System.IO.File.Exists(mainFile))
                return BadRequest("Main file not found.");

            string cronExpression = $"*/{model.IntervalMinutes} * * * *";

            _recurringJobManager.AddOrUpdate(id, () => ExecuteUtils.ExecuteProcessAsync(mainFile, model.Args),
                cronExpression);
            Console.WriteLine($"Created job {id} with cron {cronExpression}");

            return Ok($"Created job {id} with cron {cronExpression}");
        }

        [HttpPost("CreateJobByExisted")]
        public async Task<IActionResult> CreateExisted([FromBody] ExistedModel model)
        {
            string mainFile = Path.Combine(model.MainFilePath, model.MainFileName);
            if (!System.IO.File.Exists(mainFile))
                return BadRequest("Main file not found.");

            string cronExpression = $"*/{model.IntervalMinutes} * * * *";

            string id = model.MainFileName.Replace(".exe", "") + "_Re_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            _recurringJobManager.AddOrUpdate(id, () => ExecuteUtils.ExecuteProcessAsync(mainFile, model.Args),
                cronExpression);
            Console.WriteLine($"Created job {id} with cron {cronExpression}");

            return Ok($"Created job {id} with cron {cronExpression}");
        }

        /// <summary>
        /// Retrieves the list of directories in the execute library.
        /// </summary>
        /// <returns>An IActionResult containing the list of directory names.</returns>
        [HttpGet("GetExecuteLibrary")]
        public IActionResult GetExecuteLibrary()
        {
            try
            {
                var data = Directory.GetDirectories(_executeDir).Select(x => new DirectoryInfo(x).Name).ToList();
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest("Error");
            }
        }
    }
}