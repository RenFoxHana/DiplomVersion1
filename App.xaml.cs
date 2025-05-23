﻿using System.Configuration;
using System.Data;
using System.Windows;

namespace DiplomVersion1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int CurrentUserId { get; set; }
        public static string UserRole { get; set; } = string.Empty;

        public static void ClearUserData()
        {
            CurrentUserId = 0;
            UserRole = string.Empty;
        }
    }

}
