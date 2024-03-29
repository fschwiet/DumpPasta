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
            int i = 0;

            _te.WriteLine("[");

            var separator = "";

            foreach (var row in rows)
            {
                _te.WriteLine(separator + JsonConvert.SerializeObject(row));
                separator = ", ";

                if (i++ % 200 == 0)
                {
                    _te.Flush();
                }

                yield return row;
            }

            _te.WriteLine("]");
        }
    }
}