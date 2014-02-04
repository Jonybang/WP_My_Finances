using Finances.ClasesForLists;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Finances
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        { }

        public ViewModel(string article_Mode)
        {
            articleMode = article_Mode;
        }

        #region CurrentSummName
        private string currentSummName = "Суммарный счет";
        public string CurrentSummName
        {
            get
            {
                return currentSummName;
            }
            set
            {
                currentSummName = value;
            }
        }
        #endregion

        public string CurrentStrSumm
        {
            get
            {
                return CurrentData.ArtAccountList.TotalSumm.ToString();
            }
        }

        #region PropertyLists
        public ObservableRangeCollection<ArticleProperty> ArtIncomeList
        {
            get
            {
                return CurrentData.ArtPropList("Income");
            }
        }

        public ObservableRangeCollection<ArticleProperty> ArtOutlayList
        {
            get
            {
                return CurrentData.ArtPropList("Outlay");
            }           
        }

        private string articleMode;
        public ObservableRangeCollection<ArticleProperty> artPropList;
        public ObservableRangeCollection<ArticleProperty> ArtPropList
        {
            get
            {
                return CurrentData.ArtPropList(articleMode);
            }
        }

        public ObservableRangeCollection<ArticleProperty> ArtAccountList
        {
            get
            {
                return CurrentData.ArtAccountList.AccountsList;
            }
            set
            {
                CurrentData.ArtAccountList.AccountsList = value;
            }
        }
        #endregion        

        public static ObservableRangeCollection<AccountsObservableRangeCollection> PreviouslyAccountsLists
        {
            get
            {
                return CurrentData.PreviouslyAccountsLists;
            }
        }

        #region LastOperationsList
        public List<Article> LastOperationsList
        {
            get
            {
                return CurrentData.LastOperationsList;
            }
            set
            {
                LastOperationsList = value;
                CurrentData.LastOperationsList = value;
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
