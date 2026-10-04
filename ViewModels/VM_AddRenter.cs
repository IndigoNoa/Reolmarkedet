using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Reolmarkedet.ViewModels
{
    public class AddrenterViewModel
    {
        public ObservableCollection<Addrenter> AddrenterList { get; set; }

        public AddrenterViewModel()
        {
            AddrenterList = new ObservableCollection<Addrenter>();
            LoadAddrenter();
        }

        private void LoadAddrenter()
        {
            // Load data from a data source or initialize with mock data
        }
    }
}
