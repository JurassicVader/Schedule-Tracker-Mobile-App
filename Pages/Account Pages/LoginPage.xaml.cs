using C971.DB;
using C971.Pages.Account_Pages;

namespace C971.Pages;

public partial class LoginPage : ContentPage
{
    /*D424 - Capstone
     * Created By Spencer Burchfield
     * Last Updated: 6/10/2024
    */
	public LoginPage()
	{
		InitializeComponent();
	}

    private void CreateAccountBtn_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new CreateAccount());
    }

    private async void LoginBtn_Clicked(object sender, EventArgs e)
    {
        if (String.IsNullOrWhiteSpace(UsernameEntry.Text) || String.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            if (String.IsNullOrEmpty(UsernameEntry.Text) && String.IsNullOrEmpty(PasswordEntry.Text))
            {
                errorLbl.IsVisible = true;
                errorLbl.Text = "The username and password fields are empty. Please enter both a username and password.";
                return;
            } else if (String.IsNullOrEmpty(PasswordEntry.Text))
            {
                errorLbl.IsVisible = true;
                errorLbl.Text = "The password field is empty. Please enter both a username and password.";
                return;
            } else if(String.IsNullOrEmpty(UsernameEntry.Text))
            {
                errorLbl.IsVisible = true;
                errorLbl.Text = "The username field is empty. Please enter both a username and password.";
                return;
            }
            return;
        } 
        try
        {
            Account selectedAccount = await Services.CheckAccount(UsernameEntry.Text.ToLower());
            if (selectedAccount.Password != PasswordEntry.Text) 
            {
                errorLbl.IsVisible = true;
                errorLbl.Text = "Username and password do not match. Please try again.";
                Console.WriteLine("Unit Testing: The username is found, password doesn't match.");
                return;
            } else
            {
                App.Current.MainPage = new NavigationPage(new HomePage(selectedAccount));
                return;
            }
        } catch (Exception ex) 
        { 
            errorLbl.IsVisible = true; 
            errorLbl.Text = "Username and password do not match. Please try again."; 
            Console.WriteLine(ex.Message); 
        }
    }

    private void ForgotPassword_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new ForgotPassword());
    }
}