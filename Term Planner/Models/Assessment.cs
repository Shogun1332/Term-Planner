using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Term_Planner.Models
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int AssessmentID { get; set; }
        public int CourseID { get; set; }
        public int TermID { get; set; }
        public string AssessmentName { get; set; }
        public string AssessmentType { get; set; }
        public DateTime AssessmentDue { get; set; }
        public string FormattedAssessmentDue { get; set; }
        private List<string> types = new List<string> { "Objective Assessment","Performance Assessment" };
        [Ignore]
        public List<string> AssessmentTypes
        {
            set
            {
                types = value;
            }
            get
            {
                return types;
            }
        }
        public bool NotifEnabled { get; set; }
        public Assessment() { }
        public Assessment(int assessmentID, int courseID, int termID, string assessmentName, string assessmentType, DateTime assessmentDue)
        {
            AssessmentID = assessmentID;
            CourseID = courseID;
            TermID = termID;
            AssessmentName = assessmentName;
            AssessmentType = assessmentType;
            AssessmentDue = assessmentDue;
            FormattedAssessmentDue = assessmentDue.ToShortDateString();
        }

    }
}
