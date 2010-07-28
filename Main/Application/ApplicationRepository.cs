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
    using System.Threading;
    using System.Globalization;
    using Sokoban.View.Settings;
    using Sokoban.Model.Settings;
    using Sokoban.Configuration;
    using System.Reflection;
    using Sokoban.Solvers;

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
        public MainWindow MainWindow;

        /// <summary>
        /// If program is run as executable file then program finds the path itself
        /// </summary>
        public string appPath = System.Windows.Forms.Application.StartupPath;
        /// <summary>
        /// Game server with Sokoban's web application; 
        /// (note that URI has to end with trailing backslash)
        /// </summary>
        public string baseURI = @"http://sokoban.martinvseticka.eu/";

        public ApplicationParameters appParams;

        public ProfileRepository profileRepository;
        public SettingsRepository settingsRepository;

        ///
        /// VIEWS
        ///
        ChooseConnectionPresenter chooseConnectionPresenter;
        SettingsPresenter settingsPresenter;

        
        /// <summary>
        /// We don't want to initialize the application twice
        /// </summary>
        private bool alreadyStarted = false;

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        public static void Start(string[] args)
        {
            // Changes the CurrentCulture of the current thread to en-US.
            // We want double.ToString() in format 1.55 instead of 1,55; 
            // The app is in English so it is logical to have this culture set
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false); 
            
            // redirect console output to parent process;
            // must be before any calls to Console.WriteLine()
            //AttachConsole(ATTACH_PARENT_PROCESS);

            ApplicationRepository.Instance.profileRepository = ProfileRepository.Instance;
            ApplicationRepository.Instance.settingsRepository = SettingsRepository.Instance;

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
                //ApplicationRepository.Instance.OnStartUp();
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
                alreadyStarted = true;

                //
                // Load Splash
                //
                if (UserSettingsManagement.IsSplashEnabled)
                {
                    SplashScreen screen = new SplashScreen("View/Splash/SokobanSplash.png");
                    screen.Show(true);
                }

                // Paths initialization
                PluginService.Initialize(ApplicationRepository.AssemblyDirectory + @"\Plugins\");
                SolversManager.Initialize(ApplicationRepository.AssemblyDirectory + @"\Solvers\");
                GameRepository.Initialize(ApplicationRepository.AssemblyDirectory);

                // Console initialization
                ConsoleControl.Initialize(
                    UserSettingsManagement.ConsoleCommandPrefix,
                    UserSettingsManagement.ConsoleInitialText);
                    

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
                Debug.WriteLine("Application was already initialized!");
                //throw new Exception("Application was already initialized!");
            }
        }

        /// <summary>
        /// After the main window is rendered
        /// </summary>
        public void OnStartUp_PhaseTwo()
        {
            this.LoadViewChooseConnection();
        }


        /// <summary>
        /// After the user is logged in (or he/she skips the dialog window)
        /// </summary>
        public void OnStartUp_PhaseThree()
        {
            MainWindow.OnStartUp_PhaseThree();
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
            chooseConnectionPresenter.Closing += new Lib.VoidChangeDelegate(OnStartUp_PhaseThree);

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

        public SettingsPresenter LoadViewSettings()
        {
            if (settingsPresenter == null)
            {
                settingsPresenter = new SettingsPresenter(Instance.appParams.getView("Settings"), settingsRepository);
            }

            settingsPresenter.InitializeView(MainWindow);

            return settingsPresenter;
        }


        #region IBaseRepository Members

        public void Initialize()
        {
            
        }

        #endregion

        /// <summary>
        /// Returns path to the executing assembly without trailing "\"
        /// </summary>
        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path).TrimEnd(new char[] {'\\', '/'});
            }
        }
    }
}
