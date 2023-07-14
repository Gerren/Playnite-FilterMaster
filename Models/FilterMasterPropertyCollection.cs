using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FilterMaster.Models
{
    public class FilterMasterPropertyCollection: ObservableCollection<FilterMasterProperty>, INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get => name; set
            {
                name = value;
                if (PropertyChanged != null)
                PropertyChanged(this,new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private Visibility isVisible = Visibility.Collapsed;

        public FilterMasterPropertyCollection()
        {
            CollapseToggleCommand = new ActionCommand((_)=>  IsVisible = IsVisible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        public Visibility IsVisible
        {
            get => isVisible; set
            {
                isVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
            }
        }

        public ICommand CollapseToggleCommand { get; set; }


        protected override event PropertyChangedEventHandler PropertyChanged;

        internal void Sort()
        {
            List<FilterMasterProperty> properties = this.OrderBy(p => p.Name).ToList();
            Clear();
            properties.ForEach(p => Add(p));
        }
    }
}
