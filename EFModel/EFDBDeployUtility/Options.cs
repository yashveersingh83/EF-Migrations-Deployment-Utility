using CommandLine;
using CommandLine.Text;

namespace EFDBDeployUtility
{
    public class Options
    {
        [Option('c', "dbConfigurationClassName", Required = false,
        HelpText = "Name of Database configuration file inherited from DbMigrationsConfiguration.")]
        public string ConfigurationClassName { get; set; }
        [Option('l', "SourceDllPath", Required = true,
        HelpText = "Name of DLL in which entity framework migrations are present.")]
        public string DllPath { get; set; }
        [Option('s', "sourceMigration", Required = false,
        HelpText = "Name of sourceMigration.")]
        public string SourceMigration { get; set; }
        [Option('t', "targetMigration", Required = false,
        HelpText = "Name of sourceMigration.")]
        public string TargetMigration { get; set; }
        [Option('d', "databaseConnectionString", Required = false,
        HelpText = "Name of sourceMigration.")]
        public string TargetDataBaseConnectioString { get; set; }

        [Option('v', "verbose", DefaultValue = true,
        HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
        [Option('u', "update", Required = false,
        HelpText = "Update target database.")]
        public string Update { get; set; }
        [ParserState]
        public IParserState LastParserState { get; set; }
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
            (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    public class Response
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
