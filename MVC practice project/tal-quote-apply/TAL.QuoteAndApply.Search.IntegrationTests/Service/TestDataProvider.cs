using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Search.Models;

namespace TAL.QuoteAndApply.Search.IntegrationTests.Service
{
    public class TestDataProvider
    {
        private static readonly Random Random = new Random();

        public static string RandomString(int size)
        {
            string input = "abcdefghijklmnopqrstuvwxyz";
            var chars = Enumerable.Range(0, size)
                                   .Select(x => input[Random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }

        public static List<QuestionAnswerData<AnswerSearchItemDto>> GetEnglishWordData()
        {
            var lines = File.ReadAllLines("wordsEn.txt");
            return lines.Select((word, index) => new QuestionAnswerData<AnswerSearchItemDto>()
            {
                AnswerId = "1",
                Id = index.ToString(),
                RawData = new AnswerSearchItemDto()
                {
                    Text = word,
                    ResponseId = index.ToString()
                },
                Similies = BuildSimilies(word),
                Value = word
            }).ToList();
        }

        private static string BuildSimilies(string name)
        {
            var finalName = name.Replace("/", " ");
            string list = "";
            string previous = "";
            foreach (var ch in finalName.ToLower().ToCharArray())
            {
                previous += ch;
                list += " " + previous;
            }
            return string.Join(" ", list.Split(' ').Distinct());
        }

        public static List<QuestionAnswerData<AnswerSearchItemDto>> FetDataMappedFromAnswerDto(IEnumerable<AnswerSearchItemDto> data)
        {
            return data.Select((rawData, index) =>
                new QuestionAnswerData<AnswerSearchItemDto>()
                {
                    AnswerId = index.ToString(),
                    Id = index.ToString(),
                    RawData = rawData,
                    Similies = "A Ac Acc Acco Accou Accoun Account Accounta Accountan Accountant",
                    Value = "Accountant"
                }
                ).ToList();
        }

        public static List<QuestionAnswerData<AnswerSearchItemDto>> GetSampleSearchData()
        {
            return new List<QuestionAnswerData<AnswerSearchItemDto>>
            {
                new QuestionAnswerData<AnswerSearchItemDto>()
                {
                    AnswerId = "1",
                    Id = 1.ToString(),
                    RawData = new AnswerSearchItemDto()
                    {
                        Text = "Accountant"
                    },
                    Similies = "A Ac Acc Acco Accou Accoun Account Accounta Accountan Accountant",
                    Value = "Accountant"
                },
                new QuestionAnswerData<AnswerSearchItemDto>()
                {
                    AnswerId = "1",
                    Id = 1.ToString(),
                    RawData = new AnswerSearchItemDto()
                    {
                        Text = "Software Developer"
                    },
                    Similies =
                        "S So Sof Soft Softw Softwa Softwar Software D De Dev Deve Devel Develo Develop Develope Developer Software Developer",
                    Value = "Software Developer"
                }
            };
        }
    }
}