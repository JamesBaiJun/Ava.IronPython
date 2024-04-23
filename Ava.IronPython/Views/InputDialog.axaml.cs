using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

namespace Ava.IronPython.Views
{
    public partial class InputDialog : Window
    {
        public InputDialog()
        {
            InitializeComponent();
        }

        private void BtnCofirm_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Input = InputBox.Text;
            Result = true;
            Close();
        }

        private void BtnCancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Input = InputBox.Text;
            Result = false;
            Close();
        }

        public bool Result { get; set; }
        public string Input { get; set; }

        public static async Task<(bool,string)> ShowWindow()
        {
            var instance = new InputDialog();
            instance.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            await instance.ShowDialog((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);
            return (instance.Result, instance.Input);
        }
    }
}
