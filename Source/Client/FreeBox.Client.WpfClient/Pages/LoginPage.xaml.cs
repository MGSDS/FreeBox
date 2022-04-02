using System.Threading.Tasks;
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
    
    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = TbxUsername.Text;
        string password = PbxPassword.Password;
        DisableButtons();
        ApiOperations ops = new ApiOperations();
        User? user = await ops.AuthenticateUser(username, password);
        if (user == null)
        {
            MessageBox.Show("Invalid username or password");
            EnableButtons();
            return;
        }

        Globals.LoggedInUser = user;
        NavigationService.Navigate(new DetailedPage());
    }

    private void BtnRegister_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new RegisterPage());
    }
    
    private void DisableButtons()
    {
        BtnLogin.IsEnabled = false;
        BtnRegister.IsEnabled = false;
    }

    private void EnableButtons()
    {
        BtnLogin.IsEnabled = true;
        BtnRegister.IsEnabled = true;
    }
}