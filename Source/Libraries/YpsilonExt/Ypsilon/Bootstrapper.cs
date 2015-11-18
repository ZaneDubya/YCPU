using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon
{
    static class Bootstrapper
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (YpsilonGame game = new YpsilonGame())
            {
                game.Run();
            }
        }
    }
}
