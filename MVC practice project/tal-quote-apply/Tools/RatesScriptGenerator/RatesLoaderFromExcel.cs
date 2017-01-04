using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;

namespace RatesScriptGenerator
{
    public class Options
    {
        public const string PARAM_DATE_FORMAT = "dd-MM-yyyy";

        [Option('f', "file", Required = true)]
        public string RatesAlgorithmFile { get; set; }

        [Option('b', "brand-id", Required = true)]
        public int BrandId { get; set; }

        [Option('r', "factor", DefaultValue = "1", Required = false)]
        public string RatingAlterFactor { get; set; }
    }

    public  class RatesLoaderFromExcel
    {
        public static void Main(string[] args)
        {
            var options = new Options();
            var result = CommandLine.Parser.Default.ParseArguments(args, options);

            if (!result)
            {
                Environment.Exit(1);
            }

            var outputFolder = @"C:\ilsource\tal-quote-apply\Tools\output\" + $"brand-{options.BrandId}";

            EnsureFolder(outputFolder);

            ProcessSheet(options.RatesAlgorithmFile, options.BrandId, decimal.Parse(options.RatingAlterFactor), $@"{outputFolder}\01-INSERT LIFE RATES.sql", "Rates - Term", "N1:AF107");
            ProcessSheet(options.RatesAlgorithmFile, options.BrandId, decimal.Parse(options.RatingAlterFactor), $@"{outputFolder}\02-INSERT TPD RIDER RATES.sql", "Rates - TPDRider", "X1:BJ107");
            ProcessSheet(options.RatesAlgorithmFile, options.BrandId, decimal.Parse(options.RatingAlterFactor), $@"{outputFolder}\03-INSERT CI RIDER RATES.sql", "Rates - CIRider", "X1:CD107");
            ProcessSheet(options.RatesAlgorithmFile, options.BrandId, decimal.Parse(options.RatingAlterFactor), $@"{outputFolder}\04-INSERT TPD RATES.sql", "Rates - TPDSA", "N1:AF107");
            ProcessSheet(options.RatesAlgorithmFile, options.BrandId, decimal.Parse(options.RatingAlterFactor), $@"{outputFolder}\05-INSERT CI RATES.sql", "Rates - CISA", "N1:AP107");
            ProcessSheet(options.RatesAlgorithmFile, options.BrandId, decimal.Parse(options.RatingAlterFactor), $@"{outputFolder}\06-INSERT IP RATES.sql", "Rates - DI", "DT1:MX107");

            ProcessDoaSheet(options.RatesAlgorithmFile, options.BrandId, decimal.Parse(options.RatingAlterFactor), $@"{outputFolder}\07-INSERT DOA RATES.sql", "Rates - DOA", "D1:L107");


            Console.WriteLine("DONE!");
            Console.ReadLine();
        }

        private static void EnsureFolder(string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(outputFolder);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
        }

        private static void ProcessSheet(string inputFile, int brandId, decimal factor, string outputFile, string sheetName, params string[] sheetRanges)
        {
            Console.WriteLine($"------{sheetName}------");
            var data = GetData(inputFile, sheetName, sheetRanges);
            var records = ProcessDataTable(data, brandId, factor);

            var generator = new SqlInsertGenerator();

            var outputLines = records.Select(cbr => generator.Generate(cbr));

            File.WriteAllLines(outputFile, outputLines);
        }

        private static void ProcessDoaSheet(string inputFile, int brandId, decimal factor, string outputFile, string sheetName, params string[] sheetRanges)
        {
            var data = GetData(inputFile, sheetName, sheetRanges);
            var records = ProcessDataTable(data, brandId, factor);

            var doaRecords = records.Select(c=> new DayOneAccidentBaseRate
            {
                BrandId = brandId,
                PlanCode = c.PlanCode,
                CoverCode = c.CoverCode,
                Age = c.Age,
                BaseRate = c.BaseRate,
                GenderId = c.GenderId,
                PremiumTypeId = c.PremiumTypeId,
                WaitingPeriod = c.WaitingPeriod.Value
            });

            var generator = new SqlInsertGenerator();

            var outputLines = doaRecords.Select(doa => generator.Generate(doa));

            File.WriteAllLines(outputFile, outputLines);
        }

