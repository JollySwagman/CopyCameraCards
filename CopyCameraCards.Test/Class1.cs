using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyCameraCards.Test
{
    [TestFixture]
    public class Class1
    {
        private string[] _types = new string[] { "mpg", "jpg", "mod", "mov", "jpeg", "mp4" };

        [Test]
        public void Basic_()
        {
            //var types = new string[] { ".mp*", ".jp*", ".mod", ".mov" };

            var from = @"H:\";
            var to = @"D:\!CardCopier";

            //var copier = new Copier(from, to);
            //Copier.DrivInfo();

            //var result = Copier.GetLabelForDrive(from);

            //Trace.WriteLine("Label: " + result);

            var c = new Copier(from, to);

            c.RunSearch(@"H:\", _types, to);

            Trace.WriteLine("");
            Trace.WriteLine(c.ToString());
        }

        [Test]
        public void Filename_Match()
        {
            //var c = new Copier("", "");

            var pattern = Helper.MakeRegexFromPattern(_types);

            Trace.WriteLine(pattern);
            //var result = Copier.IsFilenameMatch(@"E:\CardCopier\VOL_1021757571\PRIVATE\AVCHD\BDMV\MOVIEOBJ.BDM", pattern);
            var result = Copier.IsFilenameExtensionMatch(@"MOVIEOBJ.BDM", pattern);

            Trace.WriteLine(result);
            //Trace.WriteLine(c.ToString());
        }
    }
}