using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
        }

        public string ConnectionString;
        public string DatabaseName;
        public string TableName;
        public string IdColumn;

        public override int Run(string[] remainingArguments)
        {
            var config = new ConnectionStringSettings()
            {
                Name = "First",
                ConnectionString = ConnectionString,
                ProviderName = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            };
            var reader = new ConventionInputCommandOperation(config)
            {
                Command = "SELECT * FROM " + TableName + " ORDER BY " + IdColumn
            };

            EtlProcess etlProcess = new ScanProcess();
            etlProcess.Register(reader);
            etlProcess.Register(new TraceOperation());

            etlProcess.PipelineExecuter = new SingleThreadedPipelineExecuter();

            etlProcess.Execute();

            foreach (var error in etlProcess.GetAllErrors())
                Console.WriteLine(error.ToString());

            return 0;
        }

        public class ScanProcess : EtlProcess
        {
            public ScanProcess()
            {
            }

            protected override void Initialize()
            {
            }
        }

        public class TraceOperation : AbstractOperation
        {
            public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
            {
                foreach (var row in rows)
                {
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(row));
                    yield return row;
                    Console.WriteLine();
                }

                yield break;
            }
        }
    }
}
