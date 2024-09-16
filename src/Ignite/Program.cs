using SharpIgnite.Cli;
using System;

namespace Ignite
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0) {
                var command = args[0];

                if (command == "db:seed") {
                    var assembly = args[1];
                    var connectionString = args[2];
                    new Db().Seed(assembly, connectionString);
                } else if (command == "migrate") {
                    var assembly = args[1];
                    var connectionString = args[2];
                    var providerName = args[3];
                    new Migrate().Run(assembly, connectionString, providerName);
                } else if (command == "migrate:generate") {
                    var connectionString = args[1];
                    new Migrate().Generate(connectionString);
                } else if (command == "migrate:status") {
                    var assembly = args[1];
                    var connectionString = args[2];
                    var providerName = args[3];
                    new Migrate().Status(assembly, connectionString, providerName);
                } else if (command == "make:seeder") {
                    new Make().Seeder(args[1]);
                } else if (command == "make:migration") {
                    new Make().Migration(args[1]);
                } else {
                    ShowHelp();
                }
            } else {
                ShowHelp();
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine(@"
SharpIgnite Command Line Tool

Database
  db:seed <assembly> <connectionString>     Runs seeder to populate data into the database.
  migrate <assembly> <connectionString>     Locates and runs all new migrations against the database.
  
Generators
  migrate:generate <connectionString>       Generates a migration file from database.
  make:migration <name>                     Generates a new migration file.
  make:seeder <name>                        Generates a new seeder file.
");
        }
    }

}