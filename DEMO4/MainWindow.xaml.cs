﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DEMO4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class UserSession
    {
        public static int UserId { get; set; }
        public static string Login { get; set; }
        public static byte Role { get; set; } // 0 = admin, 1 = regular user, etc.
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var login = loginBox.Text.Trim();
            var password = passwordBox.Password.Trim();

            using (var db = new DEMO_v3Entities())
            {
                var user = db.users.FirstOrDefault(u => u.login == login);
                if (user != null)
                {
                    if (user.password != password)
                    {
                        MessageBox.Show("Неверный пароль");
                    }
                    else
                    {
                        // Store user information in session
                        UserSession.UserId = user.id;
                        UserSession.Login = user.login;
                        UserSession.Role = (byte)user.role;

                        var menu = new menuWIndow();
                        menu.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не найден");
                }
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
