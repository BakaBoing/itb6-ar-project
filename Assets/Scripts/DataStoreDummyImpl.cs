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
            _db.Add("Printer_HP_Deskjet_3050", new PrinterInfo("HP Deskjet 3050", new List<PaperFormat> { PaperFormat.A4 }, "1. Press Button \r\n 2. Press other button"));
            _db.Add("3D-car", new PrinterInfo("Äutile", new List<PaperFormat> { PaperFormat.A5, PaperFormat.A4 }, "1. Get in \r\n 2. Drive"));
            _db.Add("50_CENT", new PrinterInfo("50 in tha house", new List<PaperFormat> { PaperFormat.A3 }, "1. Spend"));
            _db.Add("VARTA_AA", new PrinterInfo("Agathe Bauer", new List<PaperFormat> { PaperFormat.A5, PaperFormat.A4 }, "1. Put in \r\n 2. Energize!"));
            _db.Add("DIORITE", new PrinterInfo("MC Builders", new List<PaperFormat> { PaperFormat.A5 }, "1. No idea!"));
            _db.Add("QR1", new PrinterInfo("Uhhhh Hiding Boring Infos", new List<PaperFormat> { PaperFormat.A4 }, "1. Print out \r\n 2. Fold \r\n 3. Have fun!"));
        }

        public PrinterInfo GetPrinterInfo(string printerTrackableId)
        {
            return _db[printerTrackableId];
        }
    }
}
