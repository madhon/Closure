namespace Closure
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks.Dataflow;
    using System.Web;
    using System.Xml.Linq;

    public class Processor
    {
        private const string Data =
"js_code={0}&output_format=xml&output_info=compiled_code&compilation_level=ADVANCED_OPTIMIZATIONS";

        private const string Url = "http://closure-compiler.appspot.com/compile";

        public void ProcessFile(string fileName)
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

            var convertBlock = new TransformBlock<string, XDocument>(
                (input) =>
                {
                    Console.WriteLine("Converting to XDocument...");
                    var xml = XDocument.Parse(input);
                    return xml;
                });

            var outputBlock = new ActionBlock<XDocument>(
                (input) =>
                {
                    Console.WriteLine("Writing compressed-" + fileName);
                    XElement compiledCode = input.Element("compilationResult").Element("compiledCode");
                    File.WriteAllText("compressed-" + fileName, compiledCode.Value);
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
