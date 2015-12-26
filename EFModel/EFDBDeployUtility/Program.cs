using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;

namespace EFDBDeployUtility
{
    class Program
    {
        //public const string DirPath = @"..\\sql";
        public const string FilePath = @"Database.log";
        //public const string FullPath = DirPath + "\\" + FilePath;
        static int Main(string[] args)
        {
            var stringBulider = new StringBuilder();
            try
            {
                var lastDbMigration = string.Empty;

                stringBulider.AppendLine(
                "-----------------------------------------------------------------------------------------------------------------------------");
                stringBulider.AppendLine(
                "========================================================== DataBase Deployment ===============================================");
                var options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    // Values are available here
                    if (options.Verbose)
                    {
                        if (ParseCommandLineParameters(options, stringBulider).IsSuccess)
                        {
                            var migrator = DbConfigurationManagerHelper.CreateDbMigration(options.TargetDataBaseConnectioString, options.DllPath, options.ConfigurationClassName);
                            if (migrator != null)
                            {
                                lastDbMigration = migrator.GetDatabaseMigrations().Take(1).FirstOrDefault();
                                stringBulider.AppendLine("Last Migration Applied To Database:" + lastDbMigration);

                                stringBulider.AppendLine("Migration which are pending on current database");

                                foreach (string migration in migrator.GetPendingMigrations())
                                {
                                    stringBulider.AppendLine(migration);
                                    Console.WriteLine(migration);
                                }
                                GenrateDatabaseScript(options, stringBulider);
                                UpdateDatabase(lastDbMigration, options, migrator, stringBulider);
                            }
                        }
                    }
                }
            }
            catch (Exception t)
            {

                stringBulider.AppendLine("Error in Update DataBase." + t.InnerException + " Stackrace:" + t.StackTrace);
                System.IO.File.WriteAllText(FilePath, stringBulider.ToString());
                return -1;
            }


            System.IO.File.WriteAllText(FilePath, stringBulider.ToString());

            return 0;
        }

        private static Response ParseCommandLineParameters(Options options, StringBuilder sbBuilder)
        {
            var respose = new Response();
            respose.IsSuccess = true;
            sbBuilder.AppendLine(
              "-----------------------------------------------------------------------------------------------------------------------------");
            sbBuilder.AppendLine(
            "========================================================== Parameters Supplied ===============================================");
            sbBuilder.AppendLine(string.Format("Filename: {0}", options.ConfigurationClassName));
            sbBuilder.AppendLine(string.Format("SourceMigration: {0}", options.SourceMigration));
            sbBuilder.AppendLine(string.Format("TargetMigration: {0}", options.TargetMigration));
            sbBuilder.AppendLine(string.Format("UpdateDatabase: {0}", options.Update));
            sbBuilder.AppendLine(string.Format("ConnectionString: {0}", options.TargetDataBaseConnectioString));
            sbBuilder.AppendLine(string.Format("DLLPath: {0}", options.DllPath));

            //if( string.IsNullOrEmpty( options.TargetMigration))
            //{
            // respose.IsSuccess = false;
            // respose.Message = "Target Migration Should Be Present:";
            //}
            if (string.IsNullOrEmpty(options.DllPath))
            {
                respose.IsSuccess = false;
                respose.Message = "Please provide dll path :";
            }


            sbBuilder.AppendLine(
            "================================================================================================================================");
            return respose;
        }
        private static int UpdateDatabase(string lastDbMigration, Options options, DbMigrator migrator, StringBuilder sbBuilder)
        {
            if (options.Update.Equals("True", StringComparison.InvariantCultureIgnoreCase))
            {


                sbBuilder.AppendLine(" DataBase migration started.");
                try
                {


                    if (options.TargetMigration == null || options.TargetMigration.Equals("null", StringComparison.InvariantCultureIgnoreCase) ||
                    string.IsNullOrEmpty(options.TargetMigration))
                    {
                        Console.WriteLine(" DataBase migration started.");
                        migrator.Update();
                        sbBuilder.AppendLine("DataBase Successfully Migrated");
                    }
                    else
                    {
                        migrator.Update(options.TargetMigration);

                        sbBuilder.AppendLine("DataBase Successfully Migrated to:" + options.TargetMigration);
                    }

                }
                catch (SqlException t)
                {


                    sbBuilder.AppendLine("Error in migration DataBase.");
                    foreach (var item in t.Errors)
                    {
                        sbBuilder.AppendLine(item.ToString());
                    }


                    sbBuilder.AppendLine("Rolling back to last migration:" + lastDbMigration);
                    migrator.Update(lastDbMigration);

                    sbBuilder.AppendLine("Rolling Back Finish:");
                    System.IO.File.WriteAllText(FilePath, sbBuilder.ToString());
                    return -1;
                }
            }
            else
            {

                sbBuilder.AppendLine("No Db Update done.");
            }
            return 0;
        }
        private static int GenrateDatabaseScript(Options options, StringBuilder sbBuilder)
        {
            try
            {

                var scriptor =
                DbConfigurationManagerHelper.CreateDbMigrationScriptingDecorator(
                options.TargetDataBaseConnectioString, options.DllPath, options.ConfigurationClassName);
                var sourceMigration = string.Empty;
                var targetMigration = string.Empty;

                if (options.TargetMigration != null)
                {
                    if (options.TargetMigration.Equals("null", StringComparison.InvariantCultureIgnoreCase) ||
                    string.IsNullOrEmpty(options.TargetMigration))
                    {
                        targetMigration = null;
                    }
                    else
                    {
                        targetMigration = options.TargetMigration;
                    }
                }
                if (options.SourceMigration != null)
                {
                    if (options.SourceMigration.Equals("null", StringComparison.InvariantCultureIgnoreCase) ||
                    string.IsNullOrEmpty(options.SourceMigration))
                    {
                        sourceMigration = null;
                    }
                    else
                    {
                        sourceMigration = options.SourceMigration;
                    }
                }
                string script = scriptor.ScriptUpdate(sourceMigration: sourceMigration, targetMigration: targetMigration);
                if (script.Length > 0)
                {
                    sbBuilder.AppendLine("Creating script from migration saving the file at." + FilePath);
                    Console.WriteLine("Creating script from migration saving the file at." + FilePath);
                    Console.WriteLine(script);
                }
                sbBuilder.AppendLine(
                "--------------------------Script Generated ---------------------------------------------");
                sbBuilder.AppendLine(script);
                sbBuilder.AppendLine(
                "--------------------------Script End ---------------------------------------------");

            }
            catch (Exception t)
            {
                System.IO.File.WriteAllText(FilePath, sbBuilder.ToString());
                return -1;
            }
            return 0;
        }
    }
}
