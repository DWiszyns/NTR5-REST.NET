// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.AspNetCore.Mvc;
// using NTR2.Models;
// using NTR2.Data;
// using Microsoft.EntityFrameworkCore;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Routing;

// namespace NTR2.Controllers
// {
//     public class NoteController : Controller
//     {
//         private List<Note> Notes;
//         private ApplicationDbContext _context;
//         public NoteController(ApplicationDbContext context)
//         {
//             _context=context;
//             var notes = new List<Note>();
//             this.Notes=_context.Notes.ToList();
//         }
//         public IActionResult Index(DateTime dateFrom, DateTime dateTo, string category="",int pageNumber=1)
//         {
//             if(dateFrom==DateTime.MinValue) dateFrom = DateTime.Today.AddYears(-1);
//             if(dateTo==DateTime.MinValue) dateTo = DateTime.Today.AddDays(1);
//             if(category==null) category="";
//             List <string> possibleCategories= new List<string>{};
//             this.Notes=_context.Notes.ToList();
//             foreach(var n in _context.Categories.ToList())
//             {
//                 possibleCategories.Add(n.Title);
//             }
//             var notes = new List<Note>();
//             foreach(var n in Notes)
//             {
//                 Note tmpNote = _context.Notes.Include(i => i.NoteCategories).ThenInclude(noteCategories => noteCategories.Category).FirstOrDefault(note => note.NoteID == n.NoteID);
//                 if(tmpNote.NoteDate>=dateFrom && tmpNote.NoteDate<=dateTo && (category==""||tmpNote.NoteCategories.Where(m=>m.Category.Title==category).Any()))
//                 {
//                     notes.Add(n);
//                 }
//             }
//             PaginatedList<Note> list = new PaginatedList<Note>(notes,pageNumber,3);
//             TempData["category"]=category;
//             TempData["dateFrom"]=dateFrom.ToString();
//             TempData["dateTo"]=dateTo.ToString();
//             TempData["pageNumber"]=pageNumber;
//             return View("Index",new NoteIndexViewModel(list,possibleCategories.ToArray(),category,dateFrom,dateTo));
//         }
//         public IActionResult Clear()
//         {
//             return RedirectToAction("Index");
//         }
//         [HttpPost]
//         public IActionResult AddCategory(NoteEditViewModel model,List<NoteCategory> noteCategories)
//         {
//             model.Note.NoteCategories=noteCategories;
//             if(ModelState.IsValid)
//             {
//                 if(String.IsNullOrEmpty(model.NewCategory))
//                 {
//                     ModelState.AddModelError("Category error","Empty category");
//                     if(_context.Notes.Where(m=>m.Title==model.Note.Title).Any())
//                         return View("Edit",model);
//                     else return View("Add",model);
//                 }
//                 else if(noteCategories.Where(m=>m.Category.Title==model.NewCategory).Any())
//                 {
//                     ModelState.AddModelError("Category error","Category already exists");
//                     if(_context.Notes.Where(m=>m.Title==model.Note.Title).Any())
//                         return View("Edit",model);
//                     else return View("Add",model);
//                 }
//                 model.Note.NoteCategories=noteCategories.Append(new NoteCategory
//                     {Note=model.Note,Category=new Category(model.NewCategory)}).ToArray();
//                 model.NewCategory="";
//             }
            
//             if(Notes.Where(m=>m.Title==model.Note.Title).Any())
//                 return View("Edit",model);
//             else return View("Add",model);
//         }
//         [HttpPost]
//         public IActionResult RemoveCategories(NoteEditViewModel model,List<NoteCategory> noteCategories)
//         {
//             model.Note =  _context.Notes.Include(i => i.NoteCategories).ThenInclude(nc => nc.Category).FirstOrDefault(note => note.NoteID == model.Note.NoteID);
//             foreach(var c in model.CategoriesToRemove ?? new string[] { })
//             {
//                 model.Note.NoteCategories=model.Note.NoteCategories.Where(v=>v.Category.CategoryID.ToString()!=c).ToArray();
//             }
//             model.CategoriesToRemove=new string[]{};
//             if(Notes.Where(m=>m.Title==model.Note.Title).Any())
//                 return View("Edit",new NoteEditViewModel(model.Note));
//             else return View("Add",new NoteEditViewModel(model.Note));
//         }
//         public IActionResult Add()
//         {
//             Note n = new Note();
//             n.Title="write some title";
//             n.NoteDate=DateTime.Now;
//             n.Description="Write some text";
//             n.NoteCategories = new List<NoteCategory>();
//             return View(new NoteEditViewModel(n));
//         }
//         [HttpPost]
//         public IActionResult Add(NoteEditViewModel model, List<NoteCategory> noteCategories)
//         {
//             if(ModelState.IsValid)
//             {
//                 if(model.Note.Title=="write some title"||model.Note.Title=="")
//                 {
//                     ModelState.AddModelError("Title error","Wrong title");
//                     return View(model);
//                 }
//                 else if(Notes.Where(m=>m.Title==model.Note.Title).Any())
//                 {
//                     ModelState.AddModelError("Title error","Title already taken");
//                     return View(model);
//                 }
//                 using(var transaction = _context.Database.BeginTransaction())
//                 {
//                     _context.Notes.Add(model.Note);
//                     _context.SaveChanges();
//                     transaction.Commit();
//                 }
//                 foreach(var n in noteCategories)
//                 {
//                     Category newCategory;
//                     if(!_context.Categories.Where(m=>m.Title==n.Category.Title).Any())
//                     {
//                         using(var transaction = _context.Database.BeginTransaction())
//                         {
//                             newCategory=new Category{Title=n.Category.Title};
//                             _context.Categories.Add(newCategory);
//                             _context.SaveChanges();
//                             transaction.Commit();
//                         }
//                     }
//                     else
//                     {
//                         newCategory=_context.Categories.Where(m=>m.Title==n.Category.Title).FirstOrDefault();
//                     }
//                     using(var transaction = _context.Database.BeginTransaction())
//                     {
//                         _context.NoteCategories.Add(new NoteCategory{NoteID=model.Note.NoteID,CategoryID=newCategory.CategoryID});
//                         _context.SaveChanges();
//                         transaction.Commit();
//                     }
//                 }
//             }
//             var errors = ModelState
//             .Where(x => x.Value.Errors.Count > 0)
//             .Select(x => new { x.Key, x.Value.Errors })
//             .ToArray();
//             return ReturnToIndex();
//         }
//         public IActionResult Edit(int NoteID)
//         {
//             Note n = Notes.Where(m => m.NoteID == NoteID).FirstOrDefault();
//             if (n == null)
//             {
//                 Note deletedNote = new Note();
//                 ModelState.AddModelError(string.Empty,
//                     "Note no longer exists.");
//                 return ReturnToIndex();
//             }
//             n =  _context.Notes.Include(i => i.NoteCategories).ThenInclude(noteCategories => noteCategories.Category).FirstOrDefault(note => note.NoteID == n.NoteID);
//             return View(new NoteEditViewModel(n));
//         }
//         [HttpPost]
//         public async Task<IActionResult> Edit(NoteEditViewModel model,List<NoteCategory> noteCategories)
//         {
//             Note oldNote=_context.Notes.Find(model.Note.NoteID);
//             if (oldNote == null)
//             {
//                 Note deletedNote = new Note();
//                 await TryUpdateModelAsync(deletedNote);
//                 ModelState.AddModelError(string.Empty,
//                     "Unable to save changes. The note was deleted by another user.");
//                 return View(new NoteEditViewModel(deletedNote));
//             }
//             oldNote =  _context.Notes.Include(i => i.NoteCategories).ThenInclude(noteCategories => noteCategories.Category).FirstOrDefault(note => note.NoteID == oldNote.NoteID);
//             if(ModelState.IsValid)
//             {
//                 model.Note =  _context.Notes.Include(i => i.NoteCategories).ThenInclude(noteCategories => noteCategories.Category).FirstOrDefault(note => note.NoteID == model.Note.NoteID);
//                 if(model.Note.Title=="write some title"||model.Note.Title=="")
//                 {
//                     ModelState.AddModelError("Title error","Wrong title");
//                     return View(model);
//                 }
//                 else if(_context.Notes.Where(m=>m.Title==model.Note.Title&&m.NoteID!=model.Note.NoteID).Any())
//                 {
//                     ModelState.AddModelError("Title error","Title already taken");
//                     return View(model);
//                 }
//                 _context.Entry(oldNote).Property("Timestamp").OriginalValue = model.Note.Timestamp;
//                 if (await TryUpdateModelAsync<Note>(oldNote, "Note",
//                     n=>n.Title, n=>n.Description,  n=>n.NoteDate))
//                 {
//                     try
//                     {
//                         await _context.SaveChangesAsync();                    
//                         updateCategories(model.Note,noteCategories);
//                         return ReturnToIndex();
//                     }
//                     catch (DbUpdateConcurrencyException ex)
//                     {
//                         var entry = ex.Entries.Single();
//                         var clientValues = (Note)entry.Entity;
//                         var databaseEntry = entry.GetDatabaseValues();
//                         if (databaseEntry == null)
//                         {
//                             ModelState.AddModelError(string.Empty,
//                                 "Unable to save changes. The note was deleted by another user.");
//                         }
//                         else
//                         {
//                             var databaseValues = (Note)databaseEntry.ToObject();

//                             if (databaseValues.Title != clientValues.Title)
//                                 ModelState.AddModelError("Title", "Current value: "
//                                     + databaseValues.Title);
//                             if (databaseValues.Description != clientValues.Description)
//                                 ModelState.AddModelError("Description", "Current value: "
//                                     + String.Format("{0:c}", databaseValues.Description));
//                             if (databaseValues.NoteDate != clientValues.NoteDate)
//                                 ModelState.AddModelError("NoteDate", "Current value: "
//                                     + String.Format("{0:d}", databaseValues.NoteDate));
//                             ModelState.AddModelError(string.Empty, "The record you attempted to edit "
//                                 + "was modified by another user after you got the original value. The "
//                                 + "edit operation was canceled and the current values in the database "
//                                 + "have been displayed. If you still want to edit this record, click "
//                                 + "the Save button again. Otherwise click the Back to List hyperlink.");
//                             model.Note.Timestamp = databaseValues.Timestamp;
//                         }
//                     }
//                 }
//             }
//             return View(model);            
//         }
//         public IActionResult Delete(int noteID)
//         {
//             var note = _context.Notes.Include(i => i.NoteCategories).ThenInclude(i => i.Category).FirstOrDefault(i => i.NoteID == noteID);
//             if (note == null) {
//                     return ReturnToIndex();
//             }

//             foreach (var category in note.NoteCategories) {
//                 if (_context.NoteCategories.Where(i => i.CategoryID == category.Category.CategoryID && i.NoteID != noteID).FirstOrDefault() == null) {
//                     _context.Categories.Remove(category.Category);
//                 }
//             }
//             _context.Notes.Remove(note);
//             _context.SaveChanges();
//             return ReturnToIndex();
//         }
//         [HttpPost]
//         public JsonResult doesFileNameExist(string title)
//         {
//         var file = _context.Notes.Find(title);
//         return Json(file == null);
//         }
//         public RedirectToActionResult ReturnToIndex()
//         {
//             RouteValueDictionary dict = new RouteValueDictionary();
//             dict.Add("category", TempData.Peek("category"));
//             dict.Add("dateFrom", Convert.ToDateTime(TempData.Peek("dateFrom")));
//             dict.Add("dateTo", Convert.ToDateTime(TempData.Peek("dateTo")));
//             dict.Add("pageNumber",TempData.Peek("pageNumber"));

//             return RedirectToAction(nameof(Index), dict);
//         }
//         private void updateCategories(Note note,List<NoteCategory> noteCategories)
//         {
//             Note original = _context.Notes.Find(note.NoteID);
//             original =  _context.Notes.Include(i => i.NoteCategories).ThenInclude(nCategories => nCategories.Category).FirstOrDefault(n => n.NoteID == note.NoteID);
//             foreach (var category in original.NoteCategories) {
//                     if (!noteCategories.Where(c=>c.Category.CategoryID==category.CategoryID).Any()) {
//                         if (_context.NoteCategories.Where(i => i.CategoryID == category.Category.CategoryID && i.NoteID != note.NoteID).FirstOrDefault() == null) {
//                             _context.Categories.Remove(category.Category); //to delete category
//                         }
//                         else{
//                             _context.NoteCategories.Remove(category); //to delete category
//                         }
//                     }
//             }
//             _context.SaveChanges();
//             foreach(var n in noteCategories)
//             {
//                 if(!_context.NoteCategories.Where(nc=>nc.CategoryID==n.CategoryID&&nc.NoteID==note.NoteID).Any()){
//                     Category newCategory;
//                     if(!_context.Categories.Where(m=>m.Title==n.Category.Title).Any())
//                     {
//                             newCategory=new Category{Title=n.Category.Title};
//                             _context.Categories.Add(newCategory);
//                             _context.SaveChanges();
//                     }
//                     else
//                     {
//                         newCategory=_context.Categories.Where(m=>m.Title==n.Category.Title).FirstOrDefault();
//                     }
//                     _context.NoteCategories.Add(new NoteCategory{NoteID=note.NoteID,CategoryID=newCategory.CategoryID});
//                 }
//             }
//             _context.SaveChanges();
//         }
//     }
// }