        private static IEnumerable<CoverBaseRate> ProcessDataTable(DataTable data, int brandId, decimal factor)
        {
            var returnData = new List<CoverBaseRate>();

            const int coverFactorStartRow = 1;

            const int ratesStartRow = 22;
            const int ratesEndRow = 105;

            string[] knownCovers = new[] {"ACC", "SIC", "CANCER"};

            foreach (DataColumn col in data.Columns)
            {
                var columnValue = data.Rows[coverFactorStartRow].Field<string>(col.ColumnName);

                Console.WriteLine($"Working on {col.ColumnName} with col value: {columnValue}");

                if (knownCovers.Contains(columnValue))
                {
                    var coverCode = columnValue;
                    var planCode = data.Rows[coverFactorStartRow + 1].Field<string>(col.ColumnName);
                    var premiumType = data.Rows[coverFactorStartRow + 2].Field<string>(col.ColumnName);
                    var gender = data.Rows[coverFactorStartRow + 3].Field<string>(col.ColumnName);
                    var smokerStatus = data.Rows[coverFactorStartRow + 4].Field<string>(col.ColumnName);
                    var occupation = data.Rows[coverFactorStartRow + 5].Field<string>(col.ColumnName);
                    var buyBack = data.Rows[coverFactorStartRow + 6].Field<string>(col.ColumnName);
                    var waitingPeriod = data.Rows[coverFactorStartRow + 7].Field<string>(col.ColumnName);
                    var benefitPeriod = data.Rows[coverFactorStartRow + 8].Field<string>(col.ColumnName);

                    Console.WriteLine($"Processing on {planCode}:{coverCode}");

                    var ageCounter = 15;
                    for (var i = ratesStartRow; i <= ratesEndRow; i++)
                    {
                        ageCounter++;

                        var rate = data.Rows[i].Field<string>(col.ColumnName);

                        if (rate == null)
                            continue;

                        var coverBaseRate = new CoverBaseRate
                        {
                            BrandId = brandId,
                            Age = ageCounter,
                            BaseRate = decimal.Parse(rate) * factor,
                            BenefitPeriod = StringToNullableInt(benefitPeriod),
                            BuyBack = ConvertBuyBack(buyBack),
                            CoverCode = ConvertCoverCode(planCode, coverCode),
                            GenderId = ConvertGender(gender),
                            IsSmoker = ConvertSmoker(smokerStatus),
                            OccupationGroup = CleanString(occupation),
                            PlanCode = ConvertPlanCode(planCode),
                            PremiumTypeId = ConvertPremiumType(premiumType),
                            WaitingPeriod = StringToNullableInt(waitingPeriod)
                        };

                        returnData.Add(coverBaseRate);
                    }
                    
                }
            }

            return returnData;
        }

        private static int ConvertPremiumType(string premiumType)
        {
            if (premiumType.Equals("STEPPED", StringComparison.InvariantCultureIgnoreCase))
            {
                return 1;
            }

            return 2;
        }

        private static string CleanString(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            if (val.Equals("-"))
            {
                return null;
            }

            return val;
        }

