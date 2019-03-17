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
        [Test]
        public void Basic_()
        {
            //var types = new string[] { ".mp*", ".jp*", ".mod", ".mov" };
            var types = new string[] { "mpg", "jpg", "mod", "mov", "jpeg", "mp4" };

            var from = @"H:\";
            var to = @"E:\CardCopier";

            //var copier = new Copier(from, to);
            //Copier.DrivInfo();

            var result = Copier.GetLabelForDrive(from);

            Trace.WriteLine("Label: " + result);

            var c = new Copier("", "");

            c.RunSearch(@"H:\", types);

            Trace.WriteLine(c.ToString());
        }
    }
}