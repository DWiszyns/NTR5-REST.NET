using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NTR5.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;



namespace NTR5.Controllers
{
    [ApiController]
    [EnableCors("MyPolicy")]
    [Route("[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly ILogger<NotesController> _logger;
        private readonly NTR2019ZContext _context;

        public NotesController(ILogger<NotesController> logger, NTR2019ZContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get(int page, string dateTo, string dateFrom, string category)
        {
            var notes = _context.Note.Include(n => n.NoteCategory).ThenInclude(nc => nc.IdcategoryNavigation).ToList();
            var categories = _context.Category.ToArray();
            var newNotes = new List<NoteData>{};
            if (category != null && category != "")
            {
                notes = notes.Where(n => n.NoteCategory.Any(nc => nc.IdcategoryNavigation.Name.Equals(category))).ToList();
            }
            if (dateFrom != null && dateFrom != "Invalid date" && dateFrom != "null")
            {
                notes = notes.Where(n => n.Date >= Convert.ToDateTime(dateFrom)).ToList();
            }
            if (dateTo != null && dateTo != "Invalid date" && dateTo != "null")
            {
                notes = notes.Where(n => n.Date <= Convert.ToDateTime(dateTo)).ToList();
            }
            foreach(var n in notes){
                newNotes.Add(new NoteData(n));
            }
            PaginatedList<NoteData> list = new PaginatedList<NoteData>(newNotes, page, 4);
            return Ok(new { notes = list.ToList(), categories = categories.Select(c=>c.Name), pager = new { currentPage = list.PageIndex, endPage = list.TotalPages } });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Note note = _context.Note.Where(n => n.Idnote == id).Include(n => n.NoteCategory).ThenInclude(nc => nc.IdcategoryNavigation).FirstOrDefault();
            return Ok(new { data = new NoteData(note) });
        }

        [HttpPost]
        // [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] NoteData note)
        {
            if (_context.Note.Any(n => n.Title == note.Title))
            {
                return StatusCode(400, "Note with title - " + note.Title + " - already exists");
            }
            Note newNote = new Note() { Title = note.Title, Description = note.Text, Date = note.Date, IsMarkdown = Convert.ToInt16(note.Markdown) };
            _context.Note.Add(newNote);
            _context.SaveChanges();
            updateCategories(newNote.Idnote, note.NoteCategories);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.InnerException.Message);
            }
            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(int? id, [FromBody]NoteData note)
        {
            Note oldNote = _context.Note.Where(n => n.Idnote == id).FirstOrDefault();
            if (oldNote == null)
            {
                Note deletedNote = new Note();
                await TryUpdateModelAsync(deletedNote);
                return NotFound();
            }
            oldNote = _context.Note.Include(i => i.NoteCategory).ThenInclude(noteCategories => noteCategories.IdcategoryNavigation).FirstOrDefault(n => n.Idnote == oldNote.Idnote);
            if (_context.Note.Where(n => n.Title == note.Title && n.Idnote != id).Any())
            {
                return StatusCode(400, "Note with title - " + note.Title + " - already exists");
            }
            _context.Entry(oldNote).Property("Timestamp").OriginalValue = note.Timestamp;
            try
            {
                oldNote.Title = note.Title;
                oldNote.Description = note.Text;
                oldNote.Date = note.Date;
                oldNote.IsMarkdown = Convert.ToInt16(note.Markdown);
                await _context.SaveChangesAsync();
                updateCategories(oldNote.Idnote, note.NoteCategories);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Note)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    return NotFound();
                }
                else
                {
                    var databaseValues = (Note)databaseEntry.ToObject();
                    string error = getConcurrencyErrors(clientValues, databaseValues);
                    return StatusCode(403, "The record you attempted to edit "
                        + "was modified by another user after you got the original value. The "
                        + "edit operation was canceled and the current values in the database "
                        + "have been displayed. Click the Back to List hyperlink and if you "
                        + "still want to edit please re-enter Note. Current values:\n" + error);
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.InnerException.Message);
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int? id)
        {
            var note = _context.Note.Include(i => i.NoteCategory).ThenInclude(i => i.IdcategoryNavigation).FirstOrDefault(i => i.Idnote == id);
            if (note == null)
            {
                return NotFound();
            }

            foreach (var category in note.NoteCategory)
            {
                _context.NoteCategory.Remove(category);
                if (!_context.NoteCategory.Where(i => i.Idcategory == category.Idcategory
                             && i.Idnote != id).Any())
                {
                    _context.Category.Remove(category.IdcategoryNavigation);
                }

            }
            _context.Note.Remove(note);
            _context.SaveChanges();
            return Ok();
        }
        private void updateCategories(int id, ICollection<String> newNoteCategories)
        {
            Note original = _context.Note.Include(i => i.NoteCategory).ThenInclude(nCategories => nCategories.IdcategoryNavigation).FirstOrDefault(n => n.Idnote == id);
            foreach (var category in original.NoteCategory)
            {
                if (!newNoteCategories.Where(c => c == category.IdcategoryNavigation.Name).Any())//if in new there is no category with this name then delete it
                {
                    if (!_context.NoteCategory.Where(i => i.Idcategory == category.IdcategoryNavigation.Idcategory
                             && i.Idnote != id).Any())
                    {
                        _context.NoteCategory.Remove(category); //to delete notecategory
                        _context.Category.Remove(category.IdcategoryNavigation); //to delete category
                    }
                    else
                    {
                        _context.NoteCategory.Remove(category); //to delete notecategory
                    }
                }
            }
            _context.SaveChanges();
            Array.ForEach(newNoteCategories.ToArray(), c =>
            {
                try
                {
                    Category category;
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        var occurances = _context.Category.Where(categoryObj => categoryObj.Name == c).ToList();
                        if (occurances.Count() == 0)
                        {
                            category = new Category { Name = c };
                            _context.Category.Add(category);
                            _context.SaveChanges();
                        }
                        else
                        {
                            category = occurances.FirstOrDefault();
                        }
                        if (!_context.NoteCategory.Where(nc => nc.Idcategory == category.Idcategory && nc.Idnote == id).Any())
                        {
                            _context.NoteCategory.Add(new NoteCategory
                            {
                                Idnote = id,
                                Idcategory = category.Idcategory
                            });
                        }
                        _context.SaveChanges();

                        transaction.Commit();
                    }
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine(ex.InnerException.Data);
                }
            });
        }

        private string getConcurrencyErrors(Note clientValues, Note databaseValues)
        {
            string error = "";
            if (databaseValues.Title != clientValues.Title)
                error += "Title " + "Current value: "
                    + databaseValues.Title + "\n";
            if (databaseValues.Description != clientValues.Description)
                error += "Description " + "Current value: "
                    + String.Format("{0:c}", databaseValues.Description) + "\n";
            if (databaseValues.Date != clientValues.Date)
                error += "NoteDate " + "Current value: "
                    + String.Format("{0:d}", databaseValues.Date) + "\n";
            return error;
        }

    }
}
