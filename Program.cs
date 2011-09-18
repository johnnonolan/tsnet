using System;
using System.IO;

namespace tsCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("File name missing");
                return;
            }

            string fileName = args[0];

            string bfData = File.ReadAllText(fileName);

            using (var app = new TrollScriptApp(Path.GetFileNameWithoutExtension(fileName)))
            {
                app.BuildApp(bfData);
            }
        }
    }
}
