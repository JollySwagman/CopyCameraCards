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
        private static RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

        public string FromDrive { get; set; }
        public string ToDrive { get; set; }
        public string Pattern { get; set; }
        public string FromVolume { get; set; }

        public int TotalFilesFound = 0;
        public long TotalBytesFound = 0;
        public long TotalMatchingBytesFound = 0;
        public int TotalMatchingFilesFound = 0;

        public IList<string> ExtensionsFound { get; set; } = new List<string>();

        public int Megabyte = 1048576;
        public int Gigabyte = 1073741824;

        public Copier(string fromDrive, string toDrive)
        {
            if (string.IsNullOrWhiteSpace(fromDrive))
            {
                throw new ArgumentNullException("fromDrive");
            }

            if (string.IsNullOrWhiteSpace(toDrive))
            {
                throw new ArgumentNullException("toDrive");
            }

            FromDrive = fromDrive;
            ToDrive = toDrive;
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

        public string GetLabelForDrive(string drive, bool setToRandomIfEmptynewLabel = false)
        {
            var result = "";
            var driveFound = false;

            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                if (d.Name == drive)
                {
                    driveFound = true;

                    if (d.DriveType != DriveType.Removable)
                    {
                        throw new ArgumentException("Drive " + drive + " not removable.");
                    }

                    if (d.IsReady)
                    {
                        result = d.VolumeLabel;

                        if (string.IsNullOrWhiteSpace(result))
                        {
                            var newVolume = new Random().Next(0, int.MaxValue);

                            d.VolumeLabel = newVolume.ToString();
                        }
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

            FromVolume = result;

            return result;
        }

        public void RunSearch(string startFolder, string[] pattern, string moveTo)
        {
            var destination = new DirectoryInfo(moveTo);
            if (destination.Exists == false)
            {
                destination.Create();
            }

            ExtensionsFound = new List<string>();

            TotalFilesFound = 0;
            TotalMatchingFilesFound = 0;

            // Set volume label if empty
            var label = GetLabelForDrive(startFolder, true);

            // init regex pattern

            Pattern = Helper.MakeRegexFromPattern(pattern);

            DirSearch(startFolder, destination);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="startFolder"></param>
        /// <param name="destination"></param>
        private void DirSearch(string startFolder, DirectoryInfo destination)
        {
            foreach (string d in Directory.GetDirectories(startFolder))
            {
                foreach (string file in Directory.GetFiles(d))
                {
                    // Housekeeping
                    TotalFilesFound++;
                    UpdateExtensionList(file);
                    TotalBytesFound += new FileInfo(file).Length;

                    if (IsFilenameExtensionMatch(file, Pattern))
                    {
                        HandleMatchedFile(file, destination);
                    }
                }

                // Recurse
                DirSearch(d, destination);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsFilenameExtensionMatch(string file, string pattern)
        {
            var ext = Path.GetExtension(file);

            var result = false;
            foreach (Match m in Regex.Matches(ext, pattern, options))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        public void UpdateExtensionList(string file)
        {
            var ext = Path.GetExtension(file);
            if (ExtensionsFound.Contains(ext) == false)
            {
                ExtensionsFound.Add(ext);
            }
        }

        public void HandleMatchedFile(string filename, DirectoryInfo destination)
        {
            var originalFile = new FileInfo(filename);
            TotalMatchingBytesFound += originalFile.Length;
            TotalMatchingFilesFound++;

            var newpath = new FileInfo(filename).FullName.Replace(FromDrive, "");

            var destinationSpec = Path.Combine(this.ToDrive, "VOL_" + FromVolume, newpath);

            Trace.WriteLine("Copying to " + destinationSpec);

            var fi = new FileInfo(destinationSpec);
            if (fi.Directory.Exists == false)
            {
                fi.Directory.Create();
            }

            // This is it
            if (new FileInfo(destinationSpec).Exists == false)
            {
                File.Copy(filename, destinationSpec);
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.AppendLine("[" + this.GetType().Name + "]");
            result.AppendLine("Pattern: " + Pattern);
            result.AppendLine("TotalFilesFound: " + TotalFilesFound);
            result.AppendLine("TotalMatchingFilesFound: " + TotalMatchingFilesFound);

            result.AppendLine("TotalBytesFound: " + TotalBytesFound);
            result.AppendLine("TotalMegabytesFound: " + TotalBytesFound / Megabyte);
            result.AppendLine("TotalGigabytesFound: " + TotalBytesFound / Gigabyte);

            result.AppendLine("TotalMatchingBytesFound: " + TotalMatchingBytesFound);
            result.AppendLine("TotalMatchingMegabytesFound: " + TotalMatchingBytesFound / Megabyte);
            result.AppendLine("TotalMatchingGigabytesFound: " + TotalMatchingBytesFound / Gigabyte);

            //result.AppendLine(": " + TotalFilesFound);
            //result.AppendLine(": " + TotalFilesFound);
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