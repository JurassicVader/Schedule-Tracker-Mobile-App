using C971.DB;

namespace C971.Pages;

public partial class CourseNotesPage : ContentPage
{
	private Courses selectedCourse;
    private Terms selectedTerm;
    public static string courseNotes;
    Account originAccount;
    public CourseNotesPage(Courses course, Terms term, Account account)
	{
		InitializeComponent();

		// Editing up the Notes Page
		titlelbl.Text = "Course Notes: " + course.CourseName;
        courseNotesEditor.Text = course.CourseNotes;
		selectedCourse = course;
        selectedTerm = term;
        originAccount = account;
        populateInstructor();
    }

	private async void populateInstructor()
	{
        var instructors = await Services.getInstructor(true, selectedCourse.InstructorID);
        InstructorName.Text = "Instructor Name: " + instructors.Select(i => i.InstructorName).ToList()[0].ToString();
        InstructorPhone.Text = "Instructor Phone #: " + instructors.Select(i => i.InstructorPhone).ToList()[0].ToString();
        InstructorEmail.Text = "Instructor Email: " + instructors.Select(i => i.InstructorEmail).ToList()[0].ToString();
    }

    private async void cancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
        App.Current.MainPage = new NavigationPage(new CoursesPage(selectedTerm, originAccount));
    }

    private async void saveButton_Clicked(object sender, EventArgs e)
    {

        await Services.updateCourse(selectedCourse.Id, selectedCourse.CourseName, selectedCourse.StartDate, selectedCourse.EndDate, selectedCourse.CourseStatus, selectedCourse.CourseNotifications, courseNotesEditor.Text, selectedTerm.Id, selectedCourse.InstructorID);
        App.Current.MainPage = new NavigationPage(new CoursesPage(selectedTerm, originAccount));
    }

    private void enterButton_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new AssignmentsPage(selectedTerm, selectedCourse, originAccount)); //Enter Assignment
    }

    private async void ShareBtn_Clicked(object sender, EventArgs e)
    {
        if (courseNotesEditor.Text == selectedCourse.CourseNotes)
        {
            await Notifications.ShareText(selectedCourse.CourseNotes);
            return;
        } else
        {
            var answer = await DisplayAlert("Save Changes?", "Your notes are not saved. Would you like to save them before sharing?", "Yes", "No");
            if (answer == true)
            {
                await Services.updateCourse(selectedCourse.Id, selectedCourse.CourseName, selectedCourse.StartDate, selectedCourse.EndDate, selectedCourse.CourseStatus, selectedCourse.CourseNotifications, courseNotesEditor.Text, selectedTerm.Id, selectedCourse.InstructorID);
                await DisplayAlert("Notes Saved", "Notes have been saved", "OK");
                await Notifications.ShareText(courseNotesEditor.Text);
                return;
            }
            await Notifications.ShareText(selectedCourse.CourseNotes);
            return;
        }
        
    }
}