using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Helpers
{
    internal static class PrintToConsole
    {
        public static void Write(string text, ConsoleColor background = ConsoleColor.Black, ConsoleColor foreground = ConsoleColor.White)
        {
            Console.ResetColor();
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.Write($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: {text}");
        }

        public static void WriteLine(string text, ConsoleColor background = ConsoleColor.Black, ConsoleColor foreground = ConsoleColor.White)
        {
            Console.ResetColor();
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: {text}");
        }
    }
}
