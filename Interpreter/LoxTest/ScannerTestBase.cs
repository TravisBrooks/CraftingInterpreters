using System.Collections.Immutable;
using Lox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LoxTest
{
    public class ScannerTestBase
    {
        protected (ImmutableList<Token> Tokens, TestConsole Console) Scan(string loxCode)
        {
            var services = new ServiceCollection();
            services.AddLoxApplicationServices();

            // Replace the default IConsole implementation with TestConsole for testing purposes.
            services.RemoveAll<IConsole>();
            services.AddSingleton<IConsole, TestConsole>();

            var provider = services.BuildServiceProvider();
            var errLogger = provider.GetRequiredService<ErrorLogger>();
            var scanner = new Scanner(errLogger, loxCode);
            var tokens = scanner.ScanTokens();

            // Because we're not running the application in a console, we need to manually log any errors for them to show up in the TestConsole.OutputMessages.
            errLogger.LogAllErrors();
            errLogger.LogAllRuntimeErrors();

            var console = (TestConsole)provider.GetRequiredService<IConsole>();
            return (Tokens: tokens, Console: console);
        }

        /// <summary>
        /// Ignores the Line property
        /// </summary>
        public class TokenTestComparer : IEqualityComparer<Token>
        {
            public bool Equals(Token? x, Token? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.TokenType == y.TokenType && x.Lexeme == y.Lexeme && Equals(x.Literal, y.Literal);
            }

            public int GetHashCode(Token obj)
            {
                return HashCode.Combine((int)obj.TokenType, obj.Lexeme, obj.Literal);
            }
        }
    }
}
