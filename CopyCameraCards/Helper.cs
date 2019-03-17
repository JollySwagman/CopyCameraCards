using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyCameraCards
{
    public class Helper
    {
        public static string MakeRegexFromPattern(string[] patternList)
        {
            var result = "(";
            foreach (var item in patternList)
            {
                result += item + "|";
            }
            result = result.TrimEnd('|');
            result += ")";
            return result;
        }
    }
}