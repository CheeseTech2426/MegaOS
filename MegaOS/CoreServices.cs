using MegaOS.CMD;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS {
    internal class CoreServices {
        public static string fullVersionString = "MegaOS Nova v1.0";
        public void Log(string message, LogType type, bool playSound = true) {
            switch (type) {
                case LogType.Info:
                    Write("[INFO] ", ConsoleColor.Blue, ConsoleColor.Black);
                    Write($"{message}\n", ConsoleColor.White, ConsoleColor.Black);
                    break;
                case LogType.OK:
                    Write("[OK] ", ConsoleColor.Green, ConsoleColor.Black);
                    Write($"{message}\n", ConsoleColor.White, ConsoleColor.Black);
                    break;
                case LogType.Error:
                    if(playSound)Beep.Warning();
                    Write("[ERROR] ", ConsoleColor.Red, ConsoleColor.Black);
                    Write($"{message}\n", ConsoleColor.White, ConsoleColor.Black);
                    break;
                case LogType.Warning:
                    Write("[WARNING] ", ConsoleColor.Yellow, ConsoleColor.Black);
                    Write($"{message}\n", ConsoleColor.White, ConsoleColor.Black);
                    break;
                case LogType.Panic:
                    if(playSound)Beep.Crash();
                    Write("[PANIC] ", ConsoleColor.Red, ConsoleColor.Black);
                    Write($"{message}\n", ConsoleColor.White, ConsoleColor.Black);
                    Cosmos.Core.CPU.Halt();
                    break;
                case LogType.PanicGraphical:
                    GL.Setup();
                    GL.Clear();
                    GL.FillScreen(ConsoleColor.Blue);
                    GL.SetCursor(0, 0);
                    GL.Write("\n\n\n", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("        _                                                                       ", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("       / )                                                                      ", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("   _  / /                                                                       ", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("  (_)( (                                                                        ", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("     | |                                                                        ", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("   _ ( (                                                                        ", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("  (_) \\ \\                                                                       ", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("       \\_)                                                                      ", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("\n", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("     Oh no! Some part of the code caused a kernel panic!\n", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("     That's not ideal! \n", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write("     Please reboot your computer and try again!\n", ConsoleColor.White, ConsoleColor.Blue);
                    GL.Write($"     {message}\n", ConsoleColor.White, ConsoleColor.Blue);
                    while (true) { }
            }
        }

        private void Write(string text, ConsoleColor fg, ConsoleColor bg) {
            ConsoleColor oldBG, oldFG;
            oldBG = Console.BackgroundColor;
            oldFG = Console.ForegroundColor;
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.Write(text);
            Console.ForegroundColor = oldFG;
            Console.BackgroundColor = oldBG;
        }


        public void CheckSystem(bool bypassCheck = false) {
            GL.WriteLine("Please insert your MegaOS install CD into drive 1...");
            Console.ReadKey(true);
            if (!Directory.Exists(@"1:\") && !bypassCheck) {
                CheckSystem();
            }
            CopyFiles(2);
        }

        public static void Installer(string user, string password) {
            CoreServices core = new CoreServices();
            // ui
            GL.Clear();
            GL.Write(0, 0, new string(' ', GL.Width), ConsoleColor.Blue, ConsoleColor.Blue);
            GL.Write((GL.Width / 2) - "MegaOS Setup Wizard | Installing".Length, 0, "MegaOS Setup Wizard | Installing", ConsoleColor.White, ConsoleColor.Blue);
            GL.Write(2, 2, "All set! Now relax while your new OS installs! This won't take a minute!", ConsoleColor.White, ConsoleColor.Black);
            // check version

            GL.WriteLine("\n\n");
            core.Log("Copying files...", LogType.Info, false);
            Directory.CreateDirectory(@"0:\MegaOS\");
            CopyFiles(0);

            core.Log($@"Copied installer.sys to 0:\MegaOS\sys\", LogType.OK, false);

            core.Log("Files copied successfully!", LogType.OK, false);
            // users
            core.Log("Creating users....", LogType.Info, false);
            Kernel.user = new UserManager();
            Kernel.user.AddUser(user, password, "admin");
            // registry
            core.Log("Checking registry", LogType.Info, false);
            Kernel.registry = new Registry();
            Kernel.registry.CheckRegistry(Kernel.registry.registry, @"1:\Installer\registry.mreg");
            // complete
            core.Log("Installation complete! Press any key to reboot...", LogType.OK, false);
            Console.ReadKey(true);
            Cosmos.System.Power.Reboot();
        }

        public static string[] defaultRegistry = {
            "[MEGAOS]",
            @"version=1.0",
            @"[SOUND]",
            @"startup=0:\MegaOS\sound\startup.snd",
            @"shutdown = 0:\MegaOS\sound\shutdown.snd",
            @"warning = 0:\MegaOS\sound\warning.snd",
            @"beep = 0:\MegaOS\sound\beep.snd",
            @"error = 0:\MegaOS\sound\error.snd",
            @"crash = 0:\MegaOS\sound\crash.snd",
        };

        private static void CopyFiles(int upgrade = 0) {
            CoreServices core = new CoreServices();
            string installerPath = @"1:\installer.sys"; // Path to the installer map file
            string systemPath = @"0:\MegaOS\"; // Path to the system root folder

            try {
                string[] lines = File.ReadAllLines(installerPath);

                foreach (string line in lines) {
                    // Skip empty lines or lines starting with comment character
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("//")) {
                        continue;
                    }


                    string destinationPath = line.Replace("{0}", systemPath);
                    string destinationDirectory = Path.GetDirectoryName(destinationPath);

                    if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory)) {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    string fileName = Path.GetFileName(destinationPath);
                    string sourceFile = Path.Combine(@"1:\Installer\", fileName);

                    if (upgrade == 1) {
                        if (!File.Exists(sourceFile)) {
                            File.Copy(sourceFile, destinationPath, true);
                            core.Log($"Copied {fileName} to {destinationPath}", LogType.OK, false);
                        } else {
                            core.Log($"File {fileName} exists!", LogType.OK);
                        }
                    } else if (upgrade == 2) {
                        if (!File.Exists(sourceFile)) {
                            File.Copy(sourceFile, destinationPath, true);
                            core.Log($"Copied {fileName} to {destinationPath}", LogType.OK, false);
                        } else {
                            core.Log($"File {fileName} exists!", LogType.OK);
                        }
                    } else {
                        if (File.Exists(sourceFile)) {
                            core.Log("Running cp...", LogType.OK);
                            File.Copy(sourceFile, destinationPath, true);
                            core.Log($"Copied {fileName} to {destinationPath}", LogType.OK, false);
                        } else {
                            core.Log($"Source file not found: {sourceFile}. Press any key to continue...", LogType.Error);
                            Console.ReadKey(true);
                        }
                    }
                }

            } catch (Exception ex) {
                core.Log($"Error during installation: {ex.Message}",LogType.PanicGraphical);
            }
        }

        private static bool CheckVersion() {
            if (File.Exists(@"0:\MegaOS\sys\version.sys")) {
                string[] ver1 = File.ReadAllText(@"0:\MegaOS\sys\version.sys").Split("=");
                string[] ver2 = File.ReadAllText(@"1:\Installer\version.sys").Split("=");
                if (ver1[1] != ver2[1]) {
                    char first1 = ver1[1][0];
                    char first2 = ver2[1][0];
                    if (int.TryParse(first1.ToString(), out int a)) {
                        if (int.TryParse(first2.ToString(), out int b)) {
                            if (a > b) {
                                GL.WriteLine("Installed version is newer. Continue with installation? (y/N) ");
                                ConsoleKeyInfo key = Console.ReadKey(true);
                                if (key.Key != ConsoleKey.Y) { Cosmos.System.Power.Reboot(); } else {
                                    return false;
                                }
                            } else {
                                GL.WriteLine("Installed version is older. Upgrade? (y/N) ");
                                ConsoleKeyInfo key = Console.ReadKey(true);
                                if (key.Key != ConsoleKey.Y) {
                                    Directory.Delete(@"0:\MegaOS\");
                                    return false;
                                } else {
                                    return true;
                                }
                            }
                        } else {
                            return false;
                        }
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            } else {
                return false;
            }
            return false;
        }
    }
}




public enum LogType {
    Info,
    OK,
    Warning,
    Error,
    Panic,
    PanicGraphical
}
