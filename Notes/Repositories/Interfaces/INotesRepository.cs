using Notes.Models;

namespace Notes.Repositories.Interfaces
{
    public interface INotesRepository
    {
        Task<List<Note>> GetByPatientAsync(int patientId);
        Task<Note> GetAsync(string id);
        Task CreateAsync(Note n);
        Task UpdateAsync(string id, Note n);
        Task DeleteAsync(string id);
    }
}
