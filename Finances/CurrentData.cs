using Finances.ClasesForLists;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Finances
{
    public static class CurrentData
    {
        public static string AppMode { get; set; }

        #region ArtPropertyLists
        private static ObservableRangeCollection<ArticleProperty> artIncomeList;
        private static ObservableRangeCollection<ArticleProperty> artOutlayList;
        
        public static ObservableRangeCollection<ArticleProperty> ArtPropList(string prop)
        {
            ObservableRangeCollection<ArticleProperty> returnList;

            if (prop == "Income")
                returnList = artIncomeList;
            else if (prop == "Outlay")
                returnList = artOutlayList;
            else if (prop == "Account")
                returnList = ArtAccountList.AccountsList;
            else returnList = new ObservableRangeCollection<ArticleProperty>();
            
            if (returnList != null)
                return returnList;
            else 
                return returnList = ArticleOperations.ArticlePropertyListDeserialize(prop);
        }
  
        public static AccountsObservableRangeCollection ArtAccountList
        {
            get
            {
                if (AccountsListsAtThisMonth[AccountsListsAtThisMonth.Count() - 1].RecordDayStr != (new AccountsObservableRangeCollection()).SetRecordDayStr())
                    AccountsListsAtThisMonth.Add(new AccountsObservableRangeCollection(AccountsListsAtThisMonth[AccountsListsAtThisMonth.Count() - 1].AccountsList));

                return AccountsListsAtThisMonth[AccountsListsAtThisMonth.Count() - 1];
            }
            set
            {
                if (AccountsListsAtThisMonth[AccountsListsAtThisMonth.Count() - 1].RecordDayStr != (new AccountsObservableRangeCollection()).SetRecordDayStr())
                    AccountsListsAtThisMonth.Add(new AccountsObservableRangeCollection(value.AccountsList));
                else
                    AccountsListsAtThisMonth[AccountsListsAtThisMonth.Count() - 1] = value;
            }
        }
        #endregion

        #region PreviouslyAccountsLists

        private static ObservableRangeCollection<AccountsObservableRangeCollection> accountsListsAtThisMonth;
        public static ObservableRangeCollection<AccountsObservableRangeCollection> AccountsListsAtThisMonth
        {
            get
            {
                if (accountsListsAtThisMonth == null)
                    accountsListsAtThisMonth = ArticleOperations.GetPreviouslyAccountsList();

                if (accountsListsAtThisMonth.Count() == 0)
                    accountsListsAtThisMonth.Add(new AccountsObservableRangeCollection(new ObservableRangeCollection<ArticleProperty>()));

                return accountsListsAtThisMonth;
            }
            set
            {
                accountsListsAtThisMonth = value;
            }
        }

        private static ObservableRangeCollection<AccountsObservableRangeCollection> previouslyAccountsLists;
        public static ObservableRangeCollection<AccountsObservableRangeCollection> PreviouslyAccountsLists
        {
            get
            {
                if (previouslyAccountsLists == null)
                    previouslyAccountsLists = ArticleOperations.GetPreviouslyAccountsList(1);

                return new ObservableRangeCollection<AccountsObservableRangeCollection>(previouslyAccountsLists.Concat(CurrentData.AccountsListsAtThisMonth));
            }
        }
        #endregion

        #region LastOperationsList
        private static List<Article> lastOperationsList;
        public static List<Article> LastOperationsList
        {
            get
            {
                if (lastOperationsList == null)
                {
                    lastOperationsList = ArticleOperations.GetLastOperations();
                    lastOperationsList.Reverse();
                }
                return lastOperationsList;
            }
            set
            {
                lastOperationsList = value;
            }
        }
        #endregion

        public static void SortListByTap(string property)
        {
            var artIntermediateList = CurrentData.ArtPropList(property);

            if (property == "Income")
                artIncomeList = new ObservableRangeCollection<ArticleProperty>(artIntermediateList.OrderBy(artlist => artlist.TapNumber * (-1)));
            else if (property == "Outlay")
                artOutlayList = new ObservableRangeCollection<ArticleProperty>(artIntermediateList.OrderBy(artlist => artlist.TapNumber * (-1)));
            else if (property == "Account")
                ArtAccountList.AccountsList = new ObservableRangeCollection<ArticleProperty>(artIntermediateList.OrderBy(artlist => artlist.TapNumber * (-1)));
        }
    }
}
