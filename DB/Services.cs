using Plugin.LocalNotification;
using SQLite;

namespace C971.DB
{
    public static class Services
    {
        private static SQLiteAsyncConnection db;
        
        // THIS FUNCTION IS TO RUN ONCE. THIS ENSURES THE DATABASE IS CREATED UPON ANY OF THE BELOW FUNCTIONS. 
        static async Task Init() // Purpose is to only run this once. 
        {
            if (db != null)
            {
                Console.WriteLine("DB Already Created");
                return;
            }
                try
                {
                    Console.WriteLine("Inside of Init()");
                    //Settings.FirstRun = true;
                    var DB_Path = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");
                    //var DB_Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db");

                    db = new SQLiteAsyncConnection(DB_Path);

                    Console.WriteLine("Right Before Create Tables Async");
                    await db.CreateTablesAsync<Terms, Courses, Assignment, Instructors, Account>();
                    Console.WriteLine("Database Created SUCCESSFULLY");

                }
                catch (Exception ex) { Console.WriteLine("Database Creation FAILED:" + ex.Message); };
                return;
            
        }
        // Methods for Search Bar
        #region Searchbar Methods
        
        public static async Task<IEnumerable<Terms>> SearchTerms(int userId, string data)
        {
            await Init();

            var query = await db.Table<Terms>().Where(s => s.AccountId == userId && s.TermName.ToLower().Contains(data.ToLower())).ToListAsync();
            return query;
            
        }
        public static async Task<IEnumerable<Courses>> SearchCourses(int userId, string data)
        {
            await Init();
            //creating a list to store termIDs that are under the logged in userId
            List<int> termIDs = new List<int>();
            // getting all of the terms that contain the userId
            var query1 = await db.Table<Terms>().Where(s => s.AccountId == userId).ToListAsync();
            // Adding those terms to the new termIDs list.
            termIDs.AddRange(query1.Select(term => term.Id));

            // Fetching the courses that meet with the parameters.
            var dataquery = await db.Table<Courses>().Where(c => termIDs.Contains(c.TermId) && c.CourseName.ToLower().Contains(data.ToLower())).ToListAsync();
            return dataquery;
        }
        public static async Task<IEnumerable<Assignment>> SearchAssignments(int userId, string data)
        {
            await Init();
            //creating a list to store termIDs that are under the logged in userIds
            List<int> termIDs = new List<int>();
            // Getting all of the terms that are tied to the user.
            var termquery = await db.Table<Terms>().Where(s => s.AccountId == userId).ToListAsync();
            // This is essentiallty a foreach loop, term in Terms then adding the needed information in the list.
            termIDs.AddRange(termquery.Select(term => term.Id));

            // Doing the same thing but for the Courses now
            List<int> courseIDs = new List<int>();
            // Getting all of the courses that are tied to the terms above
            var coursequery = await db.Table<Courses>().Where(s => termIDs.Contains(s.TermId)).ToListAsync();
            courseIDs.AddRange(coursequery.Select(course => course.Id));

            var query = await db.Table<Assignment>().Where(s => courseIDs.Contains(s.CourseID) && s.AssignmentName.ToLower().Contains(data.ToLower())).ToListAsync();
            return query;
        }

        #endregion
        // Methods for Account Data
        #region Accounts Methods
        public static async Task<Account> CheckAccount(string username)
        {
            await Init();

            var query = await db.Table<Account>().Where(s => s.Username == username).FirstOrDefaultAsync();
            return query;
        } 
        public static async Task updateAccount(int id, string username, string password, DateTime creationDate, string firstname, string lastname, bool graduate, string email)
        {
            await Init();
            var accountQuery = await db.Table<Account>().Where(i => i.Id == id).FirstOrDefaultAsync();
            if (accountQuery != null)
            {
                accountQuery.Username = username;
                accountQuery.Password = password;
                accountQuery.FirstName = firstname;
                accountQuery.LastName = lastname;
                accountQuery.Graduate = graduate;
                accountQuery.Email = email;

                await db.UpdateAsync(accountQuery);
            }
        }
        public static async Task updatePassword(int id, string password)
        {
            await Init();
            var accountQuery = await db.Table<Account>().Where(i => i.Id == id).FirstOrDefaultAsync();
            if (accountQuery != null)
            {
                accountQuery.Password = password;

                await db.UpdateAsync(accountQuery);
            }
        }

        public static async Task createAccount(string username, string password, DateTime creationdate, string firstname, string lastname, bool graduate, string email)
        {
            await Init();
            try
            {
                var account = new Account()
                {
                    Username = username,
                    Password = password,
                    CreationDate = creationdate,
                    FirstName = firstname,
                    LastName = lastname,
                    Graduate = graduate,
                    Email = email
                };
                await db.InsertAsync(account);
                var aId = account.Id;
                Console.WriteLine("Account Created Successfully");
            } catch (Exception ex) { Console.WriteLine("Creation Failed: " + ex.Message); }
        }

