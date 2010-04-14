using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sokoban
{
    /// <summary>
    /// Object on game desk
    /// </summary>
    public partial class GameObject    
    {
        /// <summary>
        /// Moves GameObject on game desk
        /// </summary>
        /// <param name="whereTo">which direction should GameObject move </param>
        public void MakeMove(MovementDirection whereTo)
        {
            movementStopWatch = Stopwatch.StartNew();
            lastPosX = posX;
            lastPosY = posY;

            if (whereTo == MovementDirection.goLeft)
            {
                posX--;
            }
            else if (whereTo == MovementDirection.goRight)
            {
                posX++;
            }
            else if (whereTo == MovementDirection.goUp)
            {
                posY--;
            }
            else if (whereTo == MovementDirection.goDown)
            {
                posY++;
            }

            if (form.cbRecord.Checked == true)
            {
                string name;
                bool importantEvent = false;

                if (this.Description == "S")
                {
                    name = this.Description;
                    importantEvent = true;
                }
                else
                {
                    name = this.Description + this.objectID.ToString();
                }

                player.gameDesk.logList.AddEvent(this, this.posX, this.posY, form.lTime.Text, whereTo, importantEvent);
            }
        }

    }
}
