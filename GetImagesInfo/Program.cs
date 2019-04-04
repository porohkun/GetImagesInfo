using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GetImagesInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("need path argument");
            if(!Directory.Exists(args[0]))
                Console.WriteLine("$directory '{args[0]}' not exist");
            var sb = new StringBuilder();
            foreach (var file in Directory.GetFiles(args[0],"*.png", SearchOption.TopDirectoryOnly))
            {
                var img = System.Drawing.Image.FromFile(file);
                sb.AppendLine($"{Path.GetFileName(file)},{img.Width},{img.Height}");
            }
            File.WriteAllText(Path.Combine(args[0], "result.csv"), sb.ToString());
        }
    }
}
