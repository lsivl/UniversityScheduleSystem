using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;
using University.WebUI.Models;
using University.Domain.Abstract;
using University.Domain.Models;
using University.WebUI.Models.ShortVersionsModels;
using System.Data.Entity;
using System.Web.Routing;

namespace University.WebUI.Controllers
{
    public class EditDataController : Controller
    {
        IDataBaseRepository repository;
        List<ExcelTableRowModel> RowList = null;
        public EditDataController(IDataBaseRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Stream(string name)
        {
            var streamDB = repository.DataBase.Streams.Where(p => p.StreamName.Trim() == name.Trim()).FirstOrDefault();
            List<int> coursesNumbers = new List<int>() { 1, 2, 3, 4, 5 };
            var faculties = repository.DataBase.Faculties.ToList();
            StreamEditModel streamModel = new StreamEditModel();
            streamModel.FacultyID = streamDB.FacultyID;
            streamModel.StreamID = streamDB.StreamID;
            streamModel.StreamName = streamDB.StreamName;
            streamModel.StudentsCount = streamDB.StudentsCount;
            streamModel.YearOfStudy = streamDB.YearOfStudy;
            streamModel.CoursesNumbers = new SelectList(coursesNumbers, "YearOfStudy");
            streamModel.Faculties = new SelectList(faculties, "FacultyID", "FullName");
            if (Session["MessageSaveStream"] != null)
                streamModel.Message = Session["MessageSaveStream"].ToString();
            Session["MessageSaveStream"] = null;

            return View(streamModel);
        }

        public ActionResult SaveStream(StreamEditModel streamModel)
        {
            bool isChange = false;
            var streamDB = repository.DataBase.Streams.Where(p => p.StreamID == streamModel.StreamID).FirstOrDefault();


            if (streamDB.StreamName != streamModel.StreamName)
            {
                streamDB.StreamName = streamModel.StreamName;
                isChange = true;
            }
            if (streamDB.StudentsCount != streamModel.StudentsCount)
            {
                streamDB.StudentsCount = streamModel.StudentsCount;
                isChange = true;
            }
            if (streamDB.YearOfStudy != streamModel.YearOfStudy)
            {
                streamDB.YearOfStudy = streamModel.YearOfStudy;
                isChange = true;
            }
            if (streamDB.FacultyID != streamModel.FacultyID)
            {
                streamDB.FacultyID = streamModel.FacultyID;
                isChange = true;
            }

            if (isChange)
            {
                repository.DataBase.Streams.Attach(streamDB);
                repository.DataBase.Entry(streamDB).State = EntityState.Modified;

                Session["MessageSaveStream"] = "Зміни успішно збережені!";

                repository.DataBase.SaveChanges();
            }
            else
                Session["MessageSaveStream"] = "Ви не внесли зміни!";

            return RedirectToAction("Stream", new RouteValueDictionary(new { controller = "EditData", action = "Stream", name = streamDB.StreamName }));

        }

        [HttpPost]
        public ActionResult DeleteStream(int StreamID)
        {
            var streamDB = repository.DataBase.Streams.Where(p => p.StreamID == StreamID).FirstOrDefault();
            repository.DataBase.Streams.Remove(streamDB);
            repository.DataBase.SaveChanges();
            return PartialView(streamDB.StreamName);
        }

        public ActionResult EditDataMenu()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileInput)
        {
            UploadModel uploadModel = new UploadModel();
            try
            {
                if (fileInput.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileInput.FileName);
                    var path = Path.Combine(Server.MapPath("~/FilesForParse/"), fileName); fileInput.SaveAs(path);
                    string ext = Path.GetExtension(path);

                    if (ext == ".xls")
                    {
                        uploadModel = ImportFromExcel(path);
                        this.RowList = uploadModel.RowList;
                        Session["FileContent"] = RowList;

                    }
                    else
                    {
                        this.RowList = new List<ExcelTableRowModel>();
                        ViewBag.Message = "Некоректний тип файлу. Для завантаження даних файл має бути з розширенням '.xls'.";
                    }

                }
            }
            catch
            {
                this.RowList = new List<ExcelTableRowModel>();
                ViewBag.Message = "Файл відсутній або має некоректне розширення.";
            }

            uploadModel.RowList = null;
            return PartialView(uploadModel);
        }

        [HttpPost]
        public ActionResult ShowContent(int course)
        {
            RowList = Session["FileContent"] as List<ExcelTableRowModel>;
            ShowContentModel contentModel = new ShowContentModel()
            {
                SelectedCourse = course,
                TableRows = RowList
            };
            return PartialView(contentModel);
        }

