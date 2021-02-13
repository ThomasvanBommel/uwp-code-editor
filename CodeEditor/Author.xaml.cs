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
    public sealed partial class Author : Page {
        public Author() {
            this.InitializeComponent();
        }

        /** User would like to go back to the MainPage */
        private void Button_Click(object sender, RoutedEventArgs e) {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
