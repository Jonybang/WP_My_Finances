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
    public partial class OptionsPage : PhoneApplicationPage
    {
        public OptionsPage()
        {
            InitializeComponent();
            passTextBox.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (passTextBox.Text == "Пароль")
            {
                MessageBox.Show("Пожалуйста, примените больше фантазии", "Просьба", MessageBoxButton.OK);
                return;
            }

            Cryptographi.SaveNewPass(passTextBox.Text);
            SavePasswordSucces.Visibility = Visibility.Visible;
            passTextBox.Text = "Пароль";
        }

        private void passTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SavePasswordSucces.Visibility = Visibility.Collapsed;
            passTextBox.Text = "";
            passTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBoxResult isChange = MessageBox.Show("Удалить пароль?", "Запрос на удаление пароля", MessageBoxButton.OKCancel);
            if (isChange == MessageBoxResult.OK)
                Cryptographi.DeletePass();
        }

    }
}