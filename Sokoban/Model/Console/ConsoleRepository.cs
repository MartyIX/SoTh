using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sokoban.Model
{
    public class ConsoleRepository : IBaseRepository
    {

        /* Singleton: private instance, private constructor and Instance method */
        private static readonly ConsoleRepository instance = new ConsoleRepository();

        private ConsoleRepository() { }

        public static ConsoleRepository Instance
        {
            get
            {
                return instance;
            }
        }

        public string ProcessCommand(string command)
        {            
            Debug.WriteLine("ProcessCommand: " + command);

            switch (command)
            {
                case "help":
                    return "TODO: write the help";
                default:
                    return "[Error]: Command not found";
            }
        }

        #region IBaseRepository Members

        public void Initialize()
        {
            
        }

        #endregion
    }
}
