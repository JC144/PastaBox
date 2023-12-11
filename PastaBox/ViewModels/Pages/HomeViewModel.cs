using Newtonsoft.Json;
using PastaBox.Models;
using System.IO;
using System.Reflection;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PastaBox.ViewModels.Pages
{
    public partial class HomeViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private List<CopyableItem> _rawItems;

        [ObservableProperty]
        private List<IGrouping<string, CopyableItem>> _items;

        [ObservableProperty]
        private List<string> _categories;

        [ObservableProperty]
        private CopyableItem _selectedItem;

        [RelayCommand]
        private void Copy()
        {
            if (SelectedItem != null)
            {
                System.Windows.Clipboard.SetText(SelectedItem.Content);
                App.GetService<ISnackbarService>().Show("Message copié", SelectedItem.Content, ControlAppearance.Info, null, App.GetService<ISnackbarService>().DefaultTimeOut);
            }
        }

        [RelayCommand]
        private void Add()
        {
            SelectedItem = new CopyableItem();
            RawItems.Add(SelectedItem);
        }

        [RelayCommand]
        private void Save()
        {
            SaveInternal();
            SelectedItem = null;
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedItem != null)
            {
                if (RawItems.Any(i => i == SelectedItem))
                {
                    RawItems.Remove(SelectedItem);
                }
                SaveInternal();
                SelectedItem = null;
            }
        }

        private void SaveInternal()
        {
            File.WriteAllText(Properties.Settings.Default.DbFilePath, JsonConvert.SerializeObject(_rawItems));
            FilterItems();
        }

        public HomeViewModel()
        {
            RawItems = new List<CopyableItem>();
            Categories = new List<string>();
            Items = new List<IGrouping<string, CopyableItem>>();
        }

        public void OnNavigatedTo()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.DbFilePath))
            {
                Properties.Settings.Default.DbFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Properties.Settings.Default.Save();
            }

            if (!File.Exists(Properties.Settings.Default.DbFilePath))
            {
                File.Create(Properties.Settings.Default.DbFilePath);
            }
            else
            {
                var rawContent = File.ReadAllText(Properties.Settings.Default.DbFilePath);
                if (!string.IsNullOrEmpty(rawContent))
                {
                    RawItems = JsonConvert.DeserializeObject<List<CopyableItem>>(rawContent);
                    FilterItems();
                }
            }
        }

        public void OnNavigatedFrom()
        {
        }

        private void FilterItems()
        {
            Items = RawItems.GroupBy(i => i.Category).ToList();
            Categories = RawItems.Select(i => i.Category).Distinct().ToList();
        }
    }
}
