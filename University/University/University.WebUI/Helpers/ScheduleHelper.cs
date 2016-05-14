using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Domain.Abstract;
using University.Domain.Models;
using University.WebUI.Enums;
using University.WebUI.Models;

namespace University.WebUI.Helpers
{
    public static class ScheduleHelper
    {
        static int TypeDataNumber;
        static IDataBaseRepository repository;
        public static ScheduleTableModel GetTableModel(int Id, string typeData, int? numberFirstLesson, IDataBaseRepository p_repository)
        {
            repository = p_repository;
            if ((typeData.Equals(SearchType.stream.ToString()) || (typeData.Equals(SearchType.faculty.ToString()))))
                TypeDataNumber = 1;
            else if (typeData.Equals(SearchType.teacher.ToString()))
                TypeDataNumber = 2;
            else if (typeData.Equals(SearchType.classroom.ToString()))
                TypeDataNumber = 3;

            int firstLesson = 1;
            int groupId = 0;
            string groupName = "";
            int teacherId = 0;
            string teacherName = "";
            if (numberFirstLesson != null) { firstLesson = Convert.ToInt32(numberFirstLesson); }
            if (TypeDataNumber == 1)
            {
                groupId = Id;
                groupName = repository.DataBase.Groups.Where(p => p.GroupID == Id).Select(p => p.GroupName).FirstOrDefault();
            }
            else if (TypeDataNumber == 2)
            {
                teacherId = Id;
                var teacher = repository.DataBase.Teachers.Where(p => p.TeacherID == teacherId).FirstOrDefault();
                teacherName = teacher.Post + " " + teacher.Name;
            }

            ScheduleTableModel tableModel = new ScheduleTableModel()
            {
                GroupId = groupId,
                GroupName = groupName,
                TeacherId = teacherId,
                TeacherName = teacherName,
                FirstWeek = new WeekModel()
                {
                    Monday = GenerateDay("Понеділок", Id, 1, firstLesson, TypeDataNumber),
                    Thursday = GenerateDay("Вівторок", Id, 1, firstLesson, TypeDataNumber),
                    Wednesday = GenerateDay("Середа", Id, 1, firstLesson, TypeDataNumber),
                    Tuesday = GenerateDay("Четвер", Id, 1, firstLesson, TypeDataNumber),
                    Friday = GenerateDay("П'ятниця", Id, 1, firstLesson, TypeDataNumber)
                },
                SecondWeek = new WeekModel()
                {
                    Monday = GenerateDay("Понеділок", Id, 2, firstLesson, TypeDataNumber),
                    Thursday = GenerateDay("Вівторок", Id, 2, firstLesson, TypeDataNumber),
                    Wednesday = GenerateDay("Середа", Id, 2, firstLesson, TypeDataNumber),
                    Tuesday = GenerateDay("Четвер", Id, 2, firstLesson, TypeDataNumber),
                    Friday = GenerateDay("П'ятниця", Id, 2, firstLesson, TypeDataNumber)
                }
            };
            return tableModel;
        }

        private static DayModel GenerateDay(string dayOfWeek, int Id, int weekNum, int numberFirstLesson, int typeDataNumber)
        {
            var dayNumber = new SortedList<string, int>();
            dayNumber["Понеділок"] = 1;
            dayNumber["Вівторок"] = 2;
            dayNumber["Середа"] = 3;
            dayNumber["Четвер"] = 4;
            dayNumber["П'ятниця"] = 5;

            List<ProcedureResult> lessonsProc = new List<ProcedureResult>();

            if (typeDataNumber == 1)
                lessonsProc = repository.DataBase.Database.SqlQuery<ProcedureResult>("exec [dbo].[csp_LessonModelGet] {0}", Id).ToList<ProcedureResult>();
            else if (typeDataNumber == 2)
                lessonsProc = repository.DataBase.Database.SqlQuery<ProcedureResult>("exec [dbo].[csp_TeacherLessonModelGet] {0}", Id).ToList<ProcedureResult>();
            else if (typeDataNumber == 3)
                lessonsProc = repository.DataBase.Database.SqlQuery<ProcedureResult>("exec [dbo].[csp_ClassroomLessonModelGet] {0}", Id).ToList<ProcedureResult>();

            List<LessonModel> lessonsModel = new List<LessonModel>();
            for (int i = 1; i < 7; i++)
            {
                var lessonModel = new LessonModel()
                {
                    LessonNumber = i,
                    Subject = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.SubjectName).FirstOrDefault(),//repository.DataBase.Subjects.Where(p => p.SubjectID == item.SubjectID).Select(p => p.Name).FirstOrDefault(),
                    SubjectID = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.SubjectID).FirstOrDefault(),
                    LessonType = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.LessonTypeName).FirstOrDefault(),//repository.DataBase.LessonTypes.Where(p => p.LessonTypeID == item.LessonTypeID).Select(p => p.LessonTypeName).FirstOrDefault(),
                    LessonTypeID = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.LessonTimeID).FirstOrDefault(),
                    Teacher = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.TeacherName).FirstOrDefault(),//repository.DataBase.Teachers.Where(p => p.TeacherID == item.TeacherID).Select(p => p.Name).FirstOrDefault(),
                    TeacherID = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.TeacherID).FirstOrDefault(),
                    TeacherPost = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.Post).FirstOrDefault(),
                    Classroom = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.ClassroomName).FirstOrDefault(), //repository.DataBase.Classrooms.Where(p => p.ClassroomID == item.ClassroomID).Select(p => p.Name).FirstOrDefault()
                    ClassroomID = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.ClassroomID).FirstOrDefault(),
                    Group = lessonsProc.Where(p => p.DayOfWeek.Trim() == dayOfWeek.Trim()).Where(p => p.WeekNum == weekNum).Where(p => p.LessonTimeID == i).Select(p => p.GroupName).FirstOrDefault()
                };
                lessonsModel.Add(lessonModel);
            }
            DayModel currentDay = new DayModel()
            {
                DayName = dayOfWeek,
                DayNumber = dayNumber[dayOfWeek],
                DayLessons = lessonsModel,
                FirstLesson = numberFirstLesson,
                WeekNum = weekNum
            };
            return currentDay;
        }



        public static string TextChange(string text)
        {
            //string text = "123(текст)123";
            int pos1 = text.IndexOf('(');
            int pos2 = -1;
            if (pos1 >= 0)
                pos2 = text.IndexOf(')', pos1);
            if (pos1 >= 0 && pos2 >= 0)
                text = text.Remove(pos1, pos2 - pos1 + 1);
            pos1 = text.IndexOf('{');
            pos2 = -1;
            if (pos1 >= 0)
                pos2 = text.IndexOf('}', pos1);
            if (pos1 >= 0 && pos2 >= 0)
                text = text.Remove(pos1, pos2 - pos1 + 1);
            return text.Trim();
        }

    }
}