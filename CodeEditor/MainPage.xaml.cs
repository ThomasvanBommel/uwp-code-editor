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

namespace CodeEditor {
    /// <summary>
    /// Main page of the code editor
    /// </summary>
    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private async void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (OpenFile.IsSelected) {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.FileTypeFilter.Add("*");
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

                if (file != null) {
                    Results.Text = file.Name;
                    OpenFile.IsSelected = false;
                    Explorer.IsSelected = true;
                }
            } else if (OpenDir.IsSelected) {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.FileTypeFilter.Add("*");
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

                if (file != null) {
                    Results.Text = file.Name;
                    OpenDir.IsSelected = false;
                    Explorer.IsSelected = true;
                }
            }
        }
    }
}
