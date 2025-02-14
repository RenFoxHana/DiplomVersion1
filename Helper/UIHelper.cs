using System.Windows;

namespace DiplomVersion1.Helper
{
    public static class UIHelper
    {
        /// <summary>
        /// Метод для настройки видимости элементов управления в зависимости от роли пользователя.
        /// </summary>
        public static void ConfigureUIForWatchman(params FrameworkElement[] elements)
        {
            //Проверка роли пользователя
            if (App.UserRole == "Watchman")
            {
                //Если роль — вахтер, скрываются элементы
                foreach (var element in elements)
                {
                    element.Visibility = Visibility.Collapsed;
                }
            }
        }

        public static void ConfigureUIForAdmin(params FrameworkElement[] elements)
        {
            //Проверка роли пользователя
            if (App.UserRole == "Admin")
            {
                //Если роль — администратор, скрываются элементы
                foreach (var element in elements)
                {
                    element.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}