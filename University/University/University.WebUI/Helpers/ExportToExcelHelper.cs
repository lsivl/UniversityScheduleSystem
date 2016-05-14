using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using University.Domain.Abstract;
using University.Domain.Models;
using University.WebUI.Models;

namespace University.WebUI.Helpers
{
    public class ExportToExcelHelper
    {
        IDataBaseRepository repository;
        List<LessonEvent> lections;

        public ExportToExcelHelper(IDataBaseRepository repository)
        {
            lections = new List<LessonEvent>();
            this.repository = repository;
        }
        public string CreateExcelTable(int facultyID, string filePath)
        {
          
            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();

            string facultyName = repository.DataBase.Faculties.Where(p => p.FacultyID == facultyID).Select(p => p.FullName).FirstOrDefault();

            var newFileName = facultyName.Trim() + "(розклад).xls";
            var newFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\TextTeplates\" + newFileName;
            System.IO.File.Copy(filePath, newFilePath, true);
            filePath = newFilePath;
         //   ExcelApp.Visible = true;

            //Получаем набор объектов Workbook (массив ссылок на созданные книги)
            var excelappworkbooks = ExcelApp.Workbooks;
            //Открываем книгу и получаем на нее ссылку
            var excelAppWorkBook = ExcelApp.Workbooks.Open(filePath,
                            Type.Missing, Type.Missing, Type.Missing,
           "WWWWW", "WWWWW", Type.Missing, Type.Missing, Type.Missing,
           Type.Missing, Type.Missing, Type.Missing, Type.Missing,
           Type.Missing, Type.Missing);

            var excelSheets = excelAppWorkBook.Worksheets;

         
            List<Stream> facultyStreams = repository.DataBase.Streams.Where(p => p.FacultyID == facultyID).ToList();
            List<ScheduleTableModel> ScheduleTableModels = new List<ScheduleTableModel>();
            lections = new List<LessonEvent>();
            var column = 3;

            facultyName = facultyName.Replace("Факультет", "");

            for (int i = 1; i < 6; i++)
            {
                var courseStreams = facultyStreams.Where(p => p.YearOfStudy == i).ToList();

                var excelWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelSheets.get_Item(i);

                

                excelWorkSheet.Cells.Replace("faculty", facultyName);
                excelWorkSheet.Cells.Replace("courseNum", i);
                excelWorkSheet.Cells.Replace("year", DateTime.Now.Year);
              

                if (courseStreams != null)
                {
                    foreach (var stream in courseStreams)
                    {
                        List<Group> streamGroups = repository.DataBase.Groups.Where(p => p.StreamID == stream.StreamID).ToList();
                        foreach (var group in streamGroups)
                        {
                            // ScheduleTableModels.Add(GetTableModel(group.GroupID, "faculty", null, p_repository));
                            column = InsertGroupToExcelTable(ScheduleHelper.GetTableModel(group.GroupID, "faculty", null, repository), column, excelWorkSheet);
                        }
                    }
                }
            }
            excelAppWorkBook.Save();
            excelAppWorkBook.Close();
            excelappworkbooks.Close();
            ExcelApp.Quit();

            Marshal.ReleaseComObject(excelAppWorkBook);
            Marshal.ReleaseComObject(excelappworkbooks);
            Marshal.ReleaseComObject(ExcelApp);

            return newFileName;
        }


