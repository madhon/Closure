namespace Closure
{
    public class Program
    {
        public static void Main(string[] argv)
        {
            var args = new MainArgs(argv, exit: true);
            if (args.ArgFilename.Length > 0)
            {
                var fileProcessor = new Processor();
                fileProcessor.ProcessFile(args.ArgFilename);
            }
        }
       
    }
}
