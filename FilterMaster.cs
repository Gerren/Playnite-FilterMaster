using FilterMaster.Models;
using FilterMaster.Views;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FilterMaster
{
    public class FilterMaster : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private FilterMasterSettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("1b3e10f1-bdbd-43cf-b996-063fbe2bae8d");

        public FilterMaster(IPlayniteAPI api) : base(api)
        {
            settings = new FilterMasterSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = false
            };

            side = new FilterMasterViewSidebar();
        }
        private readonly FilterMasterViewSidebar side;
        public class FilterMasterViewSidebar : SidebarItem
        {
            public FilterMasterViewSidebar()
            {
                Type = SiderbarItemType.View;
                Title = ResourceProvider.GetString("LOCFilterMasterTitle");
                Icon = new TextBlock
                {
                    Text = "🔍",
                    FontSize = 22,
                };
                Opened = () =>
                {
                    FilterMasterSelectGameView ViewExtension = new FilterMasterSelectGameView();
                    ViewExtension.DataContext = new FilterMasterSelectGameViewModel();
                    return ViewExtension;
                };
                Visible = true;
            }
        }
        public override IEnumerable<SidebarItem> GetSidebarItems()
        {
            return new List<SidebarItem>
            {
                side
            };
        }

        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(OnGameStartedEventArgs args)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new FilterMasterSettingsView();
        }
    }
}