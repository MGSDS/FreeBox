using FreeBox.Client.WpfClient.Entitities;
using FreeBox.Client.WpfClient.ViewModels;

namespace FreeBox.Client.WpfClient.Extensions
{
    internal static class ContainerInfoExtensions
    {
        public static FileViewModel ToViewModel(this ContainerInfo containerInfo)
        {
            return new FileViewModel(containerInfo.Id, containerInfo.Name, containerInfo.Size, containerInfo.SaveDate);
        }
    }
}
