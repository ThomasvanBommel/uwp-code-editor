using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.Threading.Tasks;

namespace CodeEditor {
    public sealed partial class MainPage : Page {
        public MainPage() {
            InitializeComponent();
        }

        /** Toggle hamburger menu */
        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        /** Hamburger menu item has changed! */
        private async void HamburgerMenu_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            // User would like to open a single file
            if (OpenFile.IsSelected) {
                await PickFile();

            // User would like to open an entire folder
            } else if (OpenDir.IsSelected) {
                await PickFolder();
            }
        }

        /** Open file picker to allow user to select a single file */
        private async Task<StorageFile> PickFile() {
            // Create file-picker
            var picker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail
            };

            // Allow the user to select any file type
            picker.FileTypeFilter.Add("*");

            // Get the file the user has picked
            StorageFile file = await picker.PickSingleFileAsync();

            // Update application content
            if (file != null) {
                Results.Text = file.Name;
                OpenFile.IsSelected = false;
                Explorer.IsSelected = true;
            }

            return file;
        }

        /** Open folder picker to allow user to select an entire folder */
        private async Task<StorageFolder> PickFolder() {
            // Create folder-picker
            var picker = new FolderPicker {
                ViewMode = PickerViewMode.Thumbnail
            };

            // This is required to makes things work ? IDK why... 
            picker.FileTypeFilter.Add("*");

            // Allow user to select folder from the file-system
            StorageFolder folder = await picker.PickSingleFolderAsync();

            // Update application content
            if (folder != null) {
                Results.Text = folder.Name;
                OpenDir.IsSelected = false;
                Explorer.IsSelected = true;
            }

            return folder;
        }
    }
}
