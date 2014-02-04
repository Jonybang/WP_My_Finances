using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using System.Windows.Media;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.Windows.Interop;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Finances.ClasesForLists;
using System.Globalization;

namespace Finances
{
    public partial class AccountPage : PhoneApplicationPage
    {

        private string articleMode;

        ViewModel vM;

        int holdIndex;

        ArticleProperty aNameForRename;

        private bool isRename = false;

        public AccountPage()
        {
            InitializeComponent();
            articleMode = "Account";
        }
        
        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            CurrentData.SortListByTap(articleMode);
            vM = new ViewModel();
            listBox1.DataContext = vM;
        }

        #region events
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            tB1.Visibility = Visibility.Visible;
        }

        private void tB1_TextChanged(object sender, TextChangedEventArgs e)
        {
            tB1.Focus();
            tB1.SelectionStart = tB1.Text.Length;
        }

        private void tB_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (!isRename)
            {
                //Если создание
                int id = ArticleOperations.SearchFreeID(articleMode, CurrentData.ArtAccountList.AccountsList);
                CurrentData.ArtAccountList.AccountsList.Add(new ArticleProperty(tB1.Text, id));                
            }
            else
            {
                //Если переименование
                aNameForRename.Name = tB1.Text;
                CurrentData.ArtAccountList.AccountsList.Add(aNameForRename);
                aNameForRename = new ArticleProperty();
            }

            tB1.Text = "";
            tB1.Visibility = Visibility.Collapsed;
            ArticleOperations.ArticlePropertyListSerialize(articleMode, CurrentData.ArtAccountList.AccountsList);
        }

        private void listBox1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Выбор из списка
            if (listBox1.SelectedIndex == -1)
                return;
            GoToMainPage();
        }

        private void Grid_Hold_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Вызов контекстного меню
            var holdItem = (sender as Grid).DataContext;
            holdIndex = CurrentData.ArtAccountList.AccountsList.IndexOf((ArticleProperty)holdItem);          
        }

        private void MenuItem_Tap_Rename(object sender, System.Windows.Input.GestureEventArgs e)
        {
            tB1.Visibility = Visibility.Visible;
            aNameForRename = CurrentData.ArtAccountList.AccountsList[holdIndex];
            tB1.Text = CurrentData.ArtAccountList.AccountsList[holdIndex].Name;
            CurrentData.ArtAccountList.AccountsList.RemoveAt(holdIndex);
            isRename = true;
        }
        
        private void MenuItem_Tap_Delete(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult isDelete = MessageBox.Show("Удалить данный пункт меню \"" + CurrentData.ArtAccountList.AccountsList[holdIndex].Name + "\"?", "Запрос на удаление", MessageBoxButton.OKCancel);
            if (isDelete == MessageBoxResult.Cancel)
                return;

            CurrentData.ArtAccountList.AccountsList.RemoveAt(holdIndex);

            ArticleOperations.ArticlePropertyListSerialize(articleMode, CurrentData.ArtAccountList.AccountsList);
        }
        #endregion        

        private void GoToMainPage()
        {
            string numStr;
            NavigationContext.QueryString.TryGetValue("number", out numStr);
            string articleMode;
            NavigationContext.QueryString.TryGetValue("article", out articleMode);
            string nameIDStr;
            NavigationContext.QueryString.TryGetValue("nameId", out nameIDStr);

            ArticleOperations.AddArticle(numStr, nameIDStr, CurrentData.ArtAccountList.AccountsList[listBox1.SelectedIndex].ID.ToString(), articleMode);
            CurrentData.ArtAccountList.AccountsList[listBox1.SelectedIndex].TapNumber++;

            CurrentData.LastOperationsList = null;

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }             
    }
}