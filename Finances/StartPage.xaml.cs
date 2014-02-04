using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace Finances
{
    public partial class StartPage : PhoneApplicationPage
    {
        public StartPage()
        {
            InitializeComponent();
            passTextBox.Foreground = new SolidColorBrush(Colors.Gray);            
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!Cryptographi.IsSetPass())
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
                CurrentData.AppMode = "Autorized";
            }
        }

        private void passTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(passTextBox.Text=="Вы установили пароль")
                passTextBox.Text = "";
            passTextBox.Foreground = new SolidColorBrush(Colors.Black);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Cryptographi.isRightPassword(passTextBox.Text))
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
                CurrentData.AppMode = "Autorized";
            }
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            CurrentData.AppMode = "Demo";
        }
    }
}