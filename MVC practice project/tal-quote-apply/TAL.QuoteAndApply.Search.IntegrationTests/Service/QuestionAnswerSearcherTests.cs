using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.Search.Configuration;
using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Search.LuceneWrapper;
using TAL.QuoteAndApply.Search.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Search.IntegrationTests.Service
{
    [TestFixture]
    public class QuestionAnswerSearcherTests
    {
        private LuceneParameters GetParamsForTest()
        {
            return LuceneParameters.ForSql(ConfigurationManager.AppSettings["Search.IndexRoot"], "TestIndex",
                ConfigurationManager.AppSettings["Search.SqlConnectionString"], ConfigurationManager.AppSettings["Search.Database"]);
        }

        [Test, Ignore]
        public void TestSearch_PerformanceTest_MultipleSearches()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            FileSystemHelper.SetupAndCleanDirectory(@"c:\ilsource\tal.qap.test.index");
            var stopWatch = new Stopwatch();

            var searchData = TestDataProvider.GetEnglishWordData();
            var luceneParameters = GetParamsForTest();

            var repo = new AnswerSearchItemRepository(new SearchConfigurationProvider(), new MockCurrentUserProvider(),
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var questionAnswerWriterFactory = new QuestionAnswerWriterFactory(repo);
            var writer = questionAnswerWriterFactory.Create(luceneParameters);
            writer.AddUpdateSampleDataToIndex(searchData, searchData.Select(x => x.RawData).ToList());
            var searcher = new QuestionAnswerSearcher<AnswerSearchItemDto>(luceneParameters, searchData, searchData.Select(x => x.RawData));

            var maxTestRuns = 2000;
            var hits = 0;
            for (var i = 0; i < maxTestRuns; i ++)
            {
                var searchTerm = TestDataProvider.RandomString(3);
                stopWatch.Start();
                var result = searcher.SearchAnswers(searchTerm);
                stopWatch.Stop();
                hits = (result.Hits > 0) ? hits + 1 : hits;
            }

            Console.WriteLine("time taken to search {0} times: {1} ms ({2} +0 results)", maxTestRuns, stopWatch.ElapsedMilliseconds, hits);

            searcher.Dispose();
        }

        [Test, Ignore]
        public void TestSearcher_WithCleanIndex_UsingEngishWords()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            FileSystemHelper.SetupAndCleanDirectory(@"c:\ilsource\tal.qap.test.index");
            var stopWatch = new Stopwatch();

            var searchData = TestDataProvider.GetEnglishWordData();
            var luceneParameters = GetParamsForTest();

            var repo = new AnswerSearchItemRepository(new SearchConfigurationProvider(), new MockCurrentUserProvider(),
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            stopWatch.Start();
            var questionAnswerWriterFactory = new QuestionAnswerWriterFactory(repo);
            var writer = questionAnswerWriterFactory.Create(luceneParameters);
            writer.AddUpdateSampleDataToIndex(searchData, searchData.Select(x => x.RawData).ToList());
            var searcher = new QuestionAnswerSearcher<AnswerSearchItemDto>(luceneParameters, searchData, searchData.Select(x => x.RawData));
            stopWatch.Stop();
            
            Console.WriteLine("Time to build Index: {0} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();

            stopWatch.Start();
            var results = searcher.SearchAnswers("abandon");
            stopWatch.Stop();

            Console.WriteLine("Time to search index: {0} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();

            stopWatch.Start();
            var resultsList = results.SearchResultItems
                .OrderByDescending(x => x.Score)
                .Select(searchResultItem => searchData.First(answer => answer.Id == searchResultItem.Id))
                .ToList();
            stopWatch.Stop();

            Console.WriteLine("Time to map index results to data: {0} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();

            foreach (var result in resultsList)
            {
                Console.WriteLine(result.RawData.Text); 
            }

            Assert.That(resultsList.Count, Is.EqualTo(10));
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandon", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandoned", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandonee", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandoner", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandoners", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandoning", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandons", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandonedly", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandonment", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandonments", StringComparison.OrdinalIgnoreCase)), Is.True);

            searcher.Dispose();
        }

        [Test, Ignore]
        public void TestSearcher_ReuseExistingIndex_UsingEngishWords()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            FileSystemHelper.SetupAndCleanDirectory(@"c:\ilsource\tal.qap.test.index");
            var stopWatch = new Stopwatch();

            var searchData = TestDataProvider.GetEnglishWordData();
            var luceneParameters = GetParamsForTest();

            var repo = new AnswerSearchItemRepository(new SearchConfigurationProvider(), new MockCurrentUserProvider(),
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));


            stopWatch.Start();
            var questionAnswerWriterFactory = new QuestionAnswerWriterFactory(repo);
            var writer = questionAnswerWriterFactory.Create(luceneParameters);
            writer.AddUpdateSampleDataToIndex(searchData, searchData.Select(x => x.RawData).ToList());

            var questionAnswerReaderFactory = new QuestionAnswerReaderFactory(repo);
            var reader = questionAnswerReaderFactory.Create(luceneParameters);

            var loadedSearchData = TestDataProvider.FetDataMappedFromAnswerDto(reader.RawAnswerData);

            var searcher = new QuestionAnswerSearcher<AnswerSearchItemDto>(luceneParameters, loadedSearchData, loadedSearchData.Select(x => x.RawData));
            stopWatch.Stop();

            Console.WriteLine("Time to build Index: {0} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();

            stopWatch.Start();
            var results = searcher.SearchAnswers("abandon");
            stopWatch.Stop();

            Console.WriteLine("Time to search index: {0} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();

            stopWatch.Start();
            var resultsList = results.SearchResultItems
                .OrderByDescending(x => x.Score)
                .Select(searchResultItem => searchData.First(answer => answer.Id == searchResultItem.Id))
                .ToList();
            stopWatch.Stop();

            Console.WriteLine("Time to map index results to data: {0} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();

            foreach (var result in resultsList)
            {
                Console.WriteLine(result.RawData.Text);
            }

            Assert.That(resultsList.Count, Is.EqualTo(10));
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandon", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandoned", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandonee", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandoner", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandoners", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandoning", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandons", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandonedly", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandonment", StringComparison.OrdinalIgnoreCase)), Is.True);
            Assert.That(resultsList.Any(r => r.RawData.Text.Equals("abandonments", StringComparison.OrdinalIgnoreCase)), Is.True);

            searcher.Dispose();
        }
    }
}
