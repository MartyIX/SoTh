using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using Sokoban.View;

namespace Sokoban
{
    /// <summary>
    /// Main form
    /// </summary>
    public partial class GameDeskView : Form
    {
        /// <summary>
        /// Loads leagues at the beginning of the program.
        /// It sets cbLiga listbox.
        /// </summary>
        public void LoadInitialLeague()
        {
            LeagueChanged = true;

            string fileContent = null;
            // download list of leagues
            WebClient client = new WebClient();

            bool connectionEstablished = false;

            while (connectionEstablished == false)
            {
                try
                {
                    fileContent = client.DownloadString(BaseURI + LeaguesFileNameURI);
                    connectionEstablished = true;
                }
                catch (WebException e)
                {
                    MessageBox.Show("Error: " + e.Message);

                    // Form f = new ChooseConnectionView(/* formely: this  -- parameter */);

                    // f.ShowDialog() changes this.BaseURI and the connection to the server is 
                    // repeated thanks to while cycle
                    //if (f.ShowDialog() == DialogResult.Cancel)
                    //{
                    //    Environment.Exit(1);
                    //}
                }
            }

            // validate the list of leagues
            XmlValidator xmlValidator = new XmlValidator(Properties.Resources.sokobanLeagues, fileContent);

            if (!xmlValidator.IsValidXml)
            {
                MessageBox.Show(xmlValidator.ValidationError);
                Environment.Exit(1);
            }

            //xmlLeagues = new XmlLeagues(fileContent);
            //cbLeague.Items.AddRange(xmlLeagues.GetLeaguesNames());
            Random rnd = new Random();

            // load random League for starters; it calls LoadLeague - it's EVENT !!!
            /*
            IsPermittedToChangeLeague = false;
            cbLeague.SelectedIndex = rnd.Next(0, xmlLeagues.Count - 1);
            IsPermittedToChangeLeague = true;
            actLeagueXml = xmlLeagues.GetLeagueXML(cbLeague.SelectedIndex + 1);
            actLeagueName = (string)cbLeague.Items[cbLeague.SelectedIndex];
             */
        }

        /// <summary>
        /// Load a league XML to actLeagueXML
        /// </summary>
        /// <param name="league">League number; numbered from one</param>
        public void LoadLeague(int league)
        {
            player.round = 1;
            LeagueChanged = true;
            //actLeagueXml = xmlLeagues.GetLeagueXML(league);
            //actLeagueName = (string)cbLeague.Items[cbLeague.SelectedIndex];
        }
    }
}