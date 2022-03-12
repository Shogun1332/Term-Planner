using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Term_Planner.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int NoteID { get; set; }
        public int CourseID { get; set; }
        public DateTime NoteCreated { get; set; }
        public string FormattedNoteCreated { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public Note() { }
        public Note(int courseID, DateTime created, string title, string content)
        {
            CourseID = courseID;
            NoteCreated = created;
            NoteTitle = title;
            NoteContent = content;
            FormattedNoteCreated = created.ToShortDateString();
        }
        public Note(int noteID, int courseID, DateTime created, string title, string content)
        {
            NoteID = noteID;
            CourseID = courseID;
            NoteCreated = created;
            NoteTitle = title;
            NoteContent = content;
            FormattedNoteCreated = created.ToShortDateString();
        }

    }
}
