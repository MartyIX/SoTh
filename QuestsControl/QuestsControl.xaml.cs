using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using AvalonDock;
using System.Diagnostics;
using Sokoban.View;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Sokoban.Model.GameDesk;
using Sokoban.Lib;
using System.Data;
using Sokoban.Lib.Http;
using System.Net;
using Sokoban.Lib.Exceptions;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using Sokoban.Model.Quests;
using Sokoban.Interfaces;

namespace Sokoban.View
{
    public partial class QuestsControl : DockableContent, INotifyPropertyChanged
    {
        //
        // API
        //
        public string Status {
            get { return _status; }
            set { _status = value; Notify("Status"); }
        }
        private Categories categories;
        public Categories Categories { get { return categories; } }



        //
        // Private fields
        //
        private string _status;
        private static string server;
        private IQuestHandler questHandler = null;
        private IErrorMessagesPresenter errorPresenter = null;
		
        /// <summary>
        /// Shutdown
        /// </summary>
        public void Terminate()
        {
        }

        public QuestsControl()
        {
            InitializeComponent();
            this.DataContext = this;            
        }

        /// <summary>
        /// Should be initialized before creating instance of object
        /// </summary>
        /// <param name="_server"></param>
        public static void Initialize(string _server)
        {
            server = _server;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(IQuestHandler questHandler, IErrorMessagesPresenter errorPresenter)
        {
            this.questHandler = questHandler;
            this.errorPresenter = errorPresenter;
            this.refresh();
        }

        private void refresh_click(object sender, RoutedEventArgs e)
        {
            this.refresh();
        }

        private void refresh()
        {
            this.Status = "Connecting to the server";

            // it correctly displays the error
            string output = this.getRequestOnServer("/remote/GetInitLeagues/");

            if (output != "error")
            {
                QuestsXmlInitialLeagues response = new QuestsXmlInitialLeagues();

                try
                {
                    response.Parse(output);
                    this.Status = "Initial leagues loaded.";
                }
                catch (InvalidStateException e)
                {
                    this.Status = e.Message;
                }

                this.categories = response.Categories;
                Notify("Categories");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">i.e., "/remote/GetInitLeagues/"</param>
        /// <returns>Response of the server</returns>
        private string getRequestOnServer(string request)
        {
            if (server == null) throw new UninitializedException("Server name was not initialized.");
            
            string url = server.TrimEnd(new char[] { '/' }) + request;
            string output;

            try
            {
                output = HttpReq.GetRequest(url, "");
            }
            catch (WebException e)
            {
                output = "error";
                this.Status = "Error in communication with the server. Please try again in a while.";
                errorMessage(ErrorMessageSeverity.Medium, "Error in communication with the server. Additional information: " + e.Message);
            }
            catch (Exception e)
            {
                output = "error";
                this.Status = "Unknown error occured. Please try again in a while.";
                errorMessage(ErrorMessageSeverity.High, "Unknown error in communication with the server. Additional information: " + e.Message);
            }

            return output;
        }

        private void trv_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tvi = sender as TreeViewItem;
            League league = tvi.Header as League;

            openLeague(league, GameMode.SinglePlayer);
        }

        private void cmPlayLeague_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            TreeViewItem tvi = menu.DataContext as TreeViewItem;
            League league = tvi.Header as League;

            openLeague(league, GameMode.SinglePlayer);
        }

        private void cmPlayOverNetwork_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            TreeViewItem tvi = menu.DataContext as TreeViewItem;
            League league = tvi.Header as League;

            openLeague(league, GameMode.TwoPlayers);
        }


        private void openLeague(League league, GameMode gameMode)
        {            
            if (questHandler != null)
            {
                int id = league.ID;

                string questXml = this.getRequestOnServer("/remote/GetLeague/" + id.ToString());

                if (questXml != "error" && questXml != "")
                {
                    Quest q = new Quest(questXml);
                    questHandler.QuestSelected(q, gameMode);
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion

        private void errorMessage(ErrorMessageSeverity ems, string message)
        {
            if (errorPresenter != null)
            {
                errorPresenter.ErrorMessage(ems, "Quests", message);
            }
        }

    }
}
