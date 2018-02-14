﻿using Adit.Code.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Adit.Pages
{
    /// <summary>
    /// Interaction logic for OptionsMain.xaml
    /// </summary>
    public partial class Options : Page
    {
        public static Options Current { get; set; }
        public Options()
        {
            InitializeComponent();
            Current = this;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshUI();
        }
        private void HandleUAC_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.IsUACHandled = !(sender as Controls.ToggleSwitch).IsOn;
            Config.Save();
        }

        private void ServiceInstalled_Click(object sender, MouseButtonEventArgs e)
        {
            if (!WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
            {
                MessageBox.Show("Adit must be running as an administrator (i.e. elevated) in order to configure the service.", "Elevation Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ServiceConfig.IsServiceInstalled)
            {
                ServiceConfig.InstallService(false);
            }
            else
            {
                ServiceConfig.RemoveService(false);
            }
        }

        private void ServiceRunning_Click(object sender, MouseButtonEventArgs e)
        {
            if (!ServiceConfig.IsServiceInstalled)
            {
                MessageBox.Show("The service must be installed before you can run it.", "Service Not Installed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (ServiceConfig.IsServiceRunning)
            {
                ServiceConfig.StopService();
            }
            else
            {
                ServiceConfig.StartService();
            }
            RefreshUI();
        }

        public void RefreshUI()
        {
            toggleUAC.IsOn = Config.Current.IsUACHandled;
            toggleMaximizeViewer.IsOn = Config.Current.MaximizeViewerOnConnect;
            toggleScaleToFitViewer.IsOn = Config.Current.ViewerScaleToFit;
            toggleStartServerAutomatically.IsOn = Config.Current.IsServerAutoStartEnabled;
            toggleServiceInstalled.IsOn = ServiceConfig.IsServiceInstalled;
            toggleServiceRunning.IsOn = ServiceConfig.IsServiceRunning;
            toggleIsWelcomeVisible.IsOn = Config.Current.IsWelcomeTabVisible;
            toggleIsServerVisible.IsOn = Config.Current.IsServerTabVisible;
            toggleIsClientVisible.IsOn = Config.Current.IsClientTabVisible;
            toggleIsViewerVisible.IsOn = Config.Current.IsViewerTabVisible;
            toggleIsOptionsVisible.IsOn = Config.Current.IsOptionsTabVisible;
            toggleChangeServer.IsOn = Config.Current.IsTargetServerConfigurable;
        }

        private void MaximizeViewer_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.MaximizeViewerOnConnect = !(sender as Controls.ToggleSwitch).IsOn;
            Config.Save();
        }

        private void StartServerAutomatically_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.IsServerAutoStartEnabled = !(sender as Controls.ToggleSwitch).IsOn;
            Config.Save();
        }

        private void ScaleToFitViewer_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.ViewerScaleToFit = !(sender as Controls.ToggleSwitch).IsOn;
            Config.Save();
        }

        private void IsWelcomeVisible_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.IsWelcomeTabVisible = !(sender as Controls.ToggleSwitch).IsOn;
            MainWindow.Current.welcomeToggle.Visibility = Config.Current.IsWelcomeTabVisible ? Visibility.Visible : Visibility.Collapsed;
            Config.Save();
        }

        private void IsServerVisible_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.IsServerTabVisible = !(sender as Controls.ToggleSwitch).IsOn;
            MainWindow.Current.serverToggle.Visibility = Config.Current.IsServerTabVisible ? Visibility.Visible : Visibility.Collapsed;
            Config.Save();
        }

        private void IsClientVisible_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.IsClientTabVisible = !(sender as Controls.ToggleSwitch).IsOn;
            MainWindow.Current.clientToggle.Visibility = Config.Current.IsClientTabVisible ? Visibility.Visible : Visibility.Collapsed;
            Config.Save();
        }

        private void IsViewerVisible_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.IsViewerTabVisible = !(sender as Controls.ToggleSwitch).IsOn;
            MainWindow.Current.viewerToggle.Visibility = Config.Current.IsViewerTabVisible ? Visibility.Visible : Visibility.Collapsed;
            Config.Save();
        }

        private void IsOptionsVisible_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.IsOptionsTabVisible = !(sender as Controls.ToggleSwitch).IsOn;
            MainWindow.Current.optionsToggle.Visibility = Config.Current.IsOptionsTabVisible ? Visibility.Visible : Visibility.Collapsed;
            Config.Save();
        }

        private void CanChangeServer_Click(object sender, MouseButtonEventArgs e)
        {
            Config.Current.IsTargetServerConfigurable = !(sender as Controls.ToggleSwitch).IsOn;
            Config.Save();
        }
    }
}
