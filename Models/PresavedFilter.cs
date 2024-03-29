using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterMaster.Models
{
    public class PresavedFilter : ObservableObject
    {


        private string _FilterName;
        public string FilterName { get => _FilterName; set => SetValue(ref _FilterName, value); }

        private ObservableCollection<PresavedCondition> _Conditions = new ObservableCollection<PresavedCondition>();
        public ObservableCollection<PresavedCondition> Conditions { get => _Conditions; set => SetValue(ref _Conditions, value); }


        private bool _FilterCurrent = true;
        public bool FilterCurrent { get => _FilterCurrent; set => SetValue(ref _FilterCurrent, value); }


        private bool _ShowUnavailable = true;
        public bool ShowUnavailable { get => _ShowUnavailable; set => SetValue(ref _ShowUnavailable, value); }


        
    }
}
