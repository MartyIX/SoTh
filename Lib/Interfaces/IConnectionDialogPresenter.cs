using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model;

namespace Sokoban.Interfaces
{
    public interface IConnectionDialogPresenter
    {
        void Show(string ipAddress, int port, int leaguesID, int roundsID,
            IProfileRepository profileRepository, IErrorMessagesPresenter errorPresenter,
            IConnectionRelayer connectionRelayer, IGameMatch gameMatch);
    }
}
