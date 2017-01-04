using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

namespace RatesScriptGenerator
{
    public class Program_OLD
    {
        private static Dictionary<string, int>  _premiumTypeLookup = new Dictionary<string, int>()
            {
                {"Stepped", 1},
                {"Level", 2}
            };

        private static Dictionary<char, int> _genderLookup = new Dictionary<char, int>()
            {
                {'M',  1},
                {'F', 2}
            };

        //public static void Main(string[] args)
        //{
        //    var ipPlanTypeId = 4;
        //    var accCoverTypeId = 1;
        //    var illCoverTypeId = 2;

        //    var srcDir = @"C:\ilsource\tal-quote-apply\Tools\";

        //    var allInserts = new List<string>();

        //    using (var reader = File.OpenText(srcDir + "IP_DUMMY_ALL.csv"))
        //    {
        //        var csv = new CsvReader(reader);
        //        var records = csv.GetRecords<BaseRate>();

        //        allInserts.AddRange(GetPlanInserts(records, ipPlanTypeId));
        //    }

        //    using (var reader = File.OpenText(srcDir + "IP_DUMMY_ACC.csv"))
        //    {
        //        var csv = new CsvReader(reader);
        //        var records = csv.GetRecords<BaseRate>();

        //        allInserts.AddRange(GetCoverInserts(records, ipPlanTypeId, accCoverTypeId));
        //    }

        //    using (var reader = File.OpenText(srcDir + "IP_DUMMY_ILL.csv"))
        //    {
        //        var csv = new CsvReader(reader);
        //        var records = csv.GetRecords<BaseRate>();

        //        allInserts.AddRange(GetCoverInserts(records, ipPlanTypeId, illCoverTypeId));
        //    }

        //    File.WriteAllLines(srcDir + "IP_DUMMY_RATES_INSERT.sql", allInserts);

        //    Console.WriteLine("------DONE------");
        //    Console.ReadLine();           
        //}

        private static List<string> GetPlanInserts(IEnumerable<BaseRate> baseRates, int planTypeId)
        {
            var planBaseRateInsertTemplate = "INSERT INTO [PlanBaseRate] ([PlanTypeId],[Age],[GenderId],[PremiumTypeId],[OccupationGroup],[BenefitPeriod],[BaseRate],[CreatedBy],[CreatedTS])  VALUES({0}, {1}, {2}, {3}, '{4}', {5}, {6}, 'SYSTEM', GETDATE())";

            return baseRates.Select(br => string.Format(planBaseRateInsertTemplate, planTypeId, br.Age, _genderLookup[br.Gender], _premiumTypeLookup[br.PremiumType], br.Occ, br.BenefitPeriod, br.Rate)).ToList();
        }

        private static List<string> GetCoverInserts(IEnumerable<BaseRate> baseRates, int planTypeId, int coverTypeId)
        {
            var coverBaseRateInsertTemplate = "INSERT INTO [CoverBaseRate] ([PlanTypeId],[CoverTypeId],[Age],[GenderId],[PremiumTypeId],[OccupationGroup],[BenefitPeriod],[BaseRate],[CreatedBy],[CreatedTS])  VALUES({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7}, 'SYSTEM', GETDATE())";

            return baseRates.Select(br => string.Format(coverBaseRateInsertTemplate, planTypeId, coverTypeId, br.Age, _genderLookup[br.Gender], _premiumTypeLookup[br.PremiumType], br.Occ, br.BenefitPeriod, br.Rate)).ToList();
        }
    }

    public class BaseRate
    {
        public int Age { get; set; }
        public string PremiumType { get; set; }
        public string Occ { get; set; }
        public char Gender { get; set; }
        public int BenefitPeriod { get; set; }
        public decimal Rate { get; set; }
    }
}
