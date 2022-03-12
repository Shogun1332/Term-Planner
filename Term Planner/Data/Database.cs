using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Term_Planner.Models;
using System.Text;

namespace Term_Planner.Data
{
    public class Database
    {
        readonly SQLiteAsyncConnection database;

        public Database(string dbpath)
        {
            database = new SQLiteAsyncConnection(dbpath);
            database.CreateTableAsync<Term>().Wait();
            database.CreateTableAsync<Course>().Wait();
            database.CreateTableAsync<Assessment>().Wait();
            database.CreateTableAsync<Note>().Wait();

        }
        public Task<List<Term>> GetTermsAsync()
        {
            //Get all Terms
            return database.Table<Term>().ToListAsync();
        }

        public Task<Term> GetTermAsync(int id)
        {
            // Get a specific Term
            return database.Table<Term>()
                            .Where(i => i.TermID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveTermAsync(Term term)
        {
            if (term.TermID != 0)
            {
                // Update an existing Term
                return database.UpdateAsync(term);
            }
            else
            {
                // Save a new Term
                return database.InsertAsync(term);
            }
        }
        public Task<int> SaveTermManualAsync(Term term)
        {
            return database.InsertAsync(term);
        }

        public Task<int> DeleteTermAsync(Term term)
        {
            // Delete a Term
            return database.DeleteAsync(term);
        }
        public Task<List<Course>> GetCoursesAsync()
        {
            //Get all Terms
            return database.Table<Course>().ToListAsync();
        }
        public Task<List<Course>> GetTermCoursesAsync(Term term) //Pull all courses associated with a given term
        {
            return database.Table<Course>().Where(t => t.TermID == term.TermID).ToListAsync();
        }
        public Task<List<Assessment>> GetCourseAssessmentsAsync(Course course)
        {
            return database.Table<Assessment>().Where(t => t.CourseID == course.CourseID).ToListAsync();

        }
        public Task<List<Note>> GetCourseNotesAsync(Course course)
        {
            return database.Table<Note>().Where(t => t.CourseID == course.CourseID).ToListAsync();

        }

        public Task<Course> GetCourseAsync(int id)
        {
            // Get a specific Term
            return database.Table<Course>()
                            .Where(i => i.CourseID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveCourseAsync(Course course)
        {
            if (course.CourseID != 0)
            {
                // Update an existing Term
                return database.UpdateAsync(course);
            }
            else
            {
                // Save a new Term
                return database.InsertAsync(course);
            }
        }
        public Task<int> SaveCourseManualAsync(Course course)
        {
            return database.InsertAsync(course);
        }

        public Task<int> DeleteCourseAsync(Course course)
        {
            // Delete a Term
            return database.DeleteAsync(course);
        }
        public Task<List<Assessment>> GetAssessmentsAsync()
        {
            //Get all Terms
            return database.Table<Assessment>().ToListAsync();
        }

        public Task<Assessment> GetAssessmentAsync(int id)
        {
            // Get a specific Term
            return database.Table<Assessment>()
                            .Where(i => i.AssessmentID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            if (assessment.AssessmentID != 0)
            {
                // Update an existing Term
                return database.UpdateAsync(assessment);
            }
            else
            {
                // Save a new Term
                return database.InsertAsync(assessment);
            }
        }
        public Task<int> SaveAssessmentManualAsync(Assessment assessment)
        {
            return database.InsertAsync(assessment);
        }

        public Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            // Delete a Term
            return database.DeleteAsync(assessment);
        }
        public Task<Note> GetNoteAsync(int id)
        {
            // Get a specific Term
            return database.Table<Note>()
                            .Where(i => i.NoteID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(Note note)
        {
            if (note.NoteID != 0)
            {
                // Update an existing Term
                return database.UpdateAsync(note);
            }
            else
            {
                // Save a new Term
                return database.InsertAsync(note);
            }
        }
        public Task<int> SaveNoteManualAsync(Note note)
        {
            return database.InsertAsync(note);
        }

        public Task<int> DeleteNoteAsync(Note note)
        {
            // Delete a Term
            return database.DeleteAsync(note);
        }
    }
}
