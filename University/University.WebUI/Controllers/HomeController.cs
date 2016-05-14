using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using University.Domain.Abstract;
using University.Domain.Models;
using University.WebUI.Enums;
using University.WebUI.Helpers;
using University.WebUI.Models;

namespace University.WebUI.Controllers
{
    public class HomeController : Controller
    {
        IDataBaseRepository repository;
        GeneralSearchViewModel model;
        public HomeController(IDataBaseRepository repository)
        {
            this.repository = repository;

            this.model = new GeneralSearchViewModel()
            {
                Streams = repository.DataBase.Streams.ToList(),
                Groups = repository.DataBase.Groups.ToList(),
                Classrooms = repository.DataBase.Classrooms.ToList(),
                ClassroomTypes = repository.DataBase.ClassroomTypes.ToList(),
                Teachers = repository.DataBase.Teachers.ToList(),
                Cathedras = repository.DataBase.Cathedras.ToList(),
                Faculties = repository.DataBase.Faculties.ToList()
            };

        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();        
        }

        [HttpPost]
        public ActionResult ChangeData(string typeData)
        {
            List<string> list = new List<string>();
            if (Request.IsAjaxRequest())
            {
                if (typeData.Equals(SearchType.faculty.ToString()))
                {
                    foreach (var item in model.Faculties)
                    {
                        list.Add(item.Name +"("+item.FullName.Trim()+")");
                    }
                }
                else if (typeData.Equals(SearchType.stream.ToString()))
                {
                    foreach (var item in model.Streams)
                    {
                        list.Add(item.StreamName);
                    }
                }
                else if (typeData.Equals(SearchType.group.ToString()))
                {
                    foreach (var item in model.Groups)
                    {
                        list.Add(item.GroupName);
                    }
                }
                else if (typeData.Equals(SearchType.teacher.ToString()))
                {
                    foreach (var item in model.Teachers)
                    {
                        list.Add(item.Name);
                    }
                }
                else if (typeData.Equals(SearchType.classroom.ToString()))
                {
                    foreach (var item in model.Classrooms)
                    {
                        list.Add(item.Name);
                    }
                }
                else if (typeData.Equals(SearchType.cathedra.ToString()))
                {
                    foreach (var item in model.Cathedras)
                    {
                        list.Add(item.FullName);
                    }
                }

                return PartialView(list);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateTable(string name, string typeData)
        {
           // name = TextChange(name);
            name = name.Trim();
            if (typeData.Equals(SearchType.faculty.ToString()))
            {
                name = TextChange(name);
                Faculty faculty = model.Faculties.Where(p=>p.Name.Trim()==name.Trim()).FirstOrDefault();
                List<string> departments = model.Cathedras.Where(p=>p.FacultyID==faculty.FacultyID).Select(p=>p.Name).ToList();
                var facultyInfo = new GeneralInformation()
                {
                    FacultyID = faculty.FacultyID,
                    FacultyName = faculty.Name,
                    FullFacultyName = faculty.FullName,
                    FacultyDepartments = departments,
                    typeInformation = typeData
                };
                return PartialView(facultyInfo);
            }

            else if (typeData.Equals(SearchType.stream.ToString()))
            {               
                Stream stream = model.Streams.Where(p=>p.StreamName.Trim()==name).FirstOrDefault();
                List<string> groups = model.Groups.Where(p=>p.StreamID==stream.StreamID).Select(p=>p.GroupName).ToList();
                Faculty faculty = (model.Faculties.Where(p=>p.FacultyID == stream.FacultyID).FirstOrDefault());
                var streamInfo = new GeneralInformation()
                {
                    StreamID = stream.StreamID,
                    StreamName = stream.StreamName,
                    StreamYearOfStudy = stream.YearOfStudy,
                    StudentsCount = Convert.ToInt16(stream.StudentsCount),
                    FacultyName = faculty.Name,
                    FullFacultyName = faculty.FullName,
                    StreamGroups = groups,
                    typeInformation = typeData
                };
                return PartialView(streamInfo);
            }
            else if (typeData.Equals(SearchType.group.ToString()))
            {
                Group group = model.Groups.Where(p => p.GroupName.Trim() == name).FirstOrDefault();
                Stream stream = model.Streams.Where(p => p.StreamID == group.StreamID).FirstOrDefault();
                List<string> groups = model.Groups.Where(p => p.StreamID == stream.StreamID).Select(p => p.GroupName).ToList();
                Faculty faculty = (model.Faculties.Where(p => p.FacultyID == stream.FacultyID).FirstOrDefault());
                var streamInfo = new GeneralInformation()
                {
                    GroupID = group.GroupID,
                    GroupName = group.GroupName,
                    StreamID = stream.StreamID,
                    StreamName = stream.StreamName,
                    StreamYearOfStudy = stream.YearOfStudy,
                    StudentsCount = group.StudentCount == null ? -1 : Convert.ToInt16(group.StudentCount),
                    FacultyName = faculty.Name,
                    FullFacultyName = faculty.FullName,
                    StreamGroups = groups,
                    typeInformation = typeData
                };
                return PartialView(streamInfo);
            }
            else if (typeData.Equals(SearchType.teacher.ToString()))
            {
                Teacher teacher = model.Teachers.Where(p => p.Name.Trim() == name).FirstOrDefault();
                Cathedra cathedra = model.Cathedras.Where(p => p.CathedraID == teacher.CathedraID).FirstOrDefault();
                Faculty faculty = model.Faculties.Where(p => p.FacultyID == cathedra.FacultyID).FirstOrDefault();
                var teacherInfo = new GeneralInformation()
                {
                    TeacherID = teacher.TeacherID,
                    TeacherName = teacher.Name,
                    TeacherPost = teacher.Post,
                    CathedraName = cathedra.Name,
                    FullCathedraName = cathedra.FullName,
                    FacultyName = faculty.Name,
                    FullFacultyName = faculty.FullName,
                    typeInformation = typeData
                };
                return PartialView(teacherInfo);
            }
            else if (typeData.Equals(SearchType.classroom.ToString()))
            {
                string cathedraName;
                string fullCathedraName;
                string facultyName;
                string fullFacultyName;
                Classroom classroom = model.Classrooms.Where(p => p.Name.Trim() == name).FirstOrDefault();
                try
                {
                    Cathedra cathedra = model.Cathedras.Where(p => p.CathedraID == classroom.CathedraID).FirstOrDefault();
                    Faculty faculty = model.Faculties.Where(p => p.FacultyID == cathedra.FacultyID).FirstOrDefault();
                    cathedraName = cathedra.Name;
                    fullCathedraName = cathedra.FullName;
                    facultyName = faculty.Name;
                    fullFacultyName = faculty.FullName;
                }
                catch
                {
                    cathedraName = "Немає";
                    fullCathedraName = "Не підпорядковується кафедрам";
                    facultyName = "Немає";
                    fullFacultyName = "Не підпорядковується факультетам";

                }
                var classroomInfo = new GeneralInformation()
                {
                    ClassroomID = classroom.ClassroomID,
                    ClassroomName = classroom.Name,
                    ClassrommType = model.ClassroomTypes.Where(p=>p.ClassroomTypeID==classroom.ClassroomTypeID).Select(p=>p.Name).FirstOrDefault(),
                    ClassroomCapacity = classroom.Capacity,
                    CathedraName = cathedraName,
                    FullCathedraName = fullCathedraName,
                    FacultyName = facultyName,
                    FullFacultyName = fullFacultyName,
                    typeInformation = typeData
                };
                return PartialView(classroomInfo);
            }
            else if (typeData.Equals(SearchType.cathedra.ToString()))
            {
                Cathedra cathedra = repository.DataBase.Cathedras.Where(p=>p.FullName.Trim() == name.Trim()).FirstOrDefault();
                var facultyName = repository.DataBase.Faculties.Where(p=>p.FacultyID == cathedra.FacultyID).Select(p=>p.FullName).FirstOrDefault();
                var cathedraInfo = new GeneralInformation()
                {
                    CathedraID = cathedra.CathedraID,
                    CathedraName = cathedra.Name,
                    FullCathedraName = cathedra.FullName,
                    FacultyName = facultyName,
                    typeInformation = typeData
                };
                return PartialView(cathedraInfo);
            }

            return PartialView();
        }

        public ActionResult CreateJson()
        {
            var model = new GeneralSearchViewModel()
            {
                Streams = repository.DataBase.Streams.ToList(),
                Groups = repository.DataBase.Groups.ToList(),
                Classrooms = repository.DataBase.Classrooms.ToList(),
                Teachers = repository.DataBase.Teachers.ToList()
            };

            JsonResult jsonObj = new JsonResult();
            jsonObj.Data = model;
            jsonObj.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return Json(jsonObj.Data);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public PartialViewResult ExportToExcel(int FacultyID, string TypeData)
        {
            string templateName = String.Empty;
            string convertedName = String.Empty;
            string fileName = String.Empty;
            string shortFacultyName = repository.DataBase.Faculties.Where(p => p.FacultyID == FacultyID).Select(p => p.Name.Trim()).FirstOrDefault();


            if (TypeData == SearchType.stream.ToString())
            {
                templateName = "ScheduleStreamTemplate";
                convertedName = shortFacultyName + "_ConvertedStreamTemplate";
                fileName = shortFacultyName + "(розклад потоків)";
            }
            else if (TypeData == SearchType.teacher.ToString())
            {
                templateName = "ScheduleTeacherTemplate";
                convertedName = shortFacultyName + "_ConvertedTeacherTemplate";
                fileName = shortFacultyName + "(розклад викладачів)";
            }



            if (IsScheduleUpdated(TypeData, shortFacultyName))
            {
                var fileBytes = repository.DataBase.FileTables.Where(p => p.FileName.Trim() == templateName).Select(p => p.FileContent).FirstOrDefault();
                var pathBase = System.IO.Path.GetTempPath();
                var fileTemplate = "Schedule.xls";
                var path = pathBase + fileTemplate;


                System.IO.File.WriteAllBytes(path, fileBytes);

                ExportToExcelHelper exportHelper = new ExportToExcelHelper(repository);
                path = exportHelper.CreateExcelTable(FacultyID, path, TypeData); //



                List<byte> bytes = new List<byte>();
                byte[] buffer = new byte[1024];
                using (System.IO.Stream stream = System.IO.File.Open(path, System.IO.FileMode.Open))
                {
                    var count = stream.Read(buffer, 0, buffer.Length);
                    bytes.AddRange(buffer.Take(count));
                    while (count > 0)
                    {
                        count = stream.Read(buffer, 0, buffer.Length);
                        bytes.AddRange(buffer.Take(count));
                    }
                }

                byte[] result = bytes.ToArray();



                FileTable convertedDocument = repository.DataBase.FileTables.Where(p => p.FileName.Trim() == convertedName).FirstOrDefault(); //"ConvertedTemplate"

                if (convertedDocument == null)
                {
                    FileTable newConvertedDocument = new FileTable()
                    {
                        FileName = convertedName,
                        FileContent = result
                    };

                    repository.DataBase.FileTables.Add(newConvertedDocument);
                    repository.DataBase.Entry(newConvertedDocument).State = EntityState.Added;
                }
                else
                {
                    convertedDocument.FileContent = result;

                    repository.DataBase.FileTables.Attach(convertedDocument);
                    repository.DataBase.Entry(convertedDocument).State = EntityState.Modified;
                }

                SetScheduleUpdated(TypeData, shortFacultyName);

                repository.DataBase.SaveChanges();


            }

            ExportToExcelModel exportModel = new ExportToExcelModel() { ConvertedTemplate = convertedName, FileName = fileName }; //pathBase + fileName 

            


            return PartialView(exportModel);//File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName); // RedirectToAction("Index");
        }

        [HttpPost]
        public FileResult FileScheduleResult(string convertedName, string fileName)
        {
            //byte[] byteMas = new byte[100];
            var fileBytes = repository.DataBase.FileTables.Where(p => p.FileName.Trim() == convertedName).Select(p => p.FileContent).FirstOrDefault();
            //return File(fileBytes, "application/xls");
           return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName + ".xls");
        }

        private bool IsScheduleUpdated(string typeData, string facultyName)
        {
            var result = false;

            string key = facultyName + "_" + typeData + "_ScheduleUpdated";
            var isUpdatedSchedule = repository.DataBase.SystemConfigurations.Where(p => p.Key.Trim() == key).Select(p => p.Value.Trim()).FirstOrDefault();

            if (isUpdatedSchedule == null || isUpdatedSchedule == "true")
                result = true;

            return result;
        }

        private void SetScheduleUpdated(string typeData, string facultyName)
        {
            string key = facultyName + "_" + typeData + "_ScheduleUpdated";
            var record = repository.DataBase.SystemConfigurations.Where(p => p.Key.Trim() == key).FirstOrDefault();

            if (record == null)
            {
                var newRecord = new SystemConfiguration()
                {
                    Key = key,
                    Value = "false"
                };

                repository.DataBase.SystemConfigurations.Add(newRecord);
                repository.DataBase.Entry(newRecord).State = EntityState.Added;
            }
            else
            {
                record.Value = "false";

                repository.DataBase.SystemConfigurations.Attach(record);
                repository.DataBase.Entry(record).State = EntityState.Modified;
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
