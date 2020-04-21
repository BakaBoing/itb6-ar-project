using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class DataStoreDummyImpl : IDataStore
    {
        private Dictionary<string, PrinterInfo> _db;

        public DataStoreDummyImpl()
        {
            _db = new Dictionary<string, PrinterInfo>();
            _db.Add("Printer_HP_Deskjet_3050", new PrinterInfo("HP Deskjet 3050", new List<PaperFormat> { PaperFormat.A4 }));
            _db.Add("3D-car", new PrinterInfo("Äutile", new List<PaperFormat> { PaperFormat.A5, PaperFormat.A4 }));
        }

        public PrinterInfo GetPrinterInfo(string printerTrackableId)
        {
            return _db[printerTrackableId];
        }
    }
}
