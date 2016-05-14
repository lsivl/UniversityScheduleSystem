using System;
using System.Collections.Generic;
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
            name = TextChange(name);
            name = name.Trim();
            if (typeData.Equals(SearchType.faculty.ToString()))
            {
                
                Faculty faculty = model.Faculties.Where(p=>p.Name.Trim()==name).FirstOrDefault();
                List<string> departments = model.Cathedras.Where(p=>p.FacultyID==faculty.FacultyID).Select(p=>p.Name).ToList();
                var facultyInfo = new GeneralInformation()
                {
                    FacultyName = faculty.Name,
                    FullFacultyName = faculty.FullName,
                    FacultyID = faculty.FacultyID,
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
                    StreamName = stream.StreamName,
                    StreamYearOfStudy = stream.YearOfStudy,
                    StreamStudentsCount = Convert.ToInt16(stream.StudentsCount),
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
        public PartialViewResult ExportToExcel(int FacultyID)
        {
            var pathBase = AppDomain.CurrentDomain.BaseDirectory + @"\TextTeplates\";
            var fileTemplate = "Schedule.xls";
            var path = pathBase + fileTemplate;// @"C:\mySite\TextTeplates\Schedule.xls";//System.IO.Path.Combine(Server.MapPath("~/TextTeplates/"), "Schedule.xls");
            ExportToExcelHelper exportHelper = new ExportToExcelHelper(repository);
            var fileName = exportHelper.CreateExcelTable(FacultyID, path);
            //byte[] fileBytes = System.IO.File.ReadAllBytes(pathBase + fileName);

            ExportToExcelModel exportModel = new ExportToExcelModel() { Path = @"\TextTeplates\" + fileName };


            return PartialView(exportModel);//File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName); // RedirectToAction("Index");
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