        public static async Task DeleteAccount(int id)
        {
            await Init();

            //Getting user terms, courses, and assignments for deletion.
            IEnumerable<Terms> userTerms = await db.Table<Terms>().Where(s => s.AccountId == id).ToListAsync();
            var TermIDs = userTerms.Select(s => s.Id).ToList();

            IEnumerable<Courses> userCourses = await db.Table<Courses>().Where(s => TermIDs.Contains(s.TermId)).ToListAsync();
            var CourseIDs = userCourses.Select(s => s.Id).ToList();

            IEnumerable<Assignment> userAssignments = await db.Table<Assignment>().Where(s => CourseIDs.Contains(s.CourseID)).ToListAsync();
            var AssignmentIDs = userAssignments.Select(s => s.Id).ToList();


            // Deleting the Account and all of its following Terms, Courses, and Assignments.
            foreach (int aIds in AssignmentIDs)
            {
                await db.DeleteAsync<Assignment>(aIds);
                Console.WriteLine("Assignment ID: " + aIds + " was deleted successfully.");
            }
            foreach (int cIds in CourseIDs)
            {
                await db.DeleteAsync<Courses>(cIds);
                Console.WriteLine("Course ID: " + cIds + " was deleted successfully.");
            }
            foreach (int tIds in TermIDs)
            {
                await db.DeleteAsync<Terms>(tIds);
                Console.WriteLine("Term ID: " + tIds + " was deleted successfully.");
            }
            await db.DeleteAsync<Account>(id);
            Console.WriteLine("Account ID: " + id + " was deleted successfully.");
        }
        #endregion
        // Methods for get, add, edit, and delete Terms
        #region Terms Methods
        public static async Task<IEnumerable<Terms>> getTerms(int id, bool Specific, int accountId)
        {
            await Init();
            //await db.GetAsync
            
            if (Specific)
            {
                //var query = db.Table<Terms>().Where(s => s.TermName.StartsWith("A"));
                var query = await db.Table<Terms>().Where(s => s.Id == id).ToListAsync();
                return query;
            }
            else
            {
                var query = await db.Table<Terms>().Where(s => s.AccountId == accountId).ToListAsync();
                return query;
            }
            

            // var query = db.Table<Terms>().Where(s => s.Symbol.StartsWith("A")) // This is an example query that grabs any 
        }

        public static async Task addTerm(string termName, DateTime startDate, DateTime endDate, bool termStatus, int accountId)
        {
            await Init();
            try
            {
                var term = new Terms()
                {
                    TermName = termName,
                    StartDate = startDate,
                    EndDate = endDate,
                    TermStatus = termStatus,
                    AccountId = accountId
                };
                await db.InsertAsync(term);
                var tId = term.Id;
                Console.WriteLine("Term ID: " + tId + " created successfully.");
            } catch (Exception ex) { Console.WriteLine("Creation Failed: " + ex.Message); }
            

        }
        public static async Task updateTerm(int id, string termName, DateTime startDate, DateTime endDate, bool termStatus)
        {
            await Init();

            var termQuery = await db.Table<Terms>().Where(i => i.Id == id).FirstOrDefaultAsync();

            if (termQuery != null)
            {
                termQuery.TermName = termName;
                termQuery.StartDate = startDate;
                termQuery.EndDate = endDate;
                termQuery.TermStatus = termStatus;

                await db.UpdateAsync(termQuery);
            }

        }
        public static async Task deleteTerm(int id)
        {
            await Init();
            await db.DeleteAsync<Terms>(id);
            Console.WriteLine("Term ID: " + id + " Deletion Successful");

        }
        #endregion
        // Methods for get, add, edit, and delete Courses
        #region Course Methods
        public static async Task<IEnumerable<Courses>> getCourse(bool All, int termId, bool Specific, int courseId)
        {
            await Init();
            if (All)
            {
                var query = await db.Table<Courses>().ToListAsync();
                return query;
            }
            if(Specific)
            {
                var query = await db.Table<Courses>().Where(s => s.Id == courseId && s.TermId == termId).ToListAsync();
                return query;
            } else
            {
                var query = await db.Table<Courses>().Where(s => s.TermId == termId).ToListAsync();
                return query;
            }

        }
        public static async Task addCourse(string courseName, DateTime startDate, DateTime endDate, bool courseStatus, bool courseNotifications, string courseNotes, int termId, int instructorId)
        {
            await Init();
            try
            {
                var course = new Courses()
                {
                    CourseName = courseName,
                    StartDate = startDate,
                    EndDate = endDate,
                    CourseStatus = courseStatus,
                    CourseNotifications = courseNotifications,
                    CourseNotes = courseNotes,
                    TermId = termId,
                    InstructorID = instructorId
                };
                await db.InsertAsync(course);
                var cId = course.Id;
            } catch (Exception ex) { Console.WriteLine("Creation Failed: "+ ex.Message); }
            

        }

