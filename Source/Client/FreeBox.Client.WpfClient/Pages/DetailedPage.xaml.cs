using System.Windows;
using System.Windows.Controls;
using FreeBox.Client.WpfClient.Entitities;
using FreeBox.Client.WpfClient.Model;
using FreeBox.Client.WpfClient.Operations;

namespace FreeBox.Client.WpfClient.Pages;

public partial class DetailedPage : Page
{
    public DetailedPage()
    {
        InitializeComponent();
        FetchUserDetails();
        ShowUserInfo();
    }
    
    private void FetchUserDetails()
    {
        var ops = new ApiOperations();
        User? user = ops.AuthenticateUser(Globals.LoggedInUser.Login, Globals.LoggedInUser.Password);
        if (user == null)
        {
            MessageBox.Show("Session expired");
            NavigationService.Navigate(new LoginPage());
        }

        Globals.LoggedInUser = user;
    }
    
    private void ShowUserInfo()
    {
        TbkLogin.Text = Globals.LoggedInUser.Login;
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Globals.LoggedInUser = null;
        NavigationService.Navigate(new LoginPage());
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        var ops = new ApiOperations();
        if (ops.Delete())
        {
            MessageBox.Show("User successfully deleted");
            Globals.LoggedInUser = null;
        }
        else
        {
            MessageBox.Show("Something went wrong");
        }
        NavigationService.Navigate(new LoginPage());
    }

    private void BtnFiles_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}