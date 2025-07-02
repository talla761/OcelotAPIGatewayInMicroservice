using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Models;
using Notes.Services;

namespace Notes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly NotesService _notesService;

        public NotesController(NotesService notesService) =>
            _notesService = notesService;

        [HttpGet]
        public async Task<List<Note>> Get() =>
            await _notesService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Note>> Get(string id)
        {
            var note = await _notesService.GetAsync(id);

            if (note is null)
            {
                return NotFound();
            }

            return note;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Note newNote)
        {
            await _notesService.CreateAsync(newNote);

            return CreatedAtAction(nameof(Get), new { id = newNote.Id }, newNote);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Note updatedNote)
        {
            var note = await _notesService.GetAsync(id);

            if (note is null)
            {
                return NotFound();
            }

            updatedNote.Id = note.Id;

            await _notesService.UpdateAsync(id, updatedNote);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var note = await _notesService.GetAsync(id);

            if (note is null)
            {
                return NotFound();
            }

            await _notesService.RemoveAsync(id);

            return NoContent();
        }
    }
}