        [HttpPost]
        public ActionResult SaveToDb(int cathedraId, bool isDelete)
        {
            RowList = Session["FileContent"] as List<ExcelTableRowModel>;
            try
            {
                if (isDelete)
                {
                    repository.DataBase.Database.ExecuteSqlCommand("exec [dbo].[csp_DeleteCatedraData] {0}", cathedraId);
                }

                if (RowList[0].Subject != null)
                {
                    int lessonLectionTypeID = repository.DataBase.LessonTypes.Where(p => p.LessonTypeName.Trim() == "Лекція").Select(p => p.LessonTypeID).FirstOrDefault();
                    int lessonPraсticeTypeID = repository.DataBase.LessonTypes.Where(p => p.LessonTypeName.Trim() == "Практика").Select(p => p.LessonTypeID).FirstOrDefault();
                    int lessonLaboratoryTypeID = repository.DataBase.LessonTypes.Where(p => p.LessonTypeName.Trim() == "Лабораторна").Select(p => p.LessonTypeID).FirstOrDefault();
                    foreach (var row in RowList)
                    {
                        //int currentFacultyID = repository.DataBase.Faculties.Where(p => p.Name.Trim() == row.Faculty.Trim()).Select(p => p.FacultyID).FirstOrDefault();
                        Subject currentSubject = repository.DataBase.Subjects.Where(p => p.Name.Trim() == row.Subject.Trim()).FirstOrDefault();
                        if (currentSubject == null)
                        {
                            currentSubject = new Subject()
                            {
                                Name = row.Subject,
                                CathedraID = cathedraId
                            };
                            repository.DataBase.Subjects.Add(currentSubject);
                        }

                        List<Teacher> teachers = new List<Teacher>();

                        Teacher currentLectionTeacher = null;
                        if (row.LectionHour != 0)
                        {
                            string teacherPost = row.LectionTeacher.Split('(', ')')[1];
                            string teacherName = TextChange(row.LectionTeacher);
                            currentLectionTeacher = repository.DataBase.Teachers.Where(p => p.Name.Trim() == teacherName.Trim()).FirstOrDefault();
                            if (currentLectionTeacher == null)
                            {
                                currentLectionTeacher = new Teacher()
                                {
                                    Name = teacherName,
                                    Post = teacherPost,
                                    CathedraID = cathedraId
                                };
                                repository.DataBase.Teachers.Add(currentLectionTeacher);
                            }
                            teachers.Add(currentLectionTeacher);
                        }


                        var teachersPracticalGroups = new SortedList<string, Teacher>();
                        if (row.PracticalHour != 0)
                        {
                            List<string> practicalTeachers = row.PracticalTeacher.Split(',').ToList();
                            foreach (var teacher in practicalTeachers)
                            {
                                string teacherGroup = teacher.Split('{', '}')[1];
                                string teacherPost = teacher.Split('(', ')')[1];
                                string teacherName = TextChange(teacher);
                                Teacher currentPracticalTeacher = repository.DataBase.Teachers.Where(p => p.Name.Trim() == teacherName.Trim()).FirstOrDefault();
                                if ((currentLectionTeacher != null) && (teacherName.Trim() == currentLectionTeacher.Name.Trim())) { currentPracticalTeacher = currentLectionTeacher; }
                                if (currentPracticalTeacher == null)
                                {
                                    currentPracticalTeacher = new Teacher()
                                    {
                                        Name = teacherName,
                                        Post = teacherPost,
                                        CathedraID = cathedraId
                                    };
                                    repository.DataBase.Teachers.Add(currentPracticalTeacher);
                                }
                                var groupsForTeacher = teacherGroup.Split(';').ToList();
                                foreach (var item in groupsForTeacher) { teachersPracticalGroups[item.Trim()] = currentPracticalTeacher; }
                                teachers.Add(currentLectionTeacher);
                            }

                        }


                        var teachersLaboratoryGroups = new SortedList<string, Teacher>();
                        if (row.LabHour != 0)
                        {
                            List<string> laboratoryTeachers = row.LabTeacher.Split(',').ToList();
                            foreach (var teacher in laboratoryTeachers)
                            {
                                string teacherGroup = teacher.Split('{', '}')[1];
                                string teacherPost = teacher.Split('(', ')')[1];
                                string teacherName = TextChange(teacher);
                                Teacher currentLaboratoryTeacher = repository.DataBase.Teachers.Where(p => p.Name.Trim() == teacherName.Trim()).FirstOrDefault();
                                foreach (var item in teachers)
                                {
                                    if (item.Name.Trim() == teacherName.Trim()) { currentLaboratoryTeacher = item; break; }
                                }
                                if (currentLaboratoryTeacher == null)
                                {
                                    currentLaboratoryTeacher = new Teacher()
                                    {
                                        Name = teacherName,
                                        Post = teacherPost,
                                        CathedraID = cathedraId
                                    };
                                    repository.DataBase.Teachers.Add(currentLaboratoryTeacher);
                                }
                                var groupsForTeacher = teacherGroup.Split(';').ToList();
                                foreach (var item in groupsForTeacher) { teachersLaboratoryGroups[item.Trim()] = currentLaboratoryTeacher; }
                                teachers.Add(currentLaboratoryTeacher);
                            }

                        }

                        List<string> groupsNames = row.GroupsOfStream.Split(',').ToList();
                        List<Group> currentGroups = new List<Group>();

                        foreach (var group in groupsNames)
                        {
                            currentGroups.Add(repository.DataBase.Groups.Where(p => p.GroupName.Trim() == group.Trim()).First());
                        }


                        //if (row.UnitedGroups != null)
                        //{
                        //    var allUnitedGroups = row.UnitedGroups.Split(';').ToList();
                        //    foreach (var item in allUnitedGroups)
                        //    {
                        //        bool isCorrect = false;
                        //        var unitedGroups = item.Split(',').ToList();
                        //        int exist = 0;
                        //        foreach (var unitedGroup in unitedGroups)
                        //        {
                        //            foreach (var existgroup in currentGroups)
                        //            {
                        //                if (existgroup.GroupName.Trim() == unitedGroup.Trim())
                        //                    exist++;
                        //            }
                        //        }
                        //        if (exist == unitedGroups.Count)
                        //            isCorrect = true;
                        //        if (isCorrect == true)
                        //        {
                        //            string tempGroup;
                        //            UnitedGroup currentUnitedGroup = new UnitedGroup();
                        //            tempGroup = unitedGroups[0].Trim();
                        //            currentUnitedGroup.FirstGroupID = repository.DataBase.Groups.Where(p => p.GroupName.Trim() == tempGroup).Select(p => p.GroupID).FirstOrDefault();
                        //            tempGroup = unitedGroups[1].Trim();
                        //            currentUnitedGroup.SecondGroupID = repository.DataBase.Groups.Where(p => p.GroupName.Trim() == tempGroup).Select(p => p.GroupID).FirstOrDefault();
                        //            if (unitedGroups.Count > 2) currentUnitedGroup.ThirdGroupID = repository.DataBase.Groups.Where(p => p.GroupName.Trim() == unitedGroups[2].Trim()).Select(p => p.GroupID).FirstOrDefault();
                        //            if (unitedGroups.Count > 3) currentUnitedGroup.FourthGroupID = repository.DataBase.Groups.Where(p => p.GroupName.Trim() == unitedGroups[3].Trim()).Select(p => p.GroupID).FirstOrDefault();
                        //            if (unitedGroups.Count > 4) currentUnitedGroup.FifthGroupID = repository.DataBase.Groups.Where(p => p.GroupName.Trim() == unitedGroups[4].Trim()).Select(p => p.GroupID).FirstOrDefault();
                        //            repository.DataBase.UnitedGroups.Add(currentUnitedGroup);
                        //        }
                        //    }
                        //}


                        foreach (var group in currentGroups)
                        {
                            if (row.LectionHour != 0)
                            {
                                if (row.LectionClassroom != null)
                                {
                                    List<string> classrooms = row.LectionClassroom.Split(',').ToList();
                                    foreach (var classroom in classrooms)
                                    {
                                        int lectionClassroomId = repository.DataBase.Classrooms.Where(p => p.Name.Trim() == classroom.Trim()).Select(p => p.ClassroomID).FirstOrDefault();
                                        if (lectionClassroomId != 0)
                                        {
                                            StreamSubjectBridge currentLectionStreamSubjectBridge = new StreamSubjectBridge()
                                            {
                                                StreamID = group.StreamID,
                                                Subject = currentSubject,
                                                GroupID = group.GroupID,
                                                LessonTypeID = lessonLectionTypeID,
                                                Teacher = currentLectionTeacher,
                                                CountHours = row.LectionHour,
                                                ClassroomID = lectionClassroomId,
                                                CathedraID = cathedraId
                                            };
                                            repository.DataBase.StreamSubjectBridges.Add(currentLectionStreamSubjectBridge);
                                        }
                                        else
                                        {
                                            StreamSubjectBridge currentLectionStreamSubjectBridge = new StreamSubjectBridge()
                                            {
                                                StreamID = group.StreamID,
                                                Subject = currentSubject,
                                                GroupID = group.GroupID,
                                                LessonTypeID = lessonLectionTypeID,
                                                Teacher = currentLectionTeacher,
                                                CountHours = row.LectionHour,
                                                CathedraID = cathedraId
                                            };
                                            repository.DataBase.StreamSubjectBridges.Add(currentLectionStreamSubjectBridge);
                                        }
                                    }
                                }
                                else
                                {
                                    StreamSubjectBridge currentLectionStreamSubjectBridge = new StreamSubjectBridge()
                                    {
                                        StreamID = group.StreamID,
                                        Subject = currentSubject,
                                        GroupID = group.GroupID,
                                        LessonTypeID = lessonLectionTypeID,
                                        Teacher = currentLectionTeacher,
                                        CountHours = row.LectionHour,
                                        CathedraID = cathedraId
                                    };
                                    repository.DataBase.StreamSubjectBridges.Add(currentLectionStreamSubjectBridge);
                                }
                            }
                            if (row.PracticalHour != 0)
                            {
                                if (row.PracticalClassroom != null)
                                {
                                    List<string> classrooms = row.PracticalClassroom.Split(',').ToList();
                                    foreach (var classroom in classrooms)
                                    {
                                        int praticalClassroomId = repository.DataBase.Classrooms.Where(p => p.Name.Trim() == classroom.Trim()).Select(p => p.ClassroomID).FirstOrDefault();
                                        if (praticalClassroomId != 0)
                                        {
                                            StreamSubjectBridge currentPracticalStreamSubjectBridge = new StreamSubjectBridge()
                                            {
                                                StreamID = group.StreamID,
                                                Subject = currentSubject,
                                                GroupID = group.GroupID,
                                                LessonTypeID = lessonPraсticeTypeID,
                                                Teacher = teachersPracticalGroups[group.GroupName.Trim()],
                                                CountHours = row.PracticalHour,
                                                Сoupled = row.Coupled,
                                                ClassroomID = praticalClassroomId,
                                                CathedraID = cathedraId
                                            };
                                            repository.DataBase.StreamSubjectBridges.Add(currentPracticalStreamSubjectBridge);
                                        }
                                        else
                                        {
                                            StreamSubjectBridge currentPracticalStreamSubjectBridge = new StreamSubjectBridge()
                                            {
                                                StreamID = group.StreamID,
                                                Subject = currentSubject,
                                                GroupID = group.GroupID,
                                                LessonTypeID = lessonPraсticeTypeID,
                                                Teacher = teachersPracticalGroups[group.GroupName.Trim()],
                                                CountHours = row.PracticalHour,
                                                Сoupled = row.Coupled,
                                                CathedraID = cathedraId
                                            };
                                            repository.DataBase.StreamSubjectBridges.Add(currentPracticalStreamSubjectBridge);
                                        }
                                    }
                                }
                                else
                                {
                                    StreamSubjectBridge currentPracticalStreamSubjectBridge = new StreamSubjectBridge()
                                    {
                                        StreamID = group.StreamID,
                                        Subject = currentSubject,
                                        GroupID = group.GroupID,
                                        LessonTypeID = lessonPraсticeTypeID,
                                        Teacher = teachersPracticalGroups[group.GroupName.Trim()],
                                        CountHours = row.PracticalHour,
                                        Сoupled = row.Coupled,
                                        CathedraID = cathedraId
                                    };
                                    repository.DataBase.StreamSubjectBridges.Add(currentPracticalStreamSubjectBridge);
                                }
                            }
                            if (row.LabHour != 0)
                            {
                                if (row.LabClassroom != null)
                                {
                                    List<string> classrooms = row.LabClassroom.Split(',').ToList();
                                    foreach (var classroom in classrooms)
                                    {
                                        int laboratoryClassroomId = repository.DataBase.Classrooms.Where(p => p.Name.Trim() == classroom.Trim()).Select(p => p.ClassroomID).FirstOrDefault();
                                        if (laboratoryClassroomId != 0)
                                        {
                                            StreamSubjectBridge currentLaboratoryStreamSubjectBridge = new StreamSubjectBridge()
                                            {
                                                StreamID = group.StreamID,
                                                Subject = currentSubject,
                                                GroupID = group.GroupID,
                                                LessonTypeID = lessonLaboratoryTypeID,
                                                Teacher = teachersLaboratoryGroups[group.GroupName.Trim()],
                                                CountHours = row.LabHour,
                                                Сoupled = row.Coupled,
                                                ClassroomID = laboratoryClassroomId,
                                                CathedraID = cathedraId
                                            };
                                            repository.DataBase.StreamSubjectBridges.Add(currentLaboratoryStreamSubjectBridge);
                                        }
                                        else
                                        {
                                            StreamSubjectBridge currentLaboratoryStreamSubjectBridge = new StreamSubjectBridge()
                                            {
                                                StreamID = group.StreamID,
                                                Subject = currentSubject,
                                                GroupID = group.GroupID,
                                                LessonTypeID = lessonLaboratoryTypeID,
                                                Teacher = teachersLaboratoryGroups[group.GroupName.Trim()],
                                                CountHours = row.LabHour,
                                                Сoupled = row.Coupled,
                                                CathedraID = cathedraId
                                            };
                                            repository.DataBase.StreamSubjectBridges.Add(currentLaboratoryStreamSubjectBridge);
                                        }
                                    }
                                }
                                else
                                {
                                    StreamSubjectBridge currentLaboratoryStreamSubjectBridge = new StreamSubjectBridge()
                                    {
                                        StreamID = group.StreamID,
                                        Subject = currentSubject,
                                        GroupID = group.GroupID,
                                        LessonTypeID = lessonLaboratoryTypeID,
                                        Teacher = teachersLaboratoryGroups[group.GroupName.Trim()],
                                        CountHours = row.LabHour,
                                        Сoupled = row.Coupled,
                                        CathedraID = cathedraId
                                    };
                                    repository.DataBase.StreamSubjectBridges.Add(currentLaboratoryStreamSubjectBridge);
                                }
                            }
                        }

                        repository.DataBase.SaveChanges();

                        //SubjectClassroomBridge currentLectionSubjectClassroomBridge = new SubjectClassroomBridge()
                        //{
                        //    Subject = currentSubject,
                        //    ClassroomID = repository.DataBase.Classrooms.Where(p => p.Name.Trim() == row.LectionClassroom.Trim()).Select(p => p.ClassroomID).FirstOrDefault(),
                        //    LessonTypeID = lessonLectionTypeID
                        //};
                        //repository.DataBase.SubjectClassroomBridges.Add(currentLectionSubjectClassroomBridge);


                        //Teacher currentLectionTeacher = new Teacher()
                        //{
                        //    Name = TextChange(row.LectionTeacher),
                        //    CathedraID = cathedraId
                        //};
                        //repository.DataBase.Teachers.Add(currentLectionTeacher);


                        //TeacherSubjectBridge currentLectionTeacherSubjectBridge = new TeacherSubjectBridge()
                        //{
                        //    Teacher = currentLectionTeacher,
                        //    Subject = currentSubject,
                        //    LessonTypeID = lessonLectionTypeID
                        //};
                        //repository.DataBase.TeacherSubjectBridges.Add(currentLectionTeacherSubjectBridge);


                        //SubjectClassroomBridge currentPraсticeSubjectClassroomBridge = new SubjectClassroomBridge()
                        //{
                        //    Subject = currentSubject,
                        //    ClassroomID = repository.DataBase.Classrooms.Where(p => p.Name.Trim() == row.PracticalClassroom.Trim()).Select(p => p.ClassroomID).FirstOrDefault(),
                        //    LessonTypeID = lessonPraсticeTypeID
                        //};
                        //repository.DataBase.SubjectClassroomBridges.Add(currentPraсticeSubjectClassroomBridge);


                        //Teacher currentPraсticeTeacher = new Teacher()
                        //{
                        //    Name = TextChange(row.PracticalTeacher),
                        //    CathedraID = cathedraId
                        //};
                        //repository.DataBase.Teachers.Add(currentPraсticeTeacher);


                        //TeacherSubjectBridge currentPraсticeTeacherSubjectBridge = new TeacherSubjectBridge()
                        //{
                        //    Teacher = currentPraсticeTeacher,
                        //    Subject = currentSubject,
                        //    LessonTypeID = lessonPraсticeTypeID
                        //};
                        //repository.DataBase.TeacherSubjectBridges.Add(currentPraсticeTeacherSubjectBridge);

                        //if (row.LabHour != 0)
                        //{

                        //    SubjectClassroomBridge currentLaboratorySubjectClassroomBridge = new SubjectClassroomBridge()
                        //    {
                        //        Subject = currentSubject,
                        //        ClassroomID = repository.DataBase.Classrooms.Where(p => p.Name.Trim() == row.LabClassroom.Trim()).Select(p => p.ClassroomID).FirstOrDefault(),
                        //        LessonTypeID = lessonLaboratoryTypeID
                        //    };
                        //    repository.DataBase.SubjectClassroomBridges.Add(currentLaboratorySubjectClassroomBridge);


                        //    Teacher currentLaboratoryTeacher = new Teacher()
                        //    {
                        //        Name = TextChange(row.LabTeacher),
                        //        CathedraID = cathedraId
                        //    };
                        //    repository.DataBase.Teachers.Add(currentLaboratoryTeacher);


                        //    TeacherSubjectBridge currentLaboratoryTeacherSubjectBridge = new TeacherSubjectBridge()
                        //    {
                        //        Teacher = currentLaboratoryTeacher,
                        //        Subject = currentSubject,
                        //        LessonTypeID = lessonLaboratoryTypeID
                        //    };
                        //    repository.DataBase.TeacherSubjectBridges.Add(currentLaboratoryTeacherSubjectBridge);
                        //}

                    }
                }
                else if ((RowList[0].Subject == null) && (RowList[0].Stream != null))
                {
                    foreach (var row in RowList)
                    {
                        int currentFacultyID = repository.DataBase.Faculties.Where(p => p.Name.Trim() == row.Faculty.Trim()).Select(p => p.FacultyID).FirstOrDefault();

                        University.Domain.Models.Stream currentStream = new University.Domain.Models.Stream()
                        {
                            StreamName = row.Stream.Trim(),
                            YearOfStudy = row.Course,
                            FacultyID = currentFacultyID
                        };
                        repository.DataBase.Streams.Add(currentStream);

                        var groupsNames = row.GroupsOfStream.Split(',').ToList();
                        foreach (var group in groupsNames)
                        {
                            Group currentGroup = new Group()
                            {
                                GroupName = group.Trim(),
                                Stream = currentStream
                            };
                            repository.DataBase.Groups.Add(currentGroup);
                        }

                        repository.DataBase.SaveChanges();
                    }
                }
                else if (RowList[0].ClassroomForDownload != null)
                {
                    int currentCathedraId;
                    var classroomTypesID = new SortedList<string, int>();
                    classroomTypesID["Лек."] = repository.DataBase.ClassroomTypes.Where(p => p.ShortName.Trim() == "Лек.").Select(p => p.ClassroomTypeID).FirstOrDefault();
                    classroomTypesID["Пр."] = repository.DataBase.ClassroomTypes.Where(p => p.ShortName.Trim() == "Пр.").Select(p => p.ClassroomTypeID).FirstOrDefault();
                    classroomTypesID["Лаб."] = repository.DataBase.ClassroomTypes.Where(p => p.ShortName.Trim() == "Лаб.").Select(p => p.ClassroomTypeID).FirstOrDefault();
                    classroomTypesID["Кк."] = repository.DataBase.ClassroomTypes.Where(p => p.ShortName.Trim() == "Кк.").Select(p => p.ClassroomTypeID).FirstOrDefault();
                    foreach (var row in RowList)
                    {
                        List<string> types = row.ClassroomType.Split(',').ToList();
                        try { currentCathedraId = repository.DataBase.Cathedras.Where(p => p.Name.Trim() == row.Cathedra.Trim()).Select(p => p.CathedraID).First(); }
                        catch { currentCathedraId = 9999999; }
                        for (int i = 0; i < types.Count; i++)
                        {
                            if (currentCathedraId != 9999999)
                            {
                                Classroom currentClassroom = new Classroom()
                                {
                                    Name = row.ClassroomForDownload.Trim(),
                                    Capacity = row.ClassroomCapacity,
                                    ClassroomTypeID = classroomTypesID[types[i].Trim()],
                                    CathedraID = currentCathedraId
                                };
                                repository.DataBase.Classrooms.Add(currentClassroom);
                            }
                            else
                            {
                                Classroom currentClassroom = new Classroom()
                                {
                                    Name = row.ClassroomForDownload.Trim(),
                                    Capacity = row.ClassroomCapacity,
                                    ClassroomTypeID = classroomTypesID[types[i]],
                                };
                                repository.DataBase.Classrooms.Add(currentClassroom);
                            }
                        }
                        repository.DataBase.SaveChanges();
                    }
                }
            }
            catch
            {
                TempData["message"] = "Помилка завантаження даних";
                return RedirectToAction("EditDataMenu");
            }
            return PartialView();
        }

