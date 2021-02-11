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
            ResetUI();
        }

        /** Toggle hamburger menu */
        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        /** Toggles the FileSelectionExplorer */
        private void ToggleFileSelectionExplorer() {
            // Ensure the explorer is active (not loading the application)
            if (FileSelectionExplorer != null) {

                // Set visibility based on Explorer (search hamburger icon) being selected
                FileSelectionExplorer.Visibility = Explorer.IsSelected ? Visibility.Visible : Visibility.Collapsed;

                // Check if the FileSelectionExplorer is visible and has children
                if (FileSelectionExplorer.Visibility == Visibility.Visible && FileSelectionExplorer.Children.Count > 0) {

                    // Set panels minimum width (looks nicer)
                    FileSelectionExplorer.MinWidth = 100;

                // SelectionExplorer is not visible or does not have children
                } else {
                    // Set panels minimum width (to allow collapse)
                    FileSelectionExplorer.MinWidth = 0;
                }
            }
        }

        /** Hamburger menu item has changed! */
        private async void HamburgerMenu_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            
            // Toggle FileSelectionExplorer
            ToggleFileSelectionExplorer();

            // User would like to open a single file
            if (OpenFile.IsSelected) {
                StorageFile file = await PickFile();

                if (file != null) {
                    // Update UI after selection
                    UpdateUI(new StorageFile[] { file });
                }

                // Deselect OpenFile hamburger menu item
                OpenFile.IsSelected = false;

            // User would like to open an entire folder
            } else if (OpenDir.IsSelected) {
                StorageFolder folder = await PickFolder();

                if (folder != null) {
                    // Get every file from the folder
                    IReadOnlyList<StorageFile> files = await folder.CreateFileQuery().GetFilesAsync();

                    // Update UI with list of files
                    UpdateUI(files.ToArray());
                }

                // Deselect OpenDir hamburger menu item
                OpenDir.IsSelected = false;
            }

            // Close hamburger menu
            MySplitView.IsPaneOpen = false;
        }

        /** Update the user interface once files have been selected */
        private void UpdateUI(StorageFile[] files) {
            
            // Reset the UI (removing old files + text, if there are any)
            ResetUI();

            // Update hamburger icons to reflect what the user sees
            OpenFile.IsSelected = false;
            OpenDir.IsSelected  = false;
            Explorer.IsSelected = true;

            // Check if files exists
            if (files != null) {

                // Ensure list is not empty
                if (files.Length > 0) {

                    // For each file provided
                    for(int i = 0; i < files.Length; i++) {

                        // New row for the grid
                        RowDefinition row = new RowDefinition();
                        row.Height = GridLength.Auto;

                        // Hyperlink button for user to click when wanting to edit this file
                        HyperlinkButton btn = new HyperlinkButton();
                        btn.Content = files[i].Name;
                        btn.Margin = new Thickness(5, 5, 5, 0);
                        btn.Tag = files[i];

                        // Add hyperlink click event
                        btn.Click += HyperlinkFile_Clicked;

                        // Set grid row to i (increments)
                        Grid.SetRow(btn, i);

                        // Add hyperlink to grid
                        FileSelectionExplorer.RowDefinitions.Add(row);
                        FileSelectionExplorer.Children.Add(btn);
                    }
                } else {
                    // List is empty, alert user
                    new MessageDialog("No files were selected or you opened an empty folder.\r\nTry again...").ShowAsync();
                }
            }
        }

        /** User has selected a file they would like to load into the editor */
        private async void HyperlinkFile_Clicked(object sender, RoutedEventArgs e) {
            HyperlinkButton btn = (HyperlinkButton) sender;
            StorageFile file = (StorageFile) btn.Tag;

            // Await open attempt
            if (!await AttemptOpenHyperlinkFile(file)) {

                // Disable hyperlink click event
                btn.Click -= HyperlinkFile_Clicked;

                // Change color so the user knows this hyperlink will no longer work
                btn.Foreground = new SolidColorBrush(Windows.UI.Colors.LightGray);
                return;
            }

            // Set title to the file name of the selected hyperlink file
            Title.Text = file.Name;
        }

        /** Hyperlink click event to start editing the requested file */
        private async Task<bool> AttemptOpenHyperlinkFile(StorageFile file) {
            try {
                // Try to read selected file; ReadLinesAsync doesn't like some files...
                IList<string> lines = await FileIO.ReadLinesAsync(file);

                // Add file contents to the editor
                Editor.Text = string.Join("\n", lines);
            } catch {

                // Inform the user we were unable to read this type of file
                new MessageDialog("Unable to read this type of file: Incorrect unicode encoding").ShowAsync();

                // Failed attempt
                return false;
            }

            // Successful attempt!
            return true;
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

        /** Reset the UI's default values (empty, as if no file or folder was selected yet) */
        private void ResetUI() {
            Title.Text = "undefined";
            Editor.Text = "";
            FileSelectionExplorer.Children.Clear();
        }
    }
}
