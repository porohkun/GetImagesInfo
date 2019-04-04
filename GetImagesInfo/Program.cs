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
                if (args.Length > 0)
                    ExportInfo(args[0]);
                else
                    RegisterMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error:");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
        }

        private static void ExportInfo(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Directory '{path}' does not exists");
                Console.ReadLine();
                return;
            }
            bool pause = false;
            var sb = new StringBuilder();
            foreach (var file in Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly)
                .Union(Directory.GetFiles(path, "*.jpg", SearchOption.TopDirectoryOnly))
                .Union(Directory.GetFiles(path, "*.jpeg", SearchOption.TopDirectoryOnly))
                .Union(Directory.GetFiles(path, "*.bmp", SearchOption.TopDirectoryOnly)))
            {
                try
                {
                    var img = System.Drawing.Image.FromFile(file);
                    sb.AppendLine($"{Path.GetFileName(file)},{img.Width},{img.Height}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Cant open file '{file}': {e.Message}");
                    pause = true;
                }
            }
            File.WriteAllText(Path.Combine(path, "images_info.csv"), sb.ToString());
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
