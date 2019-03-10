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
using System.Windows.Shapes;

namespace Project.Games
{
    /// <summary>
    /// Interaction logic for ListOfPlayersWindow.xaml
    /// </summary>
    public partial class ListOfPlayersWindow : Window
    {
        public ListOfPlayersWindow(object dataContext)
        {
            InitializeComponent();
            this.DataContext = dataContext;
        }
    }
}