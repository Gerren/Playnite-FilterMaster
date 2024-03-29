using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterMaster.Models
{
    public class PresavedCondition : ObservableObject
    {

        // BUG localization dependent
        private string _PropertyName;
        public string PropertyName { get => _PropertyName; set => SetValue(ref _PropertyName, value); }


        private Guid _Id;
        public Guid Id { get => _Id; set => SetValue(ref _Id, value); }


        private SelectedState _State;
        public SelectedState State { get => _State; set => SetValue(ref _State, value); }
        
    }
}
