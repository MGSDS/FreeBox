using System.Windows;
using System.Windows.Controls;
using FreeBox.Client.WpfClient.Entitities;
using FreeBox.Client.WpfClient.Operations;

namespace FreeBox.Client.WpfClient.Pages;

public partial class RegisterPage : Page
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void BtnReg_Click(object sender, RoutedEventArgs e)
    {
        string username = TbxUsername.Text;
        string password = PbxPassword.Password;

        if (string.IsNullOrEmpty(username)
            || string.IsNullOrWhiteSpace(username)
            || string.IsNullOrEmpty(password)
            || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Login and Password should not be empty or whitespace");
            return;
        }

        DisableButtons();

        var ops = new ApiOperations();
        User? user = await ops.RegisterUser(username, password);
        if (user == null)
        {
            MessageBox.Show("User already exists");
            EnableButtons();
            return;
        }

        MessageBox.Show("User successfully registered");
        NavigationService!.GoBack();
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        NavigationService!.GoBack();
    }

    private void DisableButtons()
    {
        BtnBack.IsEnabled = false;
        BtnReg.IsEnabled = false;
    }

    private void EnableButtons()
    {
        BtnBack.IsEnabled = true;
        BtnReg.IsEnabled = true;
    }
}