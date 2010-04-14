using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sokoban.Libs.http;

namespace Sokoban.Dialogs
{
    public partial class Charts : Form
    {
        GameDeskView form;

        /// <summary>
        /// Show the player chart for given league and round
        /// </summary>
        /// <param name="form">Reference to main form</param>
        /// <param name="user">Name of player</param>
        /// <param name="league">Name of league as is written in listOfLeagues.xml</param>
        /// <param name="round">Number of round</param>
        /// <param name="stepsNo">Number of steps player needed for finishing the round</param>
        /// <param name="time">The time player needed to finish the round</param>
        public Charts(GameDeskView form, string user, string league, int round, int stepsNo, int time)
        {
            InitializeComponent();

            this.form = form;

            // Send result
            savePlayerResult(user, league, round, stepsNo, time);

            // Show list of results
            webBrowser1.Url = new Uri(form.BaseURI + @"charts.php?league="+league+"&round="+round);
        }

        /// <summary>
        /// Saves result of player last game to the charts via web application at the URL: param_URL + "index.php"
        /// </summary>
        /// <param name="user">Name of player</param>
        /// <param name="league">Name of league as is written in listOfLeagues.xml</param>
        /// <param name="round">Number of round</param>
        /// <param name="stepsNo">Number of steps player needed for finishing the round</param>
        /// <param name="time">The time player needed to finish the round</param>
        private void savePlayerResult(string user, string league, int round, int stepsNo, int time)
        {
            HttpReq.Request(form.BaseURI + "charts.php", 
                "save_result=1&league=" + league + "&round=" + round + "&player=" + user + "&time=" + time + "&steps=" + stepsNo);
        }
    }
}