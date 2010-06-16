using System.IO;
using Sokoban.Lib.Http;
using System;
using System.Net;
using System.ComponentModel;
using Sokoban.Configuration;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Configuration;
using Sokoban.Lib;


namespace Sokoban.Model
{
    public sealed class ProfileRepository : IBaseRepository, INotifyPropertyChanged
    {
        /* Singleton: private instance, private constructor and Instance method */
        private static readonly ProfileRepository instance = new ProfileRepository();
        private ProfileRepository() { }

        public static ProfileRepository Instance
        {
            get
            {
                return instance;
            }
        }

        #region Fields

        /// <summary>
        /// tells if user is logged in (may be anonymous user)
        /// </summary>
        public bool isUserAutenticated = false;

        private string server = "";
        /// <summary>
        /// Player's name
        /// </summary>
        public string Server
        {
            get
            {
                if (isUserAutenticated == true)
                {
                    return server;
                }
                else
                {
                    throw new Exception("User is not authenticated.");
                }
            }
            set { server = value; }
        }


        private string username = "";

        /// <summary>
        /// Player's name
        /// </summary>
        public string Username
        {
            get
            {
                if (isUserAutenticated == true)
                {
                    return username;
                }
                else
                {
                    return "Anonymous";
                }
            }
            set 
            { 
                username = value;                
                Notify("Username");
            }
        }


        /// <summary>
        /// We don't want let user to fill key buffer before inicialization of game
        /// </summary>
        public bool userCanPressArrows = false;

        private string loginMessage = "";

        public string LoginMessage
        {
            get { return loginMessage; }
            set { loginMessage = value; Notify("LoginMessage"); }
        }
        
        #endregion

        
        
        #region Methods
        /// <summary>
        /// User has settings saved at server and they're loaded at the start of the program.
        /// </summary>
        public void SetUserSettings()
        {
            // TODO: either remove or implement
        }

        /// <summary>
        /// Authorization of the player; possible to use "Anonymyous" account otherwise server authorization
        /// </summary>
        /// <returns></returns>
        public string getUserName()
        {
            // TODO: either remove or implement
            
            /*Form frmDialog = new Dialogs.LoggingIn(form, player);
            frmDialog.ShowDialog();

            return user;*/

            return "";
        }

        #endregion

        #region IBaseRepository Members

        public void Initialize()
        {
            
        }

        #endregion

        /// <summary>
        /// Attempt to login user to the remote server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>True if user has been logged in; false otherwise</returns>
        public bool TryLogin(string server, string username, string password)
        {
            // TODO: improve                          
            string postData = "name=" + username + "&pass=" + password;
            string url = server.TrimEnd(new char[] { '/' }) + "/login.php";
            string output;

            try
            {
                output = HttpReq.Request(url, postData);
            }
            catch (WebException e)
            {
                output = "error";
                this.LoginMessage = "Error in communication with the server. Please try again in a while.\n" +
                                    "Additional information: " + e.Message;
            }
            catch (Exception e)
            {
                output = "error";
                this.LoginMessage = "Unknow error occured.\n" +
                                    "Additional information: " + e.Message;
            }

            if (output == "1")
            {
                this.LoginMessage = "Login was successful.";
                this.isUserAutenticated = true;
                this.Username = username;
                this.Server = server;

                return true;
            }
            else if (output == "error")
            {
                this.isUserAutenticated = false;
                return false;
            }
            else
            {
                this.isUserAutenticated = false;
                this.LoginMessage = "Username or password is incorrect.";
                return false;
            }
        }

        public void SkipLogin()
        {
            this.LoginMessage = "Login was skipped.";
            this.isUserAutenticated = false;
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
    }
}