        private UploadModel ImportFromExcel(string file)
        {
            UploadModel uploadModel = new UploadModel();
            List<ExcelTableRowModel> TempRowList = new List<ExcelTableRowModel>();
            string str;
            int numList;
            int numListCount;
            int rCnt;
            int cCnt;
            bool brk = false;

            //OpenFileDialog opf = new OpenFileDialog();
            //opf.Filter = "Excel (*.XLS)|*.XLS";
            //opf.ShowDialog();
            //System.Data.DataTable tb = new System.Data.DataTable();
            string filename = file;

            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook ExcelWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet ExcelWorkSheet;
            Microsoft.Office.Interop.Excel.Range ExcelRange;

            ExcelWorkBook = ExcelApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false,
                false, 0, true, 1, 0);


            numListCount = ExcelWorkBook.Worksheets.Count;
            if (numListCount < 1)
            {
                ViewBag.Message = "Документ порожній.";
                goto ErrorLabel;
            }
            else if (numListCount > 5)
            {
                ViewBag.Message = "Максимальна дозвлена кількість сторінок у документі - 5(кожна сторінка відображає інформацію по навантаженню певного курсу від 1 до 5).";
                goto ErrorLabel;
            }
            for (numList = 1; numList <= numListCount; numList++)
            {
                ExcelWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExcelWorkBook.Worksheets.get_Item(numList);
                ExcelRange = ExcelWorkSheet.UsedRange;

                if (Convert.ToString((ExcelRange.Cells[5, 1] as Microsoft.Office.Interop.Excel.Range).Value2) != "Назви навчальних дисциплін і видів навчальної роботи")
                {
                    if (numList == 1)
                        ViewBag.Message = "Структура документа не відповідає вимогам програми.";
                    else if (numList > 1)
                    {
                        ViewBag.Message = String.Format("На листі {0}, cтруктура документа не відповідає вимогам програми.", ExcelWorkSheet.Name);
                        TempRowList = new List<ExcelTableRowModel>();
                    }
                    goto ErrorLabel;
                }

                if (numList == 1)
                {
                    string cathedraName = Convert.ToString((ExcelRange.Cells[1, 1] as Microsoft.Office.Interop.Excel.Range).Value2);
                    var cathedra = repository.DataBase.Cathedras.Where(p => p.Name.Trim() == cathedraName.Trim()).FirstOrDefault();


                    if (cathedra == null)
                    {
                        ViewBag.Message = "Назва кафедри в документі відсутня або некоректна(Перевірте коректність назви кафедри у комріці A,1)";
                        goto ErrorLabel;
                    }
                    uploadModel.CathedraName = cathedra.FullName;
                    uploadModel.CathedraID = cathedra.CathedraID;
                }



                for (rCnt = 8; rCnt <= ExcelRange.Rows.Count; rCnt++)
                {

                    ExcelTableRowModel row = new ExcelTableRowModel();
                    for (cCnt = 1; cCnt <= 21; cCnt++)
                    {
                        try
                        {

                            str = Convert.ToString((ExcelRange.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value2);

                            if ((cCnt == 1) && (str == null))
                            {
                                brk = true;
                                break;
                            }
                            else
                            {
                                if (cCnt == 1) { row.Subject = str; }
                                else if (cCnt == 3)
                                {
                                    row.Course = Convert.ToInt32(str);
                                    if (row.Course < 1 || row.Course > 5)
                                    {
                                        ViewBag.Message = String.Format("Помилка зчитування даних на {0} листі, {1} рядок, {2} стовпець. Значення номера курсу має бути від 1 до 5.", numList, rCnt, cCnt);
                                        TempRowList = new List<ExcelTableRowModel>();
                                        goto ErrorLabel;
                                    }
                                }
                                else if (cCnt == 4) { row.Coupled = Convert.ToInt32(str); }
                                else if (cCnt == 5) { row.Faculty = str; }
                                else if (cCnt == 6) { row.Stream = str; }
                                else if (cCnt == 7) { row.GroupsOfStream = str; }//{ try { row.GroupsOfStream = str.Split(',').ToList(); } catch { row.GroupsOfStream = null; } }
                                else if (cCnt == 8) { row.UnitedGroups = str; }//{ try { row.UnitedGroups = str.Split(',').ToList(); } catch { row.GroupsOfStream = null; } }
                                else if (cCnt == 13) { row.LectionHour = Convert.ToInt32(str); }
                                else if (cCnt == 14) { row.LectionTeacher = str; }
                                else if (cCnt == 15) { row.LectionClassroom = str; }
                                else if (cCnt == 16) { row.PracticalHour = Convert.ToInt32(str); }
                                else if (cCnt == 17) { row.PracticalTeacher = str; }
                                else if (cCnt == 18) { row.PracticalClassroom = str; }
                                else if (cCnt == 19) { row.LabHour = Convert.ToInt32(str); }
                                else if (cCnt == 20) { row.LabTeacher = str; }
                                else if (cCnt == 21) { row.LabClassroom = str; }
                                //dataGridView1.Rows[rCnt - 14].Cells[cCnt].Value = str;
                            }
                        }
                        catch
                        {
                            ViewBag.Message = String.Format("Помилка зчитування даних на {0} листі, {1} рядок, {2} стовпець. Значення має бути числом.", numList, rCnt, cCnt);
                            TempRowList = new List<ExcelTableRowModel>();
                            goto ErrorLabel;
                        }
                    }
                    if (brk.Equals(false)) TempRowList.Add(row);
                    else if (brk.Equals(true))
                    {
                        brk = false;
                        break;
                    }
                }

                //else if (Convert.ToString((ExcelRange.Cells[1, 1] as Microsoft.Office.Interop.Excel.Range).Value2) == "Назва Потоку")
                //{
                //    for (rCnt = 2; rCnt <= ExcelRange.Rows.Count; rCnt++)
                //    {

                //        ExcelTableRowModel row = new ExcelTableRowModel();
                //        for (cCnt = 1; cCnt <= 5; cCnt++)
                //        {

                //            str = Convert.ToString((ExcelRange.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value2);

                //            if ((cCnt == 1) && (str == null))
                //            {
                //                brk = true;
                //                break;
                //            }
                //            else
                //            {
                //                if (cCnt == 1) { row.Stream = str; }
                //                else if (cCnt == 2) { row.GroupsOfStream = str; }//{ try { row.GroupsOfStream = str.Split(',').ToList(); } catch { row.GroupsOfStream = null; } }
                //                else if (cCnt == 3) { row.StudentsCount = Convert.ToInt32(str); }
                //                else if (cCnt == 4) { row.Course = Convert.ToInt32(str); }
                //                else if (cCnt == 5) { row.Faculty = str; }
                //                //dataGridView1.Rows[rCnt - 14].Cells[cCnt].Value = str;
                //            }
                //        }
                //        if (brk.Equals(false)) TempRowList.Add(row);
                //        else if (brk.Equals(true))
                //        {
                //            brk = false;
                //            break;
                //        }
                //    }
                //else if (Convert.ToString((ExcelRange.Cells[1, 2] as Microsoft.Office.Interop.Excel.Range).Value2) == "№        аудиторії")
                //{
                //    for (rCnt = 4; rCnt <= ExcelRange.Rows.Count; rCnt++)
                //    {

                //        ExcelTableRowModel row = new ExcelTableRowModel();
                //        for (cCnt = 2; cCnt <= 5; cCnt++)
                //        {

                //            str = Convert.ToString((ExcelRange.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value2);

                //            if ((cCnt == 2) && (str == null))
                //            {
                //                brk = true;
                //                break;
                //            }
                //            else
                //            {
                //                if (cCnt == 2) { row.ClassroomForDownload = str; }
                //                else if (cCnt == 3) { row.ClassroomCapacity = Convert.ToInt32(str); }
                //                else if (cCnt == 4) { row.ClassroomType = str; }
                //                else if (cCnt == 5) { row.Cathedra = str; }
                //                //dataGridView1.Rows[rCnt - 14].Cells[cCnt].Value = str;
                //            }
                //        }
                //        if (brk.Equals(false)) TempRowList.Add(row);
                //        else if (brk.Equals(true))
                //        {
                //            brk = false;
                //            break;
                //        }
                //    }
                //}
                releaseObject(ExcelWorkSheet);
            }


        ErrorLabel:


            uploadModel.RowList = TempRowList;
            if (uploadModel.RowList.Count > 0)
                uploadModel.Courses = TempRowList.OrderBy(p => p.Course).Select(p => p.Course).Distinct().ToList();

            ExcelWorkBook.Close(true, null, null);
            ExcelApp.Quit();


            releaseObject(ExcelWorkBook);
            releaseObject(ExcelApp);

            return uploadModel;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                TempData["message"] = "Unable to release the object " + ex.ToString();
            }
            finally
            {
                GC.Collect();
            }
        }

        private string TextChange(string text)
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
