using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;

namespace Sokoban.Solvers
{

    // Return codes (for all functions returning an integer)
    public enum SOKOBAN_PLUGIN_RESULT
    {
        OK = 0,
        SUCCESS = 0,
        CONSTRAINTSVIOLATION = 1,
        INVALID = 2,
        IMPOSSIBLE = 3,
        GIVEUP = 4,
        GAMETOOLONG = 5,
        INVALIDCONFIGURATION = 6,
        FAILURE = 7,
        TIMEOUT = 8,
        TERMINATED_BY_USER = 9
    }

    // Flags for PluginStatus.uiFlags
    public enum SOKOBAN_PLUGIN_FLAG
    {
        NONE = 0,
        UNSUCCESSFUL = 0,
        SOLUTION = 1,
        MOVES = 2,
        PUSHES = 4,
        BOX_LINES = 8,
        BOX_CHANGES = 16,
        PUSHING_SESSIONS = 32,
        SECONDARY_MOVES = 64,
        SECONDARY_PUSHES = 128,
        SECONDARY_BOX_LINES = 256,
        SECONDARY_BOX_CHANGES = 512,
        SECONDARY_PUSHING_SESSIONS = 1024
    }


    static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    public class SolverLibrary
    {
        //
        // Public part
        //

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct PluginStatus
        {
            public uint uiSize;
            public uint uiFlags;
            public Int64 i64MovesGenerated;
            public Int64 i64PushesGenerated;
            public Int64 i64StatesGenerated;
            public fixed char szStatusText[256];      // char[256] 
            public uint uiPluginTimeMS;      // [out] plugin running time, not necessarily identical to clock time
        }

        public enum StatusPriority
        {
            Debug,
            Warning,
            Error,
            FunctionStatusChange
        }

        public enum AlertCodes
        {
            TerminationIsSupportedButSolverIsNotRunning,
            TerminationIsNotSupported,
            TerminationInMoment,
            TerminationUnsuccesful,
            Other
        }

        /// <summary>
        /// Callback for customers using SolversLibrary. Callback is called when a function does something interesting.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameter">Mostly null but may be useful in future</param>
        public delegate void StatusCallbackDel(StatusPriority priority, AlertCodes alertCode, string message, object parameter);

        public event StatusCallbackDel StatusCallback;

        /// <summary>
        /// When methods SolveEx or Solve return status OK then this field contain the result of work one or the other method
        /// </summary>
        public string LastSolution { get { return lastSolution; } }

        //
        // Private part
        //

        private string lastSolution;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private delegate void GetPluginNameDel(StringBuilder pcString, uint uiStringBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void ConfigureDel(IntPtr hwndParent);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void ShowAboutDel(IntPtr hwndParent);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void GetConstraintsDel(out uint puiMaxWidth, out uint puiMaxHeight, out uint puiMaxBoxes);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int IsSupportedOptimizationDel(UInt32 uiOptimization);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int TerminateDel();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int PluginCallbackDel();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SolveExDel(UInt32 uiWidth, UInt32 uiHeight, StringBuilder pcBoard, StringBuilder pcSolution,
            UInt32 uiSolutionBufferSize, ref PluginStatus psStatus, IntPtr pc);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SolveDel(UInt32 uiWidth, UInt32 uiHeight, string[] pcBoard, StringBuilder pcSolution, 
            UInt32 uiSolutionBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int OptimizeDel(UInt32 uiWidth, UInt32 uiHeight, StringBuilder pcBoard, StringBuilder pcSolution,
            UInt32 uiMovesBufferSize, ref PluginStatus psStatus, IntPtr pc);

        const int solutionBufferSize = 10400;

        private IntPtr pDll;
        private bool libraryLoaded;
        private string path;
        private Window parentWindow;
        private bool isSolverRunning = false;        
        private StringBuilder solution;
        private PluginStatus psStatus;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Path to the dll file (absolute path is preffered)</param>
        /// <param name="parentWindow">A window instance</param>
        public SolverLibrary(string path, Window parentWindow)
        {
            this.path = path;
            this.libraryLoaded = false;
            this.parentWindow = parentWindow;
            this.pDll = NativeMethods.LoadLibrary(this.path);

            if ((uint)pDll > 32)
            {
                libraryLoaded = true;
            }
            else
            {
                /*
                 0  System was out of memory, executable file was corrupt, or
                    relocations were invalid.

                 2  File was not found.

                 3  Path was not found.

                 5  Attempt was made to dynamically link to a task, or there was a
                    sharing or network-protection error.

                 6  Library required separate data segments for each task.

                 8  There was insufficient memory to start the application.

                 10 Windows version was incorrect.

                 11 Executable file was invalid. Either it was not a Windows
                    application or there was an error in the .EXE image.

                 12 Application was designed for a different operating system.

                 13 Application was designed for MS-DOS 4.0.

                 14 Type of executable file was unknown.

                 15 Attempt was made to load a real-mode application (developed for
                    an earlier version of Windows).

                 16 Attempt was made to load a second instance of an executable file
                    containing multiple data segments that were not marked read-only.

                 19 Attempt was made to load a compressed executable file. The file
                    must be decompressed before it can be loaded.

                 20 Dynamic-link library (DLL) file was invalid. One of the DLLs
                    required to run this application was corrupt.

                 21 Application requires Microsoft Windows 32-bit extensions.
                */

                alertStatusMessage(StatusPriority.Debug, "Load library error code: " + (uint)this.pDll);
                alertStatusMessage(StatusPriority.Debug, "GetLastWin32Error: " + Marshal.GetLastWin32Error().ToString());
            }

            alertStatusMessage(StatusPriority.Debug, "Library '" + path + "' loaded");
        }

        /// <summary>
        /// Returns name of the plugin
        /// </summary>
        /// <returns>Name up to 255 characters.</returns>
        public uint[] GetConstraints()
        {
            string methodName = "GetConstraints";

            if (this.libraryLoaded == true)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(this.pDll, methodName);

                if (pAddressOfFunctionToCall != IntPtr.Zero)
                {
                    // PLUGIN NAME
                    GetConstraintsDel getConstraints = (GetConstraintsDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(GetConstraintsDel));

                    uint maxWidth, maxHeight, maxBoxes;
                    getConstraints(out maxWidth, out maxHeight, out maxBoxes);
                    alertStatusMessage(StatusPriority.Debug, string.Format("Plugin constraints | maxWidth: {0}, maxHeight: {1}, maxBoxes: {2}", maxWidth, maxHeight, maxBoxes));

                    return new uint[3] { maxWidth, maxHeight, maxBoxes };
                }
                else
                {
                    alertStatusMessage(StatusPriority.Debug, "Resolving function '" + methodName + "' error: " + Marshal.GetLastWin32Error().ToString());
                    throw new Exception(methodName + ": Cannot resolve function address.");
                }
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }
        }


        public string GetPluginName()
        {
            string methodName = "GetPluginName";
            if (this.libraryLoaded == true)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(this.pDll, methodName);

                if (pAddressOfFunctionToCall != IntPtr.Zero)
                {
                    // PLUGIN NAME
                    GetPluginNameDel getPluginName = (GetPluginNameDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(GetPluginNameDel));
                    StringBuilder sb = new StringBuilder(255);
                    uint buf = 255;

                    getPluginName(sb, buf);

                    alertStatusMessage(StatusPriority.Debug, "Plugin name: " + sb.ToString());
                    return sb.ToString();
                }
                else
                {
                    alertStatusMessage(StatusPriority.Debug, "Resolving function " + methodName + " error: " + Marshal.GetLastWin32Error().ToString());
                    alertStatusMessage(StatusPriority.Debug, "Trying deprecated equivalent GetSolverName");
                    return this.GetSolverName(); // may raise exception
                }
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }
        }

        /// <summary>
        /// Depreceated version of function GetPluginName
        /// </summary>
        /// <returns></returns>
        public string GetSolverName()
        {
            string methodName = "GetSolverName";
            if (this.libraryLoaded == true)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(this.pDll, methodName);

                if (pAddressOfFunctionToCall != IntPtr.Zero)
                {
                    // PLUGIN NAME
                    GetPluginNameDel getPluginName = (GetPluginNameDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(GetPluginNameDel));
                    StringBuilder sb = new StringBuilder(255);
                    uint buf = 255;

                    getPluginName(sb, buf);

                    alertStatusMessage(StatusPriority.Debug, "Plugin name: " + sb.ToString());
                    return sb.ToString();
                }
                else
                {
                    alertStatusMessage(StatusPriority.Debug, "Resolving function " + methodName + " error: " + 
                        Marshal.GetLastWin32Error().ToString());
                    throw new Exception(methodName + ": Cannot resolve function address.");
                }
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }
        }



