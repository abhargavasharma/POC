using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Infrastructure.Tests
{
    [TestFixture]
    public class EndRequestOperationCollectionTests
    {
        [Test]
        public void ExecuteTasks_ConcurrencyTest_AllOperationsRun()
        {
            var service = new EndRequestOperationCollection(new MockHttpProvider());
            var count = 0;
            Action<string> action = message =>
            {
                Console.WriteLine(message);
                count++;
            };

            var task1 = new Task(() =>
            {
                var prefix = "t1-";
                for (var i = 1; i <= 100; i++)
                {
                    var key = $"{prefix}{i}";
                    service.AddOrUpdateAction(key, () => action($"Running {key}"));
                }
            });
            var task2 = new Task(() =>
            {
                var prefix = "t2-";
                for (var i = 1; i <= 100; i++)
                {
                    var key = $"{prefix}{i}";
                    service.AddOrUpdateAction(key, () => action($"Running {key}"));
                }
            });

            var task3 = new Task(() =>
            {
                var prefix = "t3-";
                for (var i = 1; i <= 100; i++)
                {
                    var key = $"{prefix}{i}";
                    service.AddOrUpdateAction(key, () => action($"Running {key}"));
                }
            });

            task1.Start();
            task2.Start();
            task3.Start();

            Task.WaitAll(task1, task2, task3);

            service.ExecuteTasks();

            Assert.That(count, Is.EqualTo(300));
        }

        [Test]
        public void ExecuteTasks_SameTaskRunMoreThanOnce_OnlyOnePerKeyRuns()
        {
            var service = new EndRequestOperationCollection(new MockHttpProvider());
            var count = 0;
            Action<string> action = message =>
            {
                Console.WriteLine(message);
                count++;
            };

            var task1 = new Task(() =>
            {
                var prefix = "t1-";
                for (var i = 1; i <= 100; i++)
                {
                    var key = $"{prefix}";
                    service.AddOrUpdateAction(key, () => action($"Running {key} {i}"));
                }
            });
            var task2 = new Task(() =>
            {
                var prefix = "t2-";
                for (var i = 1; i <= 100; i++)
                {
                    var key = $"{prefix}";
                    service.AddOrUpdateAction(key, () => action($"Running {key} {i}"));
                }
            });

            var task3 = new Task(() =>
            {
                var prefix = "t3-";
                for (var i = 1; i <= 100; i++)
                {
                    var key = $"{prefix}";
                    service.AddOrUpdateAction(key, () => action($"Running {key} {i}"));
                }
            });

            task1.Start();
            task2.Start();
            task3.Start();

            task1.Wait();
            task2.Wait();
            task3.Wait();

            service.ExecuteTasks();

            Assert.That(count, Is.EqualTo(3));
        }

    }
}
