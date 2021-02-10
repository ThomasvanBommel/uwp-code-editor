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
using Windows.Storage.Search;
using System.Threading.Tasks;
using Windows.UI.Popups;

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
                StorageFile file = await PickFile();

                // Update UI after selection
                UpdateUI(new StorageFile[] { file });

            // User would like to open an entire folder
            } else if (OpenDir.IsSelected) {
                StorageFolder folder = await PickFolder();

                // Get every file from the folder
                IReadOnlyList<StorageFile> files = await folder.CreateFileQuery().GetFilesAsync();

                // Update UI with list of files
                UpdateUI(files.ToArray());
            }
        }

        /** Update the user interface once files have been selected */
        private void UpdateUI(StorageFile[] files) {

            // Update hamburger icons to reflect what the user sees
            OpenFile.IsSelected = false;
            OpenDir.IsSelected  = false;
            Explorer.IsSelected = true;

            // Check if files exists
            if (files != null) {

                // Ensure list is not empty
                if (files.Length > 0) {
                    Results.Text = "";

                    // Print the names of each file to the results element
                    foreach (StorageFile file in files) {
                        Results.Text += file.Name + "\r\n";
                    }
                } else {
                    // List is empty, alert user
                    new MessageDialog("No files were selected or you opened an empty folder.\r\nTry again...").ShowAsync();
                }
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

            return folder;
        }
    }
}
