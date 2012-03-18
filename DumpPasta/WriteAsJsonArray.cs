using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace DumpPasta
{
    public class WriteAsJsonArray : AbstractOperation
    {
        readonly TextWriter _te;

        public WriteAsJsonArray(TextWriter te)
        {
            _te = te;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            _te.WriteLine("[");

            var separator = "";

            foreach (var row in rows)
            {
                Console.WriteLine(separator + JsonConvert.SerializeObject(row));
                separator = ", ";

                yield return row;
            }

            _te.WriteLine("]");
        }
    }
}