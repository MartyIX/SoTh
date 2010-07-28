using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AvalonDock;
using System.ComponentModel;
using Sokoban.Interfaces;

namespace Sokoban.View
{
    /// <summary>
    /// Interaction logic for ConsoleControl.xaml
    /// </summary>
    public partial class ConsoleControl : DockableContent, INotifyPropertyChanged, IErrorMessagesPresenter
    {
        private static string consoleCommandPrefix = "> ";
        private static string consoleInitialText = "";
        
        public ConsoleControl()
        {
            InitializeComponent();
            tbConsoleOut.AppendText(consoleInitialText + "\n");
        }

        private void appendText(string text)
        {            
            tbConsoleOut.Dispatcher.Invoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        tbConsoleOut.AppendText(text);
                    }
            ));
        }

        public static void Initialize(string _consoleCommandPrefix, string _consoleInitialText)
        {
            consoleCommandPrefix = _consoleCommandPrefix;
            consoleInitialText = _consoleInitialText;
        }

        private void tbConsoleIn_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.RunCommand(tbConsoleIn.Text);
                tbConsoleIn.Text = String.Empty;
            }
        }

        private void RunCommand(string command)
        {
            command = command.Trim().ToLower();

            this.appendText("--\n" + consoleCommandPrefix + command + "\n");

            if (command == "help")
            {
                this.appendText(">> Console is ment to be a tool for advanced users for debugging as \n" +
                                ">> more detailed error messages are being written here.\n\n" +
                                ">> Available commands so far: help\n");
            }
            else
            {
                this.appendText(">> Unknown command `" + command + "'. Available commands are listed in help.\n");
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;


        void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion


        #region IErrorMessagesPresenter Members

        public void ErrorMessage(ErrorMessageSeverity ems, string originModule, string message)
        {
            this.appendText( string.Format(">> {0} Application error occured in module `{1}':\n >> Severity: {2}\n >> {3}\n",
                DateTime.Now.ToShortTimeString(), originModule, ems, message));
        }
        
        #endregion
    }
}
