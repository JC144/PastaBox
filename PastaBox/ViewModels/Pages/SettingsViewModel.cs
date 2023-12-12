using System.Diagnostics;
using System.IO;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PastaBox.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Dark;

        [ObservableProperty]
        private string _folderPath;

        [RelayCommand]
        public void OpenFolderPicker()
        {
            string folderPath = string.Empty;
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    string newFilePath = Path.Combine(FolderPath, App.DbFileName);
                    if (!File.Exists(folderPath))
                    {
                        File.Create(newFilePath);
                    }
                    FolderPath = newFilePath;
                }
            }
        }

        [RelayCommand]
        private void OpenGitHub()
        {
            Process.Start(new ProcessStartInfo() { FileName = "https://github.com/JC144/PastaBox", UseShellExecute = true });
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
            FolderPath = Properties.Settings.Default.DbFilePath;

        }

        public void OnNavigatedFrom()
        {
                Properties.Settings.Default.DbFilePath = FolderPath;
                Properties.Settings.Default.Theme = (CurrentTheme == ApplicationTheme.Light) ? "theme_light" : "theme_dark";
                Properties.Settings.Default.Save();            
        }

        private void SetTheme(ApplicationTheme theme)
        {
            if (CurrentTheme != theme)
            {
                App.GetService<IThemeService>().SetTheme(theme);
                CurrentTheme = theme;
            }
        }

        private void InitializeViewModel()
        {
            CurrentTheme = App.GetService<IThemeService>().GetTheme();
            AppVersion = $"PastaBox - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    SetTheme(ApplicationTheme.Light);
                    break;

                default:
                    SetTheme(ApplicationTheme.Dark);

                    break;
            }
        }
    }
}
