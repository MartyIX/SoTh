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
    public partial class LoggingIn : Form
    {
        private GameDeskView GameDeskView;
        private Player player;
        private bool isNameSet = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="form">Reference to main dialog</param>
        public LoggingIn(GameDeskView form, Player player)
        {
            InitializeComponent();
            GameDeskView = form;
            this.player = player;
        }

        private bool LogIn(string name, string pass)
        {            
            string output = HttpReq.Request(GameDeskView.BaseURI + "login.php", "name=" + name + "&pass=" + pass);

            if (output == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            if (tJmeno.Text == "")
            {
                lStatus.Text = "Username is empty.";
            }
            else
            {
                if ( (tJmeno.Text == "Anonymous" && tPassword.Text == "") || LogIn(tJmeno.Text, tPassword.Text))
                {
                    player.profile.User = tJmeno.Text;
                    player.profile.SetUserSettings();
                    isNameSet = true;
                    Close();
                }
                else
                {
                    lStatus.Text = "Combination of username and password is incorrect.";
                }
            }
        }

        private void Fdialog_Load(object sender, EventArgs e)
        {
            lStatus.Text = "Input username and password.";

            if (GameDeskView.IsLogInEnabled == false)
            {
                player.profile.User = "Anonymous";
                Close();
            }

        }

        private void LogInDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isNameSet)
            {
                player.profile.User = "Anonymous";
            }
        }
    }
}
