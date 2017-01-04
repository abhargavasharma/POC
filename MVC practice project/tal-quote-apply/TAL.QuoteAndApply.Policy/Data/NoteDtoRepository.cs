using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface INoteDtoRepository
    {
        NoteDto InsertNote(NoteDto note);
        NoteDto GetNote(int id);
        void UpdateNote(NoteDto note);
        bool DeleteNote(NoteDto note);
        IEnumerable<NoteDto> GetNotesByPolicyId(int policyId);
    }

    public class NoteDtoRepository : BaseRepository<NoteDto>, INoteDtoRepository
    {
        public NoteDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public NoteDto InsertNote(NoteDto note)
        {
            return Insert(note);
        }

        public NoteDto GetNote(int id)
        {
            return Get(id);
        }

        public void UpdateNote(NoteDto note)
        {
            Update(note);
        }

        public bool DeleteNote(NoteDto note)
        {
            return Delete(note);
        }
        public IEnumerable<NoteDto> GetNotesByPolicyId(int policyId)
        {
            var notes = Where(policy => policy.PolicyId, Op.Eq, policyId);
            notes = notes.OrderByDescending(policy => policy.CreatedTS);
            return notes;
        }
    }
}
