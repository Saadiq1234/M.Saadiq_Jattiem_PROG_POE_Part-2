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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AaliyahAllie_ST10212542_PROGPART2
{
    /// <summary>
    /// Users need to either have an account or create an account to use the program further
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //redirects user to the lecturer login if they are a lecturer they will be allowed to login
        private void LecturerLogin_Click(object sender, RoutedEventArgs e)
        {
            LecturerLogin lecturerLogin = new LecturerLogin();
            lecturerLogin.Show();
        }
        //redirects user to coordinator login if they are a coordinator they will be allowed to login
        private void ProgrammeCoordinatorLogin_Click(object sender, RoutedEventArgs e)
        {
            ProgrammeCoordinatorLogin coordinatorLogin = new ProgrammeCoordinatorLogin();
            coordinatorLogin.Show();
        }

        //redirects user to the create account window if they do not have an account
        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountWindow createAccountWindow = new CreateAccountWindow();
            createAccountWindow.Show();
        }
    }
}
