using Lox.InterpreterVisitors;
using Microsoft.Extensions.DependencyInjection;

namespace Lox
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoxApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<ErrorLogger>();
            services.AddSingleton<Interpreter>();
            services.AddSingleton<EnvironmentContext>();
            services.AddSingleton<LoxService>();
            services.AddSingleton<DeclarationVisitor>();
            services.AddSingleton<StatementVisitor>();
            services.AddSingleton<ExpressionVisitor>();
            services.AddSingleton<Func<DeclarationVisitor>>(sp => sp.GetRequiredService<DeclarationVisitor>);
            services.AddSingleton<Func<StatementVisitor>>(sp => sp.GetRequiredService<StatementVisitor>);
            services.AddSingleton<IConsole, ConsoleWrapper>();

            return services;
        }

    }
}
