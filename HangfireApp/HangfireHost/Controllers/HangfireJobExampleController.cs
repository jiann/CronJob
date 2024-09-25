using Hangfire;
using HangfireApp.HangfireService;
using Microsoft.AspNetCore.Mvc;

namespace HangfireApp.Controller
{
    /// <summary>
    /// 創建任務範例
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HangfireJobExampleController : ControllerBase
    {
        private readonly IHangfireTestJobService _hangfireTestService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public HangfireJobExampleController(IHangfireTestJobService hangfireTestService,
            IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _hangfireTestService = hangfireTestService;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        /// <summary>
        /// 創建[一次性]任務
        /// </summary>
        /// <returns></returns>
        [HttpGet("/AddFireAndForgetJob")]
        public ActionResult AddFireAndForgetJob()
        {
            _backgroundJobClient.Enqueue(() => _hangfireTestService.AddFireAndForgetJob());

            return Ok();
        }

        /// <summary>
        /// 創建[一次性][延期]任務
        /// </summary>
        /// <returns></returns>
        [HttpGet("/AddDelayedJob")]
        public ActionResult AddDelayedJob()
        {
            _backgroundJobClient.Schedule(() => _hangfireTestService.AddDelayedJob(), TimeSpan.FromMinutes(2));

            return Ok();
        }

        /// <summary>
        /// 創建[重複]任務
        /// * 最小時間為1分鐘
        /// </summary>
        /// <returns></returns>
        [HttpGet("/AddRecurringJob")]
        public ActionResult AddRecurringJob()
        {
            string id = Guid.NewGuid().ToString();
            
            _recurringJobManager.AddOrUpdate(id, () => _hangfireTestService.AddReccuringJob(), Cron.Minutely);
            return Ok();
        }
        
        /// <summary>
        /// 創建[連續]任務
        /// </summary>
        /// <returns></returns>
        [HttpGet("/AddContinuationJob")]
        public ActionResult AddContinuationJob()
        {
            var parentJobId = _backgroundJobClient.Enqueue(() => _hangfireTestService.AddFireAndForgetJob());
            _backgroundJobClient.ContinueJobWith(parentJobId, () => _hangfireTestService.AddContinuationJob());

            return Ok();
        }
    }
}