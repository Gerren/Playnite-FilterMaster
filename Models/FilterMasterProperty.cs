﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Playnite.SDK.Models;

namespace FilterMaster.Models
{
    public class FilterMasterProperty : ObservableObject, IIdentifiable
    {
        public FilterMasterProperty(Guid id, string name, Func<Game, IEnumerable<Guid>> validator)
        {
            Id = id;
            Name = name;
            Validator = validator;
            ToggleSelectedCommand = new ActionCommand((_) =>
            {
                switch (selected)
                {
                    case SelectedState.NotSet:
                    case SelectedState.NotPossible:
                        Selected = SelectedState.Selected;
                        break;
                    case SelectedState.Selected:
                        Selected = SelectedState.NotSelected;
                        break;
                    case SelectedState.Maybe:
                        Selected = SelectedState.NotSelected;
                        break;
                    default:
                        Selected = SelectedState.NotSet;
                        break;
                }
                this.OnPropertyChanged(nameof(Selected));
                FilterMasterSelectGameViewModel.Instance.UpdateGamesCommand.Execute(this);
            });
            ResetSelectedCommand = new ActionCommand((_) => { 
                Selected = SelectedState.NotSet;
                this.OnPropertyChanged(nameof(Selected));
                FilterMasterSelectGameViewModel.Instance.UpdateGamesCommand.Execute(this);
            });
        }

        private SelectedState selected = SelectedState.NotPossible;
        public SelectedState Selected
        {
            get => selected; set => SetValue(ref selected, value);
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public Func<Game, IEnumerable<Guid>> Validator { get; private set; }

        public enum SelectedState
        {
            NotSet,
            NotSelected,
            Maybe,
            Selected,
            NotPossible,
        }

        internal bool Validate(Game game)
        {
            if (game == null) return true;
            IEnumerable<Guid> ids = Validator(game);
            switch (Selected)
            {
                case SelectedState.Selected: return ids != null && ids.Contains(Id);
                case SelectedState.NotSelected: return ids == null || !ids.Contains(Id);
                default:
                    return true;
            }
        }

        internal void Sniff(Game game)
        {
            if (game == null) return;
            IEnumerable<Guid> ids = Validator(game);
            if (ids != null && Selected == SelectedState.NotPossible && ids.Contains(Id)) Selected = SelectedState.NotSet;
        }

        public ICommand ToggleSelectedCommand { get; set; }
        public ICommand ResetSelectedCommand { get; set; }
    }
}