        private static bool? ConvertSmoker(string smoker)
        {
            if (smoker.Equals("NS", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (smoker.Equals("S", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return null;
        }

        private static int ConvertGender(string gender)
        {
            if (gender.Equals("M", StringComparison.InvariantCultureIgnoreCase))
            {
                return 1;
            }

            return 2;
        }

        private static Dictionary<string, string> PlanMapping = new Dictionary<string, string>
        {
            {"TERM","DTH"},
            {"TPDRIDER","TPDDTH"},
            {"CIRIDER","TRADTH"},
            {"CISA","TRS"},
            {"TPDSA","TPS"},
            {"DI","IP"},
        };

        private static Dictionary<string, string> PlanCoverMapping = new Dictionary<string, string>
        {
            {"TERMACC","DTHAC"},
            {"TERMSIC","DTHIC"},
            {"TPDRIDERACC","TPDDTHAC"},
            {"TPDRIDERSIC","TPDDTHIC"},
            {"TPDSAACC","TPSAC"},
            {"TPDSASIC","TPSIC"},
            {"CIRIDERACC","TRADTHSIC"},
            {"CIRIDERSIC","TRADTHSIN"},
            {"CIRIDERCANCER","TRADTHCC"},
            {"CISAACC","TRSSIC"},
            {"CISASIC","TRSSIN"},
            {"CISACANCER","TRSCC"},
            {"DIACC","IPSAC"},
            {"DISIC","IPSIC"},
        };

        private static string ConvertPlanCode(string planCode)
        {
            return PlanMapping[planCode.ToUpper()];
        }

        private static string ConvertCoverCode(string planCode, string coverCode)
        {
            var key = planCode.ToUpper() + coverCode;
            return PlanCoverMapping[key];
        }

        private static bool? ConvertBuyBack(string buyBack)
        {
            if (buyBack.Equals("N", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (buyBack.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return null;
        }

        private static int? StringToNullableInt(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            int converted;
            if (!int.TryParse(val, out converted))
            {
                return null;
            }

            return converted;
        }

        private static DataTable GetData(string inputFile, string sheet, params string[] sheetRanges)
        {
            var returnData = new DataTable();

            string con =
                $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={inputFile};" +
                @"Extended Properties='Excel 12.0;HDR=YES'";

            using (OleDbConnection connection = new OleDbConnection(con))
            {
                connection.Open();

                if (sheetRanges.Any())
                {
                    for (var i=0; i < sheetRanges.Length; i++ )
                    {
                        var tempData = new DataTable();

                        var query = $"select * from [{sheet}$";

                        if (!string.IsNullOrEmpty(sheetRanges[i]))
                        {
                            query += sheetRanges[i];
                        }
                        query += "]";


                        OleDbCommand command = new OleDbCommand(query, connection);
                        using (OleDbDataReader dr = command.ExecuteReader())
                        {
                            tempData.Load(dr);
                        }

                        returnData.Merge(tempData);

                        //foreach (DataColumn col in tempData.Columns)
                        //{
                        //    var newDataCol = new DataColumn(col.ColumnName, col.DataType);
                        //    returnData.Columns.Add(newDataCol);
                        //}

                        //foreach (DataRow row in tempData.Rows)
                        //{
                        //    returnData.Rows.Add(row.ItemArray);
                        //}

                        ////rename columns to avoid clash
                        //foreach (DataColumn col in returnData.Columns)
                        //{
                        //    col.ColumnName = i + col.ColumnName;
                        //}
                    }
                }
                else
                {
                    var finalisedQuery = $"select * from [{sheet}$]";

                    OleDbCommand command = new OleDbCommand(finalisedQuery, connection);
                    using (OleDbDataReader dr = command.ExecuteReader())
                    {
                        returnData.Load(dr);
                    }
                }                
            }

            return returnData;
        }
    }

    public class CsvRowGenerator
    {
        protected string _user;
        protected string _insertTime;

        public CsvRowGenerator()
        {
            _user = "SYSTEM";
            _insertTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
        }

        public virtual string Generate(CoverBaseRate coverBaseRate)
        {
            return $",{coverBaseRate.PlanCode},{coverBaseRate.CoverCode},{coverBaseRate.Age},{coverBaseRate.GenderId}," +
                   $"{FormatNullableBool(coverBaseRate.IsSmoker)},{coverBaseRate.PremiumTypeId},{FormatString(coverBaseRate.OccupationGroup)},{FormatNullableInt(coverBaseRate.BenefitPeriod)}," +
                   $"{FormatNullableInt(coverBaseRate.WaitingPeriod)},{FormatNullableBool(coverBaseRate.BuyBack)},{coverBaseRate.BaseRate},,{_user},{_insertTime},{_user},{_insertTime}";
        }

        protected virtual string FormatNullableInt(int? val)
        {
            if (val.HasValue)
            {
                return val.Value.ToString();
            }

            return "";
        }

        protected virtual string FormatString(string val)
        {
            if (val != null)
            {
                return val;
            }

            return "";
        }

        protected virtual string FormatNullableBool(bool? val)
        {
            if (val.HasValue)
            {
                return val.Value ? "1" : "0";
            }

            return "";
        }
    }


    public class SqlInsertGenerator : CsvRowGenerator
    {
        public override string Generate(CoverBaseRate coverBaseRate)
        {
            return $"INSERT INTO CoverBaseRate" + "([BrandId], [PlanCode],[CoverCode],[Age],[GenderId],[IsSmoker],[PremiumTypeId],[OccupationGroup],[BenefitPeriod],[WaitingPeriod],[BuyBack],[BaseRate],[CreatedBy],[CreatedTS],[ModifiedBy],[ModifiedTS])" +
                " VALUES " +
                $"({coverBaseRate.BrandId}, {FormatString(coverBaseRate.PlanCode)}, {FormatString(coverBaseRate.CoverCode)},{coverBaseRate.Age},{coverBaseRate.GenderId},{FormatNullableBool(coverBaseRate.IsSmoker)},{coverBaseRate.PremiumTypeId},{FormatString(coverBaseRate.OccupationGroup)},{FormatNullableInt(coverBaseRate.BenefitPeriod)}," +
                $"{FormatNullableInt(coverBaseRate.WaitingPeriod)},{FormatNullableBool(coverBaseRate.BuyBack)},{coverBaseRate.BaseRate},{FormatString(_user)},GetDate(),{FormatString(_user)},GetDate());";
        }

        public string Generate(DayOneAccidentBaseRate dayOne)
        {
            return $"INSERT INTO DayOneAccidentBaseRate" + "([BrandId], [PlanCode],[CoverCode],[Age],[GenderId],[PremiumTypeId],[WaitingPeriod],[BaseRate],[CreatedBy],[CreatedTS],[ModifiedBy],[ModifiedTS])" +
                " VALUES " +
                $"({dayOne.BrandId}, {FormatString(dayOne.PlanCode)}, {FormatString(dayOne.CoverCode)},{dayOne.Age},{dayOne.GenderId},{dayOne.PremiumTypeId}," +
                $"{FormatNullableInt(dayOne.WaitingPeriod)},{dayOne.BaseRate},{FormatString(_user)},GetDate(),{FormatString(_user)},GetDate());";
        }

        protected override string FormatString(string val)
        {
            if (val != null)
            {
                return $"'{val}'";
            }

            return "NULL";
        }

        protected override string FormatNullableBool(bool? val)
        {
            if (val.HasValue)
            {
                return val.Value ? "1" : "0";
            }

            return "NULL";
        }

        protected override string FormatNullableInt(int? val)
        {
            if (val.HasValue)
            {
                return val.Value.ToString();
            }

            return "NULL";
        }
    }

    public class DayOneAccidentBaseRate
    {
        public int BrandId { get; set; }
        public string CoverCode { get; set; }
        public string PlanCode { get; set; }
        public int Age { get; set; }
        public int PremiumTypeId { get; set; }
        public int GenderId { get; set; }
        public int WaitingPeriod { get; set; }
        public decimal BaseRate { get; set; }
    }

    public class CoverBaseRate
    {
        public int BrandId { get; set; }
        public string PlanCode { get; set; }
        public string CoverCode { get; set; }
        public int Age { get; set; }
        public int GenderId { get; set; }
        public bool? IsSmoker { get; set; }
        public int PremiumTypeId { get; set; }
        public string OccupationGroup { get; set; }
        public int? BenefitPeriod { get; set; }
        public int? WaitingPeriod { get; set; }
        public bool? BuyBack { get; set; }        
        public decimal BaseRate { get; set; }

    }
}
