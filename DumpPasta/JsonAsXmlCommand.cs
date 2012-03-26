using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ManyConsole;
using Newtonsoft.Json;

namespace DumpPasta
{
    public class JsonAsXmlCommand : ConsoleCommand
    {
        public JsonAsXmlCommand()
        {
            this.IsCommand("json-to-xml", "Reads input stream as json, converting it to XML.");
            this.HasOption("r|root=", "Outer XML tag used to contain the document", v => RootTag = v);
            this.HasOption("i|inner=", "XML tag insert in the document to make some JSON representable as XML.", v => InnerTag = v);
            this.SkipsCommandSummaryBeforeRunning();
        }

        public string RootTag = "xml";
        public string InnerTag = "member";

        public override int Run(string[] remainingArguments)
        {
            var result = Console.In.ReadToEnd();

            //  Now wrap the json in a object since the XML must have an outer tag
            XDocument inJson = null;

            try
            {
                inJson = JsonConvert.DeserializeXNode(result, RootTag);
            }
            catch (JsonSerializationException)
            {
                // I really have no idea why using InnerTag and RootTag work well this second way,
                // they seem reversed.
                inJson = JsonConvert.DeserializeXNode("{" + InnerTag + ":" + result + "}", RootTag);
            }

            Console.Out.WriteLine(inJson.ToString());

            return 0;
        }
    }
}
