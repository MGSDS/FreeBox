using System;
using System.ComponentModel;

namespace FreeBox.Client.WpfClient.ViewModels
{
    internal class FileViewModel
    {
        public FileViewModel(Guid id, string name, long size, DateTime addDate)
        {
            Name = name;
            Size = size;
            Date = addDate;
            Id = id;
        }

        [Browsable(false)]
        public Guid Id { get; }
        public string Name { get; }
        public long Size { get; }
        public DateTime Date { get; }
    }
}
