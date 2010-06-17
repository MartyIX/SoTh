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
        private delegate int OptimizeDel(UInt32 uiWidth, UInt32 uiHeight, StringBuilder pcBoard, StringBuilder pcSolution,
            UInt32 uiMovesBufferSize, ref PluginStatus psStatus, IntPtr pc);

        const int solutionBufferSize = 10400;

        private IntPtr pDll;
        private bool libraryLoaded;
        private string path;
        private Window parentWindow;
        private bool isSolverRunning = false;
        private StringBuilder board;
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

            if (pDll != IntPtr.Zero)
            {
                libraryLoaded = true;
            }
            else
            {
                Debug.WriteLine("- Load library error: " + Marshal.GetLastWin32Error().ToString());
            }

            Debug.WriteLine("- Done");
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
                    //UIntPtr[] parameters = new UIntPtr[3];
                    //parameters[0] = parameters[1] = parameters[2] = UIntPtr.Zero;

                    // PLUGIN NAME
                    GetConstraintsDel getConstraints = (GetConstraintsDel)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(GetConstraintsDel));

                    uint maxWidth, maxHeight, maxBoxes;

                    getConstraints(out maxWidth, out maxHeight, out maxBoxes);

                    Debug.WriteLine(string.Format("- Plugin constraints | maxWidth: {0}, maxHeight: {1}, maxBoxes: {2}", maxWidth, maxHeight, maxBoxes));

                    return new uint[3] { maxWidth, maxHeight, maxBoxes };
                }
                else
                {
                    Debug.WriteLine("- Resolving function '" + methodName + "' error: " + Marshal.GetLastWin32Error().ToString());
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

                    Debug.WriteLine("- Plugin name: " + sb.ToString());
                    return sb.ToString();
                }
                else
                {
                    Debug.WriteLine("- Resolving function " + methodName + " error: " + Marshal.GetLastWin32Error().ToString());
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

                    Debug.WriteLine("- IsSupportedOptimization: " + retVal.ToString());
                    return retVal;
                }
                else
                {
                    Debug.WriteLine("- Resolving function " + methodName + " error: " + Marshal.GetLastWin32Error().ToString());
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

                    Debug.WriteLine("- ShowAbout started");
                    showAbout(wih.Handle);
                    Debug.WriteLine("- ShowAbout ended");
                }
                else
                {
                    Debug.WriteLine("- Resolving function '" + methodName + "' error: " + Marshal.GetLastWin32Error().ToString());
                    throw new Exception(methodName + ": Cannot resolve function address.");
                }
            }
            else
            {
                throw new Exception(methodName + ": Solver library is not loaded.");
            }
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

                    Debug.WriteLine("- SolveEx started");

                    board = new StringBuilder(inputBoard);
                    solution = new StringBuilder(solutionBufferSize);
                    psStatus = new PluginStatus();

                    unsafe
                    {
                        int result = solveEx(mazeWidth, mazeHeight, board, solution, (uint)solutionBufferSize,
                            ref psStatus,
                            Marshal.GetFunctionPointerForDelegate(new PluginCallbackDel(this.PluginCallback)));

                        spr = (SOKOBAN_PLUGIN_RESULT)result;
                    }

                    if (spr == SOKOBAN_PLUGIN_RESULT.OK)
                    {
                        Debug.WriteLine("-- Solution is: " + solution.ToString());

                        if (psStatus.uiPluginTimeMS > 0)
                        {
                            Debug.WriteLine("-- Finished in: " + psStatus.uiPluginTimeMS.ToString());
                        }
                    }

                    Debug.WriteLine("- SolveEx ended with status: " + spr.ToString());
                }
                else
                {
                    Debug.WriteLine("- Resolving function '" + methodName + "' error: " + Marshal.GetLastWin32Error().ToString());
                    throw new Exception(methodName + ": Cannot resolve function address.");
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
                    Debug.WriteLine("- Terminate: ... is about to call the solver's method.");
                    int result = terminate();

                    if (isSolverRunning == false)
                    {
                        if (result == (int)SOKOBAN_PLUGIN_RESULT.OK)
                        {
                            Debug.WriteLine("- Terminate: Termination is supported.");
                        }
                        else
                        {
                            Debug.WriteLine("- Terminate: Termination is not supported.");
                        }
                    }
                    else
                    {
                        if (result == (int)SOKOBAN_PLUGIN_RESULT.OK)
                        {
                            Debug.WriteLine("- Terminate: The solver has accepted the command and will return shortly.");
                            wasTerminated = true;
                        }
                        else
                        {
                            Debug.WriteLine("- Terminate: The solver cannot terminate at the moment, or an attempt to terminate failed.");
                        }
                    }

                    Debug.WriteLine("- Terminate: Ended");
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

                    Debug.WriteLine("- Configuration started");
                    configure(wih.Handle);
                    Debug.WriteLine("- Configuration ended");
                }
                else
                {
                    Debug.WriteLine("- Resolving function '" + methodName + "' error: " + Marshal.GetLastWin32Error().ToString());
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
                Debug.WriteLine("- Unloaded: " + result.ToString());
            }
            else
            {
                Debug.WriteLine("- Unloaded: No. Library was not loaded at all.");
            }
        }
    }
}
