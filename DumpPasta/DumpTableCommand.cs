using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ManyConsole;
using Rhino.Etl.Core;
using Rhino.Etl.Core.ConventionOperations;
using Rhino.Etl.Core.Operations;
using Rhino.Etl.Core.Pipelines;

namespace DumpPasta
{
    public class DumpTableCommand : ConsoleCommand
    {
        public DumpTableCommand()
        {
            this.IsCommand("dump-table", "Dumps the contents of a SQL table to json.");
            this.HasRequiredOption("c=", "Connection string", v => ConnectionString = v);
            this.HasRequiredOption("d=", "Database name", v => DatabaseName = v);
            this.HasRequiredOption("t=", "Table name", v => TableName = v);
            this.HasRequiredOption("id=", "ID column to order by", v => IdColumn = v);
            this.HasOption("f=", "File to write zip stream to.", v => OutputFile = v);
            this.SkipsCommandSummaryBeforeRunning();
        }

        public string ConnectionString;
        public string DatabaseName;
        public string TableName;
        public string IdColumn;
        public string OutputFile;

        public override int Run(string[] remainingArguments)
        {
            var config = GetConnectionString();

            EtlProcess etlProcess = new EmptyProcess();
            etlProcess.PipelineExecuter = new SingleThreadedPipelineExecuter();

            etlProcess.Register(new ConventionInputCommandOperation(config)
                {
                    Command = "SELECT * FROM " + TableName + " ORDER BY " + IdColumn
                });
            
            using(var outputWriter = GetSelectedOutputWriter())
            {
                etlProcess.RegisterLast(new WriteAsJsonArray(outputWriter));

                etlProcess.Execute();
            }

            foreach (var error in etlProcess.GetAllErrors())
                Console.WriteLine(error.ToString());

            return 0;
        }

        ConnectionStringSettings GetConnectionString()
        {
            var config = new ConnectionStringSettings()
                {
                    Name = "First",
                    ConnectionString = ConnectionString,
                    ProviderName =
                        "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
                };
            return config;
        }

        TextWriter GetSelectedOutputWriter()
        {
            TextWriter tw = Console.Out;
            if (!string.IsNullOrEmpty(OutputFile))
            {
                Console.WriteLine("Writing output to " + OutputFile);
                tw = new StreamWriter(new GZipOutputStream(File.Open(OutputFile, FileMode.CreateNew)));
            }
            return tw;
        }
    }
}
