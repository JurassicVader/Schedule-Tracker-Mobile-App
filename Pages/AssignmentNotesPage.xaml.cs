using C971.DB;

namespace C971.Pages;

public partial class AssignmentNotesPage : ContentPage
{
	Terms term;
	Courses course;
	Assignment selectedAssignment;
    Account originAccount;
    public AssignmentNotesPage(Terms originTerm, Courses originCourse, Assignment assignment, Account account)
	{
		InitializeComponent();
		term = originTerm;
		course = originCourse;
		selectedAssignment = assignment;

		// Editing up the Notes Page
		titlelbl.Text = "Assignment Notes: " + assignment.AssignmentName;
		assignmentNotesEditor.Text = assignment.AssignmentNotes;
        originAccount = account;
	}

    private async void saveButton_Clicked(object sender, EventArgs e)
    {
        await Services.updateAssignment(selectedAssignment.Id, selectedAssignment.AssignmentName, selectedAssignment.Type, selectedAssignment.StartDate, selectedAssignment.EndDate, selectedAssignment.AssignmentStatus, selectedAssignment.AssignmentNotifications, course.Id, assignmentNotesEditor.Text);
        App.Current.MainPage = new NavigationPage(new AssignmentsPage(term, course, originAccount));
    }

    private async void cancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
        App.Current.MainPage = new NavigationPage(new AssignmentsPage(term, course, originAccount));
    }

    private async void ShareBtn_Clicked(object sender, EventArgs e)
    {
        if (assignmentNotesEditor.Text == selectedAssignment.AssignmentNotes)
        {
            await Notifications.ShareText(selectedAssignment.AssignmentNotes);
            return;
        }
        else
        {
            var answer = await DisplayAlert("Save Changes?", "Your notes are not saved. Would you like to save them before sharing?", "Yes", "No");
            if (answer == true)
            {
                await Services.updateAssignment(selectedAssignment.Id, selectedAssignment.AssignmentName, selectedAssignment.Type, selectedAssignment.StartDate, selectedAssignment.EndDate, selectedAssignment.AssignmentStatus, selectedAssignment.AssignmentNotifications, course.Id, assignmentNotesEditor.Text);
                await DisplayAlert("Notes Saved", "Notes have been saved", "OK");
                await Notifications.ShareText(assignmentNotesEditor.Text);
                return;
            }
            await Notifications.ShareText(selectedAssignment.AssignmentNotes);
            return;
        }
    }
}