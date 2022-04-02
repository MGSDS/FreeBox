using System;
using System.Windows;
using System.Windows.Controls;
using FreeBox.Client.WpfClient.Operations;

namespace FreeBox.Client.WpfClient.Pages;

public partial class RegisterPage : Page
{
    public RegisterPage()
    {
        InitializeComponent();
    }
    
    private void BtnReg_Click(object sender, RoutedEventArgs e)
    {
        var username = TbxUsername.Text;
        var password = PbxPassword.Password;

        if (String.IsNullOrEmpty(username) 
            || String.IsNullOrWhiteSpace(username)
            || String.IsNullOrEmpty(password) 
            || String.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Login and Password should not be empty or whitespace");
        }

        ApiOperations ops = new ApiOperations();
        var user = ops.RegisterUser(username, password);
        if (user == null)
        {
            MessageBox.Show("User already exists");
            return;
        }
        
        MessageBox.Show("User successfully registered");
        NavigationService.GoBack();
    }
    
    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }
}