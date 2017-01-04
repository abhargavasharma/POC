using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Converters;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Underwriting.Converters
{
    [TestFixture]
    public class CoverLoadingsConverterTests
    {
        private ICover _mockCover;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockCover = new CoverDto {Id = 999};
        }

        [Test]
        public void From_TotalLoadingsIsNull_EmptyListReturned()
        {
            var svc = new CoverLoadingsConverter();
            var result = svc.From(_mockCover, null);

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void From_NoLoadings_EmptyListReturned()
        {
            var readonlyTotalLoadings = new ReadOnlyTotalLoadings(
                new TotalLoadings {Fixed = 0, PerMille = 0, Variable = 0});

            var svc = new CoverLoadingsConverter();
            var result = svc.From(_mockCover, readonlyTotalLoadings);

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void From_FixedLoading_ListWithFixedLoadingReturned()
        {
            var readonlyTotalLoadings = new ReadOnlyTotalLoadings(
                new TotalLoadings { Fixed = 10, PerMille = 0, Variable = 0 });

            var svc = new CoverLoadingsConverter();
            var result = svc.From(_mockCover, readonlyTotalLoadings).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LoadingType, Is.EqualTo(LoadingType.Fixed));
            Assert.That(result[0].Loading, Is.EqualTo(readonlyTotalLoadings.Fixed));
        }

        [Test]
        public void From_VariableLoading_ListWithVariableLoadingReturned()
        {
            var readonlyTotalLoadings = new ReadOnlyTotalLoadings(
                new TotalLoadings { Fixed = 0, PerMille = 0, Variable = 10 });

            var svc = new CoverLoadingsConverter();
            var result = svc.From(_mockCover, readonlyTotalLoadings).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LoadingType, Is.EqualTo(LoadingType.Variable));
            Assert.That(result[0].Loading, Is.EqualTo(readonlyTotalLoadings.Variable));
        }

        [Test]
        public void From_PerMilleLoading_ListWithPerMilleLoadingReturned()
        {
            var readonlyTotalLoadings = new ReadOnlyTotalLoadings(
                new TotalLoadings { Fixed = 0, PerMille = 10, Variable = 0 });

            var svc = new CoverLoadingsConverter();
            var result = svc.From(_mockCover, readonlyTotalLoadings).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LoadingType, Is.EqualTo(LoadingType.PerMille));
            Assert.That(result[0].Loading, Is.EqualTo(readonlyTotalLoadings.PerMille));
        }

        [Test]
        public void From_AllLoadingTypes_ListWithAllLoadingsReturned()
        {
            var readonlyTotalLoadings = new ReadOnlyTotalLoadings(
                new TotalLoadings { Fixed = 10, PerMille = 10, Variable = 10 });

            var svc = new CoverLoadingsConverter();
            var result = svc.From(_mockCover, readonlyTotalLoadings).ToList();

            Assert.That(result.Count, Is.EqualTo(3));

            var perMilleLoading = result.First(x => x.LoadingType == LoadingType.PerMille);
            Assert.That(perMilleLoading.Loading, Is.EqualTo(readonlyTotalLoadings.PerMille));

            var variableLoading = result.First(x => x.LoadingType == LoadingType.Variable);
            Assert.That(variableLoading.Loading, Is.EqualTo(readonlyTotalLoadings.Variable));

            var fixedLoading = result.First(x => x.LoadingType == LoadingType.Fixed);
            Assert.That(fixedLoading.Loading, Is.EqualTo(readonlyTotalLoadings.Fixed));
        }
    }
}
