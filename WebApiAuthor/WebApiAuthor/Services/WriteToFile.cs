namespace WebApiAuthor.Services
{
    public class WriteToFile : IHostedService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _fileName = "File 1.txt";
        private Timer _timer;

        public WriteToFile(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Write("Process initiated");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            Write("Process completed");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Write("Execution Process: " + DateTime.Now.ToString("dd/MM/yyy hh:mm:ss"));
        }

        public void Write(string msg)
        {
            var route = $@"{_env.ContentRootPath}\wwwroot\{_fileName}";
            using (StreamWriter writer = new StreamWriter(route, append: true))
            {
                writer.WriteLine(msg);
            }
        }
    }
}

