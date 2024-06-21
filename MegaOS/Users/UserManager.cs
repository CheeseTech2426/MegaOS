using MegaOS.CMD;
using System;
using System.Collections.Generic;
using System.IO;

namespace MegaOS {
    public class UserManager {
        private List<User> users;
        private string usersFilePath = @"0:\MegaOS\usr\users.sys";

        public UserManager(bool load = true) {
            users = new List<User>();
            if(load)LoadUsers();
        }

        public void LoadUsers() {
            if (!File.Exists(usersFilePath)) {
                GL.WriteLine("Warning: Users file does not exist. Creating new file.");
                Directory.CreateDirectory(Path.GetDirectoryName(usersFilePath));
                File.Create(usersFilePath).Close();
                return;
            }

            try {
                users.Clear();
                string[] lines = File.ReadAllLines(usersFilePath);
                foreach (string line in lines) {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(new[] { '}', ')', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 3) {
                        Beep.Warning();
                        GL.WriteLine($"Error: Invalid format in users file: {line}");
                        continue;
                    }

                    string username = parts[0].Substring(1).Trim();
                    string password = parts[1].Substring(1).Trim();
                    string permissions = parts[2].Substring(1).Trim();

                    users.Add(new User(username, password, permissions));
                }
            } catch (Exception ex) {
                Beep.Warning();
                GL.WriteLine($"Error loading users: {ex.Message}");
            }
        }

        public int AuthenticateUser(string username, string password) {
            foreach (var user in users) {
                if (user.Username == username) {
                    if (user.Password == password) {
                        return 1;
                    } else {
                        return 0;
                    }
                }
            }
            return -1;
        }


        public User GetUser(string username) {
            return users.Find(u => u.Username == username);
        }

        public void AddUser(string username, string password, string permissions) {
            users.Add(new User(username, password, permissions));
            SaveUsers();
        }


        public void SaveUsers() {
            CoreServices core = new CoreServices();
            try {
                using (StreamWriter writer = new StreamWriter(usersFilePath)) {
                    foreach (User user in users) {
                        writer.WriteLine($"{{{user.Username}}}({user.Password})[{user.Permissions}]");
                    }
                }
            } catch (Exception ex) {
                Beep.Warning();
                core.Log($"An error occurred while saving users: {ex.Message}", LogType.Error);
                return;
            }
            GL.WriteLine("Users saved to file.");
        }

        public void ListUsers() {
            foreach (User user in users) {
                GL.WriteLine(user.ToString());
            }
        }

        public void Login() {
            GL.Clear();
            GL.SetCursor(0, 0);
            GL.Write(new string(' ', GL.Width), ConsoleColor.Blue, ConsoleColor.Blue);
            GL.SetCursor(0, 0);
            GL.Write($"  {CoreServices.fullVersionString} | Login", ConsoleColor.White, ConsoleColor.Blue);
            GL.SetCursor(0, 3);
            LoadUsers();
            GL.SetCursor(0,9);
            GL.Write("   Username: ");
            string username = Console.ReadLine();
            GL.SetCursor(0, 10);
            GL.Write("   Password: ");
            string password = Console.ReadLine();
            int a = AuthenticateUser(username, password);
            if (a == 1) {
                Kernel.userLoggedIn = GetUser(username).Username;
                GL.Clear();
                return;
            } else if(a == 0 || a == -1) {
                GL.SetCursor(0, 11);
                Beep.Warning();
                GL.Write("Invalid username or password!", ConsoleColor.Red);
                Console.ReadKey(true);
                Login();
            }
        }


        /* MegaOS Login 0
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */
    }
}
