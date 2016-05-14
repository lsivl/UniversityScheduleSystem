using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using University.Domain.Models.Mapping;

namespace University.Domain.Models
{
    public partial class UniversityContext : DbContext
    {
        static UniversityContext()
        {
            Database.SetInitializer<UniversityContext>(null);
        }

        public UniversityContext()
            : base("Name=UniversityContext")
        {
        }

        public DbSet<aspnet_Applications> aspnet_Applications { get; set; }
        public DbSet<aspnet_Membership> aspnet_Membership { get; set; }
        public DbSet<aspnet_Paths> aspnet_Paths { get; set; }
        public DbSet<aspnet_PersonalizationAllUsers> aspnet_PersonalizationAllUsers { get; set; }
        public DbSet<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }
        public DbSet<aspnet_Profile> aspnet_Profile { get; set; }
        public DbSet<aspnet_Roles> aspnet_Roles { get; set; }
        public DbSet<aspnet_SchemaVersions> aspnet_SchemaVersions { get; set; }
        public DbSet<aspnet_Users> aspnet_Users { get; set; }
        public DbSet<aspnet_WebEvent_Events> aspnet_WebEvent_Events { get; set; }
        public DbSet<Cathedra> Cathedras { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<ClassroomType> ClassroomTypes { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<LessonEvent> LessonEvents { get; set; }
        public DbSet<LessonTime> LessonTimes { get; set; }
        public DbSet<LessonType> LessonTypes { get; set; }
        public DbSet<Stream> Streams { get; set; }
        public DbSet<StreamID> StreamIDs { get; set; }
        public DbSet<StreamSubjectBridge> StreamSubjectBridges { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectClassroomBridge> SubjectClassroomBridges { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeacherSubjectBridge> TeacherSubjectBridges { get; set; }
        public DbSet<Time> Times { get; set; }
        public DbSet<UnitedGroup> UnitedGroups { get; set; }
        public DbSet<vIDLesoonEvent> vIDLesoonEvents { get; set; }
        public DbSet<vStreamID> vStreamIDs { get; set; }
        public DbSet<vTimeID> vTimeIDs { get; set; }
        public DbSet<vw_aspnet_Applications> vw_aspnet_Applications { get; set; }
        public DbSet<vw_aspnet_MembershipUsers> vw_aspnet_MembershipUsers { get; set; }
        public DbSet<vw_aspnet_Profiles> vw_aspnet_Profiles { get; set; }
        public DbSet<vw_aspnet_Roles> vw_aspnet_Roles { get; set; }
        public DbSet<vw_aspnet_Users> vw_aspnet_Users { get; set; }
        public DbSet<vw_aspnet_UsersInRoles> vw_aspnet_UsersInRoles { get; set; }
        public DbSet<vw_aspnet_WebPartState_Paths> vw_aspnet_WebPartState_Paths { get; set; }
        public DbSet<vw_aspnet_WebPartState_Shared> vw_aspnet_WebPartState_Shared { get; set; }
        public DbSet<vw_aspnet_WebPartState_User> vw_aspnet_WebPartState_User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new aspnet_ApplicationsMap());
            modelBuilder.Configurations.Add(new aspnet_MembershipMap());
            modelBuilder.Configurations.Add(new aspnet_PathsMap());
            modelBuilder.Configurations.Add(new aspnet_PersonalizationAllUsersMap());
            modelBuilder.Configurations.Add(new aspnet_PersonalizationPerUserMap());
            modelBuilder.Configurations.Add(new aspnet_ProfileMap());
            modelBuilder.Configurations.Add(new aspnet_RolesMap());
            modelBuilder.Configurations.Add(new aspnet_SchemaVersionsMap());
            modelBuilder.Configurations.Add(new aspnet_UsersMap());
            modelBuilder.Configurations.Add(new aspnet_WebEvent_EventsMap());
            modelBuilder.Configurations.Add(new CathedraMap());
            modelBuilder.Configurations.Add(new ClassroomMap());
            modelBuilder.Configurations.Add(new ClassroomTypeMap());
            modelBuilder.Configurations.Add(new FacultyMap());
            modelBuilder.Configurations.Add(new GroupMap());
            modelBuilder.Configurations.Add(new LessonEventMap());
            modelBuilder.Configurations.Add(new LessonTimeMap());
            modelBuilder.Configurations.Add(new LessonTypeMap());
            modelBuilder.Configurations.Add(new StreamMap());
            modelBuilder.Configurations.Add(new StreamIDMap());
            modelBuilder.Configurations.Add(new StreamSubjectBridgeMap());
            modelBuilder.Configurations.Add(new SubjectMap());
            modelBuilder.Configurations.Add(new SubjectClassroomBridgeMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new TeacherMap());
            modelBuilder.Configurations.Add(new TeacherSubjectBridgeMap());
            modelBuilder.Configurations.Add(new TimeMap());
            modelBuilder.Configurations.Add(new UnitedGroupMap());
            modelBuilder.Configurations.Add(new vIDLesoonEventMap());
            modelBuilder.Configurations.Add(new vStreamIDMap());
            modelBuilder.Configurations.Add(new vTimeIDMap());
            modelBuilder.Configurations.Add(new vw_aspnet_ApplicationsMap());
            modelBuilder.Configurations.Add(new vw_aspnet_MembershipUsersMap());
            modelBuilder.Configurations.Add(new vw_aspnet_ProfilesMap());
            modelBuilder.Configurations.Add(new vw_aspnet_RolesMap());
            modelBuilder.Configurations.Add(new vw_aspnet_UsersMap());
            modelBuilder.Configurations.Add(new vw_aspnet_UsersInRolesMap());
            modelBuilder.Configurations.Add(new vw_aspnet_WebPartState_PathsMap());
            modelBuilder.Configurations.Add(new vw_aspnet_WebPartState_SharedMap());
            modelBuilder.Configurations.Add(new vw_aspnet_WebPartState_UserMap());
        }
    }
}
