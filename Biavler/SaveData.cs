using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biavler
{
    public class SaveData
    {
        private ObservableCollection<VarroEntry> entries;

        public ObservableCollection<VarroEntry> Entries {
            get { return entries; }
            set { entries = value; }
        }

        private ObservableCollection<string> entryIds;
        public ObservableCollection<string> EntryIds {
            get { return entryIds; }
            set { entryIds = value; }
        }
    }
}
