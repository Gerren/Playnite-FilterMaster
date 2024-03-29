using Playnite.SDK;
using Playnite.SDK.Data;
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

        private string Path => System.IO.Path.Combine(API.Instance.Addons.Plugins.OfType<FilterMaster>().FirstOrDefault().GetPluginUserDataPath(), "presets.json");

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


        private ObservableCollection<PresavedFilter> _Filters = new ObservableCollection<PresavedFilter>();
        public ObservableCollection<PresavedFilter> Filters { get => _Filters; set => SetValue(ref _Filters, value); }

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
            var brackets = new List<SelectedState>() { 
                SelectedState.Bracket1, 
                SelectedState.Bracket2, 
                SelectedState.Bracket3, 
                SelectedState.Bracket4, 
                SelectedState.Bracket5 
            };

            foreach (FilterMasterPropertyCollection conditionSet in Conditions)
            {
                // if not .. all ... true, then do not show
                if (!conditionSet.Where(c => c.Selected == SelectedState.Selected).All(c => c.Validate(game))) return false;
                // if ... any ... false, then do not show
                if (conditionSet.Where(c => c.Selected == SelectedState.NotSelected).Any(c => !c.Validate(game))) return false;

                foreach (SelectedState bracket in brackets)
                {
                    if (!conditionSet.Any(c => c.Selected == bracket)) continue;
                    // if ... any ... true, then continue
                    if (conditionSet.Where(c => c.Selected == bracket).Any(c => c.Validate(game))) continue;
                    // if .. all ... false, then do not show
                    return false;
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


        private bool _IsAddingNewFilter;
        public bool IsAddingNewFilter { get => _IsAddingNewFilter; set => SetValue(ref _IsAddingNewFilter, value); }


        private string _NewFilterName = string.Empty;
        public string NewFilterName { get => _NewFilterName; set => SetValue(ref _NewFilterName, value); }


        public ICommand SwitchFilteredCommand { get; private set; }
        public ICommand UpdateGamesCommand { get; private set; }
        public ICommand GoToGameCommand { get; private set; }
        public ICommand ResetConditionsCommand { get; private set; }
        public ICommand ToggleAddingFilterCommand { get; private set; }
        public ICommand AddNewFilterCommand {  get; private set; }
        public ICommand ApplyPresavedFilterCommand { get; private set; }
        public ICommand DeletePresavedFilterCommand { get; private set; }
        public ICommand OverwritePresavedFilterCommand { get; private set; }


        private PresavedFilter _CurrentFilter;
        public PresavedFilter CurrentFilter { get => _CurrentFilter; set => SetValue(ref _CurrentFilter, value); }

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
                Filters = Instance.Filters;
            }
            else
            {
                LoadFilters();
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
                    .Where(condition => condition.Selected == SelectedState.NotSet)
                    .ForEach(condition => condition.Selected = SelectedState.NotPossible));
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
            ResetConditionsCommand = new ActionCommand(ResetConditions);
            ToggleAddingFilterCommand = new ActionCommand(ToggleAddingFilter);
            AddNewFilterCommand = new ActionCommand(AddNewFilter);
            ApplyPresavedFilterCommand = new ActionCommand(ApplyPresavedFilter);
            DeletePresavedFilterCommand = new ActionCommand(DeletePresavedFilter);
            OverwritePresavedFilterCommand = new ActionCommand(OverwritePresavedFilter);

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

        private void ResetConditions(object obj)
        {
            ResetConditions();
            CurrentFilter = null;
        }

        private void ResetConditions()
        {
            DoInit();
            conditions.ForEach(condset => condset.ForEach(condition => condition.Selected = SelectedState.NotPossible));
            UpdateGamesCommand.Execute(this);
        }

        private void OverwritePresavedFilter(object obj)
        {
            if(CurrentFilter != null)
            {
                PresavedFilter filter = ExtractPresavedFilter();
                CurrentFilter.Conditions = filter.Conditions;
                CurrentFilter.FilterCurrent = filter.FilterCurrent;
                CurrentFilter.ShowUnavailable = filter.ShowUnavailable;
                SaveFilters();
            }
        }

        private void DeletePresavedFilter(object obj)
        {
            if (CurrentFilter != null)
            {
                Filters.Remove(CurrentFilter);
                SaveFilters();
            }
        }

        private void ApplyPresavedFilter(object obj)
        {
            if(obj is PresavedFilter filter)
            {
                if(Filter != filter.FilterCurrent) Filter = filter.FilterCurrent;
                if(ShowUnavailable != filter.ShowUnavailable) ShowUnavailable = filter.ShowUnavailable;
                ResetConditions();
                foreach(FilterMasterPropertyCollection conditions in Conditions)
                {
                    foreach(PresavedCondition condition in filter.Conditions.Where(p => p.PropertyName == conditions.Name))
                    {
                        FilterMasterProperty found = conditions.FirstOrDefault(c=>c.Id==condition.Id);
                        if (found != null) found.Selected = condition.State;
                    }
                }
            }
        }

        private void ToggleAddingFilter(object obj)
        {
            if (obj is string) obj = bool.Parse(obj.ToString());
            if(obj is bool newstate && newstate == false)
            {
                NewFilterName = string.Empty;
                IsAddingNewFilter = false;
            }
            else
            {
                NewFilterName = string.Empty;
                IsAddingNewFilter = true;
            }
        }

        private void AddNewFilter(object obj)
        {
            PresavedFilter filter = ExtractPresavedFilter();
            Filters.Add(filter);
            if (SaveFilters())
                ToggleAddingFilter(false);
            CurrentFilter = filter;
        }

        private PresavedFilter ExtractPresavedFilter()
        {
            PresavedFilter filter = new PresavedFilter() 
            { 
                FilterName = NewFilterName ,
                FilterCurrent = this.Filter,
                ShowUnavailable = ShowUnavailable,
            };
            Conditions.ForEach(set => set
                .Where(condition => condition.Selected != SelectedState.NotSet
                                && condition.Selected != SelectedState.NotPossible)
                .ForEach(condition =>
                    filter.Conditions.Add(
                        new PresavedCondition()
                        {
                            PropertyName = set.Name,
                            Id = condition.Id,
                            State = condition.Selected,
                        })
                    )
            );
            return filter;
        }

        private bool SaveFilters()
        {            
            System.IO.File.WriteAllText(Path, Serialization.ToJson(Filters));
            return true;
        }

        private void LoadFilters()
        {
            try
            {
                if (Serialization.TryFromJson(System.IO.File.ReadAllText(Path), out ObservableCollection<PresavedFilter> loaded))
                {
                    Filters = loaded;
                }
            }
            catch { }
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
