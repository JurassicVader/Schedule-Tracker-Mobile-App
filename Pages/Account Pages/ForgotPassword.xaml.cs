using C971.DB;
using System.Net.Mail;

namespace C971.Pages.Account_Pages;


public partial class ForgotPassword : ContentPage
{
    Account account = new Account();
    public ForgotPassword()
	{
		InitializeComponent();

    }
    

    private async void setPasswordBtn_Clicked(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(setPasswordEntry.Text) || String.IsNullOrEmpty(confirmPasswordEntry.Text))
        {
            notMatchlbl.Text = "One or more fields are empty. Passwords do not match. Please try again.";
            notMatchlbl.TextColor = Color.Parse("Red");
            return;
        }
        if (setPasswordEntry.Text != confirmPasswordEntry.Text)
        {
            notMatchlbl.Text = "Passwords do not match. Please try again.";
            notMatchlbl.TextColor = Color.Parse("Red");
            return;
        }
        try
        {
            await Services.updatePassword(account.Id, setPasswordEntry.Text);
            await DisplayAlert("Password Changed Successfully", "Your password has been reset successfully. Returning you to the login screen.", "OK");
            App.Current.MainPage = new NavigationPage(new LoginPage());

        } catch (Exception ex)
        {
            await DisplayAlert("Error.", ex.Message, "OK");
            return;
        }
    }


    private void backBtn_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new LoginPage());
    }

    private async void requestChangeBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(usernameEntry.Text) || String.IsNullOrWhiteSpace(emailEntry.Text))
            {
                await DisplayAlert("Empty Field", "The Username or Email fields are empty. Please enter both.", "OK");
                return;
            }
            else
            {
                account = await Services.CheckAccount(usernameEntry.Text.ToLower());
                if (account.Email != emailEntry.Text.ToLower())
                {
                    await DisplayAlert("Access Denied", "The username and/or Email fields are incorrect. Please try again", "OK");
                    return;
                }
                else
                {
                    Console.WriteLine("Checkpoint 1: Valid Email.");
                    CodeLayout.IsVisible = false;
                    PasswordLayout.IsVisible = true;
                    Console.WriteLine("Checkpoint 2: Popups Enabled.");
                    return;
                }
            }
        } catch (Exception ex) { await DisplayAlert("Access Denied", "The username and/or Email fields are incorrect. Please try again", "OK"); Console.WriteLine(ex.Message); }
        
    }
}