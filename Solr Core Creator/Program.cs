using System;
using System.Collections.Generic;
using System.IO;

namespace Solr_Core_Creator
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> args_dic = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string key = "";
                string value = "";
                if (!string.IsNullOrEmpty(args[i]) && args[i].StartsWith("-"))
                {
                    key = args[i].ToLower();

                    if (i + 1 >= args.Length)   //last arg was not passed
                        break;

                    if (!string.IsNullOrEmpty(args[i + 1]))
                    {
                        value = args[i + 1];
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                args_dic.Add(key, value);
            }

            List<string> CoreNames = new List<string>();
            string DestinationPath = @".";     //Current Directory is the default

            if (args_dic.ContainsKey("-CoreNames".ToLower()))
            {
                string[] strArr_CoreNames = args_dic["-CoreNames".ToLower()].Split(',');
                CoreNames = new List<string>(strArr_CoreNames);
            }
            if (args_dic.ContainsKey("-Destination".ToLower()))
            {
                DestinationPath = args_dic["-Destination".ToLower()];
            }

            foreach (var CoreName in CoreNames)
            {
                // Copy from the current directory, include subdirectories.
                string destDirName = DestinationPath.TrimEnd('"') + @"\" + CoreName + "_index";
                DirectoryCopy(@".\core_template_index", destDirName, true, CoreName);
            }

        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string CoreName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string destPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(destPath, true);
                //Add Core Name to the "core.properties" file
                if (file.Name == "core.properties")
                {
                    string text = File.ReadAllText(file.FullName);
                    text = text.Replace("name_placeholder_index", CoreName + "_index");
                    File.WriteAllText(destPath, text);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs, CoreName);
                }
            }
        }
    }
}
