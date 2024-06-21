using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sys = Cosmos.System;

namespace MegaOS.CMD {
    internal class Edit {

        Cosmos.System.Console console;

        string[] menubar = { "New", "Open", "Save", "About", "Quit" };
        int idx;
        List<char> text;
        bool running;
        int charCount;
        object[] arguments = new object[1] { "" };
        int taskID;

        public Edit() {
            idx = 0;
            charCount = 0;
            text = new List<char>();
            running = true;

            taskID= Kernel.taskman.CreateTask("Edit", Run, new string[1] { "" });
            Kernel.taskman.StartTask(taskID);
        }

        private void Run(object[] args) {
            while (Kernel.taskman.isTaskRunning("Edit")) {
                Cosmos.Core.Memory.Heap.Collect();
                if(sys.KeyboardManager.TryReadKey(out sys.KeyEvent k)) {
                    if(k.Modifiers == ConsoleModifiers.Alt) {
                        if(k.Key == sys.ConsoleKeyEx.LeftArrow && idx > 0) {
                            idx--;
                            GL.DrawMenuBarOptions(menubar, idx, 0, 1, true);
                        }

                        if (k.Key == sys.ConsoleKeyEx.RightArrow && idx < menubar.Length - 1) {
                            idx++;
                            GL.DrawMenuBarOptions(menubar, idx, 0, 1, true);
                        }

                        if (k.Key == sys.ConsoleKeyEx.Enter) {
         
                            string filename = "";
                            switch (menubar[idx]) {
                                case "New":
                                    GL.Clear();
                                    text.Clear();
                                    Render(args);
                                    break;
                                case "Open":
                                    filename = GL.DrawDialogWithTextField(60, 12, "Open", ConsoleColor.Blue, ConsoleColor.White, "");
                                    if (!File.Exists(filename)) {
                                        GL.DrawDialog(40, 5, "Open", ConsoleColor.Blue, ConsoleColor.White, "File doesn't exist!");
                                        break;
                                    }
                                    string read = File.ReadAllText(filename);
                                    foreach (char c in read) {
                                        text.Add(c);
                                    }
                                    Render(args);
                                    break;  
                                case "Save":
                                    filename = GL.DrawDialogWithTextField(60, 12, "Save As", ConsoleColor.Blue, ConsoleColor.White, "");
                                    if (File.Exists(filename)) {
                                        GL.DrawDialog(40, 5, "Save As", ConsoleColor.Blue, ConsoleColor.White, "File already exists!");
                                        break;
                                    }
                                    File.WriteAllText(filename, Kernel.BuildString(text));
                                    break;
                                case "About":
                                    GL.DrawDialog(40, 5, "About", ConsoleColor.Blue, ConsoleColor.White, "MegaOS Text Editor v1.0");
                                    break;
                                case "Quit":
                                    Kernel.taskman.StopTask(taskID);
                                    return;
                            }
                        }
                    }

                    if(k.Key == sys.ConsoleKeyEx.Enter) {
                        text.Add('\n');
                        Render(args);
                    } else if(k.Key == sys.ConsoleKeyEx.Backspace) {
                        if (charCount < 1) {
                            
                        } else {
                            text.RemoveAt(text.Count - 1);
                            Render(args);
                        }
                    } else if (k.Key == sys.ConsoleKeyEx.Spacebar) {
                        text.Add(' ');
                        Render(args);
                    } else if(k.Key != sys.ConsoleKeyEx.LeftArrow &&
                        k.Key != sys.ConsoleKeyEx.RightArrow && 
                        k.Key != sys.ConsoleKeyEx.UpArrow && 
                        k.Key != sys.ConsoleKeyEx.DownArrow &&
                        k.Modifiers != ConsoleModifiers.Alt && 
                        k.Modifiers != ConsoleModifiers.Control) {
                        text.Add(k.KeyChar);
                        Render(args);
                    }
                } 
            }
            GL.Clear();
        }

        private void Render(object[] args) {
            GL.Clear();
            GL.DrawTitleBar("Edit");
            GL.Write(0,1, new string(' ', GL.Width), ConsoleColor.Blue, ConsoleColor.Blue);
            GL.DrawMenuBarOptions(menubar, idx, 0, 1);
            GL.SetCursor(0, 2);
            if ((string)args[0] == "") {
                charCount = 0;
                foreach (char c in text) {
                    GL.Write(c.ToString());
                    charCount++;
                }
            }

            GL.Write(GL.Width - ("Characters: ".Length + charCount.ToString().Length + 2), 1, $"Characters: {charCount}", ConsoleColor.White, ConsoleColor.Blue);
        }
    }

    public class CMDEdit : Command {
        public CMDEdit(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            Edit edit = new Edit();
            return "";
        }
    }
}


