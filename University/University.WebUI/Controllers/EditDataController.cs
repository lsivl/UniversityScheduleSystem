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
using University.WebUI.Enums;
using University.WebUI.Helpers;

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



        public ActionResult EditObject(int id, string typeObject)
        {
            StartEditObjectModel startModel = new StartEditObjectModel() { ID = id, TypeObject = typeObject.Trim() };
            return View(startModel);
        }

        public ActionResult EditObjectMenu(int id, string typeObject, int? facultyId = null, bool isDelete = false)
        {

            ObjectEditModel objectModel = new ObjectEditModel();
            objectModel.TypeObject = typeObject;
            objectModel.IsDelete = isDelete;
            objectModel.IsCreate = false;

            if (typeObject.Equals(SearchType.faculty.ToString()))
            {
                if (id != null && isDelete == false)
                {
                    var facultyDB = repository.DataBase.Faculties.Where(p => p.FacultyID == id).FirstOrDefault();

                    objectModel.ID = facultyDB.FacultyID;
                    objectModel.Name = facultyDB.FullName.Trim();
                    objectModel.ShortName = facultyDB.Name.Trim();
                    objectModel.OperationName = "РЕДАГУВАННЯ ФАКУЛЬТЕТУ";
                }
                else if (isDelete) objectModel.DeleteMessage = "УВАГА!У випадку видалення факультету, автоматично видаляються всі викладачі, кафедри, потоки, групи та навантаження з розкладом, що пов'язані з цим факультетом!\nВсе одно видалити?";
                else if (id == null)
                {
                    objectModel.OperationName = "СТВОРЕННЯ НОВОГО ФАКУЛЬТЕТУ";
                    objectModel.IsCreate = true;
                }
            }
            else if (typeObject.Equals(SearchType.cathedra.ToString()))
            {
                if (isDelete == false)
                {
                    var faculties = repository.DataBase.Faculties.ToList();
                    if (id != -1)
                    {
                        var cathedraDB = repository.DataBase.Cathedras.Where(p => p.CathedraID == id).FirstOrDefault();

                        objectModel.ID = cathedraDB.CathedraID;
                        objectModel.FacultyID = cathedraDB.FacultyID;
                        objectModel.Name = cathedraDB.FullName.Trim();
                        objectModel.ShortName = cathedraDB.Name.Trim();
                        objectModel.OperationName = "РЕДАГУВАННЯ КАФЕДРИ";
                    }
                    else
                    {
                        objectModel.OperationName = "СТВОРЕННЯ НОВОЇ КАФЕДРИ";
                    }
                    objectModel.Faculties = new SelectList(faculties, "FacultyID", "FullName");

                }
                else objectModel.DeleteMessage = "УВАГА! У випадку видалення кафедри, автоматично видаляються всі викладачі та навантаження з розкладом, які пов'язані з цією кафедрою!\nВсе одно видалити?";
            }
            else if (typeObject.Equals(SearchType.stream.ToString()))
            {
                if (isDelete == false)
                {
                    List<int> coursesNumbers = new List<int>() { 1, 2, 3, 4, 5 };
                    var faculties = repository.DataBase.Faculties.ToList();
                    if (id != -1)
                    {
                        var streamDB = repository.DataBase.Streams.Where(p => p.StreamID == id).FirstOrDefault();
                        objectModel.ID = streamDB.StreamID;
                        objectModel.FacultyID = streamDB.FacultyID;
                        objectModel.Name = streamDB.StreamName.Trim();
                        objectModel.StudentsCount = streamDB.StudentsCount;
                        objectModel.YearOfStudy = streamDB.YearOfStudy;
                        objectModel.OperationName = "РЕДАГУВАННЯ ПОТОКУ";
                    }
                    else
                    {
                        objectModel.OperationName = "СТВОРЕННЯ НОВОГО ПОТОКУ";
                        objectModel.IsCreate = true;
                    }
                    objectModel.CoursesNumbers = new SelectList(coursesNumbers, "YearOfStudy");
                    objectModel.Faculties = new SelectList(faculties, "FacultyID", "FullName");
                }
                else objectModel.DeleteMessage = "УВАГА! У випадку видалення потоку, автоматично видаляються групи та пари цього потоку!\nВсе одно видалити?";
            }
            else if (typeObject.Equals(SearchType.group.ToString()))
            {
                if (isDelete == false)
                {

                    var faculties = repository.DataBase.Faculties.ToList();
                    List<University.Domain.Models.Stream> streams = new List<Domain.Models.Stream>();
                    if (facultyId != null)
                    {
                        objectModel.FacultyID = facultyId;
                        streams = repository.DataBase.Streams.Where(p => p.FacultyID == facultyId).ToList();
                        objectModel.ChangeFaculty = "Y";
                    }
                    if (id != -1)
                    {
                        var groupDB = repository.DataBase.Groups.Where(p => p.GroupID == id).FirstOrDefault();
                        var currentStream = repository.DataBase.Streams.Where(p => p.StreamID == groupDB.StreamID).FirstOrDefault();


                        objectModel.ID = groupDB.GroupID;
                        objectModel.Name = groupDB.GroupName.Trim();
                        objectModel.StudentsCount = groupDB.StudentCount;
                        objectModel.OperationName = "РЕДАГУВАННЯ ГРУПИ";
                        if (facultyId == null || facultyId == currentStream.FacultyID)
                        {
                            streams = repository.DataBase.Streams.Where(p => p.FacultyID == currentStream.FacultyID).ToList();
                            objectModel.StreamID = currentStream.StreamID;
                            objectModel.FacultyID = currentStream.FacultyID;
                            objectModel.ChangeFaculty = "N";
                        }
                    }
                    else if (id == -1)
                    {
                        objectModel.OperationName = "СТВОРЕННЯ НОВОЇ ГРУПИ";
                        objectModel.FacultyID = faculties.Select(p => p.FacultyID).First();
                        if (facultyId == null)
                        {
                            objectModel.ChangeFaculty = "N";
                            streams = repository.DataBase.Streams.Where(p => p.FacultyID == objectModel.FacultyID).ToList();
                        }
                        objectModel.IsCreate = true;
                    }
                    objectModel.Faculties = new SelectList(faculties, "FacultyID", "FullName");
                    objectModel.Streams = new SelectList(streams, "StreamID", "StreamName");
                }
                else objectModel.DeleteMessage = "УВАГА! У випадку видалення групи, автоматично видаляються всі пари цієї групи!\nВи впевнені, що хочете видалити?";
            }
            else if (typeObject.Equals(SearchType.teacher.ToString()))
            {
                if (isDelete == false)
                {
                    var faculties = repository.DataBase.Faculties.ToList();
                    List<Cathedra> cathedras = new List<Cathedra>();
                    if (facultyId != null)
                    {
                        objectModel.FacultyID = facultyId;
                        cathedras = repository.DataBase.Cathedras.Where(p => p.FacultyID == facultyId).ToList();
                        objectModel.ChangeFaculty = "Y";
                    }
                    if (id != -1)
                    {
                        var teacherDB = repository.DataBase.Teachers.Where(p => p.TeacherID == id).FirstOrDefault();
                        var currentCathedra = repository.DataBase.Cathedras.Where(p => p.CathedraID == teacherDB.CathedraID).FirstOrDefault();
                        objectModel.ID = teacherDB.TeacherID;
                        objectModel.Name = teacherDB.Name.Trim();
                        objectModel.TeacherPost = teacherDB.Post.Trim();
                        objectModel.OperationName = "РЕДАГУВАННЯ ВИКЛАДАЧА";
                        if (facultyId == null || facultyId == currentCathedra.FacultyID)
                        {
                            cathedras = repository.DataBase.Cathedras.Where(p => p.FacultyID == currentCathedra.FacultyID).ToList();
                            objectModel.CathedraID = teacherDB.CathedraID;
                            objectModel.FacultyID = currentCathedra.FacultyID;
                            objectModel.ChangeFaculty = "N";
                        }
                    }
                    else if (id == -1)
                    {
                        objectModel.OperationName = "СТВОРЕННЯ НОВОГО ВИКЛАДАЧА";
                        objectModel.FacultyID = faculties.Select(p => p.FacultyID).First();

                        if (facultyId == null)
                        {
                            objectModel.ChangeFaculty = "N";
                            cathedras = repository.DataBase.Cathedras.Where(p => p.FacultyID == objectModel.FacultyID).ToList();
                        }
                        objectModel.IsCreate = true;
                    }
                    objectModel.Faculties = new SelectList(faculties, "FacultyID", "FullName");
                    objectModel.Cathedras = new SelectList(cathedras, "CathedraID", "FullName");

                }
                else objectModel.DeleteMessage = "УВАГА! У випадку видалення викладача, автоматично видаляються пари цього викладача!\nВсе одно видалити?";
            }
            else if (typeObject.Equals(SearchType.classroom.ToString()))
            {
                if (isDelete == false)
                {
                    var faculties = repository.DataBase.Faculties.ToList();
                    var classroomTypes = repository.DataBase.ClassroomTypes.ToList();
                    List<Cathedra> cathedras = new List<Cathedra>();
                    if (facultyId != null)
                    {
                        objectModel.FacultyID = facultyId;
                        cathedras = repository.DataBase.Cathedras.Where(p => p.FacultyID == facultyId).ToList();
                        objectModel.ChangeFaculty = "Y";
                    }
                    if (id != -1)
                    {
                        var classroomDB = repository.DataBase.Classrooms.Where(p => p.ClassroomID == id).FirstOrDefault();
                        var currentCathedra = repository.DataBase.Cathedras.Where(p => p.CathedraID == classroomDB.CathedraID).FirstOrDefault();
                        objectModel.Name = classroomDB.Name.Trim();
                        objectModel.ClassroomTypeID = classroomDB.ClassroomTypeID;
                        objectModel.StudentsCount = classroomDB.Capacity;
                        objectModel.OperationName = "РЕДАГУВАННЯ АУДИТОРІЇ";
                        if (facultyId == null || facultyId == currentCathedra.FacultyID)
                        {
                            cathedras = repository.DataBase.Cathedras.Where(p => p.FacultyID == currentCathedra.FacultyID).ToList();
                            objectModel.CathedraID = classroomDB.CathedraID;
                            objectModel.FacultyID = currentCathedra.FacultyID;
                            objectModel.ChangeFaculty = "N";
                            objectModel.IsCreate = true;
                        }
                    }
                    else if (id == -1)
                    {
                        objectModel.OperationName = "СТВОРЕННЯ НОВОЇ АУДИТОРІЇ";
                        objectModel.FacultyID = faculties.Select(p => p.FacultyID).First();
                        if (facultyId == null)
                        {
                            objectModel.ChangeFaculty = "N";
                            cathedras = repository.DataBase.Cathedras.Where(p => p.FacultyID == objectModel.FacultyID).ToList();
                        }
                    }
                    objectModel.Faculties = new SelectList(faculties, "FacultyID", "FullName");
                    objectModel.Cathedras = new SelectList(cathedras, "CathedraID", "FullName");
                    objectModel.ClassroomTypes = new SelectList(classroomTypes, "ClassroomTypeID", "Name");
                }
                else objectModel.DeleteMessage = "УВАГА! У випадку видалення аудиторії, автоматично видаляються пари цієї аудиторії!\nВсе одно видалити?";
            }
            if (Session["MessageSave"] != null)
                objectModel.Message = Session["MessageSave"].ToString();
            Session["MessageSave"] = null;

            return PartialView(objectModel);
        }

        public ActionResult SaveObject(ObjectEditModel objectModel)
        {
            bool isChange = false;
            int IdObject = 0;

            if (objectModel.TypeObject.Equals(SearchType.faculty.ToString()))
            {
                Faculty facultyDB;
                facultyDB = objectModel.ID != -1 ? repository.DataBase.Faculties.Where(p => p.FacultyID == objectModel.ID).FirstOrDefault() : new Faculty();

                if (facultyDB.FullName.Trim() != objectModel.Name.Trim() || objectModel.ID == -1)
                {
                    facultyDB.FullName = objectModel.Name.Trim();
                    isChange = true;
                }
                if (facultyDB.Name.Trim() != objectModel.ShortName.Trim() || objectModel.ID == -1)
                {
                    facultyDB.Name = objectModel.ShortName.Trim();
                    isChange = true;
                }
                if (isChange)
                {
                    if (objectModel.ID == -1)
                    {
                        repository.DataBase.Faculties.Add(facultyDB);
                        repository.DataBase.Entry(facultyDB).State = EntityState.Added;

                        Session["MessageSave"] = String.Format("Факультет {0} успішно створений!", facultyDB.FullName.Replace("Факультет", "").Trim());
                    }
                    else
                    {
                        repository.DataBase.Faculties.Attach(facultyDB);
                        repository.DataBase.Entry(facultyDB).State = EntityState.Modified;

                        Session["MessageSave"] = String.Format("Зміни у факультеті {0} успішно збережені!", facultyDB.FullName.Replace("Факультет", "").Trim());
                    }

                    repository.DataBase.SaveChanges();
                }
                IdObject = objectModel.ID != -1 ? facultyDB.FacultyID : repository.DataBase.Faculties.Where(p => p.Name.Trim() == objectModel.ShortName.Trim()).Select(p => p.FacultyID).FirstOrDefault();
            }
            else if (objectModel.TypeObject.Equals(SearchType.cathedra.ToString()))
            {
                Cathedra cathedraDB;
                cathedraDB = objectModel.ID != -1 ? repository.DataBase.Cathedras.Where(p => p.CathedraID == objectModel.ID).FirstOrDefault() : new Cathedra();

                if (objectModel.ID == -1 || cathedraDB.FullName.Trim() != objectModel.Name.Trim())
                {
                    cathedraDB.FullName = objectModel.Name.Trim();
                    isChange = true;
                }
                if (objectModel.ID == -1 || cathedraDB.Name.Trim() != objectModel.ShortName.Trim())
                {
                    cathedraDB.Name = objectModel.ShortName.Trim();
                    isChange = true;
                }
                if (objectModel.ID == -1 || cathedraDB.FacultyID != objectModel.FacultyID)
                {
                    cathedraDB.FacultyID = (int)objectModel.FacultyID;
                    isChange = true;
                }
                if (isChange)
                {
                    if (objectModel.ID == -1)
                    {
                        repository.DataBase.Cathedras.Add(cathedraDB);
                        repository.DataBase.Entry(cathedraDB).State = EntityState.Added;

                        Session["MessageSave"] = String.Format("Кафедра {0} успішно створена!", cathedraDB.FullName);
                    }
                    else
                    {
                        repository.DataBase.Cathedras.Attach(cathedraDB);
                        repository.DataBase.Entry(cathedraDB).State = EntityState.Modified;

                        Session["MessageSave"] = String.Format("Зміни у кафедрі {0} успішно збережені!", cathedraDB.FullName);
                    }

                    repository.DataBase.SaveChanges();
                }
                IdObject = objectModel.ID != -1 ? cathedraDB.CathedraID : repository.DataBase.Cathedras.Where(p => p.Name.Trim() == objectModel.ShortName.Trim()).Select(p => p.CathedraID).FirstOrDefault();
            }
            else if (objectModel.TypeObject.Equals(SearchType.stream.ToString()))
            {
                University.Domain.Models.Stream streamDB;
                streamDB = objectModel.ID != -1 ? repository.DataBase.Streams.Where(p => p.StreamID == objectModel.ID).FirstOrDefault() : new University.Domain.Models.Stream();

                if (objectModel.ID == -1 || streamDB.StreamName.Trim() != objectModel.Name.Trim())
                {
                    streamDB.StreamName = objectModel.Name;
                    isChange = true;
                }
                if (objectModel.ID == -1 || streamDB.StudentsCount != objectModel.StudentsCount)
                {
                    streamDB.StudentsCount = objectModel.StudentsCount;
                    isChange = true;
                }
                if (objectModel.ID == -1 || streamDB.YearOfStudy != objectModel.YearOfStudy)
                {
                    streamDB.YearOfStudy = objectModel.YearOfStudy;
                    isChange = true;
                }
                if (objectModel.ID == -1 || streamDB.FacultyID != objectModel.FacultyID)
                {
                    streamDB.FacultyID = (int)objectModel.FacultyID;
                    isChange = true;
                }
                if (isChange)
                {
                    if (objectModel.ID == -1)
                    {
                        repository.DataBase.Streams.Add(streamDB);
                        repository.DataBase.Entry(streamDB).State = EntityState.Added;

                        Session["MessageSave"] = String.Format("Потік {0} успішно створений!", streamDB.StreamName.Trim());
                    }
                    else
                    {
                        repository.DataBase.Streams.Attach(streamDB);
                        repository.DataBase.Entry(streamDB).State = EntityState.Modified;

                        Session["MessageSave"] = String.Format("Зміни в потоці {0} успішно збережені!", streamDB.StreamName.Trim());
                    }

                    repository.DataBase.SaveChanges();
                }
                IdObject = objectModel.ID != -1 ? streamDB.StreamID : repository.DataBase.Streams.Where(p => p.StreamName.Trim() == objectModel.Name.Trim()).Select(p => p.StreamID).FirstOrDefault();
            }
            else if (objectModel.TypeObject.Equals(SearchType.group.ToString()))
            {
                Group groupDB;
                groupDB = objectModel.ID != -1 ? repository.DataBase.Groups.Where(p => p.GroupID == objectModel.ID).FirstOrDefault() : new Group();

                if (objectModel.ID == -1 || groupDB.GroupName.Trim() != objectModel.Name.Trim())
                {
                    groupDB.GroupName = objectModel.Name;
                    isChange = true;
                }
                if (objectModel.ID == -1 || groupDB.StudentCount != objectModel.StudentsCount)
                {
                    groupDB.StudentCount = objectModel.StudentsCount;
                    isChange = true;
                }
                if (objectModel.ID == -1 || groupDB.StreamID != objectModel.StreamID)
                {
                    groupDB.StreamID = (int)objectModel.StreamID;
                    isChange = true;
                }
                if (isChange)
                {
                    if (objectModel.ID == -1)
                    {
                        repository.DataBase.Groups.Add(groupDB);
                        repository.DataBase.Entry(groupDB).State = EntityState.Added;

                        Session["MessageSave"] = String.Format("Група {0} успішно створена!", groupDB.GroupName.Trim());
                    }
                    else
                    {
                        repository.DataBase.Groups.Attach(groupDB);
                        repository.DataBase.Entry(groupDB).State = EntityState.Modified;

                        Session["MessageSave"] = String.Format("Зміни в групі {0} успішно збережені!", groupDB.GroupName.Trim());
                    }

                    repository.DataBase.SaveChanges();
                }
                IdObject = objectModel.ID != -1 ? groupDB.GroupID : repository.DataBase.Groups.Where(p => p.GroupName.Trim() == objectModel.Name.Trim()).Select(p => p.GroupID).FirstOrDefault();
            }
            else if (objectModel.TypeObject.Equals(SearchType.teacher.ToString()))
            {
                Teacher teacherDB;
                teacherDB = objectModel.ID != -1 ? repository.DataBase.Teachers.Where(p => p.TeacherID == objectModel.ID).FirstOrDefault() : new Teacher();

                if (objectModel.ID == -1 || teacherDB.Name.Trim() != objectModel.Name.Trim())
                {
                    teacherDB.Name = objectModel.Name.Trim();
                    isChange = true;
                }
                if (objectModel.ID == -1 || teacherDB.Post.Trim() != objectModel.TeacherPost.Trim())
                {
                    teacherDB.Post = objectModel.TeacherPost.Trim();
                    isChange = true;
                }
                if (objectModel.ID == -1 || teacherDB.CathedraID != objectModel.CathedraID)
                {
                    teacherDB.CathedraID = (int)objectModel.CathedraID;
                    isChange = true;
                }
                if (isChange)
                {
                    if (objectModel.ID == -1)
                    {
                        repository.DataBase.Teachers.Add(teacherDB);
                        repository.DataBase.Entry(teacherDB).State = EntityState.Added;

                        Session["MessageSave"] = String.Format("Викладач {0} успішно створений!", teacherDB.Name.Trim());
                    }
                    else
                    {
                        repository.DataBase.Teachers.Attach(teacherDB);
                        repository.DataBase.Entry(teacherDB).State = EntityState.Modified;

                        Session["MessageSave"] = String.Format("Зміни викладача {0} успішно збережені!", teacherDB.Name.Trim());
                    }

                    repository.DataBase.SaveChanges();
                }
                IdObject = objectModel.ID != -1 ? teacherDB.TeacherID : repository.DataBase.Teachers.Where(p => p.Name.Trim() == objectModel.Name.Trim()).Select(p => p.TeacherID).FirstOrDefault();
            }
            else if (objectModel.TypeObject.Equals(SearchType.classroom.ToString()))
            {
                Classroom classroomDB;
                classroomDB = objectModel.ID != -1 ? repository.DataBase.Classrooms.Where(p => p.ClassroomID == objectModel.ID).FirstOrDefault() : new Classroom();

                if (objectModel.ID == -1 || classroomDB.Name.Trim() != objectModel.Name.Trim())
                {
                    classroomDB.Name = objectModel.Name.Trim();
                    isChange = true;
                }
                if (objectModel.ID == -1 || classroomDB.Capacity != objectModel.StudentsCount)
                {
                    classroomDB.Capacity = (int)objectModel.StudentsCount;
                    isChange = true;
                }
                if (objectModel.ID == -1 || classroomDB.ClassroomTypeID != objectModel.ClassroomTypeID)
                {
                    classroomDB.ClassroomTypeID = (int)objectModel.ClassroomTypeID;
                    isChange = true;
                }
                if (objectModel.ID == -1 || classroomDB.CathedraID != objectModel.CathedraID)
                {
                    classroomDB.CathedraID = (int)objectModel.CathedraID;
                    isChange = true;
                }
                if (isChange)
                {
                    if (objectModel.ID == -1)
                    {
                        repository.DataBase.Classrooms.Add(classroomDB);
                        repository.DataBase.Entry(classroomDB).State = EntityState.Added;

                        Session["MessageSave"] = String.Format("Аудиторія {0} успішно створена!", classroomDB.Name.Trim());
                    }
                    else
                    {
                        repository.DataBase.Classrooms.Attach(classroomDB);
                        repository.DataBase.Entry(classroomDB).State = EntityState.Modified;

                        Session["MessageSave"] = String.Format("Зміни аудиторії {0} успішно збережені!", classroomDB.Name.Trim());
                    }

                    repository.DataBase.SaveChanges();
                }
                IdObject = objectModel.ID != -1 ? classroomDB.ClassroomID : repository.DataBase.Classrooms.Where(p => p.Name.Trim() == objectModel.Name.Trim()).Select(p => p.ClassroomID).FirstOrDefault();
            }

            if (!isChange) Session["MessageSave"] = "Ви не внесли зміни!";


            return RedirectToAction("EditObject", new RouteValueDictionary(new { controller = "EditData", action = "EditObject", id = IdObject, typeObject = objectModel.TypeObject }));

        }

        [HttpPost]
        public ActionResult DeleteObject(int id, string typeObject)
        {
            var result = repository.DataBase.Database.ExecuteSqlCommand("exec [dbo].[csp_DeleteObject] {0}, {1}", id, typeObject);
            return PartialView();
        }

        public ActionResult EditDataMenu()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileInput, string typeData)
        {
            UploadModel uploadModel = new UploadModel();
            try
            {
                if (fileInput.ContentLength > 0)
                {

                    //  List<byte> bytes = new List<byte>();
                    //  byte[] buffer = new byte[1024];
                    //  using (System.IO.Stream stream = fileInput.InputStream)
                    //  {
                    //      var count = stream.Read(buffer, 0, buffer.Length);
                    //      bytes.AddRange(buffer.Take(count));
                    //      while (count > 0)
                    //      {
                    //          count = stream.Read(buffer, 0, buffer.Length);
                    //          bytes.AddRange(buffer.Take(count));
                    //      }
                    //  }

                    //  byte[] result = bytes.ToArray();


                    //  FileTable newFile = new FileTable();
                    //  newFile.FileName = "ScheduleTemplate";
                    //  newFile.FileContent = result;

                    //  repository.DataBase.FileTables.Add(newFile);
                    //  repository.DataBase.Entry(newFile).State = EntityState.Added;


                    //  try
                    //  {
                    //      repository.DataBase.SaveChanges();
                    //  }
                    //  catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    //  {
                    //      Exception raise = dbEx;
                    //      foreach (var validationErrors in dbEx.EntityValidationErrors)
                    //      {
                    //          foreach (var validationError in validationErrors.ValidationErrors)
                    //          {
                    //              string message = string.Format("{0}:{1}",
                    //                  validationErrors.Entry.Entity.ToString(),
                    //                  validationError.ErrorMessage);
                    //              // raise a new exception nesting
                    //              // the current instance as InnerException
                    //              raise = new InvalidOperationException(message, raise);
                    //          }
                    //      }
                    ////      throw raise;
                    //      this.RowList = new List<ExcelTableRowModel>();
                    //      ViewBag.Message = raise.Message;
                    //  }


                    var fileName = Path.GetFileName(fileInput.FileName);
                    var path = Path.Combine(System.IO.Path.GetTempPath(), fileName); fileInput.SaveAs(path);
                    string ext = Path.GetExtension(path);

                    if (ext == ".xls")
                    {
                        uploadModel = ImportFromExcel(path, typeData);
                        this.RowList = uploadModel.RowList;
                        uploadModel.TypeData = typeData;
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
        public ActionResult ShowContent(int course, string typeData)
        {
            RowList = Session["FileContent"] as List<ExcelTableRowModel>;
            ShowContentModel contentModel = new ShowContentModel()
            {
                TypeData = typeData,
                SelectedCourse = course,
                TableRows = RowList
            };
            return PartialView(contentModel);
        }

        [HttpPost]
        public ActionResult SaveToDb(int? cathedraId, bool isDelete, string typeData)
        {
            RowList = Session["FileContent"] as List<ExcelTableRowModel>;
            SaveToDbModel saveToDbModel = new SaveToDbModel();
            string errorMessage = "Помилка завантаження даних. Перевірте коректність документу.";
            try
            {
                saveToDbModel.Correct = true;
                if (typeData.Trim() == "1" && cathedraId != null)
                {
                    int currentCathedra = (int)cathedraId;
                    if (isDelete)
                    {
                        repository.DataBase.Database.ExecuteSqlCommand("exec [dbo].[csp_DeleteCatedraData] {0}", cathedraId);
                    }
                    int lessonLectionTypeID = repository.DataBase.LessonTypes.Where(p => p.LessonTypeName.Trim() == "Лекція").Select(p => p.LessonTypeID).FirstOrDefault();
                    int lessonPraсticeTypeID = repository.DataBase.LessonTypes.Where(p => p.LessonTypeName.Trim() == "Практика").Select(p => p.LessonTypeID).FirstOrDefault();
                    int lessonLaboratoryTypeID = repository.DataBase.LessonTypes.Where(p => p.LessonTypeName.Trim() == "Лабораторна").Select(p => p.LessonTypeID).FirstOrDefault();
                    int rowNumber = 0;
                    foreach (var row in RowList)
                    {
                        rowNumber++;
                        //int currentFacultyID = repository.DataBase.Faculties.Where(p => p.Name.Trim() == row.Faculty.Trim()).Select(p => p.FacultyID).FirstOrDefault();
                        Subject currentSubject = repository.DataBase.Subjects.Where(p => p.Name.Trim() == row.Subject.Trim()).FirstOrDefault();
                        if (currentSubject == null)
                        {
                            currentSubject = new Subject()
                            {
                                Name = row.Subject,
                                CathedraID = currentCathedra
                            };
                            repository.DataBase.Subjects.Add(currentSubject);
                        }

                        List<Teacher> teachers = new List<Teacher>();

                        Teacher currentLectionTeacher = null;
                        if (row.LectionHour != 0)
                        {
                            try
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
                                        CathedraID = currentCathedra
                                    };
                                    repository.DataBase.Teachers.Add(currentLectionTeacher);
                                }
                                teachers.Add(currentLectionTeacher);
                            }
                            catch
                            {
                                errorMessage = string.Format("Помилка зчитування викладача для лекцій пар. Запис має вигляд:  '{0}'. ", row.LectionTeacher.Split('(', ')')[1]);
                                errorMessage += "Запис повинен мати вигляд: 'ПІБ викладача (посада викладача)'.";
                                errorMessage += string.Format("Поточний предмет: {0}.", row.Subject.Trim());
                                saveToDbModel.Correct = false;
                                throw new Exception();
                            }
                        }


                        var teachersPracticalGroups = new SortedList<string, Teacher>();
                        if (row.PracticalHour != 0)
                        {
                            string teacherPost = null;
                            string teacherName = null;
                            Teacher currentPracticalTeacher = null;
                            List<string> practicalTeachers = row.PracticalTeacher.Split(',').ToList();
                            foreach (var teacher in practicalTeachers)
                            {
                                try
                                {
                                    teacherPost = teacher.Split('(', ')')[1];
                                    teacherName = TextChange(teacher);
                                    currentPracticalTeacher = repository.DataBase.Teachers.Where(p => p.Name.Trim() == teacherName.Trim()).FirstOrDefault();
                                    if ((currentLectionTeacher != null) && (teacherName.Trim() == currentLectionTeacher.Name.Trim())) { currentPracticalTeacher = currentLectionTeacher; }
                                    if (currentPracticalTeacher == null)
                                    {
                                        currentPracticalTeacher = new Teacher()
                                        {
                                            Name = teacherName,
                                            Post = teacherPost,
                                            CathedraID = currentCathedra
                                        };
                                        repository.DataBase.Teachers.Add(currentPracticalTeacher);
                                    }
                                }
                                catch
                                {
                                    errorMessage = string.Format("Помилка зчитування викладача для практичнизх пар. Запис має вигляд:  '{0}'. ",  row.PracticalTeacher.Trim());
                                    errorMessage += "Запис повинен мати вигляд: 'ПІБ викладача (посада викладача) {ГРУПА1;ГРУПА2}, ПІБ наступного викладача (посада викладача) {ГРУПА3;ГРУПА4}...'. Викладачі повинні бути розділені комами(,). А групи викладача - крапками з комою(;). ";
                                    errorMessage += string.Format("Поточний предмет: {0}.", row.Subject.Trim());
                                    saveToDbModel.Correct = false;
                                    throw new Exception();
                                }
                                var groupsForTeacher = SaveFromExcelHelper.TeacherGroups(teacher, "практичних", teacherName, currentSubject.Name, ref errorMessage);
                                if (groupsForTeacher != null)
                                foreach (var item in groupsForTeacher) { teachersPracticalGroups[item.Trim()] = currentPracticalTeacher; }
                                else throw new Exception();
                                teachers.Add(currentLectionTeacher);
                            }

                        }


                        var teachersLaboratoryGroups = new SortedList<string, Teacher>();
                        if (row.LabHour != 0)
                        {
                            string teacherPost = null;
                            string teacherName = null;
                            Teacher currentLaboratoryTeacher = null;
                            List<string> laboratoryTeachers = row.LabTeacher.Split(',').ToList();
                            foreach (var teacher in laboratoryTeachers)
                            {
                                try
                                {
                                    teacherPost = teacher.Split('(', ')')[1];
                                    teacherName = TextChange(teacher);
                                    currentLaboratoryTeacher = repository.DataBase.Teachers.Where(p => p.Name.Trim() == teacherName.Trim()).FirstOrDefault();
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
                                            CathedraID = currentCathedra
                                        };
                                        repository.DataBase.Teachers.Add(currentLaboratoryTeacher);
                                    }
                                }
                                catch
                                {
                                    errorMessage = string.Format("Помилка зчитування викладача для лабораторних пар. Запис має вигляд:  '{0}'. ", row.LabTeacher.Trim());
                                    errorMessage += "Запис повинен мати вигляд: 'ПІБ викладача (посада викладача) {ГРУПА1;ГРУПА2}, ПІБ наступного викладача (посада викладача) {ГРУПА3;ГРУПА4}...'. Викладачі повинні бути розділені комами(,). А групи викладача - крапками з комою(;). ";
                                    errorMessage += string.Format("Поточний предмет: {0}.", row.Subject.Trim());
                                    saveToDbModel.Correct = false;
                                    throw new Exception();
                                }
                                var groupsForTeacher = SaveFromExcelHelper.TeacherGroups(teacher, "лабораторних", teacherName, currentSubject.Name, ref errorMessage);
                                if (groupsForTeacher != null)
                                    foreach (var item in groupsForTeacher) { teachersLaboratoryGroups[item.Trim()] = currentLaboratoryTeacher; }
                                else throw new Exception();
                                teachers.Add(currentLaboratoryTeacher);
                            }

                        }

                        List<Group> currentGroups = new List<Group>();

                        if (row.GroupsOfStream != null)
                        {
                            currentGroups = SaveFromExcelHelper.GetGroupsFromDB(repository, row.GroupsOfStream.Split(',').ToList(), row.Subject.Trim(), ref errorMessage);
                            if (currentGroups == null) throw new Exception();
                        }
                     
                        if (row.UnitedGroups != null)
                        {
                            var unitedGroups = SaveFromExcelHelper.GetGroupsFromDB(repository, row.UnitedGroups.Split(',').ToList(), row.Subject.Trim(), ref errorMessage);
                            if (unitedGroups == null) throw new Exception();

                            foreach (var currentGroup in unitedGroups)
                            {
                                UnitedGroup newUnitedGroup = new UnitedGroup()
                                {
                                    GroupID = currentGroup.GroupID,
                                    StreamID =  currentGroup.StreamID,
                                    Subject = currentSubject,
                                    Course = row.Course,
                                    Row = rowNumber,
                                    CathedraID = currentCathedra
                                };
                                repository.DataBase.UnitedGroups.Add(newUnitedGroup);

                                if (!currentGroups.Contains(currentGroup))
                                    currentGroups.Add(currentGroup);
                            }
                        }

                        if(currentGroups.Count == 0)
                        {
                            errorMessage = String.Format("У документі відсутній список груп.\nПоточний предмет: {0}.", row.Subject.Trim());
                            throw new Exception();
                        }

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
                            if (!SaveFromExcelHelper.CheckGroupsForTeachers(teachersPracticalGroups, teachersLaboratoryGroups, row.PracticalHour, row.LabHour, group, ref errorMessage))
                            {
                                errorMessage += String.Format("Поточний предмет: {0}.", row.Subject.Trim());
                                saveToDbModel.Correct = false;
                                throw new Exception();
                            }
                            if (row.PracticalHour != 0)
                            {
                                Teacher teacher;
                                    try
                                    {
                                        teacher = teachersPracticalGroups[group.GroupName.Trim()];
                                    }
                                    catch
                                    {
                                        //errorMessage = String.Format("Помилка завантаження. Група {0} відсутня у переліку груп для практичних пар викладача. Якщо така група присутня, тоді скопіюйте в документі дану групу з колонки 'Групи потоку' та замініть її в переліку груп для викладача та перевірте правильність заповнення(коми між викладачами та крапки з комою між групами). Поточний предмет: {1}", group.GroupName.Trim(), row.Subject.Trim());
                                        //saveToDbModel.Correct = false;
                                        //throw new Exception();
                                        goto LabLabel;
                                    }
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
                                                Teacher = teacher,
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
                                                Teacher = teacher,
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
                                        Teacher = teacher,
                                        CountHours = row.PracticalHour,
                                        Сoupled = row.Coupled,
                                        CathedraID = cathedraId
                                    };
                                    repository.DataBase.StreamSubjectBridges.Add(currentPracticalStreamSubjectBridge);
                                }
                            }
                        LabLabel:
                            if (row.LabHour != 0)
                            {
                                Teacher teacher;
                                //try
                                //{
                                    teacher = teachersLaboratoryGroups[group.GroupName.Trim()];
                                //}
                                //catch
                                //{
                                //    errorMessage = String.Format("Помилка завантаження. Група {0} відсутня у переліку груп для лабораторних пар викладача. Якщо така група присутня, тоді скопіюйте в документі дану групу з колонки 'Групи потоку' та замініть її в переліку груп для викладача та перевірте правильність заповнення(коми між викладачами та крапки з комою між групами). Поточний предмет: {1}", group.GroupName.Trim(), row.Subject.Trim());
                                //    saveToDbModel.Correct = false;
                                //    throw new Exception();
                                //}
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
                                                Teacher = teacher,
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
                                                Teacher = teacher,
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
                                        Teacher = teacher,
                                        CountHours = row.LabHour,
                                        Сoupled = row.Coupled,
                                        CathedraID = cathedraId
                                    };
                                    repository.DataBase.StreamSubjectBridges.Add(currentLaboratoryStreamSubjectBridge);
                                }
                            }
                        }

                        repository.DataBase.SaveChanges();
                    }
                }
                else if (typeData.Trim() == "2")
                {
                    saveToDbModel.Correct = true;
                    foreach (var row in RowList)
                    {

                            var currentFaculty = repository.DataBase.Faculties.Where(p => p.Name.Trim() == row.Faculty.Trim()).FirstOrDefault();
                            
                            if (currentFaculty == null)
                            {
                                errorMessage = String.Format("Помилка завантаження даних. Факультет {0} відсутній у базі даних.", row.Faculty.Trim());
                                saveToDbModel.Correct = false;
                                throw new Exception();
                            }

                            if (isDelete == true)
                            {
                                var streamsOfFaculty = repository.DataBase.Streams.Where(p => p.FacultyID == currentFaculty.FacultyID).ToList();
                                foreach (var stream in streamsOfFaculty)
                                {
                                    repository.DataBase.Database.ExecuteSqlCommand("exec [dbo].[csp_DeleteObject] {0}, {1}", stream.StreamID, "stream");
                                }
                                isDelete = false;
                            }

                            University.Domain.Models.Stream currentStream = repository.DataBase.Streams.Where(p => p.StreamName.Trim() == row.Stream.Trim()).FirstOrDefault();
  
                            if (currentStream == null)
                            {
                                currentStream = new University.Domain.Models.Stream()
                                {
                                    StreamName = row.Stream.Trim(),
                                    YearOfStudy = row.Course,
                                    FacultyID = currentFaculty.FacultyID
                                };
                                repository.DataBase.Streams.Add(currentStream);
                            }

                            var groupsNames = row.GroupsOfStream.Split(',').ToList();
                            foreach (var group in groupsNames)
                            {
                                var currentGroup = repository.DataBase.Groups.Where(p => p.GroupName.Trim() == group.Trim()).FirstOrDefault();
                                if (currentGroup == null)
                                {
                                    currentGroup = new Group()
                                    {
                                        GroupName = group.Trim(),
                                        Stream = currentStream
                                    };
                                    repository.DataBase.Groups.Add(currentGroup);
                                }
                            }

                            repository.DataBase.SaveChanges();
                    
                    }
                }
                else if (typeData.Trim() == "3")
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
                //TempData["message"] = "Помилка завантаження даних";
                //return RedirectToAction("EditDataMenu");
                repository.DataBase.Database.ExecuteSqlCommand("exec [dbo].[csp_DeleteCatedraData] {0}", cathedraId);
                saveToDbModel.Message = errorMessage;
                saveToDbModel.Correct = false;
            }
            if (saveToDbModel.Correct != false) 
            {
                saveToDbModel.Correct = true;
                saveToDbModel.Message = "Завантаження даних успішно завершено!";
            }
            return PartialView(saveToDbModel);
        }

        private UploadModel ImportFromExcel(string file, string type)
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



            for (numList = 1; numList <= numListCount; numList++)
            {
                ExcelWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExcelWorkBook.Worksheets.get_Item(numList);
                ExcelRange = ExcelWorkSheet.UsedRange;

                if (!TestCorrectSheet(ExcelWorkSheet, ExcelRange, numListCount, numList, type))
                {
                    TempRowList = new List<ExcelTableRowModel>();
                    goto ErrorLabel;
                }

                if (type == "1")
                {
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
                                        if (row.Course < 1 || row.Course > 6)
                                        {
                                            ViewBag.Message = String.Format("Помилка зчитування даних на {0} листі, {1} рядок, {2} стовпець. Значення номера курсу має бути від 1 до 6.", numList, rCnt, cCnt);
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


                }
                else if (type == "2")
                {

                    for (rCnt = 2; rCnt <= ExcelRange.Rows.Count; rCnt++)
                    {

                        ExcelTableRowModel row = new ExcelTableRowModel();
                        for (cCnt = 1; cCnt <= 5; cCnt++)
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
                                    if (cCnt == 1) { row.Stream = str; }
                                    else if (cCnt == 2) { row.GroupsOfStream = str; }//{ try { row.GroupsOfStream = str.Split(',').ToList(); } catch { row.GroupsOfStream = null; } }
                                    else if (cCnt == 3) { row.StudentsCount = Convert.ToInt32(str); }
                                    else if (cCnt == 4) { row.Course = Convert.ToInt32(str); }
                                    else if (cCnt == 5) { row.Faculty = str; }
                                    //dataGridView1.Rows[rCnt - 14].Cells[cCnt].Value = str;
                                }
                            }
                            catch
                            {
                                ViewBag.Message = String.Format("Помилка зчитування даних на {1} рядку, {2} стовпці. Значення має бути числом.", numList, rCnt, cCnt);
                                TempRowList = new List<ExcelTableRowModel>();
                                goto ErrorLabel;
                            }
                        }
                        if (brk.Equals(false))
                        {
                            TempRowList.Add(row);
                        }
                        else if (brk.Equals(true))
                        {
                            brk = false;
                            break;
                        }
                    }
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


                }
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

        private bool TestCorrectSheet(Microsoft.Office.Interop.Excel.Worksheet ExcelWorkSheet, Microsoft.Office.Interop.Excel.Range ExcelRange, int numListCount, int numList, string type)
        {

            if (numListCount > 6 && type == "1")
            {
                ViewBag.Message = "Максимальна дозвлена кількість сторінок у документі - 6(кожна сторінка відображає інформацію по навантаженню певного курсу від 1 до 6).";
                return false;
            }
            if (numListCount > 1 && type == "2")
            {
                ViewBag.Message = "При завантаженні нових потоків, документ повинен складатись з одної сторінки з даними.";
                return false;
            }
            if (Convert.ToString((ExcelRange.Cells[5, 1] as Microsoft.Office.Interop.Excel.Range).Value2) != "Назви навчальних дисциплін і видів навчальної роботи" && type == "1" ||
               (Convert.ToString((ExcelRange.Cells[1, 1] as Microsoft.Office.Interop.Excel.Range).Value2) != "Назва Потоку" && type == "2"))
            {
                if (numList == 1)
                    ViewBag.Message = "Структура документа не відповідає вимогам програми.";
                else if (numList > 1)
                {
                    ViewBag.Message = String.Format("На листі {0}, cтруктура документа не відповідає вимогам програми.", ExcelWorkSheet.Name);
                }
                return false;
            }
            return true;
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
