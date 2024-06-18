using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using MegaOS.CMD;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sys = Cosmos.System;

namespace MegaOS
{
    public class Kernel : Sys.Kernel {

        public static CosmosVFS vfs;
        CMDManager cm;
        List<char> keyPresses;
        public static string path;
        public static UserManager user;
        public static TaskManager taskman;
        public static string userLoggedIn;
        public static string pcname;
        CoreServices core = new CoreServices();

        public static Registry registry;

        int l = 0;

        protected override void BeforeRun() {
            core = new CoreServices();
            try {
                // Register VFS
                core.Log("Registering VFS..", LogType.Info);
                vfs = new CosmosVFS();
                VFSManager.RegisterVFS(vfs);
                core.Log("VFS Registered successfully!", LogType.OK);
                OOBE oobe = new OOBE();
                oobe.Run();
                core.Log("Starting registry....", LogType.Info);
                registry = new Registry();
                registry.CheckRegistry();
                Console.WriteLine("Recovery mode (y/N) ");
                ConsoleKeyInfo k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.Y) {
                    recoverymode();
                }

                if (Directory.Exists(@"1:\")) core.CheckSystem(true);

                core.Log("Loading users....", LogType.Info);
                user = new UserManager();
                user.LoadUsers();
                core.Log("Running Taskman...", LogType.Info);
                taskman = new TaskManager();
                core.Log("Starting GL...", LogType.Info);
                path = @"0:\";
                pcname = "megaos";
                core.Log("Starting Command Manager...", LogType.Info);
                cm = new CMDManager();
                keyPresses = new List<char>(); // real important
                GL.Setup();
                user.Login();
                GL.DrawBar();
                GL.WriteLine();
                l = GL.PrintDOS();
                Beep.Startup();

            } catch(Exception e) {
                core.Log($"An error occured in Kernel! {e.Message}", LogType.PanicGraphical);
            }
        }
        string oldDate;
        protected override void Run() {
            Kernel.taskman.RunAllTasks();
            taskman.CheckAndRestartTasks();
            if (taskman.isTaskRunning("shell")) return;
            string date = DateTime.Now.ToString("dddd d.M.yyyy HH:mm:ss");
            Cosmos.System.KeyEvent key;
            if (Sys.KeyboardManager.TryReadKey(out key)) {
                if (key.Key == Sys.ConsoleKeyEx.Enter) {
                    GL.WriteLine();
                    string input = BuildString(keyPresses);
                    keyPresses.Clear();
                    GL.WriteLine(cm.Process(input));
                    l = GL.PrintDOS();
                } else if (key.Key == Sys.ConsoleKeyEx.Backspace) {
                    try { keyPresses.RemoveAt(keyPresses.Count - 1); } catch { }
                    int x, y;
                    x = GL.GetCursor().x;
                    y = GL.GetCursor().y;
                    if(x > l) {
                        GL.Write(x - 1, y, " ", GL.Foreground, GL.Background);
                        GL.SetCursor(x - 1, y);
                    }
                } else {
                    keyPresses.Add(key.KeyChar);
                    GL.Write(key.KeyChar.ToString());
                }
            }

            if(date != oldDate) {
                GL.DrawBar();
            }

            //RingGL.DrawBar();
            Cosmos.Core.Memory.Heap.Collect();
            oldDate = date;
        }
        string BuildString(List<char> key) {
            StringBuilder sb = new StringBuilder();
            foreach (char k in key) {
                sb.Append(k);
            }
            return sb.ToString();
        }

        private void recoverymode() {
            bool running = true;
            if (registry == null) registry = new Registry();
            while (running) {
                Console.Write("Recovery> ");
                string input = Console.ReadLine();
                string[] args = input.Split(' ');
                switch (input) {
                    case "recreate admin":
                        Console.WriteLine("Adding Admin user...");
                        user.AddUser("admin", "admin", "admin");
                        user.LoadUsers();
                        user.ListUsers();
                        Console.WriteLine("Saving users...");
                        user.SaveUsers();
                        Console.WriteLine("Done! Username: admin password admin");
                        break;
                    case "sfa":
                        core.CheckSystem();
                        break;
                    case "help":
                        Console.WriteLine("Commands:\n - recreate admin - Recreates the administrator account\n - sfa - System file audit\n - help - Shows this list\n - registry /check - Checks the registry\n - registry /rebuild - Rebuilds the registry");
                        break;
                    case "registry /check":
                        Console.WriteLine("Checking..");
                        registry.CheckRegistry();
                        break;
                    case "registry /rebuild":
                        try {
                            Console.WriteLine("Rebuilding registry....\nCreating registry...");
                            if (!Directory.Exists(@"0:\MegaOS\")) {
                                core.Log("System folder does not exist, recreating...", LogType.Warning);
                                try {
                                    Directory.CreateDirectory(@"0:\MegaOS\");
                                } catch(Exception e) {
                                    core.Log($"An error '{e.Message}' occured!", LogType.Error);
                                    break;
                                }
                                core.Log("Done!", LogType.OK);
                            }
                            File.Create(registry.registry);
                            Console.WriteLine("Registry created!\nResetting registry..");
                            File.WriteAllLines(path, CoreServices.defaultRegistry);
                        } catch(Exception e) {
                            Console.WriteLine($"An error occured during registry rebuild! Error: {e.Message}");
                        }
                        break;
                    case "reinstall":
                        Console.WriteLine("Reinstalling...");
                        try {
                            Directory.Delete(@"0:\MegaOS\",true);
                        } catch (Exception e) {
                            core.Log($"Failed to reinstall MegaOS! Error: {e.Message}", LogType.PanicGraphical);
                        }
                        Cosmos.System.Power.Reboot();
                        break;
                    case "quit":
                        running = false;
                        return;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }

        private void CheckFiles() {
            if (!File.Exists(@"0:\MegaOS\sys\registry.mreg")) {
                core.Log("Registry not found! Please run sfa in recovery mode!", LogType.PanicGraphical);
            }


            if (!File.Exists(@"0:\MegaOS\usr\users.sys")) {
                core.Log("Users not found! Please run sfa in recovery mode!", LogType.PanicGraphical);
            }
        }

    }
}