        public static async Task updateCourse(int id, string courseName, DateTime startDate, DateTime endDate, bool courseStatus, bool courseNotifications, string courseNotes, int termId, int instructorId)
        {
            await Init();
            var courseQuery = await db.Table<Courses>().Where(i => i.Id == id).FirstOrDefaultAsync();

            if (courseQuery != null)
            {
                courseQuery.CourseName = courseName;
                courseQuery.StartDate = startDate;
                courseQuery.EndDate = endDate;
                courseQuery.CourseStatus = courseStatus;
                courseQuery.CourseNotifications = courseNotifications;
                courseQuery.CourseNotes = courseNotes;
                courseQuery.TermId = termId;
                courseQuery.InstructorID = instructorId;


                await db.UpdateAsync(courseQuery);
            }

        }
        public static async Task deleteCourse(int id)
        {
            await Init();
            await db.DeleteAsync<Courses>(id);

        }
        #endregion
        // Methods for get, add, edit, and delete Assignments
        #region Assignment Methods
        public static async Task<IEnumerable<Assignment>> getAssignemnt(bool All, int courseId, bool specific, int assignmentId)
        {
            await Init();
            if (All)
            {
                var query = await db.Table<Assignment>().ToListAsync();
                return query;
            }
            if (specific)
            {
                var query = await db.Table<Assignment>().Where(s => s.Id == assignmentId && s.CourseID == courseId).ToListAsync();
                return query;
            } else
            {
                var query = await db.Table<Assignment>().Where(s => s.CourseID == courseId).ToListAsync();
                return query;
            }

        }
        public static async Task addAssignment(string assignmentName, string type, DateTime startDate, DateTime endDate, bool assignmentStatus, bool assignmentNotifications, int courseId, string assignmentNotes)
        {
            await Init();
            var assignment = new Assignment() 
            { 
              AssignmentName = assignmentName,
              Type = type, 
              StartDate = startDate, 
              EndDate = endDate, 
              AssignmentStatus = assignmentStatus, 
              AssignmentNotifications = assignmentNotifications,
              CourseID = courseId,
              AssignmentNotes = assignmentNotes
            };
            var aId = await db.InsertAsync(assignment);

        }

        public static async Task updateAssignment(int id, string assignmentName, string type, DateTime startDate, DateTime endDate, bool assignmentStatus, bool assignmentNotifications, int courseId, string assignmentNotes)
        {
            await Init();
            var assignQuery = await db.Table<Assignment>().Where(i => i.Id == id).FirstOrDefaultAsync();

            if (assignQuery != null)
            {
                assignQuery.AssignmentName = assignmentName;
                assignQuery.Type = type;
                assignQuery.StartDate = startDate;
                assignQuery.EndDate = endDate;
                assignQuery.AssignmentStatus = assignmentStatus;
                assignQuery.AssignmentNotifications = assignmentNotifications;
                assignQuery.CourseID = courseId;
                assignQuery.AssignmentNotes = assignmentNotes;


                await db.UpdateAsync(assignQuery);
            }

        }
        public static async Task deleteAssignment(int id)
        {
            await Init();
            await db.DeleteAsync<Assignment>(id);

        }
        #endregion
        // Methods for get, add, edit, and delete Instructors
        #region Instructor Methods
        public static async Task<IEnumerable<Instructors>> getInstructor(bool Specific, int id)
        {
            await Init();

            if (Specific)
            {
                //var query = db.Table<Terms>().Where(s => s.TermName.StartsWith("A"));
                var query = await db.Table<Instructors>().Where(s => s.Id == id).ToListAsync();
                return query;
            }
            else
            {
                var query = await db.Table<Instructors>().ToListAsync();
                return query;
            }

        }
        public static async Task addInstructor(string name, string email, string phone)
        {
            await Init();
            var instructor = new Instructors()
            {
                InstructorName = name,
                InstructorEmail = email,
                InstructorPhone = phone
            };
            var iId = await db.InsertAsync(instructor);
            Console.WriteLine("INSTRUCTOR ID: " + iId);

        }

