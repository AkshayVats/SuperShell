﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperShell.Ui
{
    /// <summary>
    /// Interaction logic for CrossButton.xaml
    /// </summary>
    public partial class CrossButton : UserControl
    {
        
        public CrossButton()
        {
            InitializeComponent();
            this.MouseUp += CrossButton_MouseUp;
        }

        private void CrossButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.CloseWindow(Window.GetWindow(this));
        }
    }
}
