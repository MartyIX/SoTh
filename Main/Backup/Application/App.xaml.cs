using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Controls;
using Sokoban.Model;

namespace Sokoban
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : System.Windows.Application
    {

        public void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        public void Application_Startup(object sender, StartupEventArgs e)
        {

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            PresentationTraceSources.ResourceDictionarySource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.ResourceDictionarySource.Switch.Level = SourceLevels.All;
            PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
            PresentationTraceSources.DependencyPropertySource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.DependencyPropertySource.Switch.Level = SourceLevels.All;
            PresentationTraceSources.DocumentsSource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.DocumentsSource.Switch.Level = SourceLevels.All;
            PresentationTraceSources.MarkupSource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.MarkupSource.Switch.Level = SourceLevels.All;
            PresentationTraceSources.NameScopeSource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.NameScopeSource.Switch.Level = SourceLevels.All;

            

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

                        
            if ( 1 == 2 )
            {
                /* do stuff without a GUI */
            }
            else
            {
                ApplicationRepository.Start(e.Args);
            }            

            this.Shutdown();
        }
    }
}