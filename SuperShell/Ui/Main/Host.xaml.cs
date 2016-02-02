﻿using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuperShell.Ui
{
    /// <summary>
    /// Interaction logic for Host.xaml
    /// </summary>
    public partial class Host : Window
    {
        internal static TabControl Tabs
        {
            get;private set;
        }
        public Host()
        {
            InitializeComponent();
            Tabs = tabControl;

            var closeButton = (this.Template.LoadContent() as FrameworkElement).FindName("closeButton") as CrossButton;
            closeButton.MouseUp += CloseButton_MouseUp;
        }

        private void CloseButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.CloseWindow(Window.GetWindow(this));
        }

        private void Test()
        {
            
        }
    }
}
