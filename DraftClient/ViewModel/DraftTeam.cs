using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraftClient.ViewModel
{
    public class DraftTeam : BindableBase
    {
        private string _name;
        private bool _isConnected;
        private int _index;

        public string Name 
        { 
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        public bool IsConnected
        {
            get { return _isConnected; }
            set { SetProperty(ref _isConnected, value); }
        }
        public int Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }
    }
}
