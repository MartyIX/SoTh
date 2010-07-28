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
using Sokoban.Model;
using Sokoban.Networking;
using Sokoban.Xml;

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
		private IQuestHandler questHandler = null;
		private IErrorMessagesPresenter errorPresenter = null;
		private IProfileRepository profile = null;
		private TreeViewItem selected = null;

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
		/// 
		/// </summary>
		public void Initialize(IQuestHandler questHandler, IErrorMessagesPresenter errorPresenter, IProfileRepository profile)
		{
			this.profile = profile;
			this.questHandler = questHandler;
			this.errorPresenter = errorPresenter;
			this.Refresh();
		}

		private void refresh_click(object sender, RoutedEventArgs e)
		{
			this.Refresh();
		}

		public void Refresh()
		{
			this.Status = "Connecting to the server";

			// it correctly displays the error
			string output = null;
			try
			{
				output = this.getRequestOnServer("/remote/GetInitLeagues/");
			}
			catch (UninitializedException e)
			{
				this.Status = e.Message;
				output = null;
			}


			if (output != "error" && output != null)
			{
				InitialLeagues response = new InitialLeagues();

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
			string output = ApplicationHttpReq.GetRequestOnServer(request, profile, "Quests", errorPresenter);

			if (ApplicationHttpReq.LastError != String.Empty)
			{
				this.Status = ApplicationHttpReq.LastError;
			}

			return output;
		}

		private void trv_CategoryMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (selected == e.Source)
			{
				TreeViewItem tvi = sender as TreeViewItem;
				Category category = tvi.Header as Category;

				if (category.Loaded == false)
				{
					string xml = this.getRequestOnServer("/remote/GetCategoryLeagues/" + category.ID);

					if (xml != "error" && xml != "")
					{
						GetCategoryLeagues response = new GetCategoryLeagues();

						bool success = true;

						try
						{
							response.Parse(xml);
							category.Leagues = response.Leagues;
						}
						catch (InvalidStateException ex)
						{
							this.Status = ex.Message;
							success = false;
						}                   
						
						if (success)
						{
							category.Loaded = true;                                 
							this.Status = category.Leagues.Count + " leagues added.";							
						}
					}
					else
					{
						errorPresenter.ErrorMessage(ErrorMessageSeverity.Medium, "Quests", "Server returned empty response. The problem is propably at server-side.");
						MessageBox.Show("The category cannot be loaded. A problem is probably on the side of server.");
					}
				}
			}
		}

		

		private void trv_LeaguesMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (selected == e.Source)
			{
				TreeViewItem tvi = sender as TreeViewItem;
				League league = tvi.Header as League;

                if (league.Loaded == true)
                {
                    openGame(league, null, GameMode.SinglePlayer);
                }
                else
                {
                    string xml = this.getRequestOnServer("/remote/GetLeagueRounds/" + league.ID);

                    if (xml != "error" && xml != "")
                    {
                        GetLeagueRounds response = new GetLeagueRounds();

                        bool success = true;

                        try
                        {
                            response.Parse(xml);
                            league.Rounds = response.Rounds;
                        }
                        catch (InvalidStateException ex)
                        {
                            this.Status = ex.Message;
                            success = false;
                        }

                        if (success)
                        {
                            league.Loaded = true;
                            this.Status = league.Rounds.Count + " rounds added.";
                        }
                    }
                    else
                    {
                        errorPresenter.ErrorMessage(ErrorMessageSeverity.Medium, "Quests", "Server returned empty response. The problem is propably at server-side.");
                        MessageBox.Show("The league cannot be loaded. A problem is probably on the side of server.");
                    }
                }
			}
		}

		private void trv_RoundMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (selected == e.Source)
			{
				TreeViewItem tvi = sender as TreeViewItem;
				playRound(tvi, GameMode.SinglePlayer);
			}
		}

		private void trv_Selected(object sender, RoutedEventArgs e)
		{
			if (e.Source is TreeViewItem)
			{
				selected = e.Source as TreeViewItem;
			}
		} 

		private void cmPlayLeague_Click(object sender, RoutedEventArgs e)
		{
			MenuItem menu = sender as MenuItem;
			TreeViewItem tvi = menu.DataContext as TreeViewItem;
			League league = tvi.Header as League;

			openGame(league, null, GameMode.SinglePlayer);
		}

		private void playRound(TreeViewItem tvi, GameMode gameMode)
		{			            
			ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(tvi);
			League league = ((TreeViewItem)parent).Header as League;

			if (league == null)
			{
				MessageBox.Show("League cannot be run. Data are corrupted.");
			}
			else
			{
				Round round = tvi.Header as Round;

				if (round == null)
				{
					MessageBox.Show("Round cannot be run. Data are corrupted.");
				}
				else
				{
					openGame(league, round, gameMode);
				}
			} 
		}

		private void cmPlayRound_Click(object sender, RoutedEventArgs e)
		{
			MenuItem menu = sender as MenuItem;
			TreeViewItem tvi = menu.DataContext as TreeViewItem;

			playRound(tvi, GameMode.SinglePlayer);
		}

		private void cmPlayOverNetwork_Click(object sender, RoutedEventArgs e)
		{
			MenuItem menu = sender as MenuItem;
			TreeViewItem tvi = menu.DataContext as TreeViewItem;
			playRound(tvi, GameMode.TwoPlayers);
		}

		private void openGame(League league, Round round, GameMode gameMode)
		{
			int roundID = (round == null) ? -1: round.ID;
			int leagueID = (league == null) ? -1 : league.ID;
			OpenGame(leagueID, roundID, gameMode);
		}

		public void OpenGame(int leagueID, int roundID, GameMode gameMode)
		{
			if (questHandler != null)
			{
				string questXml = this.getRequestOnServer("/remote/GetLeague/" + leagueID.ToString());

				if (questXml != "error" && questXml != "")
				{
					Quest q = new Quest(questXml);
					questHandler.QuestSelected(leagueID, roundID, q, gameMode);
				}
				else
				{
					errorPresenter.ErrorMessage(ErrorMessageSeverity.Medium, "Quests", "Server returned empty response. The problem is propably at server-side.");
					MessageBox.Show("The league cannot be opened. A problem is probably on the side of server.");
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
