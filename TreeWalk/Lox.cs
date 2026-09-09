using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Lox
{
    public static class Lox
    {
        public static int Main(string[] args)
        {
            //args = ["C:\\temp\\Lox\\Test.lox"];
            if (args.Length > 1)
            {
                Console.Error.WriteLine("Usage: Lox [script], or just Lox without a param to go into Interactive Mode.");
                return 64;
            }

            var loxFileName = args.Length == 1 ? args[0] : null;
            var services = new ServiceCollection();
            services.AddLoxApplicationServices();

            using (var provider = services.BuildServiceProvider())
            {
                var loxService = provider.GetRequiredService<LoxService>();
                var timer = new Stopwatch();
                timer.Start();
                var output = loxService.Start(loxFileName);
                timer.Stop();
                if (loxFileName is not null)
                {
                    Console.WriteLine($"Execution time: {timer.Elapsed}");
                }
                return output;
            }
        }
    }
}