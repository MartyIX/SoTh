using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model;

namespace DummyObjectImplementations
{
    public class DummyProfileRepository : IProfileRepository
    {
        #region IProfileRepository Members
        public string username = "Marty";
        public string ipAddress = "127.0.0.1";
        public string password = "philips";

        public DummyProfileRepository()
        {

        }

        public bool TryLogin(string server, string username, string password)
        {
            throw new NotImplementedException();
        }

        public void SkipLogin()
        {
            throw new NotImplementedException();
        }

        public string LoginMessage
        {
            get { throw new NotImplementedException(); }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public List<string> GetServers()
        {
            throw new NotImplementedException();
        }

        public string Server
        {
            get { return "http://127.0.0.1/www/"; }
        }

        public string Username
        {
            get { return this.username; }
        }

        #endregion

        #region IBaseRepository Members

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IProfileRepository Members


        public string SessionID
        {
            get { return "4c3625ecb8796"; }
        }

        #endregion

        #region IProfileRepository Members


        public string Password
        {
            get { return this.password; }
        }

        #endregion

        #region IProfileRepository Members


        public string IPAddress
        {
            get
            {
                return this.ipAddress;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion        
    }
}