        private int InsertGroupToExcelTable(ScheduleTableModel scheduleTableModel, int columnNumber, Microsoft.Office.Interop.Excel.Worksheet excelWorkSheet)
        {
            var row = 8;

            var column = columnNumber;

            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Monday, scheduleTableModel.SecondWeek.Monday, row, column, scheduleTableModel.GroupName);
                else if (i == 1)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Thursday, scheduleTableModel.SecondWeek.Thursday, row, column);
                else if (i == 2)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Wednesday, scheduleTableModel.SecondWeek.Wednesday, row, column);
                else if (i == 3)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Tuesday, scheduleTableModel.SecondWeek.Tuesday, row, column);
                else if (i == 4)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Friday, scheduleTableModel.SecondWeek.Friday, row, column);
                row += 16;
            }
            return column += 3;
        }


        private void CreateFrame(Microsoft.Office.Interop.Excel.Worksheet excelWorkSheet, DayModel firstWeekDay, DayModel secondWeekDay, int row, int column, string groupName = null)
        {
            var LongToShortLessonType = new SortedList<string, string>();
            LongToShortLessonType["Лекція"] = "Лек.";
            LongToShortLessonType["Практика"] = "Пр.";
            LongToShortLessonType["Лабораторна"] = "Лаб.";
            Microsoft.Office.Interop.Excel.Range excelcells;
            var NumberLesson = new SortedList<string, string>();
            NumberLesson["1"] = "I";
            NumberLesson["2"] = "II";
            NumberLesson["3"] = "III";
            NumberLesson["4"] = "IV";
            NumberLesson["5"] = "V";
            NumberLesson["6"] = "VI";
            


            for (int i = column; i < column + 3; i++)
            {

                var indexRow = 0;


                for (int j = row; j < row + 16; j++)
                {
                    if (indexRow % 4 == 0 && indexRow != 0)
                    {
                        if (i == column)
                        {
                            bool doubleWeek = false;
                            var correctCell = 0;
                            var lessonEvent = firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).FirstOrDefault();
                            if (lessonEvent == null) firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4 + 3).FirstOrDefault();
                            var secondlessonEvent = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).FirstOrDefault();
                            if (secondlessonEvent == null) secondlessonEvent = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4 + 3).FirstOrDefault();
                            var lessonType = lessonEvent.LessonType;
                            if (column == 3)
                            {
                                    (excelWorkSheet.Cells[j - 4, i - 1] as Microsoft.Office.Interop.Excel.Range).Value2 = NumberLesson[(lessonEvent.LessonNumber).ToString()];
                            }
                            if (lessonEvent.Subject == secondlessonEvent.Subject && lessonEvent.Teacher == secondlessonEvent.Teacher && lessonEvent.LessonType == secondlessonEvent.LessonType)
                            {
                                doubleWeek = true;
                                correctCell = 2;
                            }
                            if (lessonType != null)
                            {
                                if (lessonType.Trim() == "Лекція")
                                {
                                    bool exist = false;
                                    var timeID = repository.DataBase.Times.Where(p => p.WeekNum == firstWeekDay.WeekNum).Where(p => p.LessonTimeID == lessonEvent.LessonNumber).Where(p => p.DayNumber == firstWeekDay.DayNumber).Select(p => p.TimeID).FirstOrDefault();
                                    var lessonEvents = repository.DataBase.LessonEvents.Where(p => p.ClassroomID == lessonEvent.ClassroomID).Where(p => p.TimeID == timeID).Where(p => p.SubjectID == lessonEvent.SubjectID).ToList();
                                    foreach (var lesson in lessonEvents)
                                    {
                                        exist = lections.Contains(lesson);
                                        if (exist == true)
                                            break;
                                    }
                                    if (exist == false)
                                    {

                                        var groupsCount = lessonEvents.Count;
                                        var correctValue = 0;
                                        if (groupsCount < 2) correctValue = -1;
                                        excelcells = excelWorkSheet.get_Range((excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range), (excelWorkSheet.Cells[j - 3 + correctCell, i + 2 * groupsCount + 1 + correctValue] as Microsoft.Office.Interop.Excel.Range));
                                        excelcells.Merge(Type.Missing);
                                        excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                        excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                                        if (doubleWeek == false)
                                            excelcells.Font.Size = 12;
                                        else if (doubleWeek == true) 
                                        excelcells.Font.Size = 13;
                                        excelcells.Value2 = lessonType.Trim() + ". " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject.Trim()).FirstOrDefault() + ". " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher.Trim()).FirstOrDefault() + " ауд. " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom.Trim()).FirstOrDefault();
                                        lections.AddRange(lessonEvents);
                                    }
                                    if (doubleWeek == false)
                                        goto NEXT;
                                    else if (doubleWeek == true)
                                        goto NEXT2;
                                }
                                if (doubleWeek == true)
                                {
                                    excelcells = excelWorkSheet.get_Range((excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range), (excelWorkSheet.Cells[j - 3 + correctCell, i + 2] as Microsoft.Office.Interop.Excel.Range));
                                    excelcells.Merge(Type.Missing);
                                    excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                    excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                                    excelcells.Font.Size = 14;
                                    excelcells.Value2 = lessonType.Trim() + ". " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject.Trim()).FirstOrDefault() + ". " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher.Trim()).FirstOrDefault() + " ауд. " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom.Trim()).FirstOrDefault();
                                    goto NEXT2;
                                }
                                (excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range).Value2 = LongToShortLessonType[lessonType.Trim()];
                            }
                        }
                        else if (i == column + 1)
                        {
                            (excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range).Value2 = firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject).FirstOrDefault();
                            (excelWorkSheet.Cells[j - 3, i] as Microsoft.Office.Interop.Excel.Range).Value2 = firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher).FirstOrDefault();
                        }
                        else if (i == column + 2)
                            (excelWorkSheet.Cells[j - 3, i] as Microsoft.Office.Interop.Excel.Range).Value2 = firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom).FirstOrDefault();

                    NEXT:

                        if (i == column)
                        {
                            var lessonEvent = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).FirstOrDefault();
                            var lessonType = lessonEvent.LessonType;
                            if (lessonType != null)
                            {
                                if (lessonType.Trim() == "Лекція")
                                {
                                    bool exist = false;
                                    var timeID = repository.DataBase.Times.Where(p => p.WeekNum == secondWeekDay.WeekNum).Where(p => p.LessonTimeID == lessonEvent.LessonNumber).Where(p => p.DayNumber == secondWeekDay.DayNumber).Select(p => p.TimeID).FirstOrDefault();
                                    var lessonEvents = repository.DataBase.LessonEvents.Where(p => p.ClassroomID == lessonEvent.ClassroomID).Where(p => p.TimeID == timeID).Where(p => p.SubjectID == lessonEvent.SubjectID).ToList();
                                    foreach (var lesson in lessonEvents)
                                    {
                                        exist = lections.Contains(lesson);
                                        if (exist == true)
                                            break;
                                    }
                                    if (exist == false)
                                    {
                                        var groupsCount = lessonEvents.Count;
                                        var correctValue = 0;
                                        if (groupsCount < 2) correctValue = -1;
                                        excelcells = excelWorkSheet.get_Range((excelWorkSheet.Cells[j - 2, i] as Microsoft.Office.Interop.Excel.Range), (excelWorkSheet.Cells[j - 1, i + 2 * groupsCount + 1 + correctValue] as Microsoft.Office.Interop.Excel.Range));
                                        excelcells.Merge(Type.Missing);
                                        excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                        excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                                        excelcells.Font.Size = 12;
                                        excelcells.Value2 = lessonType.Trim() + ". " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject.Trim()).FirstOrDefault() + ". " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher.Trim()).FirstOrDefault() + " ауд. " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom.Trim()).FirstOrDefault();
                                        lections.AddRange(lessonEvents);
                                    }
                                    goto NEXT2;
                                }

                                (excelWorkSheet.Cells[j - 2, i] as Microsoft.Office.Interop.Excel.Range).Value2 = LongToShortLessonType[lessonType.Trim()];
                            }
                        }
                        else if (i == column + 1)
                        {
                            (excelWorkSheet.Cells[j - 2, i] as Microsoft.Office.Interop.Excel.Range).Value2 = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject).FirstOrDefault();
                            (excelWorkSheet.Cells[j - 1, i] as Microsoft.Office.Interop.Excel.Range).Value2 = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher).FirstOrDefault();
                        }
                        else if (i == column + 2)
                            (excelWorkSheet.Cells[j - 1, i] as Microsoft.Office.Interop.Excel.Range).Value2 = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom).FirstOrDefault();
                    }
                NEXT2:
                    try
                    {
                        if (indexRow % 2 == 0)
                        {
                            (excelWorkSheet.Cells[j, i] as Microsoft.Office.Interop.Excel.Range).Borders[XlBordersIndex.xlEdgeTop].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                            if (j + 1 == row + 16 || j == row) (excelWorkSheet.Cells[j, i] as Microsoft.Office.Interop.Excel.Range).Borders[XlBordersIndex.xlEdgeTop].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;

                        }

                        if (i == column + 2) (excelWorkSheet.Cells[j, i] as Microsoft.Office.Interop.Excel.Range).Borders[XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                        if (j == row + 15)
                        {
                            (excelWorkSheet.Cells[j, i] as Microsoft.Office.Interop.Excel.Range).Borders[XlBordersIndex.xlEdgeBottom].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                            (excelWorkSheet.Cells[j, i] as Microsoft.Office.Interop.Excel.Range).Borders[XlBordersIndex.xlEdgeBottom].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
                        }
                    }
                    catch { }
                    if (j == 8)
                    {
                        (excelWorkSheet.Cells[row - 1, i] as Microsoft.Office.Interop.Excel.Range).Borders[XlBordersIndex.xlEdgeTop].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                        (excelWorkSheet.Cells[row - 1, i] as Microsoft.Office.Interop.Excel.Range).Borders[XlBordersIndex.xlEdgeTop].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
                        if (i == column + 1) (excelWorkSheet.Cells[row - 1, i] as Microsoft.Office.Interop.Excel.Range).Value2 = groupName;
                        if (i == column + 2) (excelWorkSheet.Cells[row - 1, i] as Microsoft.Office.Interop.Excel.Range).Borders[XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    }
                    indexRow++;
                }
            }

        }

           


    }



}