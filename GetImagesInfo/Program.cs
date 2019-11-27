using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GetImagesInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                switch (args.Length)
                {
                    case 0: RegisterMenu(); break;
                    case 1: ExportPicturesInfo(args[0]); break;
                    case 2: ExportFilesInfo(args[0], args[1]); break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error:");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
        }

        private static void ExportPicturesInfo(string path)
        {
            ExportInfo(path, false);
        }

        private static void ExportFilesInfo(string path, string arg)
        {
            if (arg == "-f")
                ExportInfo(path, true);
            else
                throw new ArgumentException($"argument {arg} is not valid");
        }

        private static void ExportInfo(string path, bool anyFiles)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Directory '{path}' does not exists");
                Console.ReadLine();
                return;
            }
            bool pause = false;
            var sb = new StringBuilder();
            var files = anyFiles ? Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                : Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly)
                .Union(Directory.GetFiles(path, "*.jpg", SearchOption.TopDirectoryOnly))
                .Union(Directory.GetFiles(path, "*.jpeg", SearchOption.TopDirectoryOnly))
                .Union(Directory.GetFiles(path, "*.bmp", SearchOption.TopDirectoryOnly));
            foreach (var file in files)
            {
                try
                {
                    if (anyFiles)
                    {
                        sb.AppendLine($"{Path.GetFileName(file)}");
                    }
                    else
                    {
                        var img = System.Drawing.Image.FromFile(file);
                        sb.AppendLine($"{Path.GetFileName(file)},{img.Width},{img.Height}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Cant open file '{file}': {e.Message}");
                    pause = true;
                }
            }
            File.WriteAllText(Path.Combine(path, "info.csv"), sb.ToString());
            if (pause)
                Console.ReadLine();
        }

        private static void RegisterMenu()
        {
            try
            {
                using (var currentUserKey = Registry.ClassesRoot.OpenSubKey("Directory\\Background\\shell", true))
                {
                    using (var getImagesKey = currentUserKey.CreateSubKey("GetImagesInfo"))
                    {
                        getImagesKey.SetValue(null, "Collect images data");
                        getImagesKey.SetValue("Icon", System.Reflection.Assembly.GetEntryAssembly().Location);
                        using (var commandKey = getImagesKey.CreateSubKey("command"))
                            commandKey.SetValue(null, $"\"{System.Reflection.Assembly.GetEntryAssembly().Location}\" \"%V\"");
                    }
                    using (var getImagesKey = currentUserKey.CreateSubKey("GetFilesInfo"))
                    {
                        getImagesKey.SetValue(null, "Collect files data");
                        getImagesKey.SetValue("Icon", System.Reflection.Assembly.GetEntryAssembly().Location);
                        using (var commandKey = getImagesKey.CreateSubKey("command"))
                            commandKey.SetValue(null, $"\"{System.Reflection.Assembly.GetEntryAssembly().Location}\" \"%V\" -f");
                    }
                }
                Console.WriteLine("Context menu item successfully registered");
            }
            catch (Exception e)
            {
                Console.WriteLine("Cant register context menu item:");
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}
