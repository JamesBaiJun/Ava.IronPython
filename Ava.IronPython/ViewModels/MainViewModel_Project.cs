using Ava.IronPython.Models;
using Ava.IronPython.Views;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ava.IronPython.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        ObservableCollection<ProjectItem> projectItems = new ObservableCollection<ProjectItem>();

        [ObservableProperty]
        ProjectItem? selectedProjectItem;
        [ObservableProperty]
        string searchText = string.Empty;

        string workSpace = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}WorkSpace";
        private void InitProjectFile()
        {
            DirectoryInfo root = new DirectoryInfo(workSpace);
            foreach (DirectoryInfo dir in root.GetDirectories())
            {
                ProjectItem projectItem = new(dir.Name, dir.FullName) { IsFolder = true };
                ProjectItems.Add(projectItem);
                GetDirectory(dir.FullName, projectItem);
            }
            foreach (var file in root.GetFiles())
            {
                ProjectItem projectItem = new ProjectItem(file.Name, file.FullName);
                ProjectItems.Add(projectItem);
            }
        }

        public void GetDirectory(string path, ProjectItem parent)
        {
            parent.Children.Clear();
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo dir in root.GetDirectories())
            {
                ProjectItem projectItem = new ProjectItem(dir.Name, dir.FullName) { IsFolder = true, Parent = parent };
                parent.Children.Add(projectItem);
                GetDirectory(dir.FullName, projectItem);
            }
            foreach (var file in root.GetFiles())
            {
                ProjectItem projectItem = new ProjectItem(file.Name, file.FullName) { Parent = parent };
                parent.Children.Add(projectItem);
            }
        }

        string currentFilePath = string.Empty;
        /// <summary>
        /// 双击文件打开
        /// </summary>
        /// <param name="e"></param>
        public void TreeDoubleTapped(TappedEventArgs? e)
        {
            if (SelectedProjectItem == null)
            {
                return;
            }

            if (!SelectedProjectItem.IsFolder)
            {
                // 读取文件
                var str = File.ReadAllText(SelectedProjectItem.FullName);
                currentFilePath = SelectedProjectItem.FullName;
                PyScript.Text = str;
            }
        }

        public void SaveFile()
        {
            if (currentFilePath == string.Empty)
            {
                return;
            }

            File.WriteAllText(currentFilePath, PyScript.Text);
        }

        public async void AddFolder()
        {
            (bool, string) result = await InputDialog.ShowWindow();
            if (result.Item1)
            {
                try
                {
                    Directory.CreateDirectory($"{SelectedProjectItem?.FullName}{Path.DirectorySeparatorChar}{result.Item2}");
                    GetDirectory(SelectedProjectItem.FullName, SelectedProjectItem);
                }
                catch (Exception e)
                {
                    WeakReferenceMessenger.Default.Send(new Tuple<int, string>(-1, e.Message));
                }
            }
        }

        public async void AddFile()
        {
            (bool, string) result = await InputDialog.ShowWindow();
            if (result.Item1)
            {
                try
                {
                    var stream = File.Create($"{SelectedProjectItem?.FullName}{Path.DirectorySeparatorChar}{result.Item2}.py");
                    stream.Close();
                    GetDirectory(SelectedProjectItem.FullName, SelectedProjectItem);
                }
                catch (Exception e)
                {
                    WeakReferenceMessenger.Default.Send(new Tuple<int, string>(-1, e.Message));
                }
              
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterTreeView(ProjectItems);
        }

        private void FilterTreeView(ObservableCollection<ProjectItem> projectItems)
        {
            HiddenAllItem(projectItems);
            foreach (var item in projectItems)
            {
                item.IsVisible = item.Name.Contains(SearchText);
                if (item.IsVisible)
                {
                    SetParentVisible(item);
                }
                if (item.Children.Count > 0)
                {
                    FilterTreeView(item.Children);
                }
            }
        }

        private void HiddenAllItem(ObservableCollection<ProjectItem> projectItems)
        {
            foreach (var item in projectItems)
            {
                item.IsVisible = false;
                item.IsExpanded = false;
                if (item.Children.Count > 0)
                {
                    HiddenAllItem(item.Children);
                }
            }
        }

        private void SetParentVisible(ProjectItem item)
        {
            if (item.Parent != null)
            {
                item.Parent.IsVisible = true;
                item.Parent.IsExpanded = true;
                SetParentVisible(item.Parent);
            }
        }
    }
}
