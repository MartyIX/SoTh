using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Interfaces;
using Sokoban.Networking;
using Sokoban.Model;
using Sokoban.Cryptography;

namespace Sokoban
{
    public partial class MainWindow : IGameServerCommunication
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="request"></param>
        /// <param name="firstPartOfHash">concatenate with session_id and password</param>
        /// <returns></returns>
        public string GetRequestOnServer(string origin, string request, string firstPartOfHash)
        {
            if (request.Contains("@session_id"))
            {
                request = request.Replace("@session_id", ProfileRepository.Instance.SessionID);
            }

            if (request.Contains("@hash"))
            {
                string hashedPassword = Hashing.CalculateSHA1(ProfileRepository.Instance.Password, Encoding.Default).ToLower();
                string toBeHashed = firstPartOfHash + ProfileRepository.Instance.SessionID + hashedPassword;
                string hashed = Hashing.CalculateSHA1(toBeHashed, Encoding.Default).ToLower();
                request = request.Replace("@hash", hashed);
            }            
            
            return ApplicationHttpReq.GetRequestOnServer(request, ProfileRepository.Instance, origin, consolePane /*errorPresenter*/);
        }

        public string LastCommunicationError
        {
            get { return ApplicationHttpReq.LastError; }
        }

    }
}
