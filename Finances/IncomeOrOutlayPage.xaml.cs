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

namespace Finances
{
    public partial class IncomeOrOutlayPage : PhoneApplicationPage
    {

        private string articleMode;

        ViewModel vM;

        int holdIndex;

        ArticleProperty aNameForRename;

        private bool isRename = false;

        public IncomeOrOutlayPage()
        {
            InitializeComponent();
        }
        
        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadPageName();

            CurrentData.SortListByTap(articleMode);

            vM = new ViewModel(articleMode);
            listBox1.DataContext = vM;
        }

        private void LoadPageName()
        {
            NavigationContext.QueryString.TryGetValue("article", out articleMode);
            if (articleMode == "Income")
                ArtNameTB.Text = "Статьи дохода";
            else if (articleMode == "Outlay")
                ArtNameTB.Text = "Статьи расхода";
        }

        #region events
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            tB1.Visibility = Visibility.Visible;
            tB1.Focus();
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
                int id = ArticleOperations.SearchFreeID(articleMode, CurrentData.ArtPropList(articleMode));

                CurrentData.ArtPropList(articleMode).Add(new ArticleProperty(tB1.Text, id));

            }
            else
            {
                //Если переименование
                aNameForRename.Name = tB1.Text;
                CurrentData.ArtPropList(articleMode).Add(aNameForRename);
                aNameForRename = new ArticleProperty();
            }

            tB1.Text = "";
            tB1.Visibility = Visibility.Collapsed;
            ArticleOperations.ArticlePropertyListSerialize(articleMode, CurrentData.ArtPropList(articleMode));

        }

        private void listBox1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Выбор из списка
            if (listBox1.SelectedIndex == -1)
                return;

            GoToAccountPage();
        }

        private void Grid_Hold_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Вызов контекстного меню
            var holdItem = (sender as Grid).DataContext;
            holdIndex = CurrentData.ArtPropList(articleMode).IndexOf((ArticleProperty)holdItem);          
        }

        private void MenuItem_Tap_Rename(object sender, System.Windows.Input.GestureEventArgs e)
        {
            tB1.Visibility = Visibility.Visible;
            aNameForRename = CurrentData.ArtPropList(articleMode)[holdIndex];
            tB1.Text = CurrentData.ArtPropList(articleMode)[holdIndex].Name;
            CurrentData.ArtPropList(articleMode).RemoveAt(holdIndex);
            isRename = true;
        }
        
        private void MenuItem_Tap_Delete(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult isDelete = MessageBox.Show("Удалить данный пункт меню \"" + CurrentData.ArtPropList(articleMode)[holdIndex].Name + "\"?", "Запрос на удаление", MessageBoxButton.OKCancel);
            if (isDelete == MessageBoxResult.Cancel)
                return;

            CurrentData.ArtPropList(articleMode).RemoveAt(holdIndex);

            ArticleOperations.ArticlePropertyListSerialize(articleMode, CurrentData.ArtPropList(articleMode));
        }
        #endregion        

        private void GoToAccountPage()
        {
            string numStr;
            NavigationContext.QueryString.TryGetValue("number", out numStr);

            CurrentData.ArtPropList(articleMode)[listBox1.SelectedIndex].TapNumber++;
            ArticleOperations.ArticlePropertyListSerialize(articleMode, CurrentData.ArtPropList(articleMode));

            NavigationService.Navigate(new Uri("/AccountPage.xaml?article=" + articleMode + "&number=" + numStr +
                                                "&nameId=" + CurrentData.ArtPropList(articleMode)[listBox1.SelectedIndex].ID, UriKind.RelativeOrAbsolute));
        }       
    }
}