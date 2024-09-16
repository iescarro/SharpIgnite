using System;
using System.IO;

namespace SharpIgnite.Cli
{
    public class Make : BaseCli
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
            string filePath = Path.Combine("Migrations", className + ".cs");
            string migrate = "";
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
${Migrate}
        }
        
        public override void Down()
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
