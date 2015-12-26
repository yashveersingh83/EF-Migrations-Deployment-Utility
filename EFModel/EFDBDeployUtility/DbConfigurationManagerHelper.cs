using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFDBDeployUtility
{
    public static class DbConfigurationManagerHelper
    {
        // ConsoleClientApp.exe  -c "Configuration" -l "D:\yashlearning\EFMigration\EFMigration\Dal\bin\Debug\Dal.dll" -s "null" -T "null" -u "true" -d "Server=localhost;Database=S1;Integrated Security=True;"
        // EFDBDeployUtility.exe  -c "Configuration" -l "D:\yashlearning\EFDatabaseDeploy\EFModel\EFDBDeployUtility\bin\Debug\EFModel.dll" -s "null" -T "null" -u "true" -d "Server=localhost;Database=Student;Integrated Security=True;"
        public static DbMigrator CreateDbMigration(string connectionString, string dllPath, string configurationClassName)
        {
            var r = Assembly.LoadFrom(dllPath);
            Console.WriteLine(r.FullName);
            var className = string.IsNullOrEmpty(configurationClassName) ? "Configuration" : configurationClassName;

            var c = r.GetTypes().FirstOrDefault(p => p.Name == className);
            Console.WriteLine(c);
            var configuration = Activator.CreateInstance(c) as DbMigrationsConfiguration;
            if (configuration != null)
            {
                //configuration.AutomaticMigrationsEnabled = true;
                configuration.AutomaticMigrationDataLossAllowed = true;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    configuration.TargetDatabase = new System.Data.Entity.Infrastructure.DbConnectionInfo(connectionString, "System.Data.SqlClient");

                }
                var migrator = new DbMigrator(configuration);
                return migrator;
            }
            return null;
        }
        public static MigratorScriptingDecorator CreateDbMigrationScriptingDecorator(string connectionString, string dllPath, string configurationClassName)
        {
            var migrator = CreateDbMigration(connectionString, dllPath, configurationClassName);
            if (migrator != null)
            {
                var scriptor = new MigratorScriptingDecorator(migrator);
                return scriptor;
            }
            return null;
        }

    }
}
