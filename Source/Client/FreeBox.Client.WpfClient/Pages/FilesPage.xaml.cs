using System;
using System.IO;
using System.Linq;
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

        private void UpdateFiles()
        {
            var ops = new ApiOperations();
            FilesList.ItemsSource = (ops.GetContainerInfos() ?? Array.Empty<ContainerInfo>()).Select(x => x.ToViewModel()).ToList();

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem is null)
            {
                MessageBox.Show("Nothing selected");
                return;
            }
            var file = FilesList.SelectedItem as FileViewModel;
            var ops = new ApiOperations();
            if (!ops.DeleteFile(file.Id))
            {
                MessageBox.Show("Something went wrong");
            }
            UpdateFiles();
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem is null)
            {
                MessageBox.Show("Nothing selected");
                return;
            }

            var ops = new ApiOperations();
            var file = FilesList.SelectedItem as FileViewModel;
            using FileContainer? fileContainer = ops.GetFile(file.Id);
            if (fileContainer is null)
            {
                MessageBox.Show("Something went wrong");
                UpdateFiles();
                return;
            }
            var saveFileDialog = new SaveFileDialog
            {
                FileName = fileContainer.Name
            };
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllBytes(saveFileDialog.FileName, fileContainer.Stream.ToArray());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            
            NavigationService.GoBack();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateFiles();
        }

    }
}
