using C971.DB;
using C971.Pages;
using Microsoft.Maui.Controls.Compatibility;
using System.Collections.ObjectModel;

namespace C971.Pages;

public partial class TermsPage : ContentPage
{
    private Terms selectedTerm;
    private bool created;
    Account originAccount;
	public TermsPage(bool create, Terms term, Account account)
	{
		InitializeComponent();
		if (create)
		{
            // Blank page for the create term page
            titlelbl.Text = "Create Term";
            saveButton.Text = "Create";
            //Reorganizing page
            ButtonGrid.SetColumn(cancelBtnBorder, 0);
            ButtonGrid.SetColumnSpan(cancelBtnBorder, 3);
            deleteButton.IsEnabled = false;
            deleteButton.IsVisible = false;
            enterButton.IsEnabled = false;
            enterButton.IsVisible = false;
            enterBtnBorder.IsEnabled = false;
            enterBtnBorder.IsVisible = false;
            deletebtnBorder.IsEnabled = false;
            deletebtnBorder.IsVisible = false;



        } else
		{
            // Full page for editing the Term

            selectedTerm = term;
            titlelbl.Text = "Edit " + term.TermName; // Title with term name
			termTitleEntry.Text = term.TermName;
			startDatePicker.Date = term.StartDate;
			endDatePicker.Date = term.EndDate;
            statusChkbox.IsChecked = term.TermStatus;
        }

        // Cancel Btn function : await Navigation.PopAsync(); // Takes us back to the previous page
        created = create;
        originAccount = account;
	}

    private async void deleteButton_Clicked(object sender, EventArgs e)
    {
        var answer = await DisplayAlert("Do you want to delete this Term and its related Courses and Assignments? Terms ID: " + selectedTerm.Id, "Delete this Term?", "Yes", "No");
        if (answer == true)
        {

            await Services.deleteTerm(selectedTerm.Id);
            await DisplayAlert("Term Deleted", "Term Deleted", "OK");
        }
        await Navigation.PopAsync();
        App.Current.MainPage = new NavigationPage(new HomePage(originAccount));
    }

    private void cancelButton_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new HomePage(originAccount));
    }

    private async void saveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(termTitleEntry.Text))
            {
                await DisplayAlert("Missing Term Name", "Please enter a value for Term Name", "OK");
                return;
            }
            if (startDatePicker.Date > endDatePicker.Date)
            {
                await DisplayAlert("Date Picker", "Start date is after the end date.", "OK");
                return;
            }
            if (created)
            {
                await Services.addTerm(termTitleEntry.Text, startDatePicker.Date, endDatePicker.Date, statusChkbox.IsChecked, originAccount.Id);
            } else
            {
                await Services.updateTerm(selectedTerm.Id, termTitleEntry.Text, startDatePicker.Date, endDatePicker.Date, statusChkbox.IsChecked);
            }

            App.Current.MainPage = new NavigationPage(new HomePage(originAccount));

        }
        catch (Exception ex) { await DisplayAlert("Something Happened", ex.Message, "OK"); return; }
        
    }

    private void enterButton_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new CoursesPage(selectedTerm, originAccount));
    }
}