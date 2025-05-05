using DbUp;
using DbUp.ScriptProviders;
using DbUp.Engine;
using DbUp.Engine.Output;
using System;
using System.Linq;
using System.IO;

class Program
{
    static int Main(string[] args)
    {
        var connectionString = args.FirstOrDefault()
            ?? "Server=Dev_Backend_PC1; Database=TestScripts; Trusted_connection=true; TrustServerCertificate=true";

        EnsureDatabase.For.SqlDatabase(connectionString);

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Connection string is missing.");
            Console.ResetColor();
            return -1;
        }

        var scriptsPath = "Scripts";

        var scripts = new FileSystemScriptProvider(scriptsPath).GetScripts(null).ToList();
        bool hasErrors = false;

        foreach (var script in scripts)
        {
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScripts(new[] { script })
                .WithTransactionPerScript()
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                hasErrors = true;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error in script {script.Name}: {result.Error.Message}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Script {script.Name} ran successfully.");
                Console.ResetColor();
            }
        }

        if (hasErrors)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Completed with errors. Check the logs above.");
            Console.ResetColor();
            return -1;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("All scripts executed successfully.");
        Console.ResetColor();
        return 0;
    }
}
