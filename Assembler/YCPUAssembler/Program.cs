using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YCPU
{
    class Program
    {
        static void Main(string[] args)
        {
            string in_path, out_path;
            args = new string[1] { "rain.yasm" };

            if (args.Length == 1)
            {
                in_path = args[0];
                out_path = args[0] + ".bin";
            }
            else if (args.Length == 2)
            {
                in_path = args[0];
                out_path = args[1];
            }
            else
            {
                Console.WriteLine("Usage: ycpuassember in_path [out_path]");
                return;
            }

            // string in_path = "../../../../Tests/rain.yasm";
            // string out_path = "rain.yasm.bin";

            string in_code = GetFileContents(in_path);
            if (in_code == null)
            {
                Console.WriteLine("In file does not exist.");
                return;
            }
            else
            {
                AssemblerResult result = Assemble(in_code, Path.GetDirectoryName(in_path), out_path);
                if (result == AssemblerResult.Success)
                {

                }
                Console.WriteLine(AssemblerResultMessages[(int)result]);
            }
        }

        static string GetFileContents(string in_path)
        {
            if (!File.Exists(in_path))
            {
                return null;
            }

            string in_code = null;
            using (StreamReader sr = new StreamReader(in_path))
            {
                in_code = sr.ReadToEnd();
            }
            return in_code;
        }

        static AssemblerResult Assemble(string document, string out_dir, string out_filename)
        {
            if (document.Trim() == string.Empty)
                return AssemblerResult.EmptyDocument;

            Assembler.Parser parser = new Assembler.Parser();
            string[] lines = document.Split('\n');
            ushort[] machineCode = parser.Parse(lines);
            if (machineCode == null)
                return AssemblerResult.ParseError;

            Assembler.Generator generator = new Assembler.Generator();
            string output = generator.Generate(machineCode, out_dir, out_filename);
            if (output == string.Empty)
                return AssemblerResult.GenerateError;

            // note both assemble.MessageOutput and generator.MessageOutput have content.
            return AssemblerResult.Success;
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
            "Success.",
            "Nothing to compile.",
            "Parser error.",
            "Generator error."
        };

    }
}