        public static async Task updateInstructor(int id, string name, string email, string phone)
        {
            await Init();
            var InstructorQuery = await db.Table<Instructors>().Where(i => i.Id == id).FirstOrDefaultAsync();

            if (InstructorQuery != null)
            {
                InstructorQuery.InstructorName = name;
                InstructorQuery.InstructorEmail = email;
                InstructorQuery.InstructorPhone = phone;

                await db.UpdateAsync(InstructorQuery);
            }

        }
        public static async Task<bool> deleteInstructor(int id)
        {
            await Init();
            var courseQuery = await db.Table<Courses>().Where(s => s.InstructorID == id).ToListAsync();
            bool result = courseQuery.Any();

            if (result == true)
            {
                Console.WriteLine("Instructor is tied to a class");
                return false;
            }
            await db.DeleteAsync<Instructors>(id);
            return true;

        }

        #endregion
        #region Reports
        public static async Task<IEnumerable<Terms>> TotalTerms(int userId)
        {
            await Init();
            var query = await db.Table<Terms>().Where(s => s.AccountId == userId).ToListAsync();
            return query;
        }
        public static async Task<IEnumerable<Courses>> TotalCourses(int userId)
        {
            await Init();
            List<int> termIDs = new List<int>();
            var termQuery = await db.Table<Terms>().Where(s => s.AccountId == userId).ToListAsync();
            termIDs.AddRange(termQuery.Select(t => t.Id));

            var query = await db.Table<Courses>().Where(s => termIDs.Contains(s.TermId)).ToListAsync();
            return query;
        }
        public static async Task<IEnumerable<Assignment>> TotalAssignments(int userId)
        {
            await Init();
            List<int> termIDs = new List<int>();
            List<int> courseIDs = new List<int>();
            var termQuery = await db.Table<Terms>().Where(s => s.AccountId == userId).ToListAsync();
            termIDs.AddRange(termQuery.Select(t => t.Id));
            var courseQuery = await db.Table<Courses>().Where(s => termIDs.Contains(s.TermId)).ToListAsync();
            courseIDs.AddRange(courseQuery.Select(c => c.Id));

            var query = await db.Table<Assignment>().Where(a => courseIDs.Contains(a.CourseID)).ToListAsync();
            return query;
        }
        #endregion
        // Sample Data
        #region C6 Sample Data
        /*public static async void LoadC6SampleData()
        {
            await Init();
            // Term for C6
            Terms term1 = new Terms
            {
                TermName = "Term 1",
                StartDate = DateTime.Parse("05/01/24"),
                EndDate = DateTime.Parse("05/31/24"),
                TermStatus = true,
                AccountId = 1
            };
            await db.InsertAsync(term1);
            // Instructor for C6
            Instructors instructor = new Instructors
            {
                InstructorName = "Anika Patel",
                InstructorPhone = "anika.patel@strimeuniversity.edu",
                InstructorEmail = "555-123-4567"
            };
            await db.InsertAsync(instructor);
            Courses course = new Courses()
            {
                CourseName = "Course 1",
                StartDate = DateTime.Parse("05/01/2024"), 
                EndDate = DateTime.Parse("05/31/2024"), 
                CourseStatus = true,
                CourseNotifications = true,
                CourseNotes = "These are some default notes for Course 1. Share me by clicking the share button above!!!",
                TermId = term1.Id,
                InstructorID = instructor.Id
            };
            await db.InsertAsync(course);
            Assignment assignment1 = new Assignment()
            {
                AssignmentName = "Assignment 1 Project",
                Type = "Objective Assessment",
                StartDate = DateTime.Parse("05/01/2024"), 
                EndDate = DateTime.Parse("05/15/2024"), 
                AssignmentStatus = true,
                AssignmentNotifications = true,
                CourseID = course.Id,
                AssignmentNotes = "These are some notes for Assignment 1. Share me by clicking the share button above!!!"
            };
            await db.InsertAsync(assignment1);
            Assignment assignment2 = new Assignment()
            {
                AssignmentName = "Assignment 2 Exam",
                Type = "Performance Assessment",
                StartDate = DateTime.Parse("05/15/2024"),
                EndDate = DateTime.Parse("05/31/2024"),
                AssignmentStatus = true,
                AssignmentNotifications = true,
                CourseID = course.Id,
                AssignmentNotes = "These are some notes for Assignment 2. Share me by clicking the share button above!!!"
            };
            await db.InsertAsync(assignment2);
        }*/

        public static async Task ClearSampleData()
        {
            await Init();

            await db.DropTableAsync<Account>();
            await db.DropTableAsync<Terms>();
            await db.DropTableAsync<Courses>();
            await db.DropTableAsync<Assignment>();
            await db.DropTableAsync<Instructors>();
            db = null;
            Console.WriteLine("Sample Data Cleared");
        }
        #endregion
    }

    public static class Settings
    {
        public static bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
        }
    }

    public class Notifications
    {
        public static async Task ShareText(string Text)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = Text,
                Title = "Share Text"
            });
        }
        public static async void CheckAndRequestLocalPermission()
        {
            bool check = await LocalNotificationCenter.Current.AreNotificationsEnabled();
            if (check == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

        }


    }
}
