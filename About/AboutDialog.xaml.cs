using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Sokoban.View.AboutDialog
{
	/// <summary>
	/// Interaction logic for ChooseConnectionWindow.xaml
	/// </summary>
	public partial class AboutDialog : Window
	{
		public AboutDialog()
		{
			InitializeComponent();

			this.DataContext = this;

		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			string navigateUri = homepageLink.NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(navigateUri));
			e.Handled = true;
		}
	}
}
