using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class PrinterInfo
    {
        public string Name { get; set; }
        
        public IEnumerable<PaperFormat> PaperFormats { get; set; }

        public PrinterInfo(string name, IEnumerable<PaperFormat> paperFormats)
        {
            Name = name;
            PaperFormats = paperFormats;
        }
    }
}
