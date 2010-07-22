using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AvalonDock;
using System.Windows;
using System.Windows.Controls;
using Sokoban.Lib;

namespace Sokoban.View
{
    public class GameManagerProperties : Panel
    {

        public static readonly DependencyProperty DockingManagerProperty;

        static GameManagerProperties()
        {
            PropertyChangedCallback dockingManagerChanged =
                new PropertyChangedCallback(OnDockingManagerChanged);
            PropertyMetadata dockingManagerMetadata =
                new PropertyMetadata(null, dockingManagerChanged);

            DockingManagerProperty = DependencyProperty.RegisterAttached("DockingManager",
                typeof(DockingManager), typeof(GameManagerProperties), dockingManagerMetadata);
        }

        public static DockingManager GetDockingManager(DependencyObject target)
        {
            return (DockingManager)target.GetValue(DockingManagerProperty);
        }

        public static void SetDockingManager(DependencyObject target, DockingManager value)
        {
            target.SetValue(DockingManagerProperty, value);
        }

        static void OnDockingManagerChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            DebuggerIX.WriteLine("[DockingManger]", "DockingManager just changed: " + e.NewValue);
        }
    }
}

