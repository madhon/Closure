using System;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;

namespace Closure
{
    public class Program
    {
        private const string Data =
            "js_code={0}&output_format=xml&output_info=compiled_code&compilation_level=SIMPLE_OPTIMIZATIONS";

        private const string Url = "http://closure-compiler.appspot.com/compile";

        public static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            string sourceFile = args[0];

            Console.WriteLine("Loading " + sourceFile);
            string content = File.ReadAllText(sourceFile);
            WebClient wClient = new WebClient();
            wClient.Headers.Add("content-type", "application/x-www-form-urlencoded");
            string apiData = string.Format(Data, HttpUtility.UrlEncode(content));

            Console.WriteLine("Processing");
            string response = wClient.UploadString(Url, apiData);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);

            Console.WriteLine("Writing compressed-" + sourceFile);
            File.WriteAllText("compressed-" + sourceFile, xml.SelectSingleNode("//compiledCode").InnerText);
        }
    }
}