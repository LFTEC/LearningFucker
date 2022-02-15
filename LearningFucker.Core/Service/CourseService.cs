using System;
using System.Collections.Generic;
using System.Text;
using LearningFucker.Models;
using System.Linq;

namespace LearningFucker.Service
{
    public class CourseService
    {
        public CourseService(Fucker fucker)
        {
            this.fucker = fucker;
        }

        private Fucker fucker;
        public async System.Threading.Tasks.Task<CourseList> GetCourseList()
        {
            try
            {
                var courseList = await fucker.GetCourseList();
                //foreach (var item in courseList?.List)
                //{
                //    await GetCourseDetail(item);
                //}
                return courseList;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async System.Threading.Tasks.Task<ElectiveCourseList> GetElectiveCourseListAsync()
        {
            try
            {
                var courseList = await fucker.GetElectiveCourseList();
                //foreach (var item in courseList?.List)
                //{
                //    await GetCourseDetail(item);
                //}
                return courseList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async System.Threading.Tasks.Task GetCourseDetail(Course course)
        {
            try
            {
                await fucker.GetCourseDetail(course);
                foreach (var ware in course.Detail.WareList)
                {
                    if (ware.AllowIntegral == 0)
                        ware.Status = StudyStatus.Completed;
                }

                if (course.Detail.WareList.All(s => s.Complete))
                    course.Status = StudyStatus.Completed;
            }
            catch(Exception ex)
            {

            }
        }

        public async System.Threading.Tasks.Task GetCourseDetail(ElectiveCourse course)
        {
            try
            {
                await fucker.GetCourseDetail(course);
                foreach (var ware in course.Detail.WareList)
                {
                    if (ware.AllowIntegral == 0)
                        ware.Status = StudyStatus.Completed;
                }

                if (course.Detail.WareList.All(s => s.Complete))
                    course.Status = StudyStatus.Completed;
            }
            catch (Exception ex)
            {

            }
        }

        public async System.Threading.Tasks.Task GetCourseAppendix(ElectiveCourse course)
        {
            try
            {
                await fucker.GetCourseAppendix(course);
            }
            catch (Exception ex)
            {

            }
        }

        public async System.Threading.Tasks.Task GetCourseAppendix(Course course)
        {
            try
            {
                await fucker.GetCourseAppendix(course);
            }
            catch(Exception ex)
            {

            }
        }

        public async System.Threading.Tasks.Task GetIntegralInfo(Course course)
        {
            try
            {
                await fucker.GetIntegralInfo(course);
            }
            catch(Exception ex)
            { }
        }

        public async System.Threading.Tasks.Task GetIntegralInfo(ElectiveCourse course)
        {
            try
            {
                await fucker.GetIntegralInfo(course);
            }
            catch (Exception ex)
            { }
        }

        public async System.Threading.Tasks.Task<ExamList> GetExamListAsync(Course course)
        {
            try
            {
                return await fucker.GetExamList(course);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async System.Threading.Tasks.Task GetExamDetailAsync(Exam exam)
        {
            try
            {
                await fucker.GetExamDetail(exam);
            }
            catch(Exception ex)
            {

            }
        }


    }
}
