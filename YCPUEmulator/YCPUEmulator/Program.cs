using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU
{
    class Program
    {
        static void Main(string[] args)
        {
            // Declare the emulator
            Emulator e = new Emulator();
            // Load the intitial RAM.
            // Lauch the emulator.
            e.Start();
        }
    }
}
