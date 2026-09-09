using Lox.InterpreterVisitors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lox
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoxApplicationServices(this IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole());
            services.AddSingleton<ErrorLogger>();
            services.AddSingleton<Interpreter>();
            services.AddSingleton<EnvironmentContext>();
            services.AddSingleton<LoxService>();
            services.AddSingleton<ExpressionVisitor>();
            services.AddSingleton<StatementVisitor>();
            services.AddSingleton<Func<StatementVisitor>>(sp => sp.GetRequiredService<StatementVisitor>);

            return services;
        }

    }
}
