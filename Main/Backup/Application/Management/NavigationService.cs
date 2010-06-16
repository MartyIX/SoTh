using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.View {
    public static class NavigationService {
        
        /// <summary>
        /// NavigationService calls NavigationController
        /// </summary>
        private static INavigationController instance;
        private static object navigationArgument;

        public static void AttachNavigator(INavigationController controller) {
            instance = controller;
        }

        /// <summary>
        /// Calls a view in NavigationnController (see NavigationController.cs)
        /// </summary>
        /// <param name="name"></param>
        public static void LoadView(string presenter, string view) {
            instance.LoadView(presenter, view);
        }

        public static void LoadView(string presenter, string name, object argument) {
            if (instance == null) {
                throw new Exception("The controller is not initialized.");
            }
            navigationArgument = argument;
            LoadView(presenter, name);
        }

        public static object Dto {
            get {
                return navigationArgument;
            }
        }

    }
}
