using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Domain.Abstract;
using University.Domain.Models;
using University.WebUI.Models;

namespace University.WebUI.Helpers
{
    public static class SaveFromExcelHelper
    {
        static IDataBaseRepository repository;
        public static List<string> TeacherGroups(string teacher, string typeLesson, string teacherName,string  currentSubject, ref string errorMessage)
        {
            List<string> groupsForTeacher = null;
            bool correct = true;
            try
            {
                string teacherGroup = teacher.Split('{', '}')[1];
                if (teacherGroup != null && teacherGroup != String.Empty)
                {
                    groupsForTeacher = teacherGroup.Split(';').ToList();
                    if (groupsForTeacher == null || groupsForTeacher.Count == 0)
                        correct = false;
                }
                else correct = false;
            }
            catch
            {
                correct = false;
            }
            if(!correct)
            {
                errorMessage = string.Format("Помилка зчитування даних груп для {0} пар викладача. Запис має вигляд:  '{1}'. ", typeLesson, teacherName.Trim());
                errorMessage += "Запис повинен мати вигляд: ПІБ викладача (посада викладача) {ГРУПА1;ГРУПА2}. ";
                errorMessage += string.Format("Поточний предмет: {0}.", currentSubject.Trim());
                return null;
            }
            return groupsForTeacher;
        }

        public static List<Group> GetGroups(IDataBaseRepository _repository, ExcelTableRowModel row, ref string errorMessage)
        {
            repository = _repository;
            List<Group> currentGroups = new List<Group>();
            List<string> groupsNames;
            //if (row.GroupsOfStream != null && row.UnitedGroups != null)
            //{
            //    errorMessage = String.Format("Групи для предмету не можуть бути вписані одночасно у колонки 'Групи потоку' та 'Об'єднані групи на певні предмети'.\nПоточний прдемет: {0}", row.Subject.Trim());
            //    return null;
            //}
            if (row.GroupsOfStream != null)
                groupsNames = row.GroupsOfStream.Split(',').ToList();
            else if (row.UnitedGroups != null)
                groupsNames = row.UnitedGroups.Split(',').ToList();
            else
            {
                errorMessage = String.Format("У документі відсутній список груп.\nПоточний предмет: {0}", row.Subject.Trim());
                return null;
            }
            bool correct = true;
            foreach (var group in groupsNames)
            {
                try
                {
                    var currentGroup = repository.DataBase.Groups.Where(p => p.GroupName.Trim() == group.Trim()).FirstOrDefault();
                    if (currentGroup != null)
                        currentGroups.Add(currentGroup);
                    else correct = false;
                }
                catch
                {
                    correct = false;
                }
                if(correct == false)
                {
                    errorMessage = String.Format("Група {0} відсутня у базі даних. Якщо назва групи не відображається, то імовірно в документі у переліку груп стоїть лишня кома. У випадку відображення некоректної назви групи, перевірте правильність заповнення даних(групи мають бути записані через кому)\nПоточний предмет: {1}.", group.Trim(), row.Subject.Trim());
                    return null;
                }
            }

            return currentGroups;
        }

        public static List<Group> GetGroupsFromDB(IDataBaseRepository _repository, List<string> groupsNames, string subjectName, ref string errorMessage)
        {
            repository = _repository;
            List<Group> currentGroups = new List<Group>();

            bool correct = true;
            foreach (var group in groupsNames)
            {
                try
                {
                    var currentGroup = repository.DataBase.Groups.Where(p => p.GroupName.Trim() == group.Trim()).FirstOrDefault();
                    if (currentGroup != null)
                        currentGroups.Add(currentGroup);
                    else correct = false;
                }
                catch
                {
                    correct = false;
                }
                if (correct == false)
                {
                    errorMessage = String.Format("Група {0} відсутня у базі даних. Якщо назва групи не відображається, то імовірно в документі у переліку груп стоїть лишня кома. У випадку відображення некоректної назви групи, перевірте правильність заповнення даних(групи мають бути записані через кому)\nПоточний предмет: {1}.", group.Trim(), subjectName);
                    return null;
                }
            }

            return currentGroups;
        }

        public static bool CheckGroupsForTeachers(SortedList<string, Teacher> teachersPracticalGroups, 
            SortedList<string, Teacher> teachersLaboratoryGroups, 
            int pracHour, 
            int labHour,
            Group currentGroup, 
            ref string errorMessage)
        {
            if (pracHour != 0 && labHour != 0)
            {
                try
                {
                    var teacher = teachersPracticalGroups[currentGroup.GroupName.Trim()];
                }
                catch
                {
                    try
                    {
                        var teacher = teachersLaboratoryGroups[currentGroup.GroupName.Trim()];
                    }
                    catch
                    {
                        errorMessage = String.Format("Помилка завантаження. Група {0} відсутня у переліку груп для пратичних та лабораторних пар викладача. Якщо така група присутня, тоді скопіюйте в документі дану групу з колонки 'Групи потоку' та замініть її в переліку груп для викладача та перевірте правильність заповнення(коми між викладачами та крапки з комами між групами). ", currentGroup.GroupName.Trim());
                        return false;
                    }
                }
            }
            else if (pracHour != 0 && labHour == 0)
            {
                try
                {
                    var teacher = teachersPracticalGroups[currentGroup.GroupName.Trim()];
                }
                catch
                {
                    errorMessage = String.Format("Помилка завантаження. Група {0} відсутня у переліку груп для пратичних пар викладача. Якщо така група присутня, тоді скопіюйте в документі дану групу з колонки 'Групи потоку' та замініть її в переліку груп для викладача та перевірте правильність заповнення(коми між викладачами та крапки з комами між групами). ", currentGroup.GroupName.Trim());
                    return false;
                }
            }
            else if (pracHour == 0 && labHour != 0)
            {
                try
                {
                    var teacher = teachersLaboratoryGroups[currentGroup.GroupName.Trim()];
                }
                catch
                {
                    errorMessage = String.Format("Помилка завантаження. Група {0} відсутня у переліку груп для лабораторних пар викладача. Якщо така група присутня, тоді скопіюйте в документі дану групу з колонки 'Групи потоку' та замініть її в переліку груп для викладача та перевірте правильність заповнення(коми між викладачами та крапки з комами між групами). ", currentGroup.GroupName.Trim());
                    return false;
                }
            }

            errorMessage = String.Empty;
            return true;
        }
    }
}