using C971.DB;

namespace C971.Pages.Account_Pages;

public partial class CreateAccount : ContentPage
{
	public CreateAccount()
	{
		InitializeComponent();
	}

    private async void createAcctBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (String.IsNullOrEmpty(usernameEntry.Text) || String.IsNullOrEmpty(emailEntry.Text) || String.IsNullOrEmpty(firstEntry.Text) || String.IsNullOrEmpty(lastEntry.Text) || String.IsNullOrEmpty(setPasswordEntry.Text) || String.IsNullOrEmpty(confirmPasswordEntry.Text))
            {
                await DisplayAlert("Empty Field", "One or more fields are empty. Please fill them out before continuing.", "OK");
                return;
            }
            if (setPasswordEntry.Text != confirmPasswordEntry.Text)
            {
                await DisplayAlert("Password do not Match", "Your passwords do not match. Please re-Enter your passwords", "OK");
                return;
            }
            if (isValidEmail(emailEntry.Text) == false)
            {
                await DisplayAlert("invalid Email", "Your email is not a valid email. Please enter a correct email address", "OK");
                return;
            }
            try
            {
                var chckusername = await Services.CheckAccount(usernameEntry.Text.ToLower());
                if (chckusername.Username == usernameEntry.Text.ToLower().Trim())
                {
                    await DisplayAlert("Username Taken", "This username has already been taken. Try adding numbers to the end.", "OK");
                    return;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message, " Account doesn't exist");
            }
            
            
            await Services.createAccount(usernameEntry.Text.ToLower(), setPasswordEntry.Text, DateTime.Now, firstEntry.Text.Trim(), lastEntry.Text.Trim(), graduateChkbox.IsChecked, emailEntry.Text.ToLower());
            await DisplayAlert("Account Creation Successful", "Account creation was successful. Welcome, " + usernameEntry.Text + " Returning you to login page", "OK");
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An Error has Occurred: " + ex.Message, "OK");
        }
    }

    private void backBtn_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new LoginPage());
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