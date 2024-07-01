using C971.DB;

namespace C971.Pages;

public partial class AddInstructorPage : ContentPage
{
    private IEnumerable<Instructors> instructors;
    private Instructors selectedInstructor;
    Terms term;
    Account originAccount;
    public AddInstructorPage(Terms originTerm, Account Account)
	{
		InitializeComponent();
        getInstructors();
        deleteButton.IsEnabled = false;
        deleteButton.IsVisible = false;
        deleteBtnBorder.IsEnabled = false;
        deleteBtnBorder.IsVisible = false;
        ButtonGrid.SetColumn(cancelbtnBorder, 0);
        ButtonGrid.SetColumnSpan(cancelbtnBorder, 3);
        term = originTerm;
        originAccount = Account;

	}

    private async void getInstructors()
    {

        instructors = await Services.getInstructor(false, 0);

        var instructorNames = instructors.Select(i => i.InstructorName).ToList();
        instructorPicker.ItemsSource = instructorNames;
        //selectedInstructor = instructors.Where(q => q.Id == selectedCourse.InstructorID).Select(i => i.InstructorName).ToList();
        //instructorPicker.SelectedItem = selectedInstructor[0];

    }

    private void cancelButton_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new CoursesPage(term, originAccount));
    }

    private async void saveButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(instructorNameEntry.Text))
        {
            await DisplayAlert("Instructor Name Empty", "Please Input a value for the Instructor Name Field", "OK");
            return;
        }
        if (isValidEmail(instructorEmailEntry.Text) == false) 
        {
            await DisplayAlert("Invalid Instructor Email", "The Instructors email Address is not valid.", "OK");
            return;
        }
        if (string.IsNullOrEmpty(instructorPhoneEntry.Text)) 
        {
            await DisplayAlert("Instructor Phone Empty", "Please Input a value for the Instructor Phone Field", "OK");
            return;
        }

        if (selectedInstructor != null)
        {
            await Services.updateInstructor(selectedInstructor.Id, instructorNameEntry.Text, instructorEmailEntry.Text, instructorPhoneEntry.Text);
            App.Current.MainPage = new NavigationPage(new CoursesPage(term, originAccount));
            return;
        } else
        {
            await Services.addInstructor(instructorNameEntry.Text, instructorEmailEntry.Text, instructorPhoneEntry.Text);
            App.Current.MainPage = new NavigationPage(new CoursesPage(term, originAccount));
            return;
        }
        
    }

    private async void deleteButton_Clicked(object sender, EventArgs e)
    {
        var answer = await DisplayAlert("Delete selected Instructor?", "Do you want to delete this Instructor? Instructor Name: " + selectedInstructor.InstructorName, "Yes", "No");
        if (answer == true)
        {

            bool result = await Services.deleteInstructor(selectedInstructor.Id);
            if (result == false)
            {
                await DisplayAlert("Instructor NOT Deleted", "Instructor can not be deleted. There are courses tied to this instructor. Delete these associated courses first.", "OK");
                return;
            }
            getInstructors();
            deleteButton.IsEnabled = false;
            deleteButton.IsVisible = false;
            deleteBtnBorder.IsEnabled = false;
            deleteBtnBorder.IsVisible = false;
            ButtonGrid.SetColumn(cancelButton, 0);
            ButtonGrid.SetColumnSpan(cancelButton, 3);
            instructorNameEntry.Text = "";
            instructorEmailEntry.Text = "";
            instructorPhoneEntry.Text = "";
            titlelbl.Text = "Add Instructor";
            await DisplayAlert("Instructor Deleted", "Instructor Deleted", "OK");
            instructorPicker.SelectedIndex = -1;
            
            
        }
        await Navigation.PopAsync();
        return;
    }

    private async void instructorPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (instructorPicker.SelectedIndex == -1)
        {
            return;
        }
        var answer = await DisplayAlert("Edit Instructor?", "Do you want to edit the instructor, " + instructorPicker.SelectedItem, "Yes", "No");
        if (answer)
        {
            ButtonGrid.SetColumn(cancelbtnBorder, 2);
            ButtonGrid.SetColumnSpan(cancelbtnBorder, 1);
            deleteButton.IsEnabled = true;
            deleteButton.IsVisible = true;
            deleteBtnBorder.IsEnabled = true;
            deleteBtnBorder.IsVisible = true;
            titlelbl.Text = "Edit Instructor";
            selectedInstructor = instructors.Where(q => q.InstructorName == instructorPicker.SelectedItem).ToList()[0];
            instructorNameEntry.Text = selectedInstructor.InstructorName;
            instructorPhoneEntry.Text = selectedInstructor.InstructorPhone;
            instructorEmailEntry.Text = selectedInstructor.InstructorEmail;

            return;
        }
        instructorPicker.SelectedIndex = -1;
        return;
    }

    private bool isValidEmail(string email)
    {
        var trimmedEmail = email.Trim(); // Trims the email
        if (trimmedEmail.EndsWith(".")) // breaks email format
        {
            return false; 
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
}