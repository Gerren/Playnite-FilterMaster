using FilterMaster.Models;
using Playnite.SDK.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FilterMaster.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FilterMasterSelectGameView : PluginUserControl
    {
        private FilterMasterSelectGameViewModel vm;

        public FilterMasterSelectGameView()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FilterMasterSelectGameViewModel model = DataContext as FilterMasterSelectGameViewModel;
            model.UpdateGamesCommand.Execute(null);
        }

        internal void Init()
        {
            vm = new FilterMasterSelectGameViewModel();
            DataContext = vm;
            vm.Init();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FilterMasterSelectGameViewModel model = DataContext as FilterMasterSelectGameViewModel;
            model.UpdateGamesCommand.Execute(null);
        }

        private void CmbPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterMasterSelectGameViewModel model = DataContext as FilterMasterSelectGameViewModel;
            model.ApplyPresavedFilterCommand.Execute(CmbPreset.SelectedItem);
        }
    }
}
