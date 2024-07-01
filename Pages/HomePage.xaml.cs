using C971.DB;
using Microsoft.Maui.Controls.Compatibility;
using Plugin.LocalNotification;

namespace C971.Pages;

public partial class HomePage : ContentPage
{
    Terms? blankTerm;
    Terms termSelected;
    private int selectedID = -1;
    Account originAccount;
    public HomePage(Account account)
	{
		InitializeComponent();
        Refresh_Notifications();
        originAccount = account;
        searchbar.Text = " ";
        searchbar.Text = "";
        searchbar.Text = "t";
        searchbar.Text = "";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        refresh_terms();
        
    }
    private async void Check_Notifications(IEnumerable<Courses> courselist, IEnumerable<Assignment> assignmentlist)
    {
        Notifications.CheckAndRequestLocalPermission();

        foreach (Courses courses in courselist)
        {
            if (courses.CourseNotifications == true)
            {
                if (courses.StartDate == DateTime.Today)
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = 1000,
                        Title = "Course Starting",
                        Subtitle = $"{courses.CourseName} starting",
                        Description = $"{courses.CourseName} begins today!! Lets get started!",
                        BadgeNumber = 10,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now.AddSeconds(5),
                            NotifyRepeatInterval = TimeSpan.FromDays(1)
                        }
                    };
                    Console.WriteLine("Triggered: Course Begins Today!");
                    await LocalNotificationCenter.Current.Show(request);
                    Console.WriteLine("Notification Sent: Course begins Today!");
                    //CrossLocalNotifications.Current.Show("Notice", $"{courses.CourseName} begins today!", notifyId); // Code from Harlan Brewer
                }
                if (courses.EndDate == DateTime.Today)
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = 1000,
                        Title = "Course Ending",
                        Subtitle = $"{courses.CourseName} Ending",
                        Description = $"{courses.CourseName} ends today!! Get it done!",
                        BadgeNumber = 10,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now.AddSeconds(5),
                            NotifyRepeatInterval = TimeSpan.FromDays(1)
                        }
                    };
                    Console.WriteLine("Triggered: Course ends Today!");
                    await LocalNotificationCenter.Current.Show(request);
                    Console.WriteLine("Notification Sent: Course ends Today!");
                }
            }
        }
        foreach (Assignment assignment in assignmentlist)
        {
            if (assignment.AssignmentNotifications == true)
            {
                if (assignment.StartDate == DateTime.Today)
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = 1000,
                        Title = "Assignment Active",
                        Subtitle = $"{assignment.AssignmentName} is open.",
                        Description = $"{assignment.AssignmentName} starts today!! Lets get started!",
                        BadgeNumber = 10,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now.AddSeconds(5),
                            NotifyRepeatInterval = TimeSpan.FromDays(1)
                        }
                    };
                    Console.WriteLine("Triggered: Assignment starts Today!");
                    await LocalNotificationCenter.Current.Show(request);
                    Console.WriteLine("Notification Sent: Assignment starts Today!");
                }
                if (assignment.EndDate == DateTime.Today)
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = 1000,
                        Title = "Assignment Due",
                        Subtitle = $"{assignment.AssignmentName} is due!",
                        Description = $"{assignment.AssignmentName} is due today!! Get it done!",
                        BadgeNumber = 10,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now.AddSeconds(5),
                            NotifyRepeatInterval = TimeSpan.FromDays(1)
                        }
                    };
                    Console.WriteLine("Triggered: Assignment due Today!");
                    await LocalNotificationCenter.Current.Show(request);
                    Console.WriteLine("Notification Sent: Assignment ends Today!");
                }
            }
        }

    }
    private async void refresh_terms()
    {
        TermsCollectionView.ItemsSource = await Services.getTerms(0, false, originAccount.Id);
    }
    public async void Refresh_Notifications()
    {
        var courselist = await Services.getCourse(true, 0, false, 0);
        var assignmentlist = await Services.getAssignemnt(true, 0, false, 0);
        Check_Notifications(courselist, assignmentlist);
    }
    private void editTerm_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            App.Current.MainPage = new NavigationPage(new TermsPage(false, termSelected, originAccount));
            termNotSelectedlbl.Text = "";
            return;
        }
        termNotSelectedlbl.Text = "Term is not selected. Please select a term you would like to edit.";
    }

    private async void deleteTerm_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            var answer = await DisplayAlert("Delete this Term?", "Do you want to delete this Term and its related Courses and Assignments? Terms ID: " + selectedID, "Yes", "No");
            if (answer == true)
            {

                await Services.deleteTerm(selectedID);
                refresh_terms();
                await DisplayAlert("Term Deleted", "Term Deleted", "OK");
                selectedID = -1;
                termSelected = new Terms();
                termNotSelectedlbl.Text = "";

            }
            await Navigation.PopAsync();
            return;
        }
        termNotSelectedlbl.Text = "Term is not selected. Please select a term you would like to delete.";

    }

    private void enterTerm_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            App.Current.MainPage = new NavigationPage(new CoursesPage(termSelected, originAccount));
            termNotSelectedlbl.Text = "";
            return;
        }
        termNotSelectedlbl.Text = "Term is not selected. Please select a term you would like to enter.";
    }

    private void CreateBtn_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new TermsPage(true, blankTerm, originAccount));
    }

    private void TermsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var term = (Terms)e.CurrentSelection.FirstOrDefault();
        if (e.CurrentSelection != null)
        {
            
            selectedID = term.Id;
            termSelected = term;
            //  App.Current.MainPage = new NavigationPage(new CoursesPage(false, term)); // This will be CoursePage when created. 
        }
    }
    private void settings_Clicked(object sender, EventArgs e) // This is for the settings app
    {
        App.Current.MainPage = new NavigationPage(new SettingsPage(originAccount));
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new LoginPage());
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;

        if (string.IsNullOrWhiteSpace(searchBar.Text) == false)
        {
            var terms = await Services.SearchTerms(originAccount.Id, searchBar.Text);
            var courses = await Services.SearchCourses(originAccount.Id, searchBar.Text);
            var assignments = await Services.SearchAssignments(originAccount.Id, searchBar.Text);
            Console.WriteLine("DEBUG: Any terms found? " + terms.Any());
            Console.WriteLine("DEBUG: Any courses found? " + courses.Any());
            Console.WriteLine("DEBUG: Any assignments found? " + assignments.Any());
            
            
            
            if (terms.Any() == true)
            {
                
                SearchResultsTerms.ItemsSource = terms;
                SearchResultsTerms.IsVisible = true;
                termNotSelectedlbl.Text = "";
            } else if (terms.Any() == false) 
            {
                SearchResultsTerms.ItemsSource = null;
                SearchResultsTerms.IsVisible = false;
            }
            if (courses.Any() == true)
            {
                SearchResultsCourses.IsVisible = true;
                SearchResultsCourses.ItemsSource = courses;
                termNotSelectedlbl.Text = "";
            } else if (courses.Any() == false) 
            {
                SearchResultsCourses.ItemsSource = null;
                SearchResultsCourses.IsVisible = false;
            }
            if (assignments.Any() == true)
            {
                SearchResultsAssignments.ItemsSource = assignments;
                SearchResultsAssignments.IsVisible = true;
                termNotSelectedlbl.Text = "";
            } else if (assignments.Any() == false)
            {
                SearchResultsAssignments.ItemsSource = null;
                SearchResultsAssignments.IsVisible = false;
            }
            if (terms.Any() == false && courses.Any() == false && assignments.Any() == false)
            {
                termNotSelectedlbl.Text = "There are no items found.";
                SearchResultsTerms.ItemsSource = null;
                SearchResultsCourses.ItemsSource = null;
                SearchResultsAssignments.ItemsSource = null;

                SearchResultsTerms.IsVisible = false;
                SearchResultsCourses.IsVisible = false;
                SearchResultsAssignments.IsVisible = false;
            }
            if(searchBar.Text == "")
            {
                SearchResultsTerms.ItemsSource = null;
                SearchResultsCourses.ItemsSource = null;
                SearchResultsAssignments.ItemsSource = null;

                SearchResultsTerms.IsVisible = false;
                SearchResultsCourses.IsVisible = false;
                SearchResultsAssignments.IsVisible = false;
            }

        } else
        {
            SearchResultsTerms.ItemsSource = null;
            SearchResultsCourses.ItemsSource = null;
            SearchResultsAssignments.ItemsSource = null;

            SearchResultsTerms.IsVisible = false;
            SearchResultsCourses.IsVisible = false;
            SearchResultsAssignments.IsVisible = false;
            return;
        }
    }

    private void SearchResultsTerms_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var selectedTerm = (Terms)e.SelectedItem;
        if (SearchResultsTerms.SelectedItem != null)
        {
            App.Current.MainPage = new NavigationPage(new TermsPage(false, selectedTerm, originAccount));
        }
    }

    private async void SearchResultsCourses_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var selectedCourse = (Courses)e.SelectedItem;
        var termsList = await Services.getTerms(selectedCourse.TermId, true, originAccount.Id); // CONVERT THIS TO A SINGLE OBJECT.
        var selectedTerm = termsList.Where(s => s.Id == selectedCourse.TermId).FirstOrDefault();
        if (SearchResultsCourses.SelectedItem != null)
        {
            App.Current.MainPage = new NavigationPage(new EditCoursesPage(false, selectedCourse, selectedTerm, originAccount));
        }
    }

    private async void SearchResultsAssignments_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var selectedAssignment = (Assignment)e.SelectedItem;
        // Getting lists of courses and terms
        var termsList = await Services.getTerms(0, false, originAccount.Id);
        var courseList = await Services.getCourse(true, 0, false, 0);
        // Making selected Term and Course
        var selectedCourse = courseList.Where(s => s.Id == selectedAssignment.CourseID).FirstOrDefault();
        var selectedTerm = termsList.Where(s => s.Id == selectedCourse.TermId).FirstOrDefault();

        if(SearchResultsAssignments.SelectedItem != null)
        {
            App.Current.MainPage = new NavigationPage(new EditAssignmentPage(selectedTerm, selectedCourse, false, selectedAssignment, originAccount));
        }

    }
}