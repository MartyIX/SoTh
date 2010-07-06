
namespace Sokoban.Lib
{

    public enum GameMode
    {
        SinglePlayer,
        TwoPlayers
    }
    
    public enum PlayingMode
    {
        League,
        Round
    }

    public enum EventCategory
    {
        goXXX = 0, // goLeft, goUp, goRight, goDown
        movingXXX, // movingLeft, movingUP,...
        movement
    }

    /// <summary>
    /// Possible moves on game desk
    /// </summary>
    public enum MovementDirection
    {
        /// <summary>
        /// Move left
        /// </summary>
        goLeft = 1,
        /// <summary>
        /// Move left
        /// </summary>
        goRight,
        /// <summary>
        /// Move up
        /// </summary>
        goUp,
        /// <summary>
        /// Move down
        /// </summary>
        goDown,
        /// <summary>
        /// No move
        /// </summary>
        none
    }

    /// <summary>
    /// Events that are possible to use in discrete simulation of round
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Object is about to go left
        /// </summary>
        goLeft = 1,   // 1
        /// <summary>
        /// Object is about to go right
        /// </summary>
        goRight,      // 2
        /// <summary>
        /// Object is about to go up
        /// </summary>
        goUp,         // 3
        /// <summary>
        /// Object is about to go down
        /// </summary>
        goDown,       // 4
        /// <summary>
        /// Nothing is about to happen
        /// </summary>
        none,         // 5
        /// <summary>
        /// Event for monsters (but can be used for other objects) which says that monster should pursuit Sokoban
        /// </summary>
        pursuit,      // 6
        /// <summary>
        /// Event for monsters. Monster moves in a row of game desk.
        /// </summary>
        guardRow,     // 7
        /// <summary>
        /// Event for monsters. Monster moves in a collumn of game desk.
        /// </summary>
        guardColumn,  // 8
        /// <summary>
        /// Event for Sokoban. Sokoban tried to go against wall.
        /// </summary>
        hitToTheWall, // 9
        /// <summary>
        /// System event for checking if game is over.
        /// </summary>
        checkIfIsEnd,  // 10
        /// <summary>
        /// Object is about to go left
        /// </summary>
        wentLeft,   // 11
        /// <summary>
        /// Object is about to go right
        /// </summary>
        wentRight,      // 12
        /// <summary>
        /// Object is about to go up
        /// </summary>
        wentUp,         // 13
        /// <summary>
        /// Object is about to go down
        /// </summary>
        wentDown,       // 14
        /// <summary>
        /// Sokoban met with a monster - end of game
        /// </summary>
        endLostGame,
        /// <summary>
        /// Changes orientation of Sokoban (maybe other object in future)
        /// </summary>
        setLeftOrientation,
        setRightOrientation,
        sokobanWasKilled,
        restartRound
    }
}