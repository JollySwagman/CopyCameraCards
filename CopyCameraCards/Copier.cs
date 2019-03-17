using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyCameraCards
{
    public class Copier
    {
        public string fromDrive { get; set; }
        public string toDrive { get; set; }

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
    }
}