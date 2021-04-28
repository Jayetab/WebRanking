using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WebRanking.BAL;
using WebRanking.Model;
using WebRanking.ViewModel;

namespace WebRankingTest
{
    [TestClass]
    public class UnitTestWebRank
    {
        [TestMethod]
        public void TestWebRankManagerForTestDataSet1()
        {
            var httpResponse = System.IO.File.ReadAllText("conveyancingsoftwareGoogleSearch.txt");
            var result = CallGetWebRank("conveyancing software", "https://www.leap.com.au", httpResponse);
            Assert.IsTrue(result == "1", "WebRank result does not match as expected");
        }

        [TestMethod]
        public void TestWebRankManagerForTestDataSet2()
        {
            var httpResponse = System.IO.File.ReadAllText("conveyancingsoftwareGoogleSearch.txt");
            var result = CallGetWebRank("conveyancing software", "https://www.leapconveyancer.com.au/", httpResponse);
            Assert.IsTrue(result == "0", "WebRank result does not match as expected");
        }

        [TestMethod]
        public void TestWebRankManagerForTestDataSet3()
        {
            var httpResponse = System.IO.File.ReadAllText("conveyancingsoftwareGoogleSearch.txt");
            var result = CallGetWebRank("conveyancing software", "https://www.leap", httpResponse);
            Assert.IsTrue(result == "0 , 1 , 16", "WebRank result does not match as expected");
        }

        [TestMethod]
        public void TestWebRankManagerForTestDataSet4()
        {
            var httpResponse = System.IO.File.ReadAllText("LegalPracticeManagementGoogleSearch.txt");
            var result = CallGetWebRank("legal practice management", "https://www.smokeball.com.au", httpResponse);
            Assert.IsTrue(result == "36", "WebRank result does not match as expected");
        }

        [TestMethod]
        public void TestWebRankManagerForEmptySearchKeyWord()
        {
            var httpResponse = System.IO.File.ReadAllText("LegalPracticeManagementGoogleSearch.txt");
            try
            {
                var result = CallGetWebRank("", "https://www.smokeball.com.au", httpResponse);
                Assert.IsTrue(string.IsNullOrEmpty(result), "WebRank result does not match as expected");
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
            

        [TestMethod]
        public void TestWebRankManagerForEmptySearchURL()
        {
            var httpResponse = System.IO.File.ReadAllText("LegalPracticeManagementGoogleSearch.txt");
            try
            {
                var result = CallGetWebRank("conveyancing software", "", httpResponse);
                Assert.IsTrue(string.IsNullOrEmpty(result), "WebRank result does not match as expected");
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        public string CallGetWebRank(string searchKeyWord, string searchURL, string httpResponse)
        {
            var webrank = new WebRank();
            webrank.SearchKeyWord = searchKeyWord;
            webrank.SearchURL = searchURL;

            var mockWebRankManager = new MockWebRankManager(webrank, httpResponse);
            mockWebRankManager.GetWebRank();
            return webrank.SearchResult;
        }

        [TestMethod]
        public void TestEmptySearchKeyWordError()
        {
            var webRankViewModel = new WebRankViewModel();
            webRankViewModel.SearchKeyWord = "";
            webRankViewModel.SearchURL = "https://www.smokeball.com.au";
            var error = webRankViewModel["SearchKeyWord"];
            Assert.AreEqual(error, "Please enter a Search Key Word");

            webRankViewModel.SearchKeyWord = "conveyancing software";
            error = webRankViewModel["SearchKeyWord"];
            Assert.IsTrue(string.IsNullOrEmpty(error));
        }

        [TestMethod]
        public void TestEmptySearchURLError()
        {
            var webRankViewModel = new WebRankViewModel();
            webRankViewModel.SearchKeyWord = "conveyancing software";
            webRankViewModel.SearchURL = "";
            var error = webRankViewModel["SearchURL"];
            Assert.AreEqual(error, "Please enter a Search URL");

            webRankViewModel.SearchURL = "https://www.smokeball.com.au";
            error = webRankViewModel["SearchURL"];
            Assert.IsTrue(string.IsNullOrEmpty(error));

        }
    }

    public class MockWebRankManager : WebRankManager
    {
        public MockWebRankManager(WebRank webRank, string httpResponse) : base(webRank)
        {
            HttpResponse = httpResponse;
        }

        string HttpResponse;

        protected override string GetHttpResponse()
        {
            return HttpResponse;
        }
    }
}
