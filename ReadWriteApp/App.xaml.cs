using System.Windows;
using ReadWriteApp.Data;
using ReadWriteApp.Views;

namespace ReadWriteApp;

/// <summary>
/// Главный класс приложения
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Обработчик запуска приложения — инициализирует данные и открывает окно входа
    /// </summary>
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // Заполняем хранилище тестовыми данными
        DataStore.Initialize();

        // Открываем окно авторизации
        var loginWindow = new LoginWindow();
        loginWindow.Show();
    }
}
