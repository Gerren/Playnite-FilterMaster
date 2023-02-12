using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterMaster.Models
{
    internal interface IGameVisibilityDeterminer
    {
        bool IsVisible(Game game);
    }
}
