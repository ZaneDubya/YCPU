using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YCPU
{
    class Program
    {
        const string errNoArguments = "ycpuassember: No input specified.";
        const string errArguments = "ycpuassembler: Incorrect argument format. Stop.\n    {0}";
        const string descAssembler = "ycpuassembler: Assembles YASM files into binary code for YCPU.\n    in:  {0}\n    out: {1}";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
#if DEBUG
                args = new string[1] { "../../../../Tests/rain.yasm" };
#else
                Console.WriteLine(errNoArguments);
                return;
#endif
            }

            string inPath, outPath, error;
            string[] options;

            if (!tryReadArguments(args, out inPath, out outPath, out options, out error))
            {
                Console.WriteLine(string.Format(errArguments, error));
                return;
            }
            else
            {
                // check for options?
            }

            Console.WriteLine(string.Format(descAssembler, inPath, outPath));

            AssemblerResult result = doAssemble(inPath, Path.GetDirectoryName(inPath), outPath);
            Console.WriteLine(AssemblerResultMessages[(int)result]);
            Console.ReadKey();
        }

        private static bool tryReadArguments(string[] args, out string inPath, out string outPath, out string[] options, out string error)
        {
            inPath = null;
            outPath = null;
            options = null;
            error = null;

            List<string> readOptions = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null || args[i].Length == 0)
                    continue;

                if (args[i][0] == '-') // option!
                {
                    // do nothing - we don't yet handle any options.
                }
                else
                {
                    if (inPath == null)
                        inPath = args[i];
                    else if (outPath == null)
                        outPath = args[i];
                    else
                    {
                        error = string.Format("Unknown parameter: {0}", args[i]);
                        return false;
                    }
                }
            }

            if (inPath == null)
                return false;

            if (outPath == null)
                outPath = inPath + ".bin";

            return true;
        }

        private static string[] getFileContents(string in_path)
        {
            if (!File.Exists(in_path))
            {
                return null;
            }

            string in_code = null;
            using (StreamReader sr = new StreamReader(in_path))
            {
                in_code = sr.ReadToEnd().Trim();
            }

            if (in_code == string.Empty)
                return null;

            string[] lines = in_code.Split('\n');

            return lines;
        }

        private static AssemblerResult doAssemble(string in_path, string out_dir, string out_filename)
        {
            string[] lines = getFileContents(in_path);
            if (lines == null)
                return AssemblerResult.EmptyDocument;

            Assembler.Parser parser = new Assembler.Parser();
            byte[] machineCode = parser.Parse(lines, Path.GetDirectoryName(in_path));

            if (machineCode == null)
            {
                Console.WriteLine(parser.MessageOutput);
                return AssemblerResult.ParseError;
            }
            else
            {

                Assembler.Generator generator = new Assembler.Generator();
                string output = generator.Generate(machineCode, out_dir, out_filename);
                if (output == string.Empty)
                    return AssemblerResult.GenerateError;
                // note both assemble.MessageOutput and generator.MessageOutput have content.
                return AssemblerResult.Success;
            }
        }

        private enum AssemblerResult
        {
            Success,
            EmptyDocument,
            ParseError,
            GenerateError
        }

        private static string[] AssemblerResultMessages = new string[4]
        {
            "Successfully compiled input file.",
            "Nothing to compile.",
            "Parser error.",
            "Generator error."
        };

    }
}
