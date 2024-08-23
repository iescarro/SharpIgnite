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
                .Where(type => type.IsClass && !type.IsAbstract && typeof(DatabaseSeeder).IsAssignableFrom(type));

            Console.WriteLine(seeders.Count() + " seeders found.");
            foreach (var seeder in seeders) {
                var obj = Activator.CreateInstance(seeder) as DatabaseSeeder;
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
                    obj.Migrate();
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

    public class Make
    {
        public void Seeder(string name)
        {
            string filePath = name + ".cs";
            string content = @"using System;
using SharpIgnite;

namespace Seeders
{
    public class ${ClassName}: DatabaseSeeder
    {
        public ${ClassName}() : base()
        {
        }
        
        public override void Run()
        {
        }
    }
}";
            content = content.Replace("${ClassName}", name);
            File.WriteAllText(filePath, content);
        }

        public void MigrationFromDB(string connectionString)
        {
            var db = WebApplication.Instance.Database;
            db.LoadConnectionString(connectionString);

            var tables = db.Query<Db.Table>(@"
SELECT TABLE_NAME Name
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE='BASE TABLE'
    AND TABLE_NAME != 'dtproperties'
    AND TABLE_NAME != 'sysdiagrams'");
            foreach (var t in tables) {
                string tableName = t.Name;
                t.Columns = db.Query<Db.Column>(@"
SELECT C.COLUMN_NAME Name, C.DATA_TYPE Type,
    CASE WHEN KCU.COLUMN_NAME IS NOT NULL THEN 'YES' ELSE 'NO' END AS IsPrimaryKey,
    CASE COLUMNPROPERTY (OBJECT_ID('" + tableName + @"'),C.COLUMN_NAME ,'IsIdentity') WHEN 1 THEN 'YES' ELSE 'NO' END IsIdentity,
    C.IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS C
LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU ON C.TABLE_NAME = KCU.TABLE_NAME
    AND C.COLUMN_NAME = KCU.COLUMN_NAME
WHERE C.TABLE_NAME = '" + tableName + "'");
            }

            foreach (var t in tables) {
                Console.WriteLine("Creating DB migration for " + t.Name);

                string className = GetClassName("Create" + t.Name);
                string tableName = t.Name;
                string filePath = className + ".cs";
                string content = @"using System;
using System.Data;
using SharpIgnite;

namespace Migrations
{
    public class ${ClassName}: Migration
    {
        public ${ClassName}() : base(""${Version}"")
        {
        }
        
        public override void Migrate()
        {
            CreateTable(
                ""${TableName}"",
${Columns}
            );
        }
        
        public override void Rollback()
        {
        }
    }
}";
                string columns = "";
                int i = 0;
                foreach (var c in t.Columns) {
                    if (i++ > 0) columns += ", " + Endl();
                    var type = ", " + c.GetDbType();
                    var primaryKey = c.IsPrimaryKey.ToUpper().Equals("YES") ? ", true" : "";
                    var autoIncrement = c.IsIdentity.ToUpper().Equals("YES") ? ", true" : "";
                    columns += "                Column(\"" + c.Name + "\"" + type + primaryKey + autoIncrement + ")";
                }
                content = content.Replace("${ClassName}", className);
                content = content.Replace("${TableName}", tableName);
                content = content.Replace("${Version}", Guid.NewGuid().ToString());
                content = content.Replace("${Columns}", columns);
                File.WriteAllText(filePath, content);
            }
            Console.WriteLine("Done" + Endl());
        }

        string Endl()
        {
            return Environment.NewLine;
        }

        string GetClassName(string name)
        {
            return "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + name;
        }

        public void Migration(string name)
        {
            string className = GetClassName(name);
            string filePath = className + ".cs";
            string migrate = "";
            string content = @"using System;
using SharpIgnite;

namespace Migrations
{
    public class ${ClassName}: Migration
    {
        public ${ClassName}() : base(""${Version}"")
        {
        }
        
        public override void Migrate()
        {
${Migrate}
        }
        
        public override void Rollback()
        {
        }
    }
}";
            content = content.Replace("${ClassName}", className);
            content = content.Replace("${Version}", Guid.NewGuid().ToString());
            content = content.Replace("${Migrate}", migrate);
            File.WriteAllText(filePath, content);
        }
    }
}
