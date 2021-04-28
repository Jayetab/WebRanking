
using System;
using System.Collections.Generic;
using System.ComponentModel;
using WPFCustomMessageBox;
using System.Windows;
using System.Windows.Input;
using WebRanking.BAL;
using WebRanking.Common;
using WebRanking.Model;

namespace WebRanking.ViewModel
{
    public class WebRankViewModel : INotifyPropertyChanged, IDataErrorInfo
    {

        private readonly WebRank webRank;
        private readonly WebRankManager webRankManager;
        private readonly ICommand getWebRankCmd;

        public WebRankViewModel()
        {
            webRank = new WebRank();
            webRankManager = new WebRankManager(webRank);
            getWebRankCmd = new RelayCommand(GetWebRank, CanGetWebRank);
        }


        #region Properties

        public string SearchKeyWord
        {
            get { return webRank.SearchKeyWord; }
            set
            {
                webRank.SearchKeyWord = value;
                OnPropertyChanged("SearchKeyWord");
            }
        }

        public string SearchURL
        {
            get { return webRank.SearchURL; }
            set
            {
                webRank.SearchURL = value;
                OnPropertyChanged("SearchURL");
            }
        }

        public string SearchResult
        {
            get { return webRank.SearchResult; }
            set
            {
                webRank.SearchResult = value;
                OnPropertyChanged("SearchResult");
            }
        }

        #endregion

        #region Commands

        public ICommand GetWebRankCmd { get { return getWebRankCmd; } }

        private bool CanGetWebRank(object obj)
        {
            return !string.IsNullOrEmpty(SearchKeyWord) && !string.IsNullOrEmpty(SearchURL);
        }

        private void GetWebRank(object obj)
        {
            if (CanGetWebRank(obj))
            {
                var result = new List<int>();
                var errorMessage = "";
                try
                {
                    webRankManager.GetWebRank();
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
                finally
                {

                }
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    CustomMessageBox.ShowOK(errorMessage, "Error", "OK", MessageBoxImage.Error);
                }
                else if (string.IsNullOrEmpty(webRank.SearchResult))
                {
                    CustomMessageBox.ShowOK("The URL does not appear in the current search", "Result", "OK", MessageBoxImage.Information);
                }
                else
                {
                    SearchKeyWord = webRank.SearchKeyWord;
                    SearchURL = webRank.SearchURL;
                    SearchResult = webRank.SearchResult;
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case "SearchKeyWord":
                        if (string.IsNullOrEmpty(SearchKeyWord))
                        {
                            result = "Please enter a Search Key Word";
                        }
                        break;
                    case "SearchURL":
                        if (string.IsNullOrEmpty(SearchURL))
                        {
                            result = "Please enter a Search URL";                            
                        }
                        break;
                }
                return result;
            }
        }

        #endregion
    }
}
