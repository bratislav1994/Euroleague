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

namespace Project.Schedules
{
    /// <summary>
    /// Interaction logic for AddRefereeWindow.xaml
    /// </summary>
    public partial class AddRefereeWindow : Window
    {
        public AddRefereeWindow(object dataContext)
        {
            InitializeComponent();
            this.DataContext = dataContext;
        }
    }
}
