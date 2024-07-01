using C971.DB;

namespace C971.Pages;

public partial class AssignmentsPage : ContentPage
{
	Terms term; // Original Term that we are in
	Courses course; // original Course that we are in
    private int selectedID = -1; // This isn't really neccessary. I just have it anyway.
    Assignment selectedAssignment; // Assignment that the user has highlighted
    Assignment? blankAssignment; // A blank assignment so if the user clicks, create assignment, I can pass in this false assignment.
    Account originAccount;
    public AssignmentsPage(Terms originTerm, Courses originCourse, Account account)
	{
		InitializeComponent();

		term = originTerm;
		course = originCourse;
        originAccount = account;
	}

    protected override void OnAppearing()
    {
        Refresh_Assignments();
    }

    private async void Refresh_Assignments()
    {
        AssignmentCollectionView.ItemsSource = await Services.getAssignemnt(false, course.Id, false, 0);
    }

    private async void CreateBtn_Clicked(object sender, EventArgs e)
    {
        var assignments = await Services.getAssignemnt(false, course.Id, false, 0);
        int count = assignments.Count();
        if (count >= 2)
        {
            assignmentNotSelectedlbl.Text = "You have already created the max number of assignments. You are limited to one of each type of assignment per course.";
            return;
        } else
        {
            App.Current.MainPage = new NavigationPage(new EditAssignmentPage(term, course, true, blankAssignment, originAccount));
            assignmentNotSelectedlbl.Text = "";
            return;
        }
        
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
        App.Current.MainPage = new NavigationPage(new CoursesPage(term, originAccount));
    }

    private void editAssignment_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            App.Current.MainPage = new NavigationPage(new EditAssignmentPage(term, course, false, selectedAssignment, originAccount));
            assignmentNotSelectedlbl.Text = "";
            return;
        }
        assignmentNotSelectedlbl.Text = "Assignment is not selected. Please select an assignment to edit.";
    }

    private async void deleteAssignment_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            var answer = await DisplayAlert("Delete this Assignment?", "Do you want to delete this Assignment? Assignment ID:" + selectedID, "Yes", "No");
            if (answer == true)
            {

                await Services.deleteAssignment(selectedID);
                Refresh_Assignments();
                await DisplayAlert("Assignment Deleted", "Assignment Deleted", "OK");
                assignmentNotSelectedlbl.Text = "";
            }
            await Navigation.PopAsync();
            return;
        }
        assignmentNotSelectedlbl.Text = "Assignment is not selected. Please select an assignment that you would like to delete.";
    }

    private void AssignmentCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var Assignment = (Assignment)e.CurrentSelection.FirstOrDefault();
        if ( e.CurrentSelection != null)
        {
            selectedID = Assignment.Id;
            selectedAssignment = Assignment;
        }
    }

    private void notes_Clicked(object sender, EventArgs e)
    {
        if (selectedID != -1)
        {
            App.Current.MainPage = new NavigationPage(new AssignmentNotesPage(term, course, selectedAssignment, originAccount));
            assignmentNotSelectedlbl.Text = "";
            return;
        }
        assignmentNotSelectedlbl.Text = "Assignment is not selected. Please select an assignment to view/edit notes for.";
    }
}