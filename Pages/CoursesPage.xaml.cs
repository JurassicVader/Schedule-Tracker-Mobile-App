using C971.DB;

namespace C971.Pages;

public partial class CoursesPage : ContentPage
{
    Terms selectedTerm;
    private int selectedID = -1;
    Courses selectedCourse;
    Courses? blankCourse;
    Account originAccount;

    public CoursesPage(Terms term, Account account)
    {
        InitializeComponent();
        selectedTerm = term;
        originAccount = account;
    }

    protected override void OnAppearing()
    {
        Refresh_Courses();
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        try
        {
            App.Current.MainPage = new NavigationPage(new HomePage(originAccount));
        } catch (Exception ex)
        {
            Console.WriteLine("Something Happened... Error: " + ex.Message);
        }
        
    }

    private void editCourse_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            App.Current.MainPage = new NavigationPage(new EditCoursesPage(false, selectedCourse, selectedTerm, originAccount));
            courseNotSelectedlbl.Text = "";
            return;
        }
        courseNotSelectedlbl.Text = "Course is not selected. Please select a course that you would like to edit.";
    }

    private async void deleteCourse_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            var answer = await DisplayAlert("Delete this Course?", "Do you want to delete this Course and its related Assignments? Course ID:" + selectedID ,"Yes", "No");
            if (answer == true)
            {

                await Services.deleteCourse(selectedID);
                Refresh_Courses();
                await DisplayAlert("Course Deleted", "Course Deleted", "OK");
                courseNotSelectedlbl.Text = "";
            }
            await Navigation.PopAsync();
            return;
        }
        courseNotSelectedlbl.Text = "Course is not selected. Please select a course that you would like to delete.";
    }

    private void enterCourse_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            App.Current.MainPage = new NavigationPage(new AssignmentsPage(selectedTerm, selectedCourse, originAccount));
            courseNotSelectedlbl.Text = "";
        }
        courseNotSelectedlbl.Text = "Course is not selected. Please select a course that you would like to enter.";

    }

    private void CoursesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var Course = (Courses)e.CurrentSelection.FirstOrDefault();
        if (e.CurrentSelection != null)
        {
            selectedID = Course.Id;
            selectedCourse = Course;
            Console.WriteLine("INSTRUCTOR ID for this COURSE: " + selectedCourse.InstructorID.ToString());
        }
    }

    private async void CreateBtn_Clicked(object sender, EventArgs e)
    {
        var courses = await Services.getCourse(false, selectedTerm.Id, false, 0);
        int count = courses.Count();
        if (count >= 6)
        {
            courseNotSelectedlbl.Text = "You have already created the max number of courses. You are limited to six courses per Term.";
            return;
        } else
        {
            App.Current.MainPage = new NavigationPage(new EditCoursesPage(true, blankCourse, selectedTerm, originAccount));
            courseNotSelectedlbl.Text = "";
        }
        

    }

    private void courseNotes_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            App.Current.MainPage = new NavigationPage(new CourseNotesPage(selectedCourse, selectedTerm, originAccount));
            courseNotSelectedlbl.Text = "";
            return;
        }
        courseNotSelectedlbl.Text = "Course is not selected. Please select a course that you would like to edit/view notes for.";
    }

    private async void Refresh_Courses()
    {
        CoursesCollectionView.ItemsSource = await Services.getCourse(false, selectedTerm.Id, false, 0);
    }

    private void CreateInstructorBtn_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new AddInstructorPage(selectedTerm, originAccount));
    }
}