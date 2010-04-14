
namespace Sokoban.Model.GameDesk
{

    /// <summary>
    /// Helper class for storing Cartesian coordinate
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// x-coordinate
        /// </summary>
        public int x;
        /// <summary>
        /// y-coordinate
        /// </summary>
        public int y;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}