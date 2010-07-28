using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using Sokoban.Model.GameDesk;
using System.Collections;
using Sokoban.Lib;
using Sokoban.Lib.Events;
using Sokoban.Model.PluginInterface;
using System.ComponentModel;
using System.Windows;
using Sokoban.Interfaces;
using Sokoban.Lib.Exceptions;
using Sokoban.Solvers;      

namespace Sokoban.Model
{
    public partial class GameRepository : IBaseRepository, IGameRepository, IPluginParent, INotifyPropertyChanged, ISolverProvider
    {        

        // Game fields
        public Calendar calendar;
        public Int64 time;
        public IsPermitted<EventType> IsPermitted;
        public event GameChangeDelegate GameChanged;
        
        public string RoundName
        {
            get { return roundName; }
            set { roundName = value; Notify("RoundName"); }
        }

        public string GameVariantName
        {
            get;
            set;
        }

        public int StepsCount
        {
            get { return stepsCountGameObject.StepsCount; }
        }

        //
        // PRIVATE FIELDS
        //


        /// <summary>
        /// Objects on the gamedesk        
        /// </summary>
        private List<IGamePlugin> gameObjects;
        /// <summary>
        /// 
        /// </summary>
        public List<IMovableElement> movableElements;
        /// <summary>
        /// 
        /// </summary>
        public List<IControllableByUserInput> controllableByUserObjects;

        /// <summary>
        /// 
        /// </summary>        
        public IGamePlugin[,] fixedElements;
        /// <summary>
        /// 
        /// </summary>        
        public IGamePlugin[,] fixedTiles;

        public IGameVariant gameVariant = null;

        
        // Repository fields
        private TimeSpan dateTime;
        private int fieldsX;
        private int fieldsY;

        private string roundName;
        private IControllableByUserInput stepsCountGameObject = null;
        private PluginService pluginService;

        private GameMode gameMode;
        private GameDisplayType gameDisplayType;
        private INetworkService networkService;
        /// <summary>
        /// Without trailing backslash
        /// </summary>
        private static string appPath = null;

        //
        // Constructors
        // 

        public GameRepository(GameMode gameMode, GameDisplayType gameDisplayType, INetworkService networkService)
        {
            if (appPath == null) throw new InvalidStateException("Application path is not set in GameRepository module.");
            
            this.gameMode = gameMode;
            this.gameDisplayType = gameDisplayType;
            this.networkService = networkService;

            this.Initialize();
        }

        //
        // Methods
        //

        public void Initialize()
        {
            dateTime = DateTime.Now.TimeOfDay;
            calendar = new Calendar();
            ud = null;
            IsPermitted = new IsPermitted<EventType>();


            fieldsX = 10;
            fieldsY = 10;
            time = 0;

            pluginService = new PluginService(this);
        }
       
        public static void Initialize(string _appPath)
        {
            appPath = _appPath;
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
