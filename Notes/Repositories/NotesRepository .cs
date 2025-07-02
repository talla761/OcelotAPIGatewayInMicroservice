using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Notes.Models;
using Notes.Repositories.Interfaces;

namespace Notes.Repositories
{
    public class NotesRepository : INotesRepository
    {
        private readonly IMongoCollection<Note> _col;
        public NotesRepository(IOptions<NotesDatabaseSettings> cfg, IMongoClient cli)
        {
            var db = cli.GetDatabase(cfg.Value.DatabaseName);
            _col = db.GetCollection<Note>(cfg.Value.NotesCollectionName);
        }

        public Task CreateAsync(Note n)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Note> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Note>> GetByPatientAsync(int patientId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(string id, Note n)
        {
            throw new NotImplementedException();
        }
    }
}
