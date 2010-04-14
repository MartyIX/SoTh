using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sokoban.View;
using Sokoban.Presenter;

namespace Sokoban {
    
    /// <summary>
    /// List of views
    /// </summary>
    public sealed class NavigationController : INavigationController {

        private ChooseConnectionPresenter chooseConnectionPresenter;
        public void LoadView(string presenter, string view)
        {
            switch (presenter)
            {
                case "ChooseConnection":

                    if (chooseConnectionPresenter == null)
                    {
                        chooseConnectionPresenter = new ChooseConnectionPresenter(view);
                    }
                    chooseConnectionPresenter.InitializeView();

                    break;                
                
                default:
                    throw new Exception("Presenter '" + presenter + "' does not exist.");                    
            }
        }        
    }
}
