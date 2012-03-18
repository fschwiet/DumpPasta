using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace DumpPasta
{
    public class TraceOperation : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                Console.WriteLine(JsonConvert.SerializeObject(row));
                yield return row;
                Console.WriteLine();
            }

            yield break;
        }
    }
}