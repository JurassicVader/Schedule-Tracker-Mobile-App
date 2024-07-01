using C971.DB;

namespace C971.Pages;

public partial class EditAssignmentPage : ContentPage
{
	private Terms term;
	private Courses course;
	private Assignment selectedAssignment;
	private bool created;
    private string type;
    Account originAccount;
    private Assignment otherAssessment = new Assignment
    {
        Id = -1,
        AssignmentName = "Blank",
        Type = "Nothing",
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddDays(5),
        AssignmentStatus = false,
        AssignmentNotifications = false,
        AssignmentNotes = "Blank Notes",
        CourseID = 0
    };
	public EditAssignmentPage(Terms originTerm, Courses originCourse, bool create, Assignment assignment, Account account)
	{
		InitializeComponent();
        if (create)
		{
			titlelbl.Text = "Create Assignment";
			saveButton.Text = "Create";
            //Reorganizing the page
            ButtonGrid.SetColumn(cancelBtnBorder, 0);
            ButtonGrid.SetColumnSpan(cancelBtnBorder, 3);
            deleteButton.IsEnabled = false;
            deleteButton.IsVisible = false;
            notesBtn.IsEnabled = false;
            notesBtn.IsVisible = false;
            notesBtnBorder.IsEnabled = false;
            notesBtnBorder.IsVisible = false;
            deletebtnBorder.IsVisible = false;
            deletebtnBorder.IsEnabled = false;
        } else
		{
            
            titlelbl.Text = "Edit/View: " + assignment.AssignmentName;
			assignmentTitleEntry.Text = assignment.AssignmentName;
			startDatePicker.Date = assignment.StartDate;
			endDatePicker.Date = assignment.EndDate;
			statusChkbox.IsChecked = assignment.AssignmentStatus;
			notificationsChkbox.IsChecked = assignment.AssignmentNotifications;
            if (assignment.Type == "Performance Assessment")
            {
                pAssessmentRdio.IsChecked = true;
            }
            else
            {
                oAssessmentRdio.IsChecked = true;
            }
        }
        course = originCourse;
        term = originTerm;
		selectedAssignment = assignment;
		created = create;
        originAccount = account;
    }

    private async void saveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assignmentTitleEntry.Text))
            {
                await DisplayAlert("Missing Course Name", "Please enter a value for Course Name", "OK");
                return;
            }
            if (pAssessmentRdio.IsChecked == false && oAssessmentRdio.IsChecked == false)
            {
                await DisplayAlert("Missing Assessment Type", "Please select the type of assignment", "OK");
                return;
            }
            if (startDatePicker.Date > endDatePicker.Date)
            {
                await DisplayAlert("Date Picker", "Start date is after the end date.", "OK");
                return;
            }

            // Checking if the other assignment is a Performance Assessment
            if (pAssessmentRdio.IsChecked)
            {
                type = "Performance Assessment";
            }
            else
            {
                type = "Objective Assessment";
            }

            var assessments = await Services.getAssignemnt(false, course.Id, false, 0);
            if (assessments == null)
            {
                Console.WriteLine("No Assessments");
            } else
            {
                foreach (var assessment in assessments)
                {
                    if (created)
                    {
                        otherAssessment = assessment;
                        break;
                    }
                    if (assessment.Id != selectedAssignment.Id)
                    {
                        otherAssessment = assessment;
                        break;
                    }
                }
            }
            if (otherAssessment.Id != -1 && otherAssessment.Type == type)
            {
                await DisplayAlert("Assessment Type", "You already have an assessment with type: " + type, "OK");
                return;
            }

            if (created)
            {
                await Services.addAssignment(assignmentTitleEntry.Text, type, startDatePicker.Date, endDatePicker.Date, statusChkbox.IsChecked, notificationsChkbox.IsChecked, course.Id, "");
            }
            else
            {
                await Services.updateAssignment(selectedAssignment.Id, assignmentTitleEntry.Text, type, startDatePicker.Date, endDatePicker.Date, statusChkbox.IsChecked, notificationsChkbox.IsChecked, course.Id, selectedAssignment.AssignmentNotes);
            }

            App.Current.MainPage = new NavigationPage(new AssignmentsPage(term, course, originAccount));

        }
        catch (Exception ex) { await DisplayAlert("Something Happened", ex.Message, "OK"); return; }
    }

    private async void cancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
        App.Current.MainPage = new NavigationPage(new AssignmentsPage(term, course, originAccount));
    }

    private async void deleteButton_Clicked(object sender, EventArgs e)
    {
        var answer = await DisplayAlert("Delete this Assignment?", "Do you want to delete this Assignment? Course ID: " + selectedAssignment.Id, "Yes", "No");
        if (answer == true)
        {
            await Services.deleteAssignment(selectedAssignment.Id);
            await DisplayAlert("Assignment Deleted", "Assignment Deleted", "OK");
        }
        await Navigation.PopAsync();
        App.Current.MainPage = new NavigationPage(new AssignmentsPage(term, course, originAccount));
    }

    private void notesBtn_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new AssignmentNotesPage(term, course, selectedAssignment, originAccount));
    }
}