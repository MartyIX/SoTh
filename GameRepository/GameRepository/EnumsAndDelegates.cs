#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using System.Diagnostics;
using Sokoban.Model.GameDesk;
#endregion usings

namespace Sokoban.Model.GameDesk
{
    public delegate void d_NetworkTimeChanged(Int64 time);
    public delegate void d_PlayerFinishedRound(Int64 time);
    public delegate void d_PlayerConnectionMessages(int messageID);
    public delegate void d_SceneEndedCallback();
    public delegate void d_ScenePlay();
    public delegate void d_StringDelegate(string msg);

    public enum PlayerType
    {
        Server,
        Client,
        NotDecided
    }

    /// <summary>
    /// Possible ends of round
    /// </summary>
    public enum RoundEnd
    {
        /// <summary>
        /// Sokoban was killed by a monster
        /// </summary>
        killedByMonster,

        /// <summary>
        /// User finished the round successfully
        /// </summary>
        taskFinished
    }

    /// <summary>
    /// States in which game may be
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Nothing is prepared for game
        /// </summary>
        NotLoaded = 0,
        /// <summary>
        /// Game is active
        /// </summary>
        Running,
        /// <summary>
        /// Game is loaded but user hasn't made a move yet
        /// </summary>
        BeforeFirstMove,
        /// <summary>
        /// Game is lost when a monster killed Sokoban
        /// </summary>
        Lost,
        /// <summary>
        /// Game is stopped
        /// </summary>
        Stopped,
        /// <summary>
        /// Game is paused
        /// </summary>
        Paused,
        /// <summary>
        /// Game is finished
        /// </summary>
        Finished
    }

    public enum GameType
    {
        SinglePlayer = 0,
        TwoPlayers
    }


    /// <summary>
    /// Events that are possible to use in discrete simulation of round
    /// </summary>
    public enum SokobanState
    {
        /// <summary>
        /// Object is about to go left
        /// </summary>
        alive = 1,   // 1
        dead = 2
    }

}