using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilterMaster.Models
{
    public class FilterMasterSelectGameViewModel : ObservableObject, IGameVisibilityDeterminer
    {
        private bool filter = true;
        public static bool showUnavailable = true; // F*CK THIS SH*T
        private ObservableCollection<FilterMasterPropertyCollection> conditions = new ObservableCollection<FilterMasterPropertyCollection>();

        public static FilterMasterSelectGameViewModel Instance { get; set; }

        public bool Filter
        {
            get => filter; set
            {
                SetValue(ref filter, value);
                FillGames();
                UpdateGamesCommand.Execute(this);
            }
        }
        public bool ShowUnavailable
        {
            get => showUnavailable; set
            {
                SetValue(ref showUnavailable, value);
                FillGames();
                UpdateGamesCommand.Execute(this);
            }
        }

        private void FillGames()
        {
            Games.Clear();
            if (filter)
                API.Instance.MainView.FilteredGames.ForEach(game => Games.Add(game));
            else
                API.Instance.Database.Games.ForEach(game => Games.Add(game));
        }

        public bool IsVisible(Game game)
        {
            foreach (FilterMasterPropertyCollection conditionSet in Conditions)
            {
                foreach (FilterMasterProperty condition in conditionSet)
                {
                    if (!condition.Validate(game))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void Sniff(Game game)
        {
            if (!IsVisible(game)) return;
            foreach (FilterMasterPropertyCollection conditionSet in Conditions)
            {
                foreach (FilterMasterProperty condition in conditionSet)
                {
                    condition.Sniff(game);
                }
            }
        }

        public ObservableCollection<Game> Games { get; set; } = new ObservableCollection<Game>();
        public ObservableCollection<FilterMasterPropertyCollection> Conditions { get => conditions; set => SetValue(ref conditions, value); }
        public ICommand SwitchFilteredCommand { get; private set; }
        public ICommand UpdateGamesCommand { get; private set; }
        public ICommand GoToGameCommand { get; private set; }
        public ICommand ResetConditionsCommand { get; private set; }

        public static Guid ToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        internal void DoInit()
        {
            FillGames();
            Sniff();
        }
        public void Init()
        {
            if (Instance != null)
            {
                Games = Instance.Games;
                Conditions = Instance.Conditions;
            }
            else
            {
                DoInit();
            }
            Instance = this;
        }

        public FilterMasterSelectGameViewModel()
        {
            SwitchFilteredCommand = new ActionCommand((_) => Filter = !Filter);
            UpdateGamesCommand = new ActionCommand((_) =>
            {
                Conditions.ForEach(cset => cset
                    .Where(condition => condition.Selected == FilterMasterProperty.SelectedState.NotSet)
                    .ForEach(condition => condition.Selected = FilterMasterProperty.SelectedState.NotPossible));
                this.OnPropertyChanged(nameof(Games));
                this.OnPropertyChanged(nameof(Conditions));
                Sniff();
            });
            GoToGameCommand = new ActionCommand((object arg) =>
            {
                if (!(arg is Guid Id)) return;
                API.Instance.MainView.SelectGame(Id);
                API.Instance.MainView.SwitchToLibraryView();
            });
            ResetConditionsCommand = new ActionCommand((_) =>
             {
                 DoInit();
                 conditions.ForEach(condset => condset.ForEach(condition => condition.Selected = FilterMasterProperty.SelectedState.NotPossible));
                 UpdateGamesCommand.Execute(this);
             });

            Dictionary<InstallationStatus, Guid> InstallationStatuses = new Dictionary<InstallationStatus, Guid>();
            foreach (InstallationStatus status in Enum.GetValues(typeof(InstallationStatus)))
            {
                InstallationStatuses.Add(status, ToGuid((int)status));
            }


            Func<Game, IEnumerable<Guid>> func;
            FilterMasterPropertyCollection collection;

            func = (Game game) => new List<Guid>() { InstallationStatuses[game.InstallationStatus] };
            collection = new FilterMasterPropertyCollection()
            {
                Name = ResourceProvider.GetString("LOCGameIsInstalledTitle"),
            };
            InstallationStatuses.Keys.ToList().ForEach(i => collection.Add(new FilterMasterProperty(InstallationStatuses[i], Enum.GetName(typeof(InstallationStatus), i), func)));
            Conditions.Add(collection);


            func = (Game game) => game.PlatformIds;
            collection = new FilterMasterPropertyCollection()
            {
                Name = ResourceProvider.GetString("LOCPlatformTitle"),
            };
            API.Instance.Database.Platforms.ForEach(p => collection.Add(new FilterMasterProperty(p.Id, p.Name, func)));
            collection.Sort();
            Conditions.Add(collection);


            func = (Game game) => new List<Guid>() { game.PluginId };
            collection = new FilterMasterPropertyCollection()
            {
                Name = ResourceProvider.GetString("LOCLibrary"),
            };
            API.Instance.Addons.Plugins
                .OfType<LibraryPlugin>()
                .ForEach(p => collection.Add(new FilterMasterProperty(p.Id, p.Name, func)));
            collection.Add(new FilterMasterProperty(Guid.Empty, "Playnite", func));
            collection.Sort();
            Conditions.Add(collection);

            func = (Game game) => new List<Guid>() { game.SourceId };
            collection = new FilterMasterPropertyCollection()
            {
                Name = ResourceProvider.GetString("LOCSourceLabel"),
            };
            API.Instance.Database.Sources.OrderBy(p => p.Name).ForEach(p => collection.Add(new FilterMasterProperty(p.Id, p.Name, func)));
            collection.Add(new FilterMasterProperty(Guid.Empty, "Playnite", func));
            Conditions.Add(collection);


            func = (Game game) => game.FeatureIds;
            collection = new FilterMasterPropertyCollection()
            {
                Name = ResourceProvider.GetString("LOCFeatureLabel"),
            };
            API.Instance.Database.Features.OrderBy(p => p.Name).ForEach(p => collection.Add(new FilterMasterProperty(p.Id, p.Name, func)));
            Conditions.Add(collection);

            func = (Game game) => game.GenreIds;
            collection = new FilterMasterPropertyCollection()
            {
                Name = ResourceProvider.GetString("LOCGenreLabel"),
            };
            API.Instance.Database.Genres.OrderBy(p => p.Name).ForEach(p => collection.Add(new FilterMasterProperty(p.Id, p.Name, func)));
            Conditions.Add(collection);

            func = (Game game) => game.CategoryIds;
            collection = new FilterMasterPropertyCollection()
            {
                Name = ResourceProvider.GetString("LOCCategoryLabel"),
            };
            API.Instance.Database.Categories.OrderBy(p => p.Name).ForEach(p => collection.Add(new FilterMasterProperty(p.Id, p.Name, func)));
            Conditions.Add(collection);

            func = (Game game) => game.TagIds;
            collection = new FilterMasterPropertyCollection()
            {
                Name = ResourceProvider.GetString("LOCTagLabel"),
            };
            API.Instance.Database.Tags.OrderBy(p => p.Name).ForEach(p => collection.Add(new FilterMasterProperty(p.Id, p.Name, func)));
            Conditions.Add(collection);

            func = (Game game) => new List<Guid>() { game.CompletionStatusId };
            collection = new FilterMasterPropertyCollection()
            {
                Name = ResourceProvider.GetString("LOCCompletionStatus"),
            };
            API.Instance.Database.CompletionStatuses.OrderBy(p => p.Name).ForEach(p => collection.Add(new FilterMasterProperty(p.Id, p.Name, func)));
            Conditions.Add(collection);
            //DoInit();
        }

        private void Sniff()
        {
            Games.ForEach(game => {
                if (IsVisible(game))
                    Sniff(game);
            });
        }
    }
}
