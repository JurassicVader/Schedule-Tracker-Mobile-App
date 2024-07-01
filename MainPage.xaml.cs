using C971.DB;
using C971.Pages;
using Plugin.LocalNotification;

namespace C971
{
    /*
     * This main page will contain the terms and a settings button.
     * The buttons will be full of terms and a create term button.
     * There is a controls bar that allows you to edit, delete and enter a term. 
     * The settings will contain the apps preloaded data and clear db buttons.
     */

    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void backBtn_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
