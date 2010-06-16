using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using System.Windows.Input;

namespace Sokoban.Lib.Events
{
    public class EventTypeLib
    {
        public static EventType ConvertFromKey(Key key)
        {
            if (key == Key.Left)
            {
                return EventType.goLeft;
            }
            else if (key == Key.Right)
            {
                return EventType.goRight;
            }
            else if (key == Key.Up)
            {
                return EventType.goUp;
            }
            else if (key == Key.Down)
            {
                return EventType.goDown;
            }
            else
            {
                return EventType.none;
            }
        }

        public static bool IsEventOfType(EventType ev, EventCategory category)
        {
            if ((category == EventCategory.goXXX &&
                (
                  ev == EventType.goDown || ev == EventType.goUp ||
                  ev == EventType.goLeft || ev == EventType.goRight)
                )
                ||
                (category == EventCategory.movingXXX &&
                (
                  ev == EventType.wentDown || ev == EventType.wentUp ||
                  ev == EventType.wentLeft || ev == EventType.wentRight)
                )
                ||
                (category == EventCategory.movement &&
                (
                  EventTypeLib.IsEventOfType(ev, EventCategory.goXXX) || 
                  EventTypeLib.IsEventOfType(ev, EventCategory.movingXXX))
                )
               )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
