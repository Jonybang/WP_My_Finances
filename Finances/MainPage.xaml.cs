using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using Finances.ClasesForLists;
using System.Globalization;

namespace Finances
{
    public partial class MainPage : PhoneApplicationPage
    {
        ViewModel vM;
        int holdAccountIndex;

        public MainPage()
        {
            InitializeComponent();
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            vM = new ViewModel();
            this.DataContext = vM;
            RefreshViewDataOnPage();

            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }

        private void OnCurrentSumm_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (accountsListBox.Visibility == Visibility.Collapsed)
                accountsListBox.Visibility = Visibility.Visible;
            else
                accountsListBox.Visibility = Visibility.Collapsed;
        }

        #region NavigateToChooseArrticlePage
        private void IncomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToChoisePage("Income");
        }

        private void OutlayButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToChoisePage("Outlay");
        }

        private void ArticleNumberTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ArticleNumberTextBlock.Text = "";
        }       

        private void NavigateToChoisePage(string ArticleMode)
        {
            if (ArticleNumberTextBlock.Text == "0" || ArticleNumberTextBlock.Text == "")
            {
                MessageBox.Show("Пожалуйста, введите значение отличное от нуля.", "Ошибка", MessageBoxButton.OK);
                return;
            }
            double num;
            string numberStr = ArticleNumberTextBlock.Text.Replace(",", ".");
            bool isSuccess = double.TryParse(numberStr, out num);
            if (isSuccess)
                NavigationService.Navigate(new Uri("/IncomeOrOutlayPage.xaml?article=" + ArticleMode + "&number=" + num, UriKind.RelativeOrAbsolute));
            else
            {
                MessageBox.Show("Произошла ошибка, пожалуйста повторите ввод.", "Ошибка", MessageBoxButton.OK);
                ArticleNumberTextBlock.Text = "0";
            }
        }
        #endregion

        #region DeleteOperations
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationBarIconButton DelButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (listBox1.SelectedIndex != -1)
                DelButton.IsEnabled = true;
            else
                DelButton.IsEnabled = false;
        }

        private void DeleteCurOperationButton_Click(object sender, EventArgs e)
        {
            string incomeOrCons = "";
            if (CurrentData.LastOperationsList[listBox1.SelectedIndex].IsIncome)
                incomeOrCons = "дохода";
            else
                incomeOrCons = "расхода";

            MessageBoxResult isDelete = MessageBox.Show("Удалить статью " + incomeOrCons + ": " + vM.LastOperationsList[listBox1.SelectedIndex].Name
                + " на сумму " + vM.LastOperationsList[listBox1.SelectedIndex].Number + "?",
                "Запрос на удаление", MessageBoxButton.OKCancel);

            if (isDelete == MessageBoxResult.OK)
            {
                ArticleOperations.DeleteArt(vM.LastOperationsList[listBox1.SelectedIndex]);
                RefreshViewDataOnPage();
            }
        }

        private void DeleteToDayOperationsButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult isDelete = MessageBox.Show("Удалить все сегодняшние операции?", "Запрос на удаление", MessageBoxButton.OKCancel);
            if (isDelete == MessageBoxResult.Cancel)
                return;

            ArticleOperations.DeleteToDayArts();
            RefreshViewDataOnPage();
        }
        #endregion

        private void RefreshViewDataOnPage()
        {
            CurrentData.LastOperationsList = null;
            vM = new ViewModel();
            this.DataContext = null;
            this.DataContext = vM;
        }
        
        #region ChangeAccountSumm
        private void AccountPanel_Hold_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Вызов контекстного меню
            var holdItem = (sender as Grid).DataContext;
            holdAccountIndex = CurrentData.ArtAccountList.AccountsList.IndexOf((ArticleProperty)holdItem);        
        }

        private void ContextMenuItem_Tap_ChangeAccSumm(object sender, System.Windows.Input.GestureEventArgs e)
        {
            accountsListBox.Visibility = Visibility.Collapsed;
            ChangeSummTextBox.Visibility = Visibility.Visible;

            DeleteIconButton("Операцию");
            AddOkCancelButtons();

            vM.CurrentSummName = CurrentData.ArtAccountList.AccountsList[holdAccountIndex].Name;
            this.DataContext = null;
            this.DataContext = vM;

            ChangeSummTextBox.Text = CurrentData.ArtAccountList.AccountsList[holdAccountIndex].Summ.ToString();
            ChangeSummTextBox.Focus();
        }

        private void ChangeSummTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Для автоматического появления клавиатуры
            ChangeSummTextBox.Focus();
            ChangeSummTextBox.SelectionStart = ChangeSummTextBox.Text.Length;
        }        

        private void OnOkChangeCurOperationButton_Click(object sender, EventArgs e)
        {
            RequestToChange();
        }

        private void RequestToChange()
        {
            MessageBoxResult isChange = MessageBox.Show("Изменить сумму счёта \"" + CurrentData.ArtAccountList.AccountsList[holdAccountIndex].Name + "\" с "
               + CurrentData.ArtAccountList.AccountsList[holdAccountIndex].Summ.ToString()
               + " на " + ChangeSummTextBox.Text + "?", "Изменение суммы счета", MessageBoxButton.OKCancel);
            if (isChange == MessageBoxResult.OK)
                AcceptSummAccountChange();

            DeleteIconButton("Применить");
            DeleteIconButton("Отменить");
            AddDeleterButton();

            ChangeSummTextBox.Text = "";
            vM.CurrentSummName = "Суммарный счёт";
            ChangeSummTextBox.Visibility = Visibility.Collapsed;

            RefreshViewDataOnPage();
        }

        private void AcceptSummAccountChange()
        {
            double num;
            string number = ChangeSummTextBox.Text.Replace(",", ".");
            bool isSuccess = double.TryParse(number, out num);
            if (isSuccess)
                ArticleOperations.AddArticleManually(num, CurrentData.ArtAccountList.AccountsList[holdAccountIndex].ID);
            else
                MessageBox.Show("Произошла ошибка, пожалуйста повторите ввод.", "Ошибка", MessageBoxButton.OK);            
        }

        private void OnCancelChangeCurOperationButton_Click(object sender, EventArgs e)
        {
            ChangeSummTextBox.Text = "";
            TextOfCurAcc.Text = "Суммарный счёт";
            ChangeSummTextBox.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region OnCharts_DoubleTap
        private void IncomeColumnChart1_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IncomeColumnChart1.Visibility = Visibility.Collapsed;
            IncomePieChart1.Visibility = Visibility.Visible;
        }

        private void IncomePieChart1_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IncomePieChart1.Visibility = Visibility.Collapsed;
            IncomeColumnChart1.Visibility = Visibility.Visible;
        }

        private void OutlayColumnChart1_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            OutlayColumnChart1.Visibility = Visibility.Collapsed;
            OutlayPieChart1.Visibility = Visibility.Visible;
        }

        private void OutlayPieChart1_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            OutlayPieChart1.Visibility = Visibility.Collapsed;
            OutlayColumnChart1.Visibility = Visibility.Visible;
        }
        #endregion

        #region ButtonsOperations
        private void AddDeleterButton()
        {
            var okButton = new ApplicationBarIconButton { IsEnabled = true, Text = "Операцию" };
            okButton.IconUri = new Uri("/Assets/AppBar/appbar.delete.rest.png", UriKind.Relative);
            ApplicationBar.Buttons.Add(okButton);
            okButton.Click += new EventHandler(DeleteCurOperationButton_Click);
        }

        private void AddOkCancelButtons()
        {
            var okButton = new ApplicationBarIconButton { IsEnabled = true, Text = "Применить" };
            okButton.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative);
            ApplicationBar.Buttons.Add(okButton);
            okButton.Click += new EventHandler(OnOkChangeCurOperationButton_Click);

            var cancelButton = new ApplicationBarIconButton { IsEnabled = true, Text = "Отменить" };
            cancelButton.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative);
            ApplicationBar.Buttons.Add(cancelButton);
            cancelButton.Click += new EventHandler(OnCancelChangeCurOperationButton_Click);
        }

        private void DeleteIconButton(string textOnButton)
        {
            foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                if (button.Text == textOnButton)
                {
                    ApplicationBar.Buttons.Remove(button);
                    break;
                }
        }
        #endregion

        private void NavigateToOptionsPage_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/OptionsPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}