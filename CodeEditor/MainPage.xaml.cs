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

            //Border b = new Border();
            //b.Background = new SolidColorBrush(Windows.UI.Colors.Red);

            //RowDefinition r = new RowDefinition();
            //r.Height = GridLength.Auto;

            //TextBlock t = new TextBlock();
            //t.Text = "t1";
            //Grid.SetRow(t, 0);

            //RowDefinition r2 = new RowDefinition();
            //r2.Height = GridLength.Auto;
            //TextBlock t2 = new TextBlock();
            //t2.Text = "t2";
            //Grid.SetRow(t, 1);

            //Results.RowDefinitions.Add(r2);


            //Results.Children.Add(t2);

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
                    for(int i = 0; i < files.Length; i++) {
                        RowDefinition row = new RowDefinition();
                        row.Height = GridLength.Auto;

                        HyperlinkButton btn = new HyperlinkButton();
                        btn.Content = files[i].Name;
                        btn.Margin = new Thickness(5, 5, 5, 0);
                        //btn.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                        //btn.visual
                        //btn.PointerEntered += (object sender, PointerRoutedEventArgs e) => { 
                        //    btn.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
                        //};
                        //btn.PointerEntered += PointerEnter;

                        Grid.SetRow(btn, i);

                        FileSelection.RowDefinitions.Add(row);
                        FileSelection.Children.Add(btn);
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
