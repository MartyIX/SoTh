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
using Sokoban.Lib;

namespace XMLSchemaTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DebuggerIX.Start(DebuggerMode.OutputWindow);

            string instance = XMLSchemaTest.Properties.Resources.TestQuest;
            string schema1 = XMLSchemaTest.Properties.Resources.QuestSchema;
            string schema2 = XMLSchemaTest.Properties.Resources.PluginSchema;
            XmlValidator validator = new XmlValidator();

            validator.AddSchema(null, schema1);
            validator.AddSchema(null, schema2);
            bool isValid = validator.IsValid(instance);
            
            textBlock.Text = "Scheme is: " + (isValid ? "Valid" : "Not valid! Error: " + validator.GetErrorMessage());

            DebuggerIX.Close();
        }
    }
}
