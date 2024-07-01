using SQLite;
using System.ComponentModel.DataAnnotations.Schema;


namespace C971.DB
{
    #region Accounts
    public class Account
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public DateTime CreationDate { get; set; }
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public bool Graduate { get; set; }
        public string Email { get; set; } = String.Empty;
    }
    #endregion
    #region Terms
    public class Terms
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TermName {  get; set; } = String.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool TermStatus { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }
    }
    #endregion
    #region Courses
    public class Courses
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string CourseName { get;set; } = String.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool CourseStatus { get; set; }
        public bool CourseNotifications { get; set; }
        public string CourseNotes { get; set; } = String.Empty;

        // Foreign Keys
        [ForeignKey(nameof(Terms))]
        public int TermId { get; set; }
        [ForeignKey(nameof(Instructors))]
        public int InstructorID { get; set; }

        /*[ForeignKey(nameof(Accounts))]
        public int AccountId { get; set; }*/

    }
    #endregion
    #region Assignment
    public class Assignment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string AssignmentName { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AssignmentStatus { get; set; }
        public bool AssignmentNotifications { get; set; }
        public string AssignmentNotes { get; set; } = String.Empty;

        // Foregin Keys
        [ForeignKey(nameof(Courses))]
        public int CourseID { get; set; }
        /*[ForeignKey(nameof(Accounts))]
        public int AccountId { get; set; }*/
    }
    #endregion
    #region Instructors
    // This is so that you will not have to recreate instructors. 
    public class Instructors
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set;}
        public string InstructorName { get; set;} = String.Empty;
        public string InstructorEmail { get; set;} = String.Empty;
        public string InstructorPhone { get; set;} = String.Empty;
    }
    #endregion
}
