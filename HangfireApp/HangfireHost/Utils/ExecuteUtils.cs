using System.Diagnostics;

namespace HangfireHost.Utils
{
    /// <summary>
    /// Utility class for executing processes.
    /// </summary>
    public static class ExecuteUtils
    {
        /// <summary>
        /// Executes a process asynchronously.
        /// </summary>
        /// <param name="exePath">The path to the executable file.</param>
        /// <param name="args">The arguments to pass to the executable file. Can be null or Empty.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task ExecuteProcessAsync(string exePath, string? args)
        {
            try
            {
                using Process explorer = new Process();
                explorer.StartInfo.FileName = exePath;
                if (!string.IsNullOrEmpty(args))
                    explorer.StartInfo.Arguments = args;

                Console.WriteLine("Executing {0}, args: {1}", exePath, args);

                explorer.Start();
                await Task.Run(() => explorer.WaitForExit());
                int exitCode = explorer.ExitCode;

                Console.WriteLine("Call {0} completed, exitCode: {1}", exePath, exitCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine(ex);
            }
        }
    }
}