using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.WebUI.Models.ShortVersionsModels;

namespace University.WebUI.Models
{
    public class TableLessonChange
    {
        public TypeLessonModel SelectedTypeLesson { get; set; }
        public List<TypeLessonModel> TypesLesson { get; set; }
        public SubjectModel SelectedSubject{ get; set; }
        public List<SubjectModel> Subjects { get; set; }
        public TeacherModel SelectedTeacher { get; set; }
        public List<TeacherModel> Teachers { get; set; }
        public ClassroomModel SelectesClassroom { get; set; }
        public List<ClassroomModel> Classrooms { get; set; }

     
        public string ParrentTableNode { get; set; }
        public List<string> TdIds { get; set; }
        public int PropertyNumber { get; set; }
        public int LessonNumber { get; set; }

        public string TextError { get; set; }
    }
}