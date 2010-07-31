using System;
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
using System.ComponentModel;
using Sokoban.Interfaces;
using Sokoban.Model;
using Sokoban.Lib;
using System.Collections.Generic;

namespace Sokoban.View
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class UserDialog : Window, INotifyPropertyChanged
    {
        public event VoidObjectStringStringDelegate Completed;

        // 
        // API
        //

        public string Message
        {
            get { return message; }
            set { message = value; Notify("Message"); }
        }


        //
        // Private fields
        //
        private string message = null;
        private IEnumerable<string> buttons;
        private string answer = null;
        private object sender = null;

        
        public UserDialog(string message, IEnumerable<string> buttons, object sender)
        {
            InitializeComponent();
            this.DataContext = this;

            this.buttons = buttons;
            this.message = message;
            this.sender = sender;
        }
        
        public void AppendMessage(string message)
        {
            Message = Message + "\n" + message;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            foreach (string text in buttons)
            {
                if (answer == null) answer = text;

                Button b = new Button();

                TextBlock tb = new TextBlock();
                tb.Margin = new Thickness(5,1,5,1);
                tb.Text = text;

                b.Content = tb;
                b.Click += new RoutedEventHandler(b_Click);
                buttonContainer.Children.Add(b);                
            }            
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Answer is set to the first one from Window_Loaded
            if (e.Key == Key.Enter)
            {
                this.Close();
            }
        }
        
        private void b_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            answer = ((TextBlock)b.Content).Text;
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Completed != null)
            {
                Completed(this.sender, this.message, this. answer);
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        #endregion

        
    }

}