using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Interfaces
{
    public interface IGameServerCommunication
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="firstPartOfHash">concatenate with session_id and password</param>
        /// <returns></returns>
        string GetRequestOnServer(string origin, string request, string firstPartOfHash);
        string LastCommunicationError { get; }
    }
}
