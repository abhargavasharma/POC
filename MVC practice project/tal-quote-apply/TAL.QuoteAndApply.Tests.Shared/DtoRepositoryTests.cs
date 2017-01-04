using System;
using System.Reflection;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Tests.Shared
{
    public abstract class DtoRepositoryTests<TDto> 
        where TDto : DbItem
    {
        public abstract BaseRepository<TDto> Repo { get; }
        public abstract TDto InsertDto { get; }
        public abstract Action<TDto> UPdateDtoAction { get; }
        public abstract Assembly CallingAssembly { get; }

        [Test]
        public void Insert_Get_Update_Delete()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps(CallingAssembly);

            var insertedDbItem = Repo.Insert(InsertDto);
            Assert.That(insertedDbItem.Id, Is.GreaterThan(0));

            UPdateDtoAction.Invoke(insertedDbItem);
            var updateResult = Repo.TryUpdate(insertedDbItem);
            Assert.That(updateResult, Is.True);

            var insertedDbItem2 = Repo.Get(insertedDbItem.Id);

            Assert.That(insertedDbItem2.Id, Is.EqualTo(insertedDbItem.Id));
            Assert.That(insertedDbItem2.CreatedBy, Is.EqualTo(insertedDbItem.CreatedBy));
            Assert.That(insertedDbItem2.CreatedTS, Is.EqualTo(insertedDbItem.CreatedTS));
            Assert.That(insertedDbItem2.RV, Is.Not.EqualTo(insertedDbItem.RV));

            var deleteResult = Repo.Delete(insertedDbItem);
            Assert.That(deleteResult, Is.False);

            deleteResult = Repo.Delete(insertedDbItem2);
            Assert.That(deleteResult, Is.True);

            var dbItem3 = Repo.Get(insertedDbItem.Id);
            Assert.That(dbItem3, Is.Null);
        }
    }
}