using System;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Threading.Tasks.Dataflow;

namespace Closure
{
    public class Program
    {
        private const string Data =
     "js_code={0}&output_format=xml&output_info=compiled_code&compilation_level=ADVANCED_OPTIMIZATIONS";

        private const string Url = "http://closure-compiler.appspot.com/compile";

        public static void Main(string[] argv)
        {
            var args = new MainArgs(argv, exit: true);
            if (args.ArgFilename.Length > 0)
            {
                ProcessFile(args.ArgFilename);
            }
        }

        private static void ProcessFile(string fileName)
        {
            var inputBlock = new BufferBlock<string>();

            var readBlock = new TransformBlock<string, string>(
                (input) =>
                {
                    Console.WriteLine("Loading " + input);
                    return File.ReadAllText(input);
                });

            var compileBlock = new TransformBlock<string, string>(
                (input) =>
                {
                    Console.WriteLine("Processing...");
                    var client = new WebClient();
                    client.Headers.Add("content-type", "application/x-www-form-urlencoded");
                    string apiData = string.Format(Data, HttpUtility.UrlEncode(input));
                    return client.UploadString(Url, apiData);
                });

            var convertBlock = new TransformBlock<string, XmlDocument>(
                (input) =>
                {
                    Console.WriteLine("Converting...");
                    var xml = new XmlDocument();
                    xml.LoadXml(input);
                    return xml;
                });

            var outputBlock = new ActionBlock<XmlDocument>(
                (input) =>
                {
                    Console.WriteLine("Writing compressed-" + fileName);
                    File.WriteAllText("compressed-" + fileName, input.SelectSingleNode("//compiledCode").InnerText);
                });

            inputBlock.LinkTo(readBlock, new DataflowLinkOptions { PropagateCompletion = true });
            readBlock.LinkTo(compileBlock, new DataflowLinkOptions { PropagateCompletion = true });
            compileBlock.LinkTo(convertBlock, new DataflowLinkOptions { PropagateCompletion = true });
            convertBlock.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

            inputBlock.Post(fileName);
            inputBlock.Complete();
            outputBlock.Completion.Wait();
        }
    

    }
}
