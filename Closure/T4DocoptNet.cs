
using System.Collections;
using System.Collections.Generic;
using DocoptNet;

namespace Closure
{
	// Generated class for Main.usage.txt
	public class MainArgs
	{
		public const string USAGE = @"Closure Compiler CLI

Usage:
  closure FILENAME

Explanation:
    Processes file using the closure compiler ";
		private readonly IDictionary<string, ValueObject> _args;
		public MainArgs(ICollection<string> argv, bool help = true,
													  object version = null, bool optionsFirst = false, bool exit = false)
		{
			_args = new Docopt().Apply(USAGE, argv, help, version, optionsFirst, exit);
		}

		public IDictionary<string, ValueObject> Args
		{
			get { return _args; }
		}

		public string ArgFilename { get { return _args["FILENAME"].ToString(); } }
	
	}

	
}

