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

        public static void SaveGroups(IDataBaseRepository p_repository, List<Group> groups, Time currentTime, int lessonTypeId, int subjectId, int teacherId, int classroomId)
        {
            repository = p_repository;
            foreach (var unGroup in groups)
            {
                repository.DataBase.Database.ExecuteSqlCommand("exec [dbo].[csp_LessonSave] {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", unGroup.GroupID, currentTime.WeekNum, currentTime.DayNumber, currentTime.LessonTimeID, lessonTypeId, subjectId, teacherId, classroomId);
            }
        }

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


        public static string CellValueGenerator(DayModel dayModel, string lessonType, string typeData, int indexRow, bool doubleWeek)
        {
            string result = String.Empty;

            if (doubleWeek == true || lessonType.Trim() == "Лекція")
            {
                if (typeData == SearchType.stream.ToString()) 
                    result = lessonType.Trim() + ". " 
                        + dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject.Trim()).FirstOrDefault() + ". " 
                        + dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " 
                        + dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher.Trim()).FirstOrDefault() + " ауд. " 
                        + dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom.Trim()).FirstOrDefault();
                else if (typeData == SearchType.teacher.ToString())
                {
                    string groupsSection;
                    if (lessonType == "Лекція")
                    {
                        var lessonModel = GetLessonModel(dayModel, indexRow);
                        groupsSection = " Групи: " + GetGroupsOfLection(lessonModel, dayModel);
                    }
                    else 
                        groupsSection = " Група: " + dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p=>p.Group.Trim()).FirstOrDefault();

                    result = lessonType.Trim() + ". "
                        + dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject.Trim()).FirstOrDefault() 
                        + groupsSection
                        + " ауд. " + dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom.Trim()).FirstOrDefault();
                    }
            }
            else if (doubleWeek == false)
            {
                if (typeData == SearchType.stream.ToString())
                    result = dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " 
                        + dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher).FirstOrDefault();
                else if (typeData == SearchType.teacher.ToString())
                {
                    try
                    {
                        result = "Група: " + dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Group.Trim()).FirstOrDefault();
                    }
                    catch
                    {
                        result = String.Empty;
                    }
                }
            }

            return result;    
        }

        public static string GetGroupsOfLection(LessonModel lessonModel, DayModel dayModel)
        {
            string groupsNames = null;

            var timeID = repository.DataBase.Times.Where(p => p.WeekNum == dayModel.WeekNum).Where(p => p.LessonTimeID == lessonModel.LessonNumber).Where(p => p.DayNumber == dayModel.DayNumber).Select(p => p.TimeID).FirstOrDefault();
            var lessonEvents = repository.DataBase.LessonEvents.Where(p => p.ClassroomID == lessonModel.ClassroomID).Where(p => p.TimeID == timeID).Where(p => p.SubjectID == lessonModel.SubjectID).ToList();

            for (int i = 0; i < lessonEvents.Count; i++)
            {
                var groupName = repository.DataBase.Groups.Where(p => p.GroupID == lessonEvents[i].GroupID).Select(p => p.GroupName).FirstOrDefault();

                if (groupName != null)
                {
                    if (i == 0 || groupName == null)
                        groupsNames = groupName;
                    else
                        groupsNames += ", " + groupName; 
                }
            }

            return groupsNames;
        }

        public static LessonModel GetLessonModel(DayModel dayModel, int indexRow)
        {
            var lessonEvent = dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4).FirstOrDefault();
            if (lessonEvent == null) lessonEvent = dayModel.DayLessons.Where(p => p.LessonNumber == indexRow / 4 + 3).FirstOrDefault();

            return lessonEvent;
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