using C971.DB;

namespace C971.Pages.Account_Pages;

public partial class Reports : ContentPage
{
	Account originAccount;
	public Reports(Account account, int type)
	{
		InitializeComponent();
		originAccount = account;
		switch (type)
		{
			case 0:
				topicLbl.Text = "Total Items Created Report";
                created.IsVisible = true;
                popTotal(account);
                break;
			case 1:
                topicLbl.Text = "Account Details Report";
                details.IsVisible = true;
                popDetails(account);
                break;
			case 2:
                topicLbl.Text = "Longest Item Name";
                LongestName.IsVisible = true;
                popLargest(account);
                break;
        }
	}
	private async void popTotal(Account account)
	{
        var terms = await Services.TotalTerms(account.Id);
        var courses = await Services.TotalCourses(account.Id);
        var assignments = await Services.TotalAssignments(account.Id);

        totalTermslbl.Text = "Total Terms Created: " + terms.Count();
        totalCourseslbl.Text = "Total Courses Created: " + courses.Count();
        totalAssignmentlbl.Text = "Total Assignments Created: " + assignments.Count();
	}
    private void popDetails(Account account)
    {
        usernamelbl.Text = "Username: " + account.Username;
        emaillbl.Text = "Email: " + account.Email;
        firstlastlbl.Text = "First and Last Name: " + account.FirstName + " " + account.LastName;
        if (account.Graduate)
        {
            gradlbl.Text = "Graduate Student: Yes";
        } else
        {
            gradlbl.Text = "Graduate Student: No";
        }
        createdlbl.Text = "Account Created: " + account.CreationDate.ToString("MMMM dd, yyyy") + ".";
    }
    private async void popLargest(Account account)
    {
        var terms = await Services.TotalTerms(account.Id);
        var courses = await Services.TotalCourses(account.Id);
        var assignments = await Services.TotalAssignments(account.Id);

        var termslist = terms.Select(s => s.TermName).ToList();
        string longTerm = termslist.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
        var courselist = courses.Select(s => s.CourseName).ToList();
        string longCourse = courselist.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
        var assignmentlist = assignments.Select(s => s.AssignmentName).ToList();
        string longAssign = assignmentlist.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);

        longTermlbl.Text = "Longest Term Name: " + longTerm;
        longCourselbl.Text = "Longest Course Name: " + longCourse;
        longAssignmentlbl.Text = "Longest Assignment Name: " + longAssign;

    }
    private void Back_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new HomePage(originAccount));
        return;
    }
}