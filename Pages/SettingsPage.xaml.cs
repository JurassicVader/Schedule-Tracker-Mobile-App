using C971.DB;
using C971.Pages.Account_Pages;

namespace C971.Pages;

public partial class SettingsPage : ContentPage
{
    Account originAccount;
    int reportSelection = -1;
	public SettingsPage(Account account)
	{
		InitializeComponent();
        originAccount = account;
        ReportPicker.SelectedIndex = -1;
        List<string> pickerItems = new List<string>() {"Total Items Created", "Account Details", "Longest Item Name" };
        ReportPicker.ItemsSource = pickerItems;
	}

    

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
        App.Current.MainPage = new NavigationPage(new HomePage(originAccount));
        return;
    }

    private void editAccount_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new EditAccount(originAccount));
    }

    private void GenerateReports_Clicked(object sender, EventArgs e)
    {
        if (ReportPicker.SelectedIndex == -1)
        {
            errorLbl.Text = "No report has been selected. Go to the picker and select a report that you would like to run.";
            errorLbl.IsVisible = true;
            return;
        }
        Console.WriteLine("About to generate report. Selection is: " + reportSelection);
        App.Current.MainPage = new NavigationPage(new Reports(originAccount, reportSelection));
    }

    private void ReportPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ReportPicker.SelectedIndex == -1)
        {
            return;
        }
        reportSelection = ReportPicker.SelectedIndex;
        Console.WriteLine("Report selection is: " + reportSelection);
    }
}