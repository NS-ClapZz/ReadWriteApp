using ReadWriteApp.Models;

namespace ReadWriteApp.Data
{
    public static class DataStore
    {
        public static User? CurrentUser { get; set; } = null;

        public static void Initialize()
        {
            DatabaseHelper.InitializeDatabase();
        }
    }
}
