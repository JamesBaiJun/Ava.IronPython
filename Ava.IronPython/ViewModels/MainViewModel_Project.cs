using Ava.IronPython.Models;
using Ava.IronPython.Views;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ava.IronPython.ViewModels
{
    public partial class MainViewModel
    {
        [ObservableProperty]
        ObservableCollection<ProjectItem> projectItems = new ObservableCollection<ProjectItem>();

        [ObservableProperty]
        ProjectItem? selectedProjectItem;

        string workSpace = ".\\WorkSpace";
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
                ProjectItem projectItem = new ProjectItem(dir.Name, dir.FullName) { IsFolder = true };
                parent.Children.Add(projectItem);
                GetDirectory(dir.FullName, projectItem);
            }
            foreach (var file in root.GetFiles())
            {
                ProjectItem projectItem = new ProjectItem(file.Name, file.FullName);
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
                Directory.CreateDirectory($"{SelectedProjectItem?.FullName}\\{result.Item2}");
                GetDirectory(SelectedProjectItem.FullName, SelectedProjectItem);
            }
        }

        public async void AddFile()
        {
            (bool, string) result = await InputDialog.ShowWindow();
            if (result.Item1)
            {
                var stream = File.Create($"{SelectedProjectItem?.FullName}\\{result.Item2}.py");
                stream.Close();
                GetDirectory(SelectedProjectItem.FullName, SelectedProjectItem);
            }
        }
    }
}
