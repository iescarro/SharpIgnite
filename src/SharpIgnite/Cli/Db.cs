using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpIgnite.Cli
{
    public class Db
    {
        public void Seed(string assemblyPath, string connectionString)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            var config = GetConfigFromAssembly(assembly);

            var seeders = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(Seeder).IsAssignableFrom(type));

            Console.WriteLine(seeders.Count() + " seeders found.");
            foreach (var seeder in seeders) {
                var obj = Activator.CreateInstance(seeder) as Seeder;
                Console.WriteLine("Running seed " + obj);
                obj.DatabaseConnectionLoad += delegate (object sender, DatabaseEventArgs e) {
                    var connection = e.Connection;
                    var seederConnectionString = config.AppSettings.Settings[connection].Value;
                    obj.Database.LoadConnectionString(seederConnectionString);
                };
                obj.Initialize();
                obj.Run();
            }
            Console.WriteLine("Done");
            Console.WriteLine();
        }

        static Configuration GetConfigFromAssembly(Assembly assembly)
        {
            string assemblyDirectory = Path.GetDirectoryName(assembly.Location);
            string configPath = Path.Combine(assemblyDirectory, assembly.GetName().Name + ".dll.config");

            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = configPath };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            return config;
        }

        public void Migrate(string assemblyPath, string connectionString)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            var config = GetConfigFromAssembly(assembly);

            var migrations = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(Migration).IsAssignableFrom(type));

            int count = 0;
            foreach (var migration in migrations) {
                var obj = Activator.CreateInstance(migration) as Migration;
                obj.Migrating += delegate (object sender, MigrationEventArgs e) {
                    Console.Write(e.Message + " ");
                };
                obj.Migrated += delegate (object sender, MigrationEventArgs e) {
                    Console.WriteLine(e.Message);
                };
                obj.DatabaseConnectionLoad += delegate (object sender, DatabaseEventArgs e) {
                    var connection = e.Connection;
                    var migrationConnectionString = config.AppSettings.Settings[connection].Value;
                    obj.Database.LoadConnectionString(migrationConnectionString);
                };
                obj.Initialize();
                if (!obj.SchemaExists) {
                    obj.CreateTable(
                        "SchemaMigration",
                        obj.Column("SchemaMigrationID", DbType.Int32, true, true),
                        obj.Column("Version", DbType.String)
                    );
                }
                if (!obj.VersionExists) {
                    obj.Up();
                    obj.Database.Insert("SchemaMigration", new { Version = obj.Version });
                    count++;
                }
            }
            Console.WriteLine(count + " migrations found.");
            Console.WriteLine("Done");
            Console.WriteLine();
        }

        public class Table
        {
            public string Name { get; set; }
            public List<Column> Columns { get; set; }
        }

        public class Column
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string IsPrimaryKey { get; set; }
            public string IsIdentity { get; set; }

            public string GetDbType()
            {
                var t = Type.ToUpper();
                if (t == "INT") {
                    return "DbType.Int32";
                }
                return "DbType.String";
            }
        }
    }

    public class BaseCli
    {
        public string Endl()
        {
            return Environment.NewLine;
        }
    }
}
