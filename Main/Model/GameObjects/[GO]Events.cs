using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban
{
    /// <summary>
    /// Object on game desk
    /// </summary>
    public partial class GameObject
    {
		#region Methods (2) 

		// Private Methods (2) 

        void GuardColumn()
        {
            if (gameDesk.IsObstructorOnPosition(this, View.GameDesk.MoveDirCordX(posX, this.direction),
                                                      View.GameDesk.MoveDirCordY(posY, this.direction), this.direction))
            {
                this.direction = (this.direction == MovementDirection.goUp) ? MovementDirection.goDown : MovementDirection.goUp;
            }

            model.MakePlan(this.model.Time + this.Speed, this, (EventType)direction);
            model.MakePlan(this.model.Time + this.Speed, this, EventType.guardColumn);
        }

        void GuardRow()
        {
            if (gameDesk.IsObstructorOnPosition(this, View.GameDesk.MoveDirCordX(posX, this.direction),
                                                      View.GameDesk.MoveDirCordY(posY, this.direction), this.direction))
            {
                this.direction = (this.direction == MovementDirection.goLeft) ? MovementDirection.goRight : MovementDirection.goLeft;
            }

            model.MakePlan(this.model.Time, this, (EventType)direction);
            model.MakePlan(this.model.Time + this.Speed + 1, this, EventType.guardRow);
        }

		#endregion Methods 
    }
}