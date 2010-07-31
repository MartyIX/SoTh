using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Interfaces;
using Sokoban.Networking;
using Sokoban.Lib.Exceptions;

namespace Sokoban.View.GameDocsComponents
{
    public partial class GameDeskControl : IGameMatch
    {
        #region IGameMatch Members

        public void SetNetworkConnection(IConnection connection)
        {
            if (this.networkModule != null)
            {                
                this.networkModule.SetNetworkConnection(connection);
                this.removeBlurEffect();
            }
            else
            {
                throw new InvalidStateException("NetworkModule is not initalized.");
            }
        }

        #endregion
    }
}
