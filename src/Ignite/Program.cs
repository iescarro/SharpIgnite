using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using SharpIgnite;
using SharpIgnite.Cli;

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
                    new Db().Migrate(args[0], args[1]);
                    
                } else if (command == "make:seeder") {
                    new Make().Seeder(args[1]);
                } else if (command == "make:dbmigration") {
                    var connectionString = args[1];
                    new Make().MigrationFromDB(connectionString);
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
  make:dbmigration <connectionString>       Generates a migration file from database.
  make:migration <name>                     Generates a new migration file.
  make:seeder <name>                        Generates a new seeder file.
");
        }
    }
    
}