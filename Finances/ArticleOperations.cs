using Finances.ClasesForLists;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Finances
{
    public static class ArticleOperations
    {
        public static List<Article> GetLastOperations()
        {//Необходимо проведение уточнения по количеству
            int count = 15;
            var ISD = new IsolatedStorageDeserializer<List<Article>>();
            var lastOpList = ISD.XmlDeserialize(DateTime.Today.ToString("d.M.y"), CurrentData.AppMode + "\\" + "Articles" + "\\" + DateTime.Today.ToString("M.y"));
            if (lastOpList == null)
                 lastOpList = new List<Article>();

            int n = -1;

            while (lastOpList.Count < count)
            {
                var previouslyDayList = ISD.XmlDeserialize(DateTime.Now.AddDays(n).ToString("d.M.y"), CurrentData.AppMode + "\\" + "Articles" + "\\" + DateTime.Now.AddDays(n).ToString("M.y"));
                if (previouslyDayList == null || previouslyDayList.Count == 0)
                    break;
                lastOpList.InsertRange(0, previouslyDayList);
                n--;
            }

            for (int i = 0; i < lastOpList.Count; i++)
                if (lastOpList[i].NameID == 0)
                {
                    lastOpList.RemoveAt(i);
                    i--;
                }

            return lastOpList;
        }

        #region AddArticles
        public static void AddArticle(string numStr, string nameIDStr, string accountIDStr, string articleMode)
        {
            double number;
            double.TryParse(numStr, out number);

            int nameID;
            int.TryParse(nameIDStr, out nameID);

            int accountID;
            int.TryParse(accountIDStr, out accountID);

            bool isIncome = false;
            if (articleMode == "Income")
                isIncome = true;

            ArtSerialize(new Article
            {
                Number = number,
                IsIncome = isIncome,
                NameID = nameID,
                AccountID = accountID,
                ADateTime = DateTime.Now
            });

            AddSummToAccountProperty(number, isIncome, accountID);
            AddSummToIncomeOrOutlayProperty(number, isIncome, nameID);
            SetCurrentAccountsStatusToList();
        }

        public static void AddArticleManually(double summToChange, int accountId)
        {
            //в случае если пользователь в ручную изменил текущий счёт без создания статьи расхода/дохода
            bool isIncome = false;

            double difference = SetSummToAccount(summToChange, accountId);

            if (difference > 0)
                isIncome = true;

            difference = Math.Abs(difference);

            ArtSerialize(new Article
            {
                Name = "Изменено вручную",
                Number = difference,
                IsIncome = isIncome,
                NameID = 0,
                AccountID = accountId,
                ADateTime = DateTime.Today
            });

            AddSummToIncomeOrOutlayProperty(difference, isIncome, 0);
            SetCurrentAccountsStatusToList();
        }
        #endregion

        private static void CreateDictionaties(List<Article> lastOpList, Dictionary<int, double> outlayDict, Dictionary<int, double> incomeDict)
        {
            foreach (Article a in lastOpList)
                if (a.IsIncome)
                {
                    if (incomeDict.ContainsKey(a.NameID))
                        incomeDict[a.NameID] += a.Number;
                    else
                        incomeDict.Add(a.NameID, a.Number);
                }
                else
                {
                    if (outlayDict.ContainsKey(a.NameID))
                        outlayDict[a.NameID] += a.Number;
                    else
                        outlayDict.Add(a.NameID, a.Number);
                }
        }

        public static void SetIncomeAndOutlayForNDays(int nDays)
        {//Необходимо проведение уточнения по количеству
            var ISD = new IsolatedStorageDeserializer<List<Article>>();
            var lastOpList = ISD.XmlDeserialize(DateTime.Today.ToString("d.M.y"), CurrentData.AppMode + "\\" + "Articles" + "\\" + DateTime.Today.ToString("M.y"));
            if (lastOpList == null)
                lastOpList = new List<Article>();

            for (int i = 1; i < (nDays + 1); i++)
            {
                var previouslyDayList = ISD.XmlDeserialize(DateTime.Now.AddDays(-i).ToString("d.M.y"), CurrentData.AppMode + "\\" + "Articles" + "\\" + DateTime.Now.AddDays(-i).ToString("M.y"));
                if (previouslyDayList != null && previouslyDayList.Count != 0)
                    lastOpList.InsertRange(0, previouslyDayList);
            }

            for (int i = 0; i < lastOpList.Count; i++)
                if (lastOpList[i].NameID == 0)
                {
                    lastOpList.RemoveAt(i);
                    i--;
                }

            Dictionary<int, double> OutlayDict = new Dictionary<int, double>();
            Dictionary<int, double> IncomeDict = new Dictionary<int, double>();

            CreateDictionaties(lastOpList, OutlayDict, IncomeDict);

            foreach (ArticleProperty aP in CurrentData.ArtPropList("Income"))
                if (IncomeDict.ContainsKey(aP.ID))
                    aP.Summ = IncomeDict[aP.ID];

            foreach (ArticleProperty aP in CurrentData.ArtPropList("Income"))
                if (OutlayDict.ContainsKey(aP.ID))
                    aP.Summ = OutlayDict[aP.ID];
        }

        #region Add&SetSumms
        private static double SetSummToAccount(double Summ, int accountID)
        {
            //Присваивает счёту с указанным ID сумму Summ и возвращает разницу
            double difference=0;
            for (int n = 0; n < (int)CurrentData.ArtAccountList.AccountsList.LongCount(); n++)
            {
                try
                {
                    if (CurrentData.ArtAccountList.AccountsList[n].ID == accountID)
                    {
                        difference = Summ - CurrentData.ArtAccountList.AccountsList[n].Summ;
                        CurrentData.ArtAccountList.AccountsList[n].Summ = Summ;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
            return difference;
        }

        private static void AddSummToIncomeOrOutlayProperty(double Summ, bool isIncome, int ID)
        {
            //Вычитает или прибавляет Summ к сумме счета с указанным ID, в зависимости от isIncome
            string property;
            if (isIncome)
                property = "Income";
            else
                property = "Outlay";

            for (int n = 0; n < (int)CurrentData.ArtPropList(property).LongCount(); n++)
            {
                try
                {
                    if (CurrentData.ArtPropList(property)[n].ID == ID)
                    {
                       CurrentData.ArtPropList(property)[n].Summ += Summ;                       
                        break;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
            ArticlePropertyListSerialize(property, CurrentData.ArtPropList(property));
        }

        private static void AddSummToAccountProperty(double Summ, bool isIncome, int ID)
        {
            //Вычитает или прибавляет Summ к сумме счета с указанным ID, в зависимости от isIncome
            for (int n = 0; n < (int)CurrentData.ArtAccountList.AccountsList.LongCount(); n++)
            {
                try
                {
                    if (CurrentData.ArtAccountList.AccountsList[n].ID == ID)
                    {
                        if (isIncome)
                            CurrentData.ArtAccountList.AccountsList[n].Summ += Summ;
                        else
                            CurrentData.ArtAccountList.AccountsList[n].Summ -= Summ;
                        break;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
        }
        #endregion

        #region AssignmentID
        private static void AssignmentArticleAccountID(List<Article> aL)
        {//Присвоение имен и счетов по ID
            foreach (Article a in aL)
                for (int n = 0; n < CurrentData.ArtAccountList.AccountsList.Count(); n++)
                    if (CurrentData.ArtAccountList.AccountsList[n].ID == a.AccountID)
                    {
                        a.Account = CurrentData.ArtAccountList.AccountsList[n].Name;
                        break;
                    }
        }

        private static void AssignmentArticleIncomeOrOutlayID(List<Article> aL, string property)
        {//Присвоение имен и счетов по ID
            foreach (Article a in aL)
                for (int n = 0; n < CurrentData.ArtPropList(property).Count(); n++)
                    if (CurrentData.ArtPropList(property)[n].ID == a.NameID)
                    {
                        a.Name = CurrentData.ArtPropList(property)[n].Name;
                        break;
                    }
        }
        #endregion

        #region SearchFreID
        public static int SearchFreeID(string articleMode, ObservableRangeCollection<ArticleProperty> aPropList)
        {
            int minusOne;
            if (articleMode == "Outlay")
                minusOne = -1;
            else minusOne = 1;
            int id;
            for (int i = 1; ; i++)
            {
                try
                {
                    id = ((int)aPropList.LongCount() + i) * minusOne;
                }
                catch (ArgumentNullException)
                {
                    return 1 * minusOne;
                }
                if ((int)aPropList.LongCount() == 0)
                    return 1 * minusOne;
                for (int n = 0; n < (int)aPropList.LongCount(); n++)
                {
                    if (aPropList[n].ID == id)
                        break;
                    if (n == (int)aPropList.LongCount() - 1)
                        return id;
                }
            }
        }

        public static int SearchFreeID(List<Article> aList)
        {
            int id;
            for (int i = 1; ; i++)
            {
                try
                {
                    id = (int)aList.LongCount() + i;
                }
                catch (ArgumentNullException)
                {
                    return 1;
                }
                if ((int)aList.LongCount() == 0)
                    return 1;
                for (int n = 0; n < (int)aList.LongCount(); n++)
                {
                    if (aList[n].DateID == id)
                        break;
                    if (n == (int)aList.LongCount() - 1)
                        return id;
                }
            }
        }
        #endregion

        private static void SearchAndRemoveArticle(List <Article> artList, Article article)
        {
            foreach (Article a in artList)
                if (a.DateID == article.DateID)
                {
                    artList.Remove(a);
                    return;
                }
        }

        #region DeleteOperations
        public static void DeleteToDayArts()
        {
            var ISD = new IsolatedStorageDeserializer<List<Article>>();

            var artList = ISD.XmlDeserialize(DateTime.Today.ToString("d.M.y"), CurrentData.AppMode + "\\" + "Articles" 
                + "\\" + DateTime.Today.ToString("M.y"));
            if (artList == null)
            {
                artList = new List<Article>();
                return;
            }

            foreach (Article a in artList)
            {
                AddSummToAccountProperty(- a.Number, a.IsIncome, a.AccountID);
                AddSummToIncomeOrOutlayProperty(- a.Number, a.IsIncome, a.NameID);
            }

            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (!storage.DirectoryExists(CurrentData.AppMode + "\\" + "Articles\\" + DateTime.Today.ToString("M.y")))
                storage.CreateDirectory(CurrentData.AppMode + "\\" + "Articles\\" + DateTime.Today.ToString("M.y"));

            if (storage.FileExists(CurrentData.AppMode + "\\" + "Articles\\" + DateTime.Today.ToString("M.y") 
                + "\\" + DateTime.Today.ToString("d.M.y") + ".xml"))
                storage.DeleteFile(CurrentData.AppMode + "\\" + "Articles\\" + DateTime.Today.ToString("M.y") 
                    + "\\" + DateTime.Today.ToString("d.M.y") + ".xml");

            CurrentData.LastOperationsList = new List<Article>();
        }

        public static void DeleteArt(Article a)
        {
            var ISD = new IsolatedStorageDeserializer<List<Article>>();

            var artList = ISD.XmlDeserialize(a.ADateTime.ToString("d.M.y"), CurrentData.AppMode + "\\" + "Articles" + "\\" + a.ADateTime.ToString("M.y"));
            if (artList == null)
                artList = new List<Article>();

            SearchAndRemoveArticle(artList, a);

            ISD.XmlSerialize(artList, a.ADateTime.ToString("d.M.y"), true, CurrentData.AppMode + "\\" + "Articles" + "\\" + a.ADateTime.ToString("M.y"));

            CurrentData.LastOperationsList = new List<Article>();

            AddSummToAccountProperty(- a.Number, a.IsIncome, a.AccountID);
            AddSummToIncomeOrOutlayProperty(-a.Number, a.IsIncome, a.NameID);
        }
        #endregion

        #region Serialize&DeserializeOperations
        private static void ArtSerialize(Article art)
        {
            IsolatedStorageDeserializer<List<Article>> ISD = new IsolatedStorageDeserializer<List<Article>>();
            var articleList = new List<Article>();

            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (!storage.DirectoryExists(CurrentData.AppMode + "\\" + "Articles\\" + art.ADateTime.ToString("M.y")))
                storage.CreateDirectory(CurrentData.AppMode + "\\" + "Articles\\" + art.ADateTime.ToString("M.y"));

            int id = 1;
            if (storage.FileExists(CurrentData.AppMode + "\\" + "Articles\\" + art.ADateTime.ToString("M.y") + "\\" + art.ADateTime.ToString("d.M.y") + ".xml"))
            {
                articleList = ISD.XmlDeserialize(art.ADateTime.ToString("d.M.y"), CurrentData.AppMode + "\\" + "Articles" + "\\" + art.ADateTime.ToString("M.y"));
                if (articleList == null)
                    articleList = new List<Article>();
                else
                    id = SearchFreeID(articleList);
            }
            art.DateID = id;
            string property;
            if (art.IsIncome)
                property = "Income";
            else
                property = "Outlay";

            articleList.Add(art);

            AssignmentArticleAccountID(articleList);
            AssignmentArticleIncomeOrOutlayID(articleList, property);

            ISD.XmlSerialize(articleList, art.ADateTime.ToString("d.M.y"), true, CurrentData.AppMode + "\\" + "Articles" + "\\" + art.ADateTime.ToString("M.y"));
        } 

        public static void ArticlePropertyListSerialize(string property, ObservableRangeCollection<ArticleProperty> ArtPropList)
        {
            var ISD = new IsolatedStorageDeserializer<ObservableRangeCollection<ArticleProperty>>();
            ISD.XmlSerialize(ArtPropList, property, true,  CurrentData.AppMode);
        }

        public static ObservableRangeCollection<ArticleProperty> ArticlePropertyListDeserialize(string property)
        {
            var ArtPropList = new ObservableRangeCollection<ArticleProperty>();

            var ISD = new IsolatedStorageDeserializer<ObservableRangeCollection<ArticleProperty>>();

            ArtPropList = ISD.XmlDeserialize(property,  CurrentData.AppMode);
            if (ArtPropList == null)
                switch (property)
                {
                    case "Income":
                        ArtPropList = new ObservableRangeCollection<ArticleProperty>
                        {
                            new ArticleProperty("Зарплата",1),
                            new ArticleProperty("Подработка",2),
                        };
                        break;
                    case "Outlay":
                        ArtPropList = new ObservableRangeCollection<ArticleProperty>
                        {
                            new ArticleProperty("Продукты",-1),
                            new ArticleProperty("За квартиру",-2),
                        };
                        break;
                    default:
                        ArtPropList = new ObservableRangeCollection<ArticleProperty>();
                        break;
                }
            return ArtPropList;
        }
       
        public static ObservableRangeCollection<AccountsObservableRangeCollection> GetPreviouslyAccountsList(int monthNumberToPast = 0)
        {
            var ISD = new IsolatedStorageDeserializer<ObservableRangeCollection<AccountsObservableRangeCollection>>();

            var PrevAccLists = ISD.XmlDeserialize(DateTime.Today.AddMonths(-monthNumberToPast).ToString("M.y"), CurrentData.AppMode + "\\" + "AccountStatuses");

            if (PrevAccLists != null)
                return PrevAccLists;                
            else
                return new ObservableRangeCollection<AccountsObservableRangeCollection>();
        }
        public static void SetCurrentAccountsStatusToList()
        {//Решить проблему с годом(потом)
            var ISD = new IsolatedStorageDeserializer<ObservableRangeCollection<AccountsObservableRangeCollection>>();

            CurrentData.ArtAccountList.SetRecordDayStr();

            ISD.XmlSerialize(CurrentData.AccountsListsAtThisMonth, CurrentData.ArtAccountList.GetRecordDay().ToString("M.y")
                , true, CurrentData.AppMode + "\\" + "AccountStatuses");
        }

        public static void SetCurrentAccountsStatusToList(double n)
        {//Для отладки
            var ISD = new IsolatedStorageDeserializer<ObservableRangeCollection<AccountsObservableRangeCollection>>();

            var listForDeserialize = new ObservableRangeCollection<AccountsObservableRangeCollection>();
            listForDeserialize.Add(new AccountsObservableRangeCollection(CurrentData.ArtAccountList.AccountsList
                                , (new AccountsObservableRangeCollection()).SetRecordDayStr((int)n)));

            ISD.XmlSerialize(listForDeserialize, DateTime.Today.AddDays(-(int)n).ToString("M.y"), true, CurrentData.AppMode + "\\" + "AccountStatuses");
        }         
        #endregion
    }
}
