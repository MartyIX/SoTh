using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sokoban.Model.Quests;
using System.Diagnostics;
using Sokoban.View;
using Sokoban.Interfaces;
using Sokoban.Lib;
using Sokoban.Model;

namespace QuestsControlTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ErrorMessagePresenter errorMessagePresenter = new ErrorMessagePresenter();   

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IQuestHandler iquestHandler = new QuestHandler();
            questsControlPane.Initialize(iquestHandler, errorMessagePresenter, null);
        }
    }


    public class ErrorMessagePresenter : IErrorMessagesPresenter
    {
        #region IErrorMessagesPresenter Members

        public void ErrorMessage(ErrorMessageSeverity ems, string originModule, string message)
        {
            Debug.WriteLine(string.Format(">> {0} Application error occured in module `{1}':\n >> Severity: {2}\n >> {3}\n",
                DateTime.Now.ToShortTimeString(), originModule, ems, message));
        }

        #endregion
    }

    public class QuestHandler : IQuestHandler
    {
        #region IQuestHandler Members

        IGameMatch IQuestHandler.QuestSelected(int leaguesID, int roundsID, Sokoban.Model.GameDesk.IQuest quest, GameMode gameMode)
        {
            Debug.WriteLine("Selected game mode: " + gameMode.ToString());
            Debug.WriteLine(quest.WholeQuestXml);
            return null;
        }

        #endregion
    }

}
