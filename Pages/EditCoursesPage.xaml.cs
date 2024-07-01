using C971.DB;
using System.Linq;

namespace C971.Pages;

public partial class EditCoursesPage : ContentPage
{
    private Courses selectedCourse;
    private IEnumerable<Instructors> instructors;
    private Terms selectedTerm;
    private List<string> selectedInstructor;
    private bool created;
    Account originAccount;
    public EditCoursesPage(bool create, Courses course, Terms term, Account account)
	{
		InitializeComponent();
        if (create)
        {
            titlelbl.Text = "Create Course";
            saveButton.Text = "Create";
            //Reorganizing page
            ButtonGrid.SetColumn(cancelBtnBorder, 0);
            ButtonGrid.SetColumnSpan(cancelBtnBorder, 3);
            deleteButton.IsEnabled = false;
            deleteButton.IsVisible = false;
            enterButton.IsEnabled = false;
            enterButton.IsVisible = false;
            deletebtnBorder.IsEnabled = false;
            deletebtnBorder.IsVisible = false;
            enterBtnBorder.IsVisible = false;
            enterBtnBorder.IsEnabled = false;
            getInstructors(false);

        } else
        {
            getInstructors(true);
            titlelbl.Text = "Edit: " + course.CourseName;
            courseTitleEntry.Text = course.CourseName;
            startDatePicker.Date = course.StartDate;
            endDatePicker.Date = course.EndDate;
            statusChkbox.IsChecked = course.CourseStatus;
            notificationsChkbox.IsChecked = course.CourseNotifications;
        }
        selectedTerm = term;
        selectedCourse = course;
        created = create;
        originAccount = account;
        
    }
    private async void getInstructors(bool specific)
    {
        
        instructors = await Services.getInstructor(false, 0);
        
        var instructorNames = instructors.Select(i => i.InstructorName).ToList();
        instructorPicker.ItemsSource = instructorNames;
        if (specific)
        {
            selectedInstructor = instructors.Where(q => q.Id == selectedCourse.InstructorID).Select(i => i.InstructorName).ToList();
            instructorPicker.SelectedItem = selectedInstructor[0];
        }
        
    }

    private void enterButton_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new AssignmentsPage(selectedTerm, selectedCourse, originAccount));
    }

    private async void saveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(courseTitleEntry.Text))
            {
                await DisplayAlert("Missing Course Name", "Please enter a value for Course Name", "OK");
                return;
            }
            if (instructorPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Missing Instructor", "Please enter the instructor for your course", "OK");
                return;
            }
            if (startDatePicker.Date > endDatePicker.Date)
            {
                await DisplayAlert("Date Picker", "Start date is after the end date.", "OK");
                return;
            }

            //Getting instructor information one more time.
            instructors = await Services.getInstructor(false, 0);
            var selectedInstructorID = instructors.Where(q => q.InstructorName == instructorPicker.SelectedItem.ToString()).Select(i => i.Id).ToList();
            if (created)
            {
                await Services.addCourse(courseTitleEntry.Text, startDatePicker.Date, endDatePicker.Date, statusChkbox.IsChecked, notificationsChkbox.IsChecked, "", selectedTerm.Id, selectedInstructorID[0]);
            } else
            {
                await Services.updateCourse(selectedCourse.Id, courseTitleEntry.Text, startDatePicker.Date, endDatePicker.Date, statusChkbox.IsChecked, notificationsChkbox.IsChecked, selectedCourse.CourseNotes, selectedTerm.Id, selectedInstructorID[0]);
            }
            
            App.Current.MainPage = new NavigationPage(new CoursesPage(selectedTerm, originAccount));

        }
        catch (Exception ex) { await DisplayAlert("Something Happened", ex.Message, "OK"); return; }
    }

    private async void cancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
        App.Current.MainPage = new NavigationPage(new CoursesPage(selectedTerm, originAccount));
    }

    private async void deleteButton_Clicked(object sender, EventArgs e)
    {
        var answer = await DisplayAlert( "Delete this Course?", "Do you want to delete this Course and its Assignments? Course ID: " + selectedCourse.Id, "Yes", "No");
        if (answer == true)
        {

            await Services.deleteCourse(selectedCourse.Id);
            await DisplayAlert("Course Deleted", "Course Deleted", "OK");
        }
        await Navigation.PopAsync();
        App.Current.MainPage = new NavigationPage(new CoursesPage(selectedTerm, originAccount));
    }
}