using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CopyCameraCards
{
    public class Copier
    {
        public string fromDrive { get; set; }
        public string toDrive { get; set; }
        public string Pattern { get; set; }

        public int TotalFilesFound = 0;
        public int TotalMatchingFilesFound = 0;

        public IList<string> ExtensionsFound { get; set; } = new List<string>();

        public Copier(string fromDrive, string toDrive)
        {
        }

        public static void CopyAll(string fromDrive, string toDrive, ICollection<string> types)
        {
        }

        public static void DrivInfo()
        {
            var allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                Trace.WriteLine(string.Format("Drive {0}", d.Name));
                Trace.WriteLine(string.Format("  Drive type: {0}", d.DriveType.ToString()));
                if (d.IsReady == true)
                {
                    Trace.WriteLine(string.Format("  Volume label: {0}", d.VolumeLabel));
                    Trace.WriteLine(string.Format("  File system: {0}", d.DriveFormat));
                    Trace.WriteLine(string.Format("  Available space to current user:{0, 15} bytes", d.AvailableFreeSpace));
                    Trace.WriteLine(string.Format("  Total available space:          {0, 15} bytes", d.TotalFreeSpace));
                    Trace.WriteLine(string.Format("  Total size of drive:            {0, 15} bytes ", d.TotalSize));
                }
            }
        }

        public static string GetLabelForDrive(string drive)
        {
            var result = "";
            var driveFound = false;

            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                if (d.Name == drive)
                {
                    driveFound = true;
                    if (d.IsReady)
                    {
                        result = d.VolumeLabel;
                    }
                    else
                    {
                        throw new IOException("Drive " + drive + " not ready.");
                    }
                }
            }

            if (driveFound == false)
            {
                throw new ArgumentException("Drive " + drive + " not found.");
            }

            return result;
        }

        public void RunSearch(string startFolder, string[] pattern)
        {
            ExtensionsFound = new List<string>();

            TotalFilesFound = 0;
            TotalMatchingFilesFound = 0;

            var r = "(";
            foreach (var item in pattern)
            {
                r += item + "|";
            }
            r = r.TrimEnd('|');
            r += ")";

            Pattern = r;

            DirSearch(startFolder, pattern);
        }

        private void DirSearch(string startFolder, string[] pattern)
        {
            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

            foreach (string d in Directory.GetDirectories(startFolder))
            {
                foreach (string file in Directory.GetFiles(d))
                {
                    TotalFilesFound++;

                    foreach (Match m in Regex.Matches(file, Pattern, options))
                    {
                        TotalMatchingFilesFound++;
                        //Trace.WriteLine(string.Format("'{0}' found at index {1}.", m.Value, m.Index));
                        HandleFile(file);
                    }

                    var ext = Path.GetExtension(file);
                    if (ExtensionsFound.Contains(ext) == false)
                    {
                        ExtensionsFound.Add(ext);
                    }
                }

                // Recurse
                DirSearch(d, pattern);
            }
        }

        public void HandleFile(string filename)
        {
            //Trace.WriteLine(string.Format("'{0}' found.", filename));
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.AppendLine("[" + this.GetType().Name + "]");
            result.AppendLine("Pattern: " + Pattern);
            result.AppendLine("TotalFilesFound: " + TotalFilesFound);
            result.AppendLine("TotalMatchingFilesFound: " + TotalMatchingFilesFound);
            //result.AppendLine(": " + TotalFilesFound);
            result.AppendLine("ExtensionsFound:");
            foreach (var item in ExtensionsFound)
            {
                result.AppendLine("  " + item);
            }

            return result.ToString();
        }
    }
}