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
        public IActionResult Get(int page, DateTime dateTo, DateTime dateFrom, string category)
        {
            //var notes = _context.Note.ToArray();
            var categories = _context.Category.ToArray();
            PaginatedList<Note> list = new PaginatedList<Note>(_context.Note.ToList(),page,4);
            return Ok(new { notes=list.ToList(), categories=categories, pager= new {currentPage=list.PageIndex, endPage=list.TotalPages }});
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int? id)
        {
            if (id == null) {
                return NotFound();
            }
            Note note = _context.Note.Where(n=>n.Idnote==id).Include(n => n.NoteCategory).ThenInclude(nc => nc.IdcategoryNavigation).FirstOrDefault();
            //return Ok(note);
            return Ok(new {data=new NoteData(note)});
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int? id, [FromBody]Models.Note note)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int? id)
        {
            return Ok();
        }

    }
}
