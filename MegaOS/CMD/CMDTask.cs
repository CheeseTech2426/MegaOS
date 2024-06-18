using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS.CMD {
    internal class CMDTask : Command {
        public CMDTask(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            int id = 1;
            string response = "";
            try {
                switch (args[0]) {
                    case "create":
                        StartTask(args);
                        break;
                    case "list":
                        Kernel.taskman.ListTasks();
                        break;
                    case "start":
                        if (int.TryParse(args[1], out id)) {
                            try {
                                Kernel.taskman.StartTask(id);
                            } catch (Exception e) {
                                Beep.Warning();
                                response = $"Failed to start task {id}! Reason: {e.Message}";
                                break;
                            }
                        } else {
                            Beep.Warning();
                            response = $"{args[1]} is not a valid number!";
                        }
                        break;
                    case "stop":
                        if (int.TryParse(args[1], out id)) {
                            try {
                                Kernel.taskman.StopTask(id);
                            } catch (Exception e) {
                                Beep.Warning();
                                response = $"Failed to stop task {id}! Reason: {e.Message}";
                                break;
                            }
                        } else {
                            Beep.Warning();
                            response = $"{args[1]} is not a valid number!";
                        }
                        break;
                    default:
                        break;
                }
            } catch (Exception e) {
                Beep.Warning();
                GL.WriteLine($"An error occured in Taskman! {e.Message} ");
            }
            //GL.WriteLine(response);
            return response;
        }

        private void StartTask(string[] args) {
            string name = args[1].ToLower();
            switch (name) {
                case "test":
                    Kernel.taskman.CreateTask(name, hello, new string[4] { args[2], args[3], args[4], args[5] });
                    break;
                case "shell":
                    Shell.Shell shell = new Shell.Shell();
                    break;

                default:
                    break;
            }
        }

        void hello(object[] args) {
            foreach (object arg in args) {
                GL.WriteLine(arg.ToString());
            }
            return;
        }
    }
}
