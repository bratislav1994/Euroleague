using Project.HelperClasses;
using Project.Login;
using Project.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            HomeVM homeVM = new HomeVM();
            this.DataContext = homeVM;
            homeVM.HomeWin = new HomeWindow(this.DataContext);
            homeVM.HomeWin.ShowDialog();
            
            if (homeVM.Account == null)
            {
                this.Close();
            }
            else if (!string.IsNullOrEmpty(homeVM.Account.ULOGA_NLG))
            {
                if (homeVM.Account.ULOGA_NLG.Equals(Roles.Admin.ToString()))
                {
                    this.DataContext = new AdminMasterVM();
                }
                else if (homeVM.Account.ULOGA_NLG.Equals(Roles.User.ToString()))
                {
                    this.DataContext = new UserMasterVM();
                }
            }
            else
            {
                this.Close();
            }
        }
    }
}
