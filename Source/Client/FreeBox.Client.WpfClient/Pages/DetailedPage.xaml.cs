using System.Windows;
using System.Windows.Controls;
using FreeBox.Client.WpfClient.Model;
using FreeBox.Client.WpfClient.Operations;
using Microsoft.Win32;

namespace FreeBox.Client.WpfClient.Pages;

public partial class DetailedPage : Page
{
    public DetailedPage()
    {
        InitializeComponent();
        ShowUserInfo();
    }

    private void ShowUserInfo()
    {
        TbkLogin.Text = Globals.LoggedInUser!.Login;
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Globals.LoggedInUser = null;
        NavigationService!.Navigate(new LoginPage());
    }

    private async void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        DisableButtons();
        var ops = new ApiOperations();
        if (!await ops.DeleteUser())
        {
            MessageBox.Show("Something went wrong");
            EnableButtons();
            return;
        }

        MessageBox.Show("User successfully deleted");
        Globals.LoggedInUser = null;
        NavigationService!.Navigate(new LoginPage());
        }

    private void BtnFiles_Click(object sender, RoutedEventArgs e)
    {
        NavigationService!.Navigate(new FilesPage());
    }

    private async void BtnUpload_Click(object sender, RoutedEventArgs e)
    {
        DisableButtons();
        var ops = new ApiOperations();
        var openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() != true) return;
        MessageBox.Show(await ops.UploadFile(openFileDialog.FileName)
            ? "File successfully uploaded"
            : "Something went wrong, try again later");
        EnableButtons();
    }

    private void DisableButtons()
    {
        BtnFiles.IsEnabled = false;
        BtnDelete.IsEnabled = false;
        BtnLogout.IsEnabled = false;
        BtnUpload.IsEnabled = false;
    }

    private void EnableButtons()
    {
        BtnFiles.IsEnabled = true;
        BtnDelete.IsEnabled = true;
        BtnLogout.IsEnabled = true;
        BtnUpload.IsEnabled = true;
    }
}