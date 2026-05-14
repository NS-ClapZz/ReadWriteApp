using System.Windows;
using ReadWriteApp.Data;
using ReadWriteApp.Views;

namespace ReadWriteApp;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        DataStore.Initialize();

        var loginWindow = new LoginWindow();
        loginWindow.Show();
    }
}
