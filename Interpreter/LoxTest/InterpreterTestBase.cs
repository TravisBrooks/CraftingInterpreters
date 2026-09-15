using Microsoft.Extensions.DependencyInjection;
using Lox;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LoxTest
{
    public abstract class InterpreterTestBase
    {
        protected TestConsole Interpret(string source)
        {
            var services = new ServiceCollection();
            services.AddLoxApplicationServices();

            // Replace the default IConsole implementation with TestConsole for testing purposes.
            services.RemoveAll<IConsole>();
            services.AddSingleton<IConsole, TestConsole>();

            var provider = services.BuildServiceProvider();
            var loxService = provider.GetRequiredService<LoxService>();

            loxService.Run(source);

            // Because we're not running the application in a console, we need to manually log any errors for them to show up in the TestConsole.OutputMessages.
            var errLogger = provider.GetRequiredService<ErrorLogger>();
            errLogger.LogAllErrors();
            errLogger.LogAllRuntimeErrors();

            var console = (TestConsole)provider.GetRequiredService<IConsole>();
            return console;
        }
    }
}
