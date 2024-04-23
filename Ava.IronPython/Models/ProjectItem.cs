using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ava.IronPython.Models
{
    public partial class ProjectItem : ObservableObject
    {
        [ObservableProperty]
        string name = string.Empty;

        [ObservableProperty]
        string fullName = string.Empty;

        [ObservableProperty]
        bool isFolder;

        [ObservableProperty]
        ObservableCollection<ProjectItem> children = [];

        public ProjectItem(string name, string fullName)
        {
            Name = name;
            FullName = fullName;
        }
    }
}
