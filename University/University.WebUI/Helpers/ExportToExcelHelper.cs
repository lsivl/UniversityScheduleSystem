
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using University.Domain.Abstract;
using University.Domain.Models;
using University.WebUI.Models;
using University.WebUI.Enums;

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

        //public static bool GetMicrosoftOfficeExcelWorkbook(System.IO.FileStream pDataFile, out Lite.ExcelLibrary.SpreadSheet.Workbook pWorkbook)
        //{
        //    bool vReturn = false;

        //    pWorkbook = null;
        //    if (pDataFile == null)
        //        return vReturn;

        //    try
        //    {
        //        pWorkbook = Lite.ExcelLibrary.SpreadSheet.Workbook.Open(pDataFile);
        //        vReturn = true;
        //    }
        //    catch
        //    {
        //        pWorkbook = null;
        //        vReturn = false;
        //    }
        //    return vReturn;
        //}



        public string CreateExcelTable(int facultyID, string filePath, string typeData)
        {

            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();

            var faculty = repository.DataBase.Faculties.Where(p => p.FacultyID == facultyID).FirstOrDefault();

            var newFileName = faculty.FullName.Trim() + "(розклад).xls";
            var newFilePath = System.IO.Path.GetTempPath() + newFileName;
            System.IO.File.Copy(filePath, newFilePath, true);
            filePath = newFilePath;
            ExcelApp.Visible = true;

            //Получаем набор объектов Workbook (массив ссылок на созданные книги)
            var excelappworkbooks = ExcelApp.Workbooks;
            //Открываем книгу и получаем на нее ссылку
            var excelAppWorkBook = ExcelApp.Workbooks.Open(filePath,
                            Type.Missing, Type.Missing, Type.Missing,
           "WWWWW", "WWWWW", Type.Missing, Type.Missing, Type.Missing,
           Type.Missing, Type.Missing, Type.Missing, Type.Missing,
           Type.Missing, Type.Missing);


            var excelSheets = excelAppWorkBook.Worksheets;

            if (typeData == SearchType.stream.ToString())
                GenerateStreamsTable(faculty, excelSheets);
            else if (typeData == SearchType.teacher.ToString())
                GenerateTeachersTable(faculty, excelSheets);



            //faculty.FullName = faculty.FullName.Replace("Факультет", "");

            //List<Stream> facultyStreams = repository.DataBase.Streams.Where(p => p.FacultyID == facultyID).ToList();
            //List<ScheduleTableModel> ScheduleTableModels = new List<ScheduleTableModel>();
            //lections = new List<LessonEvent>();


            //for (int i = 1; i < 6; i++)
            //{
            //    var column = 3;
            //    List<Stream> orderedStreams = new List<Stream>();

            //    var courseStreams = facultyStreams.Where(p => p.YearOfStudy == i).ToList();

            //    var excelWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelSheets[i];

            //    excelWorkSheet.Cells.Font.Italic = false;
            //    excelWorkSheet.Cells.Font.Name = "Times New Roman";

            //    excelWorkSheet.Cells.Replace("faculty", facultyName);
            //    excelWorkSheet.Cells.Replace("courseNum", i);
            //    excelWorkSheet.Cells.Replace("year", DateTime.Now.Year);


            //    if (courseStreams != null)
            //    {

            //        for (int streamNumber = 0; streamNumber < courseStreams.Count; streamNumber++)
            //        {
            //            var currentStream = courseStreams[streamNumber];
            //            List<Group> streamGroups = repository.DataBase.Groups.Where(p => p.StreamID == currentStream.StreamID).ToList();
            //            for (int groupNumber = 0; groupNumber < streamGroups.Count; groupNumber++)
            //            {
            //                column = InsertGroupToExcelTable(ScheduleHelper.GetTableModel(streamGroups[groupNumber].GroupID, "faculty", null, repository), column, excelWorkSheet);

            //                if (groupNumber == streamGroups.Count - 1
            //                    && orderedStreams.Where(p => p.StreamID == currentStream.StreamID).FirstOrDefault() == null)
            //                {
            //                    var nextStreams = NextStreams(currentStream, courseStreams);
            //                    if (nextStreams.Count > 0)
            //                    {
            //                        foreach (var nextStream in nextStreams)
            //                        {
            //                            courseStreams.Remove(nextStream);
            //                        }
            //                        courseStreams.InsertRange(streamNumber + 1, nextStreams);
            //                        orderedStreams.AddRange(nextStreams);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            excelAppWorkBook.Save();
            excelAppWorkBook.Close();
            ExcelApp.Quit();

           // Marshal.ReleaseComObject(ExcelApp);

            return filePath;
        }

        private void GenerateStreamsTable(Faculty faculty, Sheets excelSheets)
        {
            List<Stream> facultyStreams = repository.DataBase.Streams.Where(p => p.FacultyID == faculty.FacultyID).ToList();
            List<ScheduleTableModel> ScheduleTableModels = new List<ScheduleTableModel>();
            lections = new List<LessonEvent>();


            for (int i = 1; i < 6; i++)
            {
                var column = 3;
                List<Stream> orderedStreams = new List<Stream>();

                var courseStreams = facultyStreams.Where(p => p.YearOfStudy == i).ToList();

                var excelWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelSheets[i];

                excelWorkSheet.Cells.Font.Italic = false;
                excelWorkSheet.Cells.Font.Name = "Times New Roman";

                string facultyName = faculty.FullName.Replace("Факультет", String.Empty);
                excelWorkSheet.Cells.Replace("faculty", facultyName);
                excelWorkSheet.Cells.Replace("courseNum", i);
                excelWorkSheet.Cells.Replace("year", DateTime.Now.Year);


                if (courseStreams != null)
                {

                    for (int streamNumber = 0; streamNumber < courseStreams.Count; streamNumber++)
                    {
                        var currentStream = courseStreams[streamNumber];
                        List<Group> streamGroups = repository.DataBase.Groups.Where(p => p.StreamID == currentStream.StreamID).ToList();
                        for (int groupNumber = 0; groupNumber < streamGroups.Count; groupNumber++)
                        {
                            column = InsertGroupToExcelTable(ScheduleHelper.GetTableModel(streamGroups[groupNumber].GroupID, SearchType.stream.ToString(), null, repository), column, excelWorkSheet, SearchType.stream.ToString());

                            if (groupNumber == streamGroups.Count - 1
                                && orderedStreams.Where(p => p.StreamID == currentStream.StreamID).FirstOrDefault() == null)
                            {
                                var nextStreams = NextStreams(currentStream, courseStreams);
                                if (nextStreams.Count > 0)
                                {
                                    foreach (var nextStream in nextStreams)
                                    {
                                        courseStreams.Remove(nextStream);
                                    }
                                    courseStreams.InsertRange(streamNumber + 1, nextStreams);
                                    orderedStreams.AddRange(nextStreams);
                                }
                            }
                        }
                    }
                }
            }
        }


        private void GenerateTeachersTable(Faculty faculty, Sheets excelSheets)
        {
            List<int> cathedraIds = repository.DataBase.Cathedras.Where(p => p.FacultyID == faculty.FacultyID).Select(p => p.CathedraID).ToList();

            var facultyTeachers = new List<Teacher>();


            foreach (var cathedraId in cathedraIds)
            {
                var cathedraTeachers = repository.DataBase.Teachers.Where(p => p.CathedraID == cathedraId).ToList();

                if (cathedraTeachers.Count > 0)
                    facultyTeachers.AddRange(cathedraTeachers);
            }



            List<ScheduleTableModel> ScheduleTableModels = new List<ScheduleTableModel>();



            var column = 3;
            List<Stream> orderedStreams = new List<Stream>();


            var excelWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelSheets[1];

            excelWorkSheet.Cells.Font.Italic = false;
            excelWorkSheet.Cells.Font.Name = "Times New Roman";

            string facultyName = faculty.FullName.Replace("Факультет", String.Empty);
            excelWorkSheet.Cells.Replace("faculty", facultyName);

            foreach (var teacher in facultyTeachers)
            {
                column = InsertGroupToExcelTable(ScheduleHelper.GetTableModel(teacher.TeacherID, SearchType.teacher.ToString(), null, repository), column, excelWorkSheet, SearchType.teacher.ToString());
            }

        }

        private int InsertGroupToExcelTable(ScheduleTableModel scheduleTableModel, int columnNumber, Microsoft.Office.Interop.Excel.Worksheet excelWorkSheet, string typeData)
        {
            var row = 8;

            var column = columnNumber;

            string columnName = null;

            if (typeData == SearchType.stream.ToString())
                columnName = scheduleTableModel.GroupName.Trim();
            else if (typeData == SearchType.teacher.ToString())
                columnName = scheduleTableModel.TeacherName;

            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Monday, scheduleTableModel.SecondWeek.Monday, row, column, typeData, columnName);
                else if (i == 1)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Thursday, scheduleTableModel.SecondWeek.Thursday, row, column, typeData);
                else if (i == 2)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Wednesday, scheduleTableModel.SecondWeek.Wednesday, row, column, typeData);
                else if (i == 3)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Tuesday, scheduleTableModel.SecondWeek.Tuesday, row, column, typeData);
                else if (i == 4)
                    CreateFrame(excelWorkSheet, scheduleTableModel.FirstWeek.Friday, scheduleTableModel.SecondWeek.Friday, row, column, typeData);
                row += 16;
            }
            return column += 3;
        }


        private void CreateFrame(Microsoft.Office.Interop.Excel.Worksheet excelWorkSheet, DayModel firstWeekDay, DayModel secondWeekDay, int row, int column, string typeData, string columnName = null)
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


                for (int j = row; j <= row + 16; j++)
                {
                    if (indexRow % 4 == 0 && indexRow != 0)
                    {
                        if (i == column)
                        {
                            bool doubleWeek = false;
                            var correctCell = 0;
                            var lessonModel = ScheduleHelper.GetLessonModel(firstWeekDay, indexRow);
                            var secondLessonModel = ScheduleHelper.GetLessonModel(secondWeekDay, indexRow);
                            //var lessonEvent = firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).FirstOrDefault();
                            //if (lessonEvent == null) lessonEvent = firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4 + 3).FirstOrDefault();
                            //var secondlessonEvent = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).FirstOrDefault();
                            //if (secondlessonEvent == null) secondlessonEvent = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4 + 3).FirstOrDefault();
                            var lessonType = lessonModel.LessonType;
                            if (column == 3)
                            {
                                (excelWorkSheet.Cells[j - 4, i - 1] as Microsoft.Office.Interop.Excel.Range).Value2 = NumberLesson[(lessonModel.LessonNumber).ToString()];
                            }
                            if (lessonModel.Subject == secondLessonModel.Subject && lessonModel.Teacher == secondLessonModel.Teacher && lessonModel.LessonType == secondLessonModel.LessonType)
                            {
                                doubleWeek = true;
                                correctCell = 2;
                            }
                            if (lessonType != null)
                            {
                                if (lessonType.Trim() == "Лекція")
                                {
                                    bool exist = false;

                                    var timeID = repository.DataBase.Times.Where(p => p.WeekNum == firstWeekDay.WeekNum).Where(p => p.LessonTimeID == lessonModel.LessonNumber).Where(p => p.DayNumber == firstWeekDay.DayNumber).Select(p => p.TimeID).FirstOrDefault();
                                    var lessonEvents = repository.DataBase.LessonEvents.Where(p => p.ClassroomID == lessonModel.ClassroomID).Where(p => p.TimeID == timeID).Where(p => p.SubjectID == lessonModel.SubjectID).ToList();
                                    var groupsCount = lessonEvents.Count;
                                    if (groupsCount < 2 || typeData == SearchType.teacher.ToString())
                                        goto NOT_LECTION_SECTION;
                                    foreach (var lesson in lessonEvents)
                                    {
                                        exist = lections.Contains(lesson);
                                        if (exist == true)
                                            break;
                                    }
                                    if (exist == false)
                                    {
                                        var correctValue = 0;
                                        excelcells = excelWorkSheet.get_Range((excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range), (excelWorkSheet.Cells[j - 3 + correctCell, i + 3 * groupsCount - 1 + correctValue] as Microsoft.Office.Interop.Excel.Range));
                                        excelcells.Merge(Type.Missing);
                                        excelcells.Cells.Columns.AutoFit();
                                        excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                        excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                                        if (doubleWeek == false)
                                            excelcells.Font.Size = 14;
                                        else if (doubleWeek == true)
                                            excelcells.Font.Size = 15;
                                        excelcells.Font.Bold = true;
                                        //excelcells.Value2 = lessonType.Trim() + ". " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject.Trim()).FirstOrDefault() + ". " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher.Trim()).FirstOrDefault() + " ауд. " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom.Trim()).FirstOrDefault();
                                        excelcells.Value2 = ScheduleHelper.CellValueGenerator(firstWeekDay, lessonType, typeData, indexRow, doubleWeek);
                                        lections.AddRange(lessonEvents);
                                    }
                                    if (doubleWeek == false)
                                        goto NEXT;
                                    else if (doubleWeek == true)
                                        goto NEXT2;
                                }
                            NOT_LECTION_SECTION:
                                if (doubleWeek == true)
                                {
                                    excelcells = excelWorkSheet.get_Range((excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range), (excelWorkSheet.Cells[j - 3 + correctCell, i + 2] as Microsoft.Office.Interop.Excel.Range));
                                    excelcells.Merge(Type.Missing);
                                    excelcells.Cells.Columns.AutoFit();
                                    excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                    excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                                    excelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                                    excelcells.Font.Size = 14;
                                    //excelcells.Value2 = lessonType.Trim() + ". " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject.Trim()).FirstOrDefault() + ". " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher.Trim()).FirstOrDefault() + " ауд. " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom.Trim()).FirstOrDefault();
                                    excelcells.Value2 = ScheduleHelper.CellValueGenerator(firstWeekDay, lessonType, typeData, indexRow, doubleWeek);
                                    goto NEXT2;
                                }
                                SetCellProperties(excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range, LongToShortLessonType[lessonType.Trim()]);
                                //(excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range).Value2 = LongToShortLessonType[lessonType.Trim()];
                            }
                        }
                        else if (i == column + 1)
                        {
                            SetCellProperties(excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range, firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject).FirstOrDefault());
                            //(excelWorkSheet.Cells[j - 4, i] as Microsoft.Office.Interop.Excel.Range).Value2 = firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject).FirstOrDefault();
                            SetCellProperties(excelWorkSheet.Cells[j - 3, i] as Microsoft.Office.Interop.Excel.Range, ScheduleHelper.CellValueGenerator(firstWeekDay, String.Empty, typeData, indexRow, false));
                            //(excelWorkSheet.Cells[j - 3, i] as Microsoft.Office.Interop.Excel.Range).Value2 = ScheduleHelper.CellValueGenerator(firstWeekDay, String.Empty, typeData, indexRow, false);
                            //firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher).FirstOrDefault();
                        }
                        else if (i == column + 2)
                            SetCellProperties(excelWorkSheet.Cells[j - 3, i] as Microsoft.Office.Interop.Excel.Range, firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom).FirstOrDefault());
                            //(excelWorkSheet.Cells[j - 3, i] as Microsoft.Office.Interop.Excel.Range).Value2 = firstWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom).FirstOrDefault();

                    NEXT:

                        if (i == column)
                        {
                            var lessonModel = ScheduleHelper.GetLessonModel(secondWeekDay, indexRow);
                            //secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).FirstOrDefault();
                            var lessonType = lessonModel.LessonType;
                            if (lessonType != null)
                            {
                                if (lessonType.Trim() == "Лекція")
                                {
                                    bool exist = false;
                                    var timeID = repository.DataBase.Times.Where(p => p.WeekNum == secondWeekDay.WeekNum).Where(p => p.LessonTimeID == lessonModel.LessonNumber).Where(p => p.DayNumber == secondWeekDay.DayNumber).Select(p => p.TimeID).FirstOrDefault();
                                    var lessonEvents = repository.DataBase.LessonEvents.Where(p => p.ClassroomID == lessonModel.ClassroomID).Where(p => p.TimeID == timeID).Where(p => p.SubjectID == lessonModel.SubjectID).ToList();
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
                                        if (groupsCount < 2 || typeData == SearchType.teacher.ToString())
                                            goto NOT_LECTION_SECTION;
                                        excelcells = excelWorkSheet.get_Range((excelWorkSheet.Cells[j - 2, i] as Microsoft.Office.Interop.Excel.Range), (excelWorkSheet.Cells[j - 1, i + 3 * groupsCount - 1 + correctValue] as Microsoft.Office.Interop.Excel.Range));
                                        excelcells.Merge(Type.Missing);
                                        excelcells.Cells.Columns.AutoFit();
                                        excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                        excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                                        excelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                                        excelcells.Font.Size = 14;
                                        excelcells.Value2 = ScheduleHelper.CellValueGenerator(secondWeekDay, lessonType, typeData, indexRow, false);
                                        //lessonType.Trim() + ". " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject.Trim()).FirstOrDefault() + ". " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher.Trim()).FirstOrDefault() + " ауд. " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom.Trim()).FirstOrDefault();
                                        lections.AddRange(lessonEvents);
                                    }
                                    goto NEXT2;
                                }
                            NOT_LECTION_SECTION:
                                SetCellProperties(excelWorkSheet.Cells[j - 2, i] as Microsoft.Office.Interop.Excel.Range, LongToShortLessonType[lessonType.Trim()]);
                                //(excelWorkSheet.Cells[j - 2, i] as Microsoft.Office.Interop.Excel.Range).Value2 = LongToShortLessonType[lessonType.Trim()];
                            }
                        }
                        else if (i == column + 1)
                        {
                            SetCellProperties(excelWorkSheet.Cells[j - 2, i] as Microsoft.Office.Interop.Excel.Range, secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject).FirstOrDefault());
                            //(excelWorkSheet.Cells[j - 2, i] as Microsoft.Office.Interop.Excel.Range).Value2 = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Subject).FirstOrDefault();
                            SetCellProperties(excelWorkSheet.Cells[j - 1, i] as Microsoft.Office.Interop.Excel.Range, ScheduleHelper.CellValueGenerator(secondWeekDay, String.Empty, typeData, indexRow, false));
                            //(excelWorkSheet.Cells[j - 1, i] as Microsoft.Office.Interop.Excel.Range).Value2 = ScheduleHelper.CellValueGenerator(secondWeekDay, String.Empty, typeData, indexRow, false);
                            //secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.TeacherPost == null ? "" : p.TeacherPost.Trim()).FirstOrDefault() + " " + secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Teacher).FirstOrDefault();
                        }
                        else if (i == column + 2)
                            SetCellProperties(excelWorkSheet.Cells[j - 1, i] as Microsoft.Office.Interop.Excel.Range, secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom).FirstOrDefault());
                            //(excelWorkSheet.Cells[j - 1, i] as Microsoft.Office.Interop.Excel.Range).Value2 = secondWeekDay.DayLessons.Where(p => p.LessonNumber == indexRow / 4).Select(p => p.Classroom).FirstOrDefault();
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
                        if (i == column + 1) (excelWorkSheet.Cells[row - 1, i] as Microsoft.Office.Interop.Excel.Range).Value2 = columnName;
                        if (i == column + 2) (excelWorkSheet.Cells[row - 1, i] as Microsoft.Office.Interop.Excel.Range).Borders[XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    }
                    indexRow++;
                }
            }

        }

        private void SetCellProperties(Microsoft.Office.Interop.Excel.Range excellCell, string value)
        {
            //excellCell.Columns.AutoFit();
            excellCell.Rows.AutoFit();
            excellCell.Columns.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            excellCell.Rows.VerticalAlignment = XlHAlign.xlHAlignCenter;
            excellCell.Font.Size = 12;
            excellCell.Font.Name = "Times New Roman";
            excellCell.Font.Italic = false;
            excellCell.Font.Bold = false;
            excellCell.Value2 = value;
        }

        private List<Stream> NextStreams(Stream currentStream, List<Stream> allStreams)
        {
            List<Stream> nextSteams = new List<Stream>();
            List<LessonEvent> largestLection = new List<LessonEvent>();
            var lections = repository.DataBase.LessonEvents.Where(p => p.LessonTypeID == 1 && p.StreamID == currentStream.StreamID).ToList();
            foreach (var lection in lections)
            {
                var lectionsWithOtherStreams = repository.DataBase.LessonEvents.Where(p => p.LessonTypeID == lection.LessonTypeID && p.TimeID == lection.TimeID &&
                    p.SubjectID == lection.SubjectID && p.ClassroomID == lection.ClassroomID && p.StreamID != lection.StreamID).ToList();
                if (lectionsWithOtherStreams.Count > largestLection.Count)
                {
                    largestLection = lectionsWithOtherStreams;
                }
            }

            if (largestLection.Count > 0)
            {
                foreach (var lectionWithOtherStream in largestLection)
                {
                    foreach (var candidatForNext in allStreams)
                    {
                        if (candidatForNext.StreamID == lectionWithOtherStream.StreamID)
                        {
                            if (!nextSteams.Exists(p => p.StreamID == candidatForNext.StreamID))
                                nextSteams.Add(candidatForNext);
                        }
                    }
                }
            }

            return nextSteams;
        }


    }



}