using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using University.Domain.Abstract;
using University.Domain.Models;
using University.WebUI.Enums;
using University.WebUI.Helpers;
using University.WebUI.Models;
using University.WebUI.Models.ShortVersionsModels;

namespace University.WebUI.Controllers
{
    public class ScheduleController : Controller
    {
        IDataBaseRepository repository;
        Faculty currentFaculty;
        string typeData;
        int TypeDataNumber;
        string name;

        List<LessonEvent> sessionLessonEvents;
        public ScheduleController(IDataBaseRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public ActionResult Schedule(string name, string typeData)
        {

            string teacherName = null;
            int GenTeacherId = 0;
            string cathedraName = null;
            string streamName = null;
            int streamId = 0;
            int classroomId = 0;
            int cathedraId = 0;
            string classroomName = null;
            string facultyName = null;
            int facultyId = 0;
            name = ScheduleHelper.TextChange(name);
            this.typeData = typeData;


            if (typeData.Equals(SearchType.faculty.ToString()))
            {
                currentFaculty = repository.DataBase.Faculties.Where(p => p.Name.Trim() == name.Trim()).FirstOrDefault();
                facultyName = currentFaculty.FullName;
                facultyId = currentFaculty.FacultyID;
                TypeDataNumber = 1;

            }
            else if (typeData.Equals(SearchType.stream.ToString()))
            {
                var currentStream = repository.DataBase.Streams.Where(p => p.StreamName.Trim() == name.Trim()).FirstOrDefault();
                //var currentCathedra = repository.DataBase.Cathedras.Where(p=>p.CathedraID==currentStream.CathedraID).FirstOrDefault();
                currentFaculty = repository.DataBase.Faculties.Where(p => p.FacultyID == currentStream.FacultyID).FirstOrDefault();
                //cathedraName = currentCathedra.FullName;
                streamName = currentStream.StreamName;
                streamId = currentStream.StreamID;
                TypeDataNumber = 1;
                facultyName = currentFaculty.FullName;
            }
            else if (typeData.Equals(SearchType.teacher.ToString()))
            {
                teacherName = name;
                var teacher = repository.DataBase.Teachers.Where(p => p.Name.Trim() == name.Trim()).FirstOrDefault();
                var cathedra = repository.DataBase.Cathedras.Where(p => p.CathedraID == teacher.CathedraID).FirstOrDefault();
                currentFaculty = repository.DataBase.Faculties.Where(p => p.FacultyID == cathedra.FacultyID).FirstOrDefault();
                TypeDataNumber = 2;
                GenTeacherId = teacher.TeacherID;
                facultyName = currentFaculty.FullName;
            }
            else if (typeData.Equals(SearchType.classroom.ToString()))
            {
                classroomName = name.Trim();
                var classroom = repository.DataBase.Classrooms.Where(p => p.Name.Trim() == name.Trim()).FirstOrDefault();
                var cathedra = repository.DataBase.Cathedras.Where(p => p.CathedraID == classroom.CathedraID).FirstOrDefault();
                try
                {
                    currentFaculty = repository.DataBase.Faculties.Where(p => p.FacultyID == cathedra.FacultyID).FirstOrDefault();
                    facultyName = currentFaculty.FullName;
                }
                catch
                {
                    facultyName = "Не підпорядковується факультетам";
                }
                classroomId = classroom.ClassroomID;
                TypeDataNumber = 3;
            }
            else if (typeData.Equals(SearchType.cathedra.ToString()))
            {
                var cathedra = repository.DataBase.Cathedras.Where(p => p.FullName.Trim() == name.Trim()).FirstOrDefault();
                facultyName = repository.DataBase.Faculties.Where(p => p.FacultyID == cathedra.FacultyID).Select(p => p.FullName).FirstOrDefault();
                cathedraName = cathedra.FullName;
                cathedraId = cathedra.CathedraID;
            }
            ScheduleModel scheduleMod = new ScheduleModel()
            {
                FacultyName = facultyName,
                FacultyId = facultyId,
                CathdraName = cathedraName,
                CathedraID = cathedraId,
                StreamName = streamName,
                StreamId = streamId,
                TypeData = typeData,
                TeacherName = name,
                TypeDataNumber = TypeDataNumber,
                TeacherId = GenTeacherId,
                ClassroomName = classroomName,
                ClassroomId = classroomId
            };

            return View(scheduleMod);
        }


        [HttpPost]
        public ActionResult FacultyStreams(string name, string courseValue)
        {
            int yearOfStudy = Convert.ToInt32(courseValue.Remove(1));
            int currentFacultyId = repository.DataBase.Faculties.Where(p => p.FullName.Trim() == name.Trim()).Select(p => p.FacultyID).FirstOrDefault();
            //List<int> IdCathedras = repository.DataBase.Cathedras.Where(p => p.FacultyID == currentFacultyId).Select(p => p.CathedraID).ToList();
            //List<Stream> streams = repository.DataBase.Streams.Where(p => IdCathedras.Contains(p.CathedraID)).Where(p => p.YearOfStudy == yearOfStudy).ToList();
            List<Stream> streams = repository.DataBase.Streams.Where(p => p.FacultyID == currentFacultyId).Where(p => p.YearOfStudy == yearOfStudy).ToList();
            return PartialView(streams);
        }

        [HttpPost]
        public ActionResult StreamGroups(int streamId)
        {
            List<Group> groups = repository.DataBase.Groups.Where(p => p.StreamID == streamId).ToList();
            return PartialView(groups);
        }

        [HttpPost]
        public ActionResult TeacherClassroom(string Name, string TypeData)
        {
            ScheduleTeacherClassroomModel Model = null;
            if (TypeData.Equals(SearchType.teacher.ToString()))
            {
                var teacher = repository.DataBase.Teachers.Where(p => p.Name.Trim() == Name.Trim()).FirstOrDefault();
                var cathedra = repository.DataBase.Cathedras.Where(p => p.CathedraID == teacher.CathedraID).FirstOrDefault();

                Model = new ScheduleTeacherClassroomModel()
                {
                    TeacherName = teacher.Post + " " + teacher.Name,
                    TeacherId = teacher.TeacherID,
                    CathedraName = cathedra.Name + " (" + cathedra.FullName + ")",
                };
            }
            else if (TypeData.Equals(SearchType.classroom.ToString()))
            {
                var classroom = repository.DataBase.Classrooms.Where(p => p.Name.Trim() == Name.Trim()).FirstOrDefault();
                string cathedraName;
                try
                {
                    var cathedra = repository.DataBase.Cathedras.Where(p => p.CathedraID == classroom.CathedraID).FirstOrDefault();
                    cathedraName = cathedra.Name + " (" + cathedra.FullName + ")";
                }
                catch
                {
                    cathedraName = "Не підпорядковується кафедрам";

                }

                Model = new ScheduleTeacherClassroomModel()
                {
                    ClassroomName = classroom.Name,
                    ClassroomId = classroom.ClassroomID,
                    CathedraName = cathedraName
                };
            }
            else if (TypeData.Equals(SearchType.cathedra.ToString()))
            {
                var cathedra = repository.DataBase.Cathedras.Where(p => p.FullName.Trim() == Name.Trim()).FirstOrDefault();
                Model = new ScheduleTeacherClassroomModel()
                {
                    CathedraName = cathedra.Name + " (" + cathedra.FullName + ")",
                    CathdraId = cathedra.CathedraID
                };
            }

            return PartialView(Model);
        }

        [HttpPost]
        public ActionResult ScheduleTable(int Id, string typeData, string secondTypeData = null, int? numberFirstLesson = null)
        {
            if (secondTypeData != null)
            {
                if (typeData.Equals(SearchType.cathedra.ToString()))
                {
                    ScheduleTableModel scheduleModel = new ScheduleTableModel();
                    List<ScheduleTableModel> scheduleModels = new List<ScheduleTableModel>();

                    if (secondTypeData.Equals(SearchType.teacher.ToString()))
                    {
                        var teachers = repository.DataBase.Teachers.Where(p => p.CathedraID == Id).ToList();

                        foreach (var teacher in teachers)
                        {
                            scheduleModels.Add(ScheduleHelper.GetTableModel(teacher.TeacherID, SearchType.teacher.ToString(), null, repository));
                        }
                    }
                    else if (secondTypeData.Equals(SearchType.classroom.ToString()))
                    {
                        var classrooms = repository.DataBase.Classrooms.Where(p => p.CathedraID == Id).ToList();

                        foreach (var classroom in classrooms)
                        {
                            scheduleModels.Add(ScheduleHelper.GetTableModel(classroom.ClassroomID, SearchType.classroom.ToString(), null, repository));
                        }
                    }

                    scheduleModel = new ScheduleTableModel()
                    {
                        ListScheduleTableModel = scheduleModels
                    };

                    return PartialView(scheduleModel);
                }
            }
            return PartialView(ScheduleHelper.GetTableModel(Id, typeData, numberFirstLesson, repository));
        }

        [HttpPost]
        public ActionResult CathedraTable(int Id, string typeData, int? numberFirstLesson)
        {
            var teachers = repository.DataBase.Teachers.Where(p => p.CathedraID == Id).ToList();
            List<ScheduleTableModel> scheduleModels = new List<ScheduleTableModel>();
            foreach (var teacher in teachers)
            {
                scheduleModels.Add(ScheduleHelper.GetTableModel(teacher.TeacherID, SearchType.teacher.ToString(), null, repository));
            }

            CathedraTableModel TableModel = new CathedraTableModel()
            {
                ListScheduleTableModel = scheduleModels
            };


            return View(TableModel);

        }

        [HttpPost]
        public ActionResult ChangeData(string ParentNode, string ElementId, string ElementValue, string TdElementsID, string TdElementsValues, string CurrentGroupId, string NumberLesson)
        {
            TableLessonChange lessonChange = new TableLessonChange();
            sessionLessonEvents = Session["sessionLessonEvents"] as List<LessonEvent>;
            if (sessionLessonEvents == null) sessionLessonEvents = new List<LessonEvent>();
            var tdElementsID = TdElementsID.Split(',').ToList();
            var tdElementsValues = TdElementsValues.Split(',').ToList();
            lessonChange.TdIds = tdElementsID;
            lessonChange.ParrentTableNode = ParentNode.Trim();
            lessonChange.LessonNumber = Convert.ToInt32(NumberLesson.Trim());
            int currentGroupId = Convert.ToInt16(CurrentGroupId.Trim());

            var dayNumber = new SortedList<int, string>();
            dayNumber[1] = "Понеділок";
            dayNumber[2] = "Вівторок";
            dayNumber[3] = "Середа";
            dayNumber[4] = "Четвер";
            dayNumber[5] = "П'ятниця";

            LessonEvent busyDB = null;
            LessonEvent busySession = null;

            var currentProperty = ElementId.Split('-');

            string day = dayNumber[Convert.ToInt32(currentProperty[1].Trim())];
            int week = Convert.ToInt32(currentProperty[0].Trim());
            int lessId = Convert.ToInt32(currentProperty[2].Trim());
            var currentTimeID = repository.DataBase.Times.Where(p => p.WeekNum == week).Where(p => p.DayOfWeek.Trim() == day).Where(p => p.LessonTimeID == lessId).Select(p => p.TimeID).FirstOrDefault();

            int lessonTypeId = 0;
            int subjectId = 0;
            int teacherId = 0;
            int classroomId = 0;


            lessonChange.PropertyNumber = Convert.ToInt16(currentProperty[3].Trim());

        Selection:

            if ((currentProperty[3].Trim() == "2") || (currentProperty[3].Trim() == "3") || (currentProperty[3].Trim() == "4") || (currentProperty[3].Trim() == "5"))
            {
                lessonTypeId = Convert.ToInt16(tdElementsValues[0]);
                var lessonTypeName = repository.DataBase.LessonTypes.Where(p => p.LessonTypeID == lessonTypeId).Select(p => p.LessonTypeName).FirstOrDefault();
                TypeLessonModel selectedLessonType = new TypeLessonModel()
                {
                    LessonTypeId = lessonTypeId,
                    LessonTypeName = lessonTypeName.Trim()
                };
                var typeLessons = repository.DataBase.LessonTypes.Where(p => p.LessonTypeID != lessonTypeId).ToList();
                List<TypeLessonModel> lessonTypeModels = new List<TypeLessonModel>();
                foreach (var lessonType in typeLessons)
                {
                    TypeLessonModel typeLessonModel = new TypeLessonModel()
                    {
                        LessonTypeId = lessonType.LessonTypeID,
                        LessonTypeName = lessonType.LessonTypeName
                    };
                    lessonTypeModels.Add(typeLessonModel);
                }
                lessonChange.SelectedTypeLesson = selectedLessonType;
                lessonChange.TypesLesson = lessonTypeModels;

                if ((currentProperty[3].Trim() == "3") || (currentProperty[3].Trim() == "4") || (currentProperty[3].Trim() == "5"))
                {

                    subjectId = Convert.ToInt16(tdElementsValues[1]);
                    var subjectName = repository.DataBase.Subjects.Where(p => p.SubjectID == subjectId).Select(p => p.Name).FirstOrDefault();
                    SubjectModel selectedSubject = new SubjectModel()
                    {
                        SubjectId = subjectId,
                        SubjectName = subjectName
                    };
                    var subjectsBridge = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.LessonTypeID == lessonTypeId).Where(p => p.SubjectID != subjectId).ToList();
                    var subjectsIds = subjectsBridge.Select(p => p.SubjectID).ToList();
                    var subjects = repository.DataBase.Subjects.Where(p => subjectsIds.Contains(p.SubjectID)).Distinct().ToList();
                    List<SubjectModel> subjectsModels = new List<SubjectModel>();
                    foreach (var subject in subjects)
                    {
                        var subjectModel = new SubjectModel()
                        {
                            SubjectId = subject.SubjectID,
                            SubjectName = subject.Name + " (" + subjectsBridge.Where(p => p.SubjectID == subject.SubjectID).Select(p => p.CountHours).FirstOrDefault() + " год.)"
                        };
                        subjectsModels.Add(subjectModel);
                    }
                    lessonChange.SelectedSubject = selectedSubject;
                    lessonChange.Subjects = subjectsModels;

                    if ((currentProperty[3].Trim() == "4") || (currentProperty[3].Trim() == "5"))
                    {
                        teacherId = Convert.ToInt16(tdElementsValues[2]);
                        var teacherName = repository.DataBase.Teachers.Where(p => p.TeacherID == teacherId).Select(p => p.Name).FirstOrDefault();
                        TeacherModel selectedTeacher = new TeacherModel()
                        {
                            TeacherId = teacherId,
                            TeacherName = teacherName.Trim()
                        };
                        var teachersId = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.LessonTypeID == lessonTypeId).Where(p => p.SubjectID == subjectId).Where(p => p.TeacherID != teacherId).Select(p => p.TeacherID).ToList();
                        var teachers = repository.DataBase.Teachers.Where(p => teachersId.Contains(p.TeacherID)).Distinct().ToList();
                        var teacherModels = new List<TeacherModel>();
                        foreach (var teacher in teachers)
                        {
                            var teacherModel = new TeacherModel()
                            {
                                TeacherId = teacher.TeacherID,
                                TeacherName = teacher.Name
                            };
                            teacherModels.Add(teacherModel);
                        }
                        lessonChange.SelectedTeacher = selectedTeacher;
                        lessonChange.Teachers = teacherModels;
                    }
                }


            }
            if (currentProperty[3].Trim() == "2")
            {
                var subjectsBridge = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.LessonTypeID == lessonTypeId).ToList();
                var subjectsId = subjectsBridge.Select(t => t.SubjectID).ToList();
                var subjects = repository.DataBase.Subjects.Where(p => subjectsId.Contains(p.SubjectID)).Distinct().ToList();
                List<SubjectModel> subjectsModels = new List<SubjectModel>();
                foreach (var subject in subjects)
                {
                    var subjectModel = new SubjectModel()
                    {
                        SubjectId = subject.SubjectID,
                        SubjectName = subject.Name + " (" + subjectsBridge.Where(p => p.SubjectID == subject.SubjectID).Select(p => p.CountHours).FirstOrDefault() + " год.)"
                    };
                    var currentHoursCount = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.SubjectID == subject.SubjectID).Where(p => p.LessonTypeID == lessonTypeId).Select(p => p.CountHours).FirstOrDefault(); //фильтр
                    var countDBSubjects = repository.DataBase.LessonEvents.Where(p => p.SubjectID == subject.SubjectID).Where(p => p.LessonTypeID == lessonTypeId).Where(p => p.GroupID == currentGroupId).Count();
                    //var countSessionsSubjects = sessionLessonEvents.Where(p => p.SubjectID == subject.SubjectID).Where(p => p.LessonTypeID == lessonTypeId).Where(p => p.GroupID == currentGroupId).Count();

                    if (countDBSubjects < currentHoursCount)
                    {

                        subjectsModels.Add(subjectModel);
                    }
                }
                lessonChange.Subjects = subjectsModels;



                return PartialView(lessonChange);
            }
            else if (currentProperty[3].Trim() == "3")
            {
                var currentHoursCount = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.SubjectID == subjectId).Where(p => p.LessonTypeID == lessonTypeId).Select(p => p.CountHours).FirstOrDefault();
                var countDBSubjects = repository.DataBase.LessonEvents.Where(p => p.SubjectID == subjectId).Where(p => p.LessonTypeID == lessonTypeId).Where(p => p.GroupID == currentGroupId).Count();
                //  var countSessionsSubjects = sessionLessonEvents.Where(p => p.SubjectID == subjectId).Where(p => p.LessonTypeID == lessonTypeId).Where(p => p.GroupID == currentGroupId).Count();
                int coupled = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.SubjectID == subjectId).Where(p => p.LessonTypeID == lessonTypeId).Select(p => p.Сoupled).FirstOrDefault();
                bool correct = true;


                if (countDBSubjects < currentHoursCount)
                {
                    var teachersId = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.LessonTypeID == lessonTypeId).Where(p => p.SubjectID == subjectId).Select(p => p.TeacherID).ToList();
                    var teachers = repository.DataBase.Teachers.Where(p => teachersId.Contains(p.TeacherID)).Distinct().ToList();
                    var teacherModels = new List<TeacherModel>();
                    foreach (var teacher in teachers)
                    {
                        var teacherModel = new TeacherModel()
                        {
                            TeacherId = teacher.TeacherID,
                            TeacherName = teacher.Name
                        };
                        teacherModels.Add(teacherModel);
                    }
                    lessonChange.Teachers = teacherModels;
                }
                else
                {
                    lessonChange.TextError = String.Format("Помилка!\nДаний предмет вже досягнув ліміт у {0} год.", currentHoursCount);
                    correct = false;
                }

                if (coupled > 1)
                {
                    string errorCoupled = "Помилка! \nВибрано некоректне значення, оскільки даний предмет може читатись виключно підряд у складі " + coupled + "-х однакових пар, а в навчальний день не може нараховувати більше 3-х пар.";
                    if (lessId <= 3)
                    {
                        if (coupled + lessId - 1 > 3)
                        {
                            lessonChange.TextError = errorCoupled;
                            correct = false;
                        }
                    }
                    else if (lessId > 3)
                    {
                        if (coupled + lessId - 1 > 6)
                        {
                            lessonChange.TextError = errorCoupled;
                            correct = false;
                        }
                    }
                    for (int i = 1; i < coupled; i++)
                    {
                        int timeID = repository.DataBase.Times.Where(p => p.WeekNum == week).Where(p => p.DayOfWeek.Trim() == day).Where(p => p.LessonTimeID == lessId + i).Select(p => p.TimeID).FirstOrDefault();
                        var busyLessonEvent = repository.DataBase.LessonEvents.Where(p => p.GroupID == currentGroupId).Where(p => p.TimeID == timeID).FirstOrDefault();
                        if (busyLessonEvent != null)
                        {
                            correct = false;
                            lessonChange.TextError = "Помилка! \nВибрано некоректне значення, оскільки " + (lessId + i) + "-а пара зайнята іншим предметом у той час, коли вибрана дисципліна може читатись виключно у складі  " + coupled + "-х однакових пар підряд.";
                            break;
                        }
                    }
                }

                if (correct == false)
                {
                    currentProperty[3] = "2";
                    lessonChange.PropertyNumber--;
                    goto Selection;
                }

                return PartialView(lessonChange);
            }
            else if (currentProperty[3].Trim() == "4")
            {

                if (lessonTypeId != 1)
                {
                    busyDB = repository.DataBase.LessonEvents.Where(p => p.TimeID == currentTimeID).Where(p => p.TeacherID == teacherId).FirstOrDefault();
                    //  busySession = sessionLessonEvents.Where(p => p.TimeID == currentTimeID).Where(p => p.TeacherID == teacherId).FirstOrDefault();
                }
                if (busyDB == null)
                {
                    List<Classroom> classrooms;
                    List<Cathedra> cathedras;
                    var classroomsId = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.LessonTypeID == lessonTypeId).Where(p => p.SubjectID == subjectId).Where(p => p.TeacherID == teacherId).Select(p => p.ClassroomID).ToList();
                    classrooms = repository.DataBase.Classrooms.Where(p => classroomsId.Contains(p.ClassroomID)).Distinct().ToList();
                    cathedras = repository.DataBase.Cathedras.ToList();

                    if (classrooms.Count == 0) classrooms = repository.DataBase.Classrooms.Where(p => p.ClassroomTypeID == lessonTypeId).ToList();

                    var classroomModels = new List<ClassroomModel>();
                    foreach (var classroom in classrooms)
                    {
                        string cathedraName = cathedras.Where(p => p.CathedraID == classroom.CathedraID).Select(p => p.Name).FirstOrDefault();
                        cathedraName = (cathedraName != null) ? cathedraName : "Загальноуніверситетська";
                        var classroomModel = new ClassroomModel()
                        {
                            ClassroomId = classroom.ClassroomID,
                            ClassroomName = classroom.Name + " (" + cathedraName.Trim() + ")"
                        };
                        classroomModels.Add(classroomModel);
                    }
                    lessonChange.Classrooms = classroomModels;

                    return PartialView(lessonChange);
                }
                else
                {
                    string groupName = "";
                    if (busyDB != null) groupName = repository.DataBase.Groups.Where(p => p.GroupID == busyDB.GroupID).Select(p => p.GroupName).FirstOrDefault();
                    else if (busySession != null) groupName = repository.DataBase.Groups.Where(p => p.GroupID == busySession.GroupID).Select(p => p.GroupName).FirstOrDefault();
                    lessonChange.TextError = "Помилка!\nОбраний викладач в цей час зайнятий з групою " + groupName.Trim() + ".";
                    currentProperty[3] = "3";
                    lessonChange.PropertyNumber--;
                    goto Selection;
                }
            }
            else if (currentProperty[3].Trim() == "5")
            {
                var correct = true;
                if (lessonTypeId != 1)
                {
                    busyDB = repository.DataBase.LessonEvents.Where(p => p.TimeID == currentTimeID).Where(p => p.ClassroomID == classroomId).FirstOrDefault();
                }
                if (busyDB == null)
                {
                    classroomId = Convert.ToInt16(tdElementsValues[3]);
                    var classrrom = repository.DataBase.Classrooms.Where(p => p.ClassroomID == classroomId).FirstOrDefault();
                    var classroomName = classrrom.Name;

                    ClassroomModel selectedClassroom = new ClassroomModel()
                    {
                        ClassroomId = classroomId,
                        ClassroomName = classroomName
                    };

                    List<Classroom> classrooms;

                    var classroomsId = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.LessonTypeID == lessonTypeId).Where(p => p.SubjectID == subjectId).Where(p => p.TeacherID == teacherId).Where(p => p.ClassroomID != classroomId).Select(p => p.ClassroomID).ToList();
                    classrooms = repository.DataBase.Classrooms.Where(p => classroomsId.Contains(p.ClassroomID)).Distinct().ToList();


                    if (classrooms.Count == 0) classrooms = repository.DataBase.Classrooms.Where(p => p.ClassroomTypeID == lessonTypeId).Where(p => p.ClassroomID != classroomId).ToList();

                    var classroomModels = new List<ClassroomModel>();
                    foreach (var classroomItem in classrooms)
                    {
                        var classroomModel = new ClassroomModel()
                        {
                            ClassroomId = classroomItem.ClassroomID,
                            ClassroomName = classroomItem.Name
                        };
                        classroomModels.Add(classroomModel);
                    }

                    var stream = repository.DataBase.Groups.Where(p => p.GroupID == currentGroupId).FirstOrDefault();
                    int streamID = stream.StreamID;
                    var currentTime = repository.DataBase.Times.Where(p => p.TimeID == currentTimeID).FirstOrDefault();
                    int coupled = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == currentGroupId).Where(p => p.SubjectID == subjectId).Where(p => p.LessonTypeID == lessonTypeId).Select(p => p.Сoupled).FirstOrDefault();
                    //LessonEvent sessionLessonEvent = new LessonEvent()
                    //{
                    //    StreamID = streamID,
                    //    GroupID = currentGroupId,
                    //    LessonTypeID = lessonTypeId,
                    //    SubjectID = subjectId,
                    //    TeacherID = teacherId,
                    //    ClassroomID = classroomId,
                    //    TimeID = currentTimeID
                    //};
                    if (lessonTypeId == 1)
                    {
                        if (stream.StudentCount > classrrom.Capacity)
                        {
                            lessonChange.TextError = "Помилка!\nКількість студентів потоку перевищує місткість аудиторії.";
                            correct = false;
                        }
                    }
                    if (correct == true)
                    {
                        if (coupled > 1)
                        {
                            for (int i = 0; i < coupled; i++)
                            {
                                repository.DataBase.Database.ExecuteSqlCommand("exec [dbo].[csp_LessonSave] {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", currentGroupId, currentTime.WeekNum, currentTime.DayNumber, currentTime.LessonTimeID + i, lessonTypeId, subjectId, teacherId, classroomId);
                            }
                        }
                        else repository.DataBase.Database.ExecuteSqlCommand("exec [dbo].[csp_LessonSave] {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", currentGroupId, currentTime.WeekNum, currentTime.DayNumber, currentTime.LessonTimeID, lessonTypeId, subjectId, teacherId, classroomId);

                        lessonChange.TextError = "Пара успішно збережена!";
                        lessonChange.SelectesClassroom = selectedClassroom;
                        lessonChange.Classrooms = classroomModels;
                    }



                    //sessionLessonEvents.Add(sessionLessonEvent);

                    //  Session["sessionLessonEvents"] = sessionLessonEvents;


                }
                else
                {
                    string groupName = "";
                    if (busyDB != null) groupName = repository.DataBase.Groups.Where(p => p.GroupID == busyDB.GroupID).Select(p => p.GroupName).FirstOrDefault();
                    lessonChange.TextError = "Помилка!\nОбрана аудиторія в цей час зайнята групою " + groupName + ".";
                    correct = false;
                }

                if (correct == false)
                {
                    currentProperty[3] = "4";
                    lessonChange.PropertyNumber--;
                    goto Selection;
                }
                return PartialView(lessonChange);
            }

            return View();
        }

        [HttpPost]
        public ActionResult SaveToDb(string Ids, string Values, int GroupID)
        {
            List<string> currentIdslesson = new List<string>();
            List<string> uncorrectCells = new List<string>();
            List<string> IdsLessons = Ids.Split(',').ToList();
            List<string> valuesLessons = Values.Split(',').ToList();
            bool correctLesson = true;
            int lessonIndex = 0;
            //int lessonValuesIndex = 0;
            if (IdsLessons.Count == valuesLessons.Count)
            {
                while (lessonIndex != IdsLessons.Count)
                {
                    int j = 0;
                    LessonModel currentLesson = new LessonModel();
                    List<string> Idslesson = IdsLessons[lessonIndex].Split('-').ToList();
                    currentLesson.NumberWeek = Convert.ToInt32(Idslesson[0]);
                    currentLesson.NumberDay = Convert.ToInt32(Idslesson[1]);
                    currentLesson.LessonNumber = Convert.ToInt32(Idslesson[2]);
                    for (int i = lessonIndex; i < lessonIndex + 4; i++)
                    {
                        j++;
                        try
                        {
                            if (j == 1)
                            {
                                currentLesson.LessonTypeID = Convert.ToInt32(valuesLessons[i]);
                            }
                            else if (j == 2)
                            {
                                currentLesson.SubjectID = Convert.ToInt32(valuesLessons[i]);
                            }
                            else if (j == 3)
                            {
                                currentLesson.TeacherID = Convert.ToInt32(valuesLessons[i]);
                            }
                            else if (j == 4)
                            {
                                currentLesson.ClassroomID = Convert.ToInt32(valuesLessons[i]);
                            }
                        }
                        catch
                        {
                            if (j > 1)
                            {

                                for (int un = j + 1; un <= 5; un++)
                                {
                                    string uncorrectCell = "#" + currentLesson.NumberWeek + "-" + currentLesson.NumberDay + "-" + currentLesson.LessonNumber + "-" + un;//IdsLessons[lessonIndex].ToString() + "-" + un;
                                    uncorrectCells.Add(uncorrectCell);
                                }
                            }
                            correctLesson = false;
                            i = lessonIndex + 4;
                        }
                    }

                    if (correctLesson == true)
                    {
                        var result = repository.DataBase.Database.ExecuteSqlCommand("exec [dbo].[csp_LessonSave] {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", GroupID.ToString(), currentLesson.NumberWeek.ToString(), currentLesson.NumberDay.ToString(), currentLesson.LessonNumber.ToString(), currentLesson.LessonTypeID.ToString(), currentLesson.SubjectID.ToString(), currentLesson.TeacherID.ToString(), currentLesson.ClassroomID.ToString()); //.SqlQuery<bool>("exec [dbo].[csp_LessonSave] {0}, {1}, {2}, {3}, {4}, {5}, {6}", GroupID.ToString(), currentLesson.NumberWeek.ToString(), currentLesson.NumberDay.ToString(), currentLesson.LessonNumber.ToString(), currentLesson.LessonTypeID.ToString(), curren
                    }
                    else if (correctLesson == false)
                    {
                        correctLesson = true;
                    }
                    lessonIndex += 4;
                }
            }
            SaveResultModel saveResult = new SaveResultModel();
            if (uncorrectCells.Count > 0)
            {
                saveResult.Message = "Коректно введені зміни збережені!\nНекоректні заповнені поля виділені червоним кольором.";
                saveResult.UncorrectCells = uncorrectCells;
            }
            else
            {
                saveResult.Message = "Зміни успішно збережені!";
            }

            return PartialView(saveResult);
        }

        [HttpPost]
        public ActionResult DeleteLesson(string ParentNode, int CurrentGroupId, string TdElementsID)
        {
            var NodeIds = ParentNode.Split('-');
            int weekNum = Convert.ToInt32(NodeIds[0].Trim());
            int numberDay = Convert.ToInt32(NodeIds[1].Trim());
            int numberLesson = Convert.ToInt32(NodeIds[2].Trim());
            DeleteLessonModel deleteLesson = new DeleteLessonModel();
            deleteLesson.TdIds = TdElementsID.Split(',').ToList();
            deleteLesson.LessonNumber = numberLesson;

            int timeID = repository.DataBase.Times.Where(p => p.LessonTimeID == numberLesson).Where(p => p.DayNumber == numberDay).Where(p => p.WeekNum == weekNum).Select(p => p.TimeID).FirstOrDefault();

            var currentlesson = repository.DataBase.LessonEvents.Where(p => p.TimeID == timeID).Where(p => p.GroupID == CurrentGroupId).FirstOrDefault();

            int coupled = repository.DataBase.StreamSubjectBridges.Where(p => p.GroupID == CurrentGroupId).Where(p => p.SubjectID == currentlesson.SubjectID).Where(p => p.LessonTypeID == currentlesson.LessonTypeID).Select(p => p.Сoupled).FirstOrDefault();

            List<LessonEvent> Lessons = new List<LessonEvent>();
            if (coupled > 1)
            {
                var currentTime = repository.DataBase.Times.Where(p => p.TimeID == timeID).FirstOrDefault();
                int tempTimeId = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (numberLesson <= 3) tempTimeId = 1;
                    else if (numberLesson > 3) tempTimeId = 4;
                    tempTimeId = repository.DataBase.Times.Where(p => p.LessonTimeID == tempTimeId + i).Where(p => p.WeekNum == currentTime.WeekNum).Where(p => p.DayNumber == currentTime.DayNumber).Select(p => p.TimeID).FirstOrDefault();
                    var tempLessonEvent = repository.DataBase.LessonEvents.Where(p => p.TimeID == tempTimeId).Where(p => p.GroupID == currentlesson.GroupID).Where(p => p.SubjectID == currentlesson.SubjectID).FirstOrDefault();
                    if (tempLessonEvent != null) Lessons.Add(tempLessonEvent);
                }
            }
            else
            {
                Lessons.Add(currentlesson);
            }

            foreach (var lesson in Lessons)
            {
                if (lesson.LessonTypeID == 1)
                {
                    List<LessonEvent> lectionLessons = repository.DataBase.LessonEvents.Where(p => p.TimeID == lesson.TimeID).Where(p => p.StreamID == lesson.StreamID).ToList();
                    foreach (var lectionLesson in lectionLessons)
                    {
                        repository.DataBase.LessonEvents.Remove(lectionLesson);
                    }
                }
                else
                {
                    repository.DataBase.LessonEvents.Remove(lesson);
                }
            }


            repository.DataBase.SaveChanges();

            return PartialView(deleteLesson);
        }



    }
}
