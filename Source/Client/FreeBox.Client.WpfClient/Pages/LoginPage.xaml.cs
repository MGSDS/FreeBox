using System.Windows;
using System.Windows.Controls;
using FreeBox.Client.WpfClient.Entitities;
using FreeBox.Client.WpfClient.Model;
using FreeBox.Client.WpfClient.Operations;

namespace FreeBox.Client.WpfClient.Pages;

public partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();
    }
    
    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = TbxUsername.Text;
        string password = PbxPassword.Password;

        ApiOperations ops = new ApiOperations();
        User? user = ops.AuthenticateUser(username, password);
        if (user == null)
        {
            MessageBox.Show("Invalid username or password");
            return;
        }

        Globals.LoggedInUser = user;
        NavigationService.Navigate(new DetailedPage());
    }

    private void BtnRegister_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new RegisterPage());
    }
}