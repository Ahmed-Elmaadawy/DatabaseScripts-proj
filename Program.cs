using DbUp;
using System;
using System.Linq;  // Make sure to include this for LINQ methods like FirstOrDefault
using System.IO;

class Program
{
    static int Main(string[] args)
    {
        // Get connection string, fallback to default if not provided
        var connectionString = args.FirstOrDefault()
            ?? "Server=Dev_Backend_PC1; Database=TestScripts; Trusted_connection=true ; TrustServerCertificate=true ";
      
        //if database does not exist, create it with the given database name
        EnsureDatabase.For.SqlDatabase(connectionString);
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Connection string is missing.");
            Console.ResetColor();
            return -1;
        }

        // Setup DbUp to run SQL scripts from 'Scripts' folder
        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsFromFileSystem("Scripts")
            .WithTransactionPerScript()  // Use transaction for each script
            .LogToConsole()
            .Build();

        // Perform the upgrade (run the scripts)
        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            // Print error in red
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            return -1;
        }

        // Success message in green
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ResetColor();
        return 0;
    }
}
