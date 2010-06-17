using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.View;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using Sokoban.Lib.Http;
using System.Net;
using Sokoban.Application;
using System.IO;
using System.Windows;

namespace Sokoban.Model
{
    using Application = System.Windows.Forms.Application;
    using Sokoban.View.ChooseConnection;

    /// <summary>
    /// Singleton !
    /// </summary>
    public sealed class ApplicationRepository : IBaseRepository
    {
        /* Singleton: private instance, private constructor and Instance method */
        private static readonly ApplicationRepository instance = new ApplicationRepository();

        private ApplicationRepository() {}

        public static ApplicationRepository Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Main window reference
        /// </summary>
        public Window MainWindow;

        #region Fields (20)
        /// <summary>
        /// Name of actual league
        /// </summary>
        public string actLeagueName = "";
        /// <summary>
        /// XML file with rounds definitions
        /// </summary>                                                       
        public string actLeagueXml = "";
        /// <summary>
        /// If program is run as executable file then program finds the path itself
        /// </summary>
        public string appPath = System.Windows.Forms.Application.StartupPath;
        /// <summary>
        /// Game server with Sokoban's web application; 
        /// (note that URI has to end with trailing backslash)
        /// </summary>
        public string baseURI = @"http://sokoban.martinvseticka.eu/";
        /// <summary>
        /// This path is relevant only if program is run from Visual Studio
        /// </summary>
        public string DEBUG_PATH = @"D:\Skola\Rocnikovy projekt - specifikace\Program\Sokoban_source\Sokoban";
        /// <summary>
        /// Enables/disables refresh of XNA window
        /// </summary>
        public bool IsGraphicsChangeEnabled = true;
        /// <summary>
        /// Enable/disable initial displaying of login form
        /// </summary>
        public readonly bool IsLogInEnabled = false;
        /// <summary>
        /// Name of file where is list of leagues
        /// </summary>
        public readonly string LeaguesFileNameURI = @"listOfLeagues.xml";

        public ApplicationParameters appParams;

        public ProfileRepository profileRepository;

        ///
        /// VIEWS
        ///
        ChooseConnectionPresenter chooseConnectionPresenter;

        #endregion Fields

        /// <summary>
        /// We don't want to initialize the application twice
        /// </summary>
        private bool alreadyStarted = false;

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        public static void Start(string[] args)
        {
            // redirect console output to parent process;
            // must be before any calls to Console.WriteLine()
            AttachConsole(ATTACH_PARENT_PROCESS);

            ApplicationRepository.Instance.profileRepository = ProfileRepository.Instance;

            Instance.appParams = new ApplicationParameters();

            try
            {
                instance.appParams.ProcessParameters(args);
            }
            catch (ApplicationParametersException e)
            {
                System.Windows.Forms.MessageBox.Show("Parameters contain an error.\nError: " + e.Message, "Sokoban");
                Environment.Exit(2);
            }

            // Command-line regime
            if (args.Length > 0 && args[0] == "/cmd")
            {
                // Todo: 
                ApplicationRepository.Instance.OnStartUp();
            }
            else
            {                
                // launch the WPF application like usual                
                ApplicationRepository.Instance.MainWindow = new MainWindow();
                ApplicationRepository.Instance.MainWindow.ShowDialog();
            }
        }
        
        /// <summary>
        /// Initialization of application from the game perspective; initialization of MainForm is in the constructor
        /// </summary>
        public void OnStartUp()
        {
            if (alreadyStarted == false)
            {
                this.LoadViewChooseConnection();

                alreadyStarted = true;

                // Open windows from command-line
                if (Instance.appParams.OpenViewsOnStart != null)
                {
                    foreach (string window in Instance.appParams.OpenViewsOnStart)
                    {
                        //NavigationService.LoadView(window, Instance.appParams.getView(window));
                    }
                }
            }
            else
            {
                throw new Exception("Application was already initialized!");
            }
        }

        public static bool ContentFileExists(string file)
        {
            return File.Exists(ApplicationRepository.GetAppPath() + "/Content/" + file);
        }

        public static string GetAppPath()
        {
            if (File.Exists(Application.StartupPath + "/Sokoban.exe"))
            {
                return Application.StartupPath;
            }
            else
            {
                return @"D:\Skola\Rocnikovy projekt - specifikace\Program\SokobanMVP_source\ModelViewPresenter\bin\Release";
            }
        }

        public void Close()
        {
            Debug.WriteLine("Shutting down the whole application.");
        }

        public void RegisterMdiWindow(Window window)
        {
            window.Owner = this.MainWindow;
        }

        public static string GetHttpRequest(string file)
        {
            if (ProfileRepository.Instance.isUserAutenticated == false)
            {
                throw new ConnectionFailureException("User is not authenticated.");
            }
            else
            {
                string output;
                string fullUrl = ApplicationRepository.Instance.baseURI + "/" + file;

                try
                {
                    output = HttpReq.GetRequest(fullUrl);
                }
                catch (WebException e)
                {
                    throw new ConnectionFailureException("File `" + fullUrl + "` is unavailable. Exception message: " + e.Message);
                }
                return output;
            }
        }

        private void LoadViewChooseConnection()
        {
            chooseConnectionPresenter = new ChooseConnectionPresenter(Instance.appParams.getView("ChooseConnection"), profileRepository);


            if (appParams.credentials != null)
            {
                chooseConnectionPresenter.InitializeView_preFill(MainWindow,
                    appParams.credentials["server"],
                    appParams.credentials["username"],
                    appParams.credentials["password"]);
            }
            else
            {
                chooseConnectionPresenter.InitializeView(MainWindow);
            }                        
        }

        #region IBaseRepository Members

        public void Initialize()
        {
            
        }

        #endregion

    }
}
