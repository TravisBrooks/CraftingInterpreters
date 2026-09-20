using System.Reflection;

namespace LoxTest
{
    internal static class EmbeddedLoxFile
    {
        public static string GetSource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("LoxTest.LoxFiles." + fileName)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}