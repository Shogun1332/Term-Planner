using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Term_Planner.Models;
using Term_Planner.Data;
using Plugin.LocalNotifications;
using System.Threading.Tasks;

namespace Term_Planner
{
    public static class Notifications
    {
        public static async Task NotifyOnStart()
        {
            DateTime today = DateTime.Today;
            List<Course> courses = await App.Database.GetCoursesAsync();
            List<Assessment> assessments = await App.Database.GetAssessmentsAsync();

            courses.ForEach(course =>
            {
                if(course.NotifEnabled && course.CourseStart.ToLocalTime().Month == today.Month && course.CourseStart.ToLocalTime().Day == today.Day && course.CourseStart.ToLocalTime().Year == today.Year)
                {
                    CrossLocalNotifications.Current.Show("Course Start Notification", $"{course.CourseName} will be starting today!");
                }
                if (course.NotifEnabled && course.CourseEnd.ToLocalTime().Month == today.Month && course.CourseEnd.ToLocalTime().Day == today.Day && course.CourseEnd.ToLocalTime().Year == today.Year)
                {
                    CrossLocalNotifications.Current.Show("Course End Notification", $"{course.CourseName} will be ending today!");
                }
            });
            assessments.ForEach(assessment =>
            {
                if (assessment.NotifEnabled && assessment.AssessmentDue.ToLocalTime().Month == today.Month && assessment.AssessmentDue.ToLocalTime().Day == today.Day && assessment.AssessmentDue.ToLocalTime().Year == today.Year)
                {
                    CrossLocalNotifications.Current.Show("Assessment Due!", $"{assessment.AssessmentName} is due today!");
                }
            });
        }

    }
}
