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

namespace QuestsControlTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            QuestsControl.Initialize("http://127.0.0.1/");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IQuestHandler iquestHandler = new QuestHandler();

            questsControlPane.Initialize(iquestHandler);
        }
    }

    public class QuestHandler : IQuestHandler
    {


        #region IQuestHandler Members

        public void QuestSelected(Sokoban.Model.GameDesk.IQuest quest)
        {
            Debug.WriteLine(quest.WholeQuestXml);
        }

        #endregion
    }

}
