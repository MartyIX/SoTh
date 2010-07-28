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
using System.Windows.Shapes;
using Sokoban.Presenter;
using Sokoban.Model;
using System.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Sokoban.View.ChooseConnection
{
	/// <summary>
	/// Interaction logic for ChooseConnectionWindow.xaml
	/// </summary>
	public partial class ChooseConnectionDialog : Window, IChooseConnectionView, INotifyPropertyChanged
	{
		private IProfileRepository model;
		public ChooseConnectionPresenter presenter;
		private ObservableCollection<string> gameServers;
		public ObservableCollection<string> GameServers { get { return gameServers; } }

		private string username;
		private string password;

		public ChooseConnectionDialog(ChooseConnectionPresenter presenter, IProfileRepository model)
		{
			InitializeComponent();

			this.DataContext = this;
			tbStatus.DataContext = model;
			this.presenter = presenter;
			this.model = model;
			btnConnect.Click += new RoutedEventHandler(btnConnect_Click);            

			loadServers();
		}

		public void loadServers()
		{
			List<string> servers = this.model.GetServers();
			
			gameServers = new ObservableCollection<string>();            

			foreach (string item in servers)
			{
				gameServers.Add(item);
			}
		}

		void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			
			
			// Validate all controls
			if (ValidateBindings(this))
			{
				this.presenter.Login();
			}        
		}


		// This is here 'til future versions of WPF provide this functionality
		public static bool ValidateBindings(DependencyObject parent)
		{
			// Validate all the bindings on the parent
			bool valid = true;
			LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
			while (localValues.MoveNext())
			{
				LocalValueEntry entry = localValues.Current;
				if (BindingOperations.IsDataBound(parent, entry.Property))
				{
					Binding binding = BindingOperations.GetBinding(parent, entry.Property);
					foreach (ValidationRule rule in binding.ValidationRules)
					{
						// TODO: where to get correct culture info?
						ValidationResult result = rule.Validate(parent.GetValue(entry.Property), null);
						if (!result.IsValid)
						{
							BindingExpression expression = BindingOperations.GetBindingExpression(parent, entry.Property);
							Validation.MarkInvalid(expression, new ValidationError(rule, expression, result.ErrorContent, null));
							valid = false;
						}
					}
				}
			}

			// Validate all the bindings on the children
			for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);
				if (!ValidateBindings(child)) { valid = false; }
			}

			return valid;
		}

		public string SelectedURL
		{
			get
			{
				if (cbServer.Items.Count == 0)
				{
					return "";
				}
				else
				{
					return cbServer.SelectedItem.ToString();
				}
			}

			set
			{
				int i = 0;
				foreach (string item in cbServer.Items)
				{
					if (item == value) {
						cbServer.SelectedIndex = i;
						break;
					}
					i++;
				}                
			}
		}

		public string Username
		{
			get { return username; }
			set { username = value; Notify("Username"); }
		}

		public string Password
		{
			get { return password; }
			set { password = value; Notify("Password"); }
		}

		#region IChooseConnectionView Members

		public void CloseWindow()		
		{		     
			this.Close();			
		}

		#endregion

		#region IBaseView Members

		string IBaseView.Name
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

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

		private bool reallyCloseWindow()
		{
			return MessageBox.Show("Do you really want to exit application?", "SoTh", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
			
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{			
			this.CloseWindow();			
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (presenter.isConnected == false)
			{
				if (!reallyCloseWindow())
				{
					e.Cancel = true;
				}
			}
		}
	}

	// Validation rules

	public class NonZeroLength : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			if (string.IsNullOrEmpty((string)value))
			{
				return new ValidationResult(false, "Please enter something");
			}

			return ValidationResult.ValidResult;
		}
	}

	public class ErrorContentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var errors = value as ReadOnlyObservableCollection<ValidationError>;
			if (errors == null) return "";

			return errors.Count > 0 ? errors[0].ErrorContent : "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
