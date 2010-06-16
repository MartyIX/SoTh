using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sokoban.View;
using Sokoban.Presenter;
using System.Windows;
using Sokoban.Model;
using Sokoban.View.ChooseConnection;

namespace Sokoban {
    
    /// <summary>
    /// List of views
    /// </summary>
    public sealed class NavigationController : INavigationController {

        private ChooseConnectionPresenter chooseConnectionPresenter;
        private ApplicationRepository applicationRepository;

        public NavigationController(ApplicationRepository applicationRepository)
        {
            this.applicationRepository = applicationRepository;
        }
        
        public void LoadView(string presenter, string view)
        {
            switch (presenter)
            {
                case "ChooseConnection":

                    if (chooseConnectionPresenter == null)
                    {
                        chooseConnectionPresenter = new ChooseConnectionPresenter(view, applicationRepository.profileRepository);
                    }
                    chooseConnectionPresenter.InitializeView(applicationRepository.MainWindow);

                    break;                
                
                default:
                    throw new Exception("Presenter '" + presenter + "' does not exist.");                    
            }
        }        
    }
}