        public bool IsSupportedOptimization(UInt32 uiOptimization)
        {
            string methodName = "IsSupportedOptimization";
            if (this.libraryLoaded == true)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(this.pDll, methodName);

                if (pAddressOfFunctionToCall != IntPtr.Zero)
                {
                    // PLUGIN NAME
                    IsSupportedOptimizationDel isSupportedOptimization = (IsSupportedOptimizationDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(IsSupportedOptimizationDel));

                    int result = isSupportedOptimization(uiOptimization);
                    bool retVal = (result == 0) ? false : true;

                    alertStatusMessage(StatusPriority.Debug, "IsSupportedOptimization: " + retVal.ToString());
                    return retVal;
                }
                else
                {
                    alertStatusMessage(StatusPriority.Debug, "Resolving function " + methodName + " error: " + Marshal.GetLastWin32Error().ToString());
                    throw new Exception(methodName + ": Cannot resolve function address.");
                }
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }
        }

        public void ShowAbout()
        {
            string methodName = "ShowAbout";
            if (this.libraryLoaded == true)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(pDll, methodName);

                if (pAddressOfFunctionToCall != IntPtr.Zero)
                {
                    // PLUGIN CONFIGURATION
                    ShowAboutDel showAbout = (ShowAboutDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(ShowAboutDel));

                    WindowInteropHelper wih = new WindowInteropHelper(this.parentWindow);

                    alertStatusMessage(StatusPriority.Debug, "ShowAbout started");
                    showAbout(wih.Handle);
                    alertStatusMessage(StatusPriority.Debug, "ShowAbout ended");
                }
                else
                {
                    alertStatusMessage(StatusPriority.Debug, "Resolving function '" + methodName + "' error: " + 
                        Marshal.GetLastWin32Error().ToString());
                    throw new Exception(methodName + ": Cannot resolve function address.");
                }
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }
        }


        /// <summary>
        /// Deprecated function; newer function is SolveEx
        /// </summary>
        /// <param name="mazeWidth"></param>
        /// <param name="mazeHeight"></param>
        /// <param name="inputBoard"></param>
        /// <returns></returns>
        public SOKOBAN_PLUGIN_RESULT Solve(uint mazeWidth, uint mazeHeight, string inputBoard)
        {
            SOKOBAN_PLUGIN_RESULT spr = SOKOBAN_PLUGIN_RESULT.FAILURE; // just default value; will be overwritten

            string methodName = "Solve";
            if (this.libraryLoaded == true)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(pDll, methodName);

                if (pAddressOfFunctionToCall != IntPtr.Zero)
                {
                    // PLUGIN CONFIGURATION
                    SolveDel solve = (SolveDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(SolveDel));

                    alertStatusMessage(StatusPriority.Debug, "Solve started");
                    alertStatusMessage(StatusPriority.FunctionStatusChange, "Solver is being initialized.");

                    string[] board = new string[mazeHeight];

                    for (int i = 0; i < mazeHeight; i++)
                    {
                        board[i] = inputBoard.Substring(i * (int)mazeWidth, (int)mazeWidth);
                    }

                    solution = new StringBuilder(solutionBufferSize);

                    isSolverRunning = true;
                    unsafe
                    {
                        int result = solve(mazeWidth, mazeHeight, board, solution, (uint)solutionBufferSize);

                        spr = (SOKOBAN_PLUGIN_RESULT)result;
                    }
                    isSolverRunning = false;

                    if (spr == SOKOBAN_PLUGIN_RESULT.OK)
                    {
                        alertStatusMessage(StatusPriority.FunctionStatusChange, "Solution is: " + solution.ToString());

                        if (psStatus.uiPluginTimeMS > 0)
                        {
                            alertStatusMessage(StatusPriority.FunctionStatusChange, "Finished in: " + psStatus.uiPluginTimeMS.ToString() + " miliseconds");
                        }

                        lastSolution = solution.ToString();
                    }
                    else
                    {
                        alertStatusMessage(StatusPriority.Debug, "Solve ended with status: " + spr.ToString());
                        alertStatusMessage(StatusPriority.FunctionStatusChange, "Solver ended with status: " + spr.ToString());
                    }
                }
                else
                {
                    alertStatusMessage(StatusPriority.Debug, "Resolving function '" + methodName + "' error: " + 
                        Marshal.GetLastWin32Error().ToString());
                    throw new Exception(methodName + ": Cannot resolve function address.");
                }
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }


            return spr;
        }


        public SOKOBAN_PLUGIN_RESULT SolveEx(uint mazeWidth, uint mazeHeight, string inputBoard)
        {
            SOKOBAN_PLUGIN_RESULT spr = SOKOBAN_PLUGIN_RESULT.FAILURE; // just default value; will be overwritten

            string methodName = "SolveEx";
            if (this.libraryLoaded == true)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(pDll, methodName);

                if (pAddressOfFunctionToCall != IntPtr.Zero)
                {
                    // PLUGIN CONFIGURATION
                    SolveExDel solveEx = (SolveExDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(SolveExDel));

                    alertStatusMessage(StatusPriority.Debug, "SolveEx started");
                    alertStatusMessage(StatusPriority.FunctionStatusChange, "Solver is being initialized.");

                    StringBuilder board = new StringBuilder(inputBoard);
                    solution = new StringBuilder(solutionBufferSize);
                    psStatus = new PluginStatus();

                    this.pluginCallback = new PluginCallbackDel(this.PluginCallback);
                    isSolverRunning = true;

                    unsafe
                    {
                        int result = solveEx(mazeWidth, mazeHeight, board, solution, (uint)solutionBufferSize,
                            ref psStatus,
                            Marshal.GetFunctionPointerForDelegate(this.pluginCallback));

                        spr = (SOKOBAN_PLUGIN_RESULT)result;
                    }

                    isSolverRunning = false;

                    if (spr == SOKOBAN_PLUGIN_RESULT.OK)
                    {
                        alertStatusMessage(StatusPriority.Debug, "Solution is: " + solution.ToString());

                        if (psStatus.uiPluginTimeMS > 0)
                        {
                            alertStatusMessage(StatusPriority.FunctionStatusChange, "Finished in: " + psStatus.uiPluginTimeMS.ToString());
                        }

                         lastSolution = solution.ToString();
                    }
                    else
                    {
                        alertStatusMessage(StatusPriority.Debug, "SolveEx ended with status: " + spr.ToString());
                        alertStatusMessage(StatusPriority.FunctionStatusChange, "Solver ended with status: " + spr.ToString());
                    }
                                       
                }
                else
                {
                    alertStatusMessage(StatusPriority.Debug, "Resolving function '" + methodName + "' error: " + 
                        Marshal.GetLastWin32Error().ToString());
                    alertStatusMessage(StatusPriority.Debug, "Trying deprecated function 'Solve'");
                    alertStatusMessage(StatusPriority.FunctionStatusChange, "Solver uses old API. Progress of the plugin can't be tracked.");
                    return Solve(mazeWidth, mazeHeight, inputBoard);
                }
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }


            return spr;
        }

        /// <summary>
        /// The PluginCallback function can be called by the plugin to notify the host program that the PluginStatus 
        /// structure has been updated. Not only in this case though.
        /// </summary>
        /// <returns></returns>
        public int PluginCallback()
        {
            return 1;
        }

        public bool Terminate()
        {
            bool wasTerminated = false;
            string methodName = "Terminate";

            if (this.libraryLoaded == true)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(pDll, methodName);

                if (pAddressOfFunctionToCall != IntPtr.Zero)
                {
                    // PLUGIN CONFIGURATION
                    TerminateDel terminate = (TerminateDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(TerminateDel));
                    alertStatusMessage(StatusPriority.Debug, "Terminate: ... is about to call the solver's method.");
                    int result = terminate();

                    if (isSolverRunning == false)
                    {
                        if (result == (int)SOKOBAN_PLUGIN_RESULT.OK)
                        {
                            alertStatusMessage(StatusPriority.FunctionStatusChange, AlertCodes.TerminationIsSupportedButSolverIsNotRunning, 
                                "Terminate solver: Termination is supported but solver is not solving right now.");
                        }
                        else
                        {
                            alertStatusMessage(StatusPriority.FunctionStatusChange, AlertCodes.TerminationIsNotSupported, 
                                "Terminate solver: Termination is not supported.");
                        }
                    }
                    else
                    {
                        if (result == (int)SOKOBAN_PLUGIN_RESULT.OK)
                        {
                            alertStatusMessage(StatusPriority.FunctionStatusChange, AlertCodes.TerminationInMoment, 
                                "Terminate solver: The solver has accepted the command and will return shortly.");
                            wasTerminated = true;
                        }
                        else
                        {
                            alertStatusMessage(StatusPriority.FunctionStatusChange, AlertCodes.TerminationUnsuccesful, 
                                "Terminate solver: The solver cannot terminate at the moment, or an attempt to terminate failed.");
                        }
                    }

                    alertStatusMessage(StatusPriority.Debug, "Terminate: Ended");
                }
                else
                {
                    Debug.WriteLine("- Resolving function '" + methodName + "' error: " + Marshal.GetLastWin32Error().ToString());
                    throw new Exception(methodName + ": Cannot resolve function address.");
                }

                return wasTerminated;
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }
        }


        /// <summary>
        /// Shows configuration dialog
        /// </summary>
        public void Configure()
        {
            string methodName = "Configure";
            if (this.libraryLoaded == true)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(pDll, methodName);

                if (pAddressOfFunctionToCall != IntPtr.Zero)
                {
                    // PLUGIN CONFIGURATION
                    ConfigureDel configure = (ConfigureDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(ConfigureDel));

                    WindowInteropHelper wih = new WindowInteropHelper(this.parentWindow);
                    //wih.Handle    

                    alertStatusMessage(StatusPriority.Debug, "Configuration started");
                    configure(wih.Handle);
                    alertStatusMessage(StatusPriority.Debug, "Configuration ended");
                }
                else
                {
                    alertStatusMessage(StatusPriority.Debug, "Resolving function '" + methodName + "' error: " + 
                        Marshal.GetLastWin32Error().ToString());
                    throw new Exception(methodName + ": Cannot resolve function address.");
                }
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }
        }

        /// <summary>
        /// Don't forget to call this method at the end of your work to dispose the resources.
        /// </summary>
        public void Unload()
        {
            if (this.libraryLoaded)
            {
                bool result = NativeMethods.FreeLibrary(this.pDll);
                alertStatusMessage(StatusPriority.Debug, "Unloaded: " + result.ToString());
            }
            else
            {
                alertStatusMessage(StatusPriority.Debug, "Unloaded: No. Library was not loaded at all.");
            }
        }

        public string GetLastSolution()
        {
            return lastSolution;
        }

        // Private methods

        private void alertStatusMessage(StatusPriority priority, string message)
        {
            alertStatusMessage(priority, AlertCodes.Other, message, null);
        }

        private void alertStatusMessage(StatusPriority priority, AlertCodes alertCode, string message)
        {
            alertStatusMessage(priority, alertCode, message, null);
        }

        private void alertStatusMessage(StatusPriority priority, AlertCodes alertCode, string message, object parameter)
        {
            if (StatusCallback != null)
            {
                StatusCallback(priority, alertCode, message, parameter);
            }
        }

        private PluginCallbackDel pluginCallback { get; set; }
    }
}
