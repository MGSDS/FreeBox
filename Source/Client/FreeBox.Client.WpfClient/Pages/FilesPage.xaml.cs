using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FreeBox.Client.WpfClient.Entitities;
using FreeBox.Client.WpfClient.Extensions;
using FreeBox.Client.WpfClient.Operations;
using FreeBox.Client.WpfClient.ViewModels;
using Microsoft.Win32;

namespace FreeBox.Client.WpfClient.Pages
{
    /// <summary>
    /// Interaction logic for FilesPage.xaml
    /// </summary>
    ///
    public partial class FilesPage : Page
    {
        public FilesPage()
        {
            InitializeComponent();
            UpdateFiles();
        }

        private async Task UpdateFiles()
        {
            var ops = new ApiOperations();
            FilesList.ItemsSource = (await ops.GetContainerInfos() ?? Array.Empty<ContainerInfo>())
                .Select(x => x.ToViewModel()).ToList();

        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem is null)
            {
                MessageBox.Show("Nothing selected");
                return;
            }
            
            DisableButtons();
            var file = FilesList.SelectedItem as FileViewModel;
            var ops = new ApiOperations();
            if (!await ops.DeleteFile(file.Id))
            {
                MessageBox.Show("Something went wrong");
            }
            
            UpdateFiles();
           EnableButtons();
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem is null)
            {
                MessageBox.Show("Nothing selected");
                return;
            }

            DisableButtons();
            
            var ops = new ApiOperations();
            var file = FilesList.SelectedItem as FileViewModel;
            using FileContainer? fileContainer = await ops.GetFile(file.Id);
            if (fileContainer is null)
            {
                MessageBox.Show("Something went wrong");
                UpdateFiles();
                EnableButtons();
                return;
            }
            var saveFileDialog = new SaveFileDialog
            {
                FileName = fileContainer.Name
            };
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllBytes(saveFileDialog.FileName, fileContainer.Stream.ToArray());
            EnableButtons();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            UpdateFiles();
            EnableButtons();
        }

        private void DisableButtons()
        {
            BtnBack.IsEnabled = false;
            BtnDelete.IsEnabled = false;
            BtnDownload.IsEnabled = false;
            BtnUpdate.IsEnabled = false;
        }

        private void EnableButtons()
        {
            BtnBack.IsEnabled = true;
            BtnDelete.IsEnabled = true;
            BtnDownload.IsEnabled = true;
            BtnUpdate.IsEnabled = true;
        }
    }
}
