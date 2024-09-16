using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpIgnite.Cli
{
    public class Migrate : BaseCli
    {
        public void Status(string assemblyPath, string connectionString, string providerName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            //var config = GetConfigFromAssembly(assembly);

            var migrations = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(Migration).IsAssignableFrom(type));

            foreach (var migration in migrations) {
                var obj = Activator.CreateInstance(migration) as Migration;
                var db = obj.Database
                    .Adapter(providerName)
                    .LoadConnectionString(connectionString);
                if (!obj.SchemaExists) {
                    Console.WriteLine("Database not created.");
                } else {
                    Console.Write(obj);
                    var dbMigration = db.From("SchemaMigration")
                        .Where(Array.New("Version", obj.Version))
                        .Row();
                    if (dbMigration != null) {
                        Console.WriteLine(" [Up]");
                    } else {
                        Console.WriteLine(" [Down]");
                    }
                }
            }
            Console.WriteLine("Done");
            Console.WriteLine();
        }

        public void Run(string assemblyPath, string connectionString, string providerName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            //var config = GetConfigFromAssembly(assembly);

            //var connection = config.ConnectionStrings.ConnectionStrings["DbConnection"];
            //var connectionString = connection.ConnectionString;
            //var providerName = connection.ProviderName;

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
                //obj.DatabaseConnectionLoad += delegate (object sender, DatabaseEventArgs e) {

                obj.Database.Adapter(providerName)
                    .LoadConnectionString(connectionString);
                //};
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

        public void Generate(string connectionString)
        {
            var db = Application.Instance.Database;
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
                string filePath = Path.Combine("Migrations", className + ".cs");
                string content = @"using SharpIgnite;
using System.Data;

namespace Migrations
{
    public class ${ClassName}: Migration
    {
        public ${ClassName}() : base(""${Version}"")
        {
        }
        
        public override void Up()
        {
            CreateTable(
                ""${TableName}"",
${Columns}
            );
        }
        
        public override void Down()
        {
            DropTable(""${TableName}"");
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

        string GetClassName(string name)
        {
            DateTime now = DateTime.Now;
            string formattedDate = now.ToString("yyyyMMddHHmmss");
            return $"_{formattedDate}";
        }
    }
}
