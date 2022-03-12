using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using SQLite;
using System.Threading.Tasks;

namespace Term_Planner.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int CourseID { get; set; }
        public int TermID { get; set; } //Essentially an FK but not really
        public string CourseName { get; set; }
        public string CourseStatus { get; set; }
        public DateTime CourseStart { get; set; }
        public DateTime CourseEnd { get; set; }
        public string FormattedCourseStart { get; set; }
        public string FormattedCourseEnd { get; set; }
        public string InstructorName { get; set; }
        public string InstructorEmail { get; set; }
        public string InstructorPhone { get; set; }
        [Ignore]
        public List<Assessment> CourseOwnsAssessments { get; set; }
        [Ignore]
        public List<Note> CourseOwnsNotes { get; set; }

        private List<string> statuses = new List<string> { "Not Started", "In Progress", "Completed", "Dropped", "Plan to Take" };
        [Ignore]
        public List<string> CourseStatuses
        {
            set
            {
                statuses = value;
            }
            get
            {
                return statuses;
            }
        }
        public bool NotifEnabled { get; set; }

        public Course() { }
        public Course(int termID, string courseName, string courseStatus, DateTime courseStart, DateTime courseEnd, string instructorName, string instructorEmail, string instructorPhone)
        {
            TermID = termID;
            CourseName = courseName;
            CourseStatus = courseStatus;
            CourseStart = courseStart;
            CourseEnd = courseEnd;
            InstructorName = instructorName;
            InstructorEmail = instructorEmail;
            InstructorPhone = instructorPhone;
            FormattedCourseStart = courseStart.ToShortDateString();
            FormattedCourseEnd = courseEnd.ToShortDateString();

        }
        public Course(int courseID, int termID, string courseName, string courseStatus, DateTime courseStart, DateTime courseEnd, string instructorName, string instructorEmail, string instructorPhone, bool notif)
        {
            CourseID = courseID;
            TermID = termID;
            CourseName = courseName;
            CourseStatus = courseStatus;
            CourseStart = courseStart;
            CourseEnd = courseEnd;
            InstructorName = instructorName;
            InstructorEmail = instructorEmail;
            InstructorPhone = instructorPhone;
            NotifEnabled = notif;
            FormattedCourseStart = courseStart.ToShortDateString();
            FormattedCourseEnd = courseEnd.ToShortDateString();
        }
        public static bool IsInstructorEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();
                    string domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }
            try
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public async Task DeleteCourseAndChildren(int courseID)
        {
            var course = await App.Database.GetCourseAsync(courseID);
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
}
