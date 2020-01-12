using System;
using System.Collections.Generic;

namespace NTR5.Models
{
    public partial class Note
    {
        public Note()
        {
            NoteCategory = new HashSet<NoteCategory>();
        }

        public int Idnote { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public short IsMarkdown { get; set; }
        public DateTime Date { get; set; }
        public byte[] Timestamp { get; set; }
        public virtual ICollection<NoteCategory> NoteCategory { get; set; }
    }
}
