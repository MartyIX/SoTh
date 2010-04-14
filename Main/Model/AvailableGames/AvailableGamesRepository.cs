using System.Windows.Forms;
using System.IO;
using Sokoban.Model;
using System;
using System.Net;
using System.Collections.Generic;
using Sokoban.Lib;


namespace Sokoban
{
    public sealed class AvailableGamesRepository : IBaseRepository
    {
        /* Singleton: private instance, private constructor and Instance method */
        private static readonly AvailableGamesRepository instance = new AvailableGamesRepository();
        private AvailableGamesRepository() { }
        private XmlLeagues leagues;

        public static AvailableGamesRepository Instance
        {
            get
            {
                return instance;
            }
        }


        public void Initialize()
        {
        
        }

        /// <summary>
        /// Returns leagues names. The method may end up with exception: ConnectionFailureException
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetLeagues()
        {
            string fileContent;
            fileContent = ApplicationRepository.GetHttpRequest("listOfLeagues.xml");
            return XmlLeagues.GetLeaguesNames(fileContent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetLeagueRoundsNames(string leagueID)
        {
            string fileContent;
            fileContent = ApplicationRepository.GetHttpRequest("round-01.xml");
            return XmlLeagues.GetLeagueRoundsNames(fileContent);
        }

    }
}