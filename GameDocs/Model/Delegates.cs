using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Model.GameDesk
{
    public delegate void SimulationEventHandler(int eventID, int objectID, Int64 when, EventType what, int posX, int posY);
    public delegate void TimeDelegate(ref Int64 time);
    public delegate void SetSizeDelegate(int fieldsX, int fieldsY);
    public delegate void GameObjectsLoadedDelegate(List<GameObject> gameObjects);
    public delegate void SizeChangedDelegate(double width, double height, double fieldSize);
}
