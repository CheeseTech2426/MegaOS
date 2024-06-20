using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MegaOS {
    internal class OOBE {

        string username;
        string password;
        string pcname;

        public void Run(bool check = true) {
            if (Directory.Exists(@"0:\MegaOS\") && check) return;
            Kernel.registry = new Registry();
            GL.Setup();
            
            GL.Clear();
            GL.Write(0, 0, new string(' ', GL.Width), ConsoleColor.Blue, ConsoleColor.Blue);
            GL.Write((GL.Width / 2) - "MegaOS Setup Wizard | Collecting Info".Length, 0, "MegaOS Setup Wizard | Collecting Info\n", ConsoleColor.White, ConsoleColor.Blue);
            GL.Write(2, 2, "Welcome to MegaOS Setup Wizard by Mr_Cheese\n", ConsoleColor.White, ConsoleColor.Black);
            GL.Write(2, 3, "Before we continue, let's configure MegaOS to your liking!\n", ConsoleColor.White, ConsoleColor.Black);
            askname();
            GL.Write(2, 8, "Okay! Now let's choose an Administrator password!", ConsoleColor.White, ConsoleColor.Black);
            askpassword();
            GL.Write(2, 13, "Got it! One more question:", ConsoleColor.White, ConsoleColor.Black);
            askpcname();
            GL.WriteLine("\n");
            CoreServices.Installer(username, password, pcname);
        }

        private void askpcname() {
            GL.Write(2, 14, "PC name: ", ConsoleColor.White, ConsoleColor.Black);
            GL.SetCursor("PC name: ".Length + 2, 14);
            pcname = Console.ReadLine();
            if (string.IsNullOrEmpty(pcname)) {
                askpcname();
            }


        }

        private void askname() {
            GL.Write(2, 4, "What is your name? ", ConsoleColor.White, ConsoleColor.Black);
            GL.SetCursor(2 + "What is your name? ".Length, 4);
            string name = Console.ReadLine();
            GL.Write(2, 5, $"Is '{name}' correct?", ConsoleColor.White, ConsoleColor.Black);
            bool selection = true;
            int idx = 0;
            string[] options = { "Yes", "No" };
            DrawSelection(2, 6, options, 0);
            while (selection) {
                if (Cosmos.System.KeyboardManager.TryReadKey(out Cosmos.System.KeyEvent k)) {
                    if (k.Key == Cosmos.System.ConsoleKeyEx.UpArrow && idx > 0) {
                        idx--;
                        DrawSelection(2, 6, options, idx);
                    } 
                    if (k.Key == Cosmos.System.ConsoleKeyEx.DownArrow && idx < 1) {
                        idx++;
                        DrawSelection(2, 6, options, idx);
                    }

                    if (k.Key == Cosmos.System.ConsoleKeyEx.Enter) {
                        if (options[idx] == "Yes") {
                            selection = false;
                            username = name;
                            break;
                        } else {
                            for(int x = 0; x < GL.Width; x++) {
                                for(int y = 4; y < 8; y++) {
                                    GL.Write(x, y, " ", ConsoleColor.White, ConsoleColor.Black);
                                }
                            }
                            idx = 0;
                            askname();
                        }
                    }
                }
            }
            return;
        }

        private void askpassword() {
            GL.Write(2, 9, "Password: ", ConsoleColor.White, ConsoleColor.Black);
            GL.SetCursor(2 + "Password: ".Length, 9);
            string pw = Console.ReadLine();
            GL.Write(2, 10, $"Is '{pw}' correct?", ConsoleColor.White, ConsoleColor.Black);
            bool selection = true;
            int idx = 0;
            string[] options = { "Yes", "No" };
            DrawSelection(2, 11, options, 0);
            while (selection) {
                if (Cosmos.System.KeyboardManager.TryReadKey(out Cosmos.System.KeyEvent k)) {
                    if (k.Key == Cosmos.System.ConsoleKeyEx.UpArrow && idx > 0) {
                        idx--;
                        DrawSelection(2, 11, options, idx);
                    }
                    if (k.Key == Cosmos.System.ConsoleKeyEx.DownArrow && idx < 1) {
                        idx++;
                        DrawSelection(2, 11, options, idx);
                    }

                    if (k.Key == Cosmos.System.ConsoleKeyEx.Enter) {
                        if (options[idx] == "Yes") {
                            selection = false;
                            password = pw;
                            break;
                        } else {
                            for (int x = 0; x < GL.Width; x++) {
                                for (int y = 4; y < 8; y++) {
                                    GL.Write(x, y, " ", ConsoleColor.White, ConsoleColor.Black);
                                }
                            }
                            idx = 0;
                            askpassword();
                            break;
                        }
                    }
                }
            }
            return;
        }

        private void DrawSelection(int x, int y, string[] options, int idx) {
            GL.SetCursor(x, y);
            int ny = y;
            for(int i = 0; i < options.Length; i++) { 
                GL.SetCursor(2, ny);
                if (i == idx) {
                    GL.Write($"[X] {options[i]}");
                } else {
                    GL.Write($"[ ] {options[i]}");
                }
                ny++;
            }
        }
    }
}