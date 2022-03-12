using System;
using SQLite;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Term_Planner.Models
{
    public class Term
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int TermID { get; set; }
        public string TermName { get; set; }
        public DateTime TermStart { get; set; }
        public DateTime TermEnd { get; set; }
        public string FormattedTermStart { get; set; }
        public string FormattedTermEnd { get; set; }
        [Ignore]
        public List<Course> TermOwnsCourses { get; set; }
        [Ignore]
        public List<Assessment> TermOwnsAssessments { get; set; }
        public Term() { }
        public Term(string termName, DateTime termStart, DateTime termEnd)
        {
            TermName = termName;
            TermStart = termStart;
            TermEnd = termEnd;
            FormattedTermStart = termStart.ToShortDateString();
            FormattedTermEnd = termEnd.ToShortDateString();
        }
        public Term(int termID, string termName, DateTime termStart, DateTime termEnd)
        {
            TermID = termID;
            TermName = termName;
            TermStart = termStart;
            TermEnd = termEnd;
            FormattedTermStart = termStart.ToShortDateString();
            FormattedTermEnd = termEnd.ToShortDateString();
        }
        public static async Task GenerateStartingData()
        {
            Term term = new Term(1, "Freshman Term 1", DateTime.Today.ToUniversalTime(), DateTime.Today.ToUniversalTime().AddDays(180));
            await App.Database.SaveTermManualAsync(term);
            Course course = new Course(1, 1, "University Foundations", "Not Started", DateTime.Today.ToUniversalTime(), DateTime.Today.ToUniversalTime().AddDays(60), "Dominik Tolman", "dtolma4@wgu.edu", "208-573-4146", true);
            await App.Database.SaveCourseManualAsync(course);
            Assessment assessment1 = new Assessment(1, 1, 1, "University Foundations OA", "Objective Assessment", DateTime.Today.ToUniversalTime().AddDays(30));
            await App.Database.SaveAssessmentManualAsync(assessment1);
            Assessment assessment2 = new Assessment(2, 1, 1, "University Foundations PA", "Performance Assessment", DateTime.Today.ToUniversalTime().AddDays(60));
            await App.Database.SaveAssessmentManualAsync(assessment2);
            Note note = new Note(1, 1, DateTime.Today.ToUniversalTime(), "Test Note", "This is a test note. Lorem Ipsum whatever whatever");
            await App.Database.SaveNoteManualAsync(note);
            return;

        }
        public async Task DeleteTermChildren(int termID)
        {
            var term = await App.Database.GetTermAsync(termID);
            List<Course> termCourses = await App.Database.GetTermCoursesAsync(term);
            try
            {
                foreach (Course course in termCourses)
                {
                    List<Assessment> courseAssessments = await App.Database.GetCourseAssessmentsAsync(course);
                    List<Note> courseNotes = await App.Database.GetCourseNotesAsync(course);
                    try
                    {
                        foreach (Assessment assessment in courseAssessments)
                        {
                            await App.Database.DeleteAssessmentAsync(assessment);
                        }
                        try
                        {
                            foreach (Note note in courseNotes)
                            {
                                await App.Database.DeleteNoteAsync(note);
                            }
                            try
                            {
                                await App.Database.DeleteCourseAsync(course);

                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Failed to delete course");

                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Failed to delete notes owned by Course");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to delete assessments owned by Course");
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load courses");

            }
        }
    }
}
