namespace C971.Pages.Account_Pages;
using C971.DB;

public partial class EditAccount : ContentPage
{
	Account originAccount;
	public EditAccount(Account account)
	{
		InitializeComponent();
		originAccount = account;
		titlelbl.Text = "Edit: " + account.Username;
		UsernameEntry.Text = account.Username;
		FirstNameEntry.Text = account.FirstName;
		LastNameEntry.Text = account.LastName;
		EmailEntry.Text = account.Email;
		gradChkbox.IsChecked = account.Graduate;
	}

    private async void deleteButton_Clicked(object sender, EventArgs e)
	{
		var answer = await DisplayAlert("Delete User Account " + originAccount.Username + "?", "Are you sure that you want to delete your user account? This action cannot be undone.", "Yes", "No");
		if (answer == true)
		{
			await Services.DeleteAccount(originAccount.Id);
			await DisplayAlert("Account Deleted", "Your user account, " + originAccount.Username + " was successfully deleted.\nReturning you to the login screen.", "OK");
			App.Current.MainPage = new NavigationPage(new LoginPage());
			return;
		}

	}
	private async void cancelButton_Clicked(object sender, EventArgs e)
    {
		if (UsernameEntry.Text.ToLower() != originAccount.Username || FirstNameEntry.Text.Trim() != originAccount.FirstName || LastNameEntry.Text.Trim() != originAccount.LastName || EmailEntry.Text.ToLower() != originAccount.Email || gradChkbox.IsChecked != originAccount.Graduate)
		{
			var answer = await DisplayAlert("Cancel Changes?", "You have made changes. Are you sure that you want to cancel your changes?", "Yes", "No");
			if (answer == true)
			{
                App.Current.MainPage = new NavigationPage(new HomePage(originAccount));
				return;
            }
			return;
		} else
		{
            App.Current.MainPage = new NavigationPage(new HomePage(originAccount));
			return;
        }
		
    }
    private async void saveBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (String.IsNullOrEmpty(UsernameEntry.Text) || String.IsNullOrEmpty(EmailEntry.Text) || String.IsNullOrEmpty(FirstNameEntry.Text) || String.IsNullOrEmpty(LastNameEntry.Text))
            {
                errorLbl.Text = "One or more fields are empty. Please fill every field out before continuing.";
                errorLbl.IsVisible = true;
                return;
            }
            if (isValidEmail(EmailEntry.Text) == false)
            {
                errorLbl.Text = "Your email is not a valid email. Please enter a correct email address";
                errorLbl.IsVisible = true;
                return;
            }
            try
            {
                var chckusername = await Services.CheckAccount(UsernameEntry.Text.ToLower());
                if (chckusername.Username == UsernameEntry.Text.ToLower().Trim() && chckusername.Username != originAccount.Username)
                {
                    errorLbl.Text = "This username has already been taken. You can try adding numbers to the end.";
                    errorLbl.IsVisible = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, " Account doesn't exist");
            }


            await Services.updateAccount(originAccount.Id, UsernameEntry.Text.ToLower(), originAccount.Password, originAccount.CreationDate, FirstNameEntry.Text.Trim(), LastNameEntry.Text.Trim(), gradChkbox.IsChecked, EmailEntry.Text.Trim().ToLower());
            await DisplayAlert("Account Update Successful", "Account update was successful for, " + UsernameEntry.Text + " Returning you to login page", "OK");
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An Error has Occurred: " + ex.Message, "OK");
        }
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