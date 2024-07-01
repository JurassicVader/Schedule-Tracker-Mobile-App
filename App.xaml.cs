using C971.DB;
using C971.Pages;

namespace C971
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            /*if (Settings.FirstRun)
            {
                //Notifications.CheckAndRequestLocalPermission();
                Settings.FirstRun = false;
            }*/
            Application.Current.UserAppTheme = AppTheme.Light;
            

            MainPage = new AppShell();
            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
