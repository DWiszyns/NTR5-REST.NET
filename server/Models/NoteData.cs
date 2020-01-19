using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace NTR5.Models
{
    public class NoteData
    {
        public int NoteID {get;set;}
        public string Title { get; set; }

        public bool Markdown {get;set;}
        public DateTime Date { get; set; }
        public ICollection<String> NoteCategories { get; set; }
        public string Text { get; set; }
        public byte[] Timestamp { get; set; }
        public NoteData(){
            
        }
        public NoteData(Note note){
            NoteID=note.Idnote;
            Title=note.Title;
            Date=note.Date;
            Markdown = note.IsMarkdown==1?true:false;
            NoteCategories=note.NoteCategory.Select(nc => nc.IdcategoryNavigation.Name).ToList();
            Text=note.Description;
            Timestamp=note.Timestamp;
        }
    }
}