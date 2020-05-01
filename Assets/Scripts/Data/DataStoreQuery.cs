using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class DataStoreQuery
    {
        private static DataStoreQuery _query = null;
        private IDataStore _dataStore;

        private static DataStoreQuery _instance
        {
            get => _query ?? new DataStoreQuery();
        }

        private DataStoreQuery()
        {
            _dataStore = new DataStoreDummyImpl();
        }

        public static PrinterInfo GetPrinterInfo(string trackablprinterTrackableId)
        {
            return _instance._dataStore.GetPrinterInfo(trackablprinterTrackableId);
        }
    }
}
