using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace YLauncher
{
    public class Launcher : MarshalByRefObject
    {
        public static void Main(string[] args)
        {
            string path = Assembly.GetExecutingAssembly().Location;
            path = Path.GetDirectoryName(path);
            Directory.SetCurrentDirectory(path + @"\App");
            // Environment.CurrentDirectory += @"\App";
            /*Assembly asm = Assembly.LoadFrom(Environment.CurrentDirectory + @"\YCPUXNA.exe");
            asm.EntryPoint.Invoke(null, new object[] { new string[0] });*/
            Process.Start(@"YCPUXNA.exe");
        }

        /// <summary>
        /// This gets executed in the temporary appdomain.
        /// No error handling to simplify demo.
        /// </summary>
        public void Execute(string appPath)
        {
            // load the bytes and run Main() using reflection
            // working with bytes is useful if the assembly doesn't come from disk
            /*byte[] bytes = File.ReadAllBytes(appPath);
            Assembly assembly = Assembly.Load(bytes);
            MethodInfo main = assembly.EntryPoint;
            main.Invoke(null, new object[] { null });*/
        }
    }
}