namespace HangfireApp.HangfireService
{
    public class HangfireTestJobService : IHangfireTestJobService
    {
        public void AddContinuationJob()
        {
            Console.WriteLine($"{DateTime.Now.ToString()} - This is a continuation job!");
        }

        public void AddDelayedJob()
        {
            Console.WriteLine($"{DateTime.Now.ToString()} - This is a Delayed job!");
        }

        public void AddFireAndForgetJob()
        {
            Console.WriteLine($"{DateTime.Now.ToString()} - This is a Fire and Forget job!");
        }

        public void AddReccuringJob()
        {
            Console.WriteLine($"{DateTime.Now.ToString()} - This is a Recurring job!");
        }
    }
}