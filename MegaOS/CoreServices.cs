using MegaOS.CMD;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS {
    internal class CoreServices {
        public static string fullVersionString = $"MegaOS ";
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

        public static void Setup() {
            fullVersionString = $"MegaOS {Kernel.registry.GetValue("MEGAOS", "versionname")} {Kernel.registry.GetValue("MEGAOS", "version")}";
        }

        public void CheckSystem(bool bypassCheck = false) {
            if(!bypassCheck)GL.WriteLine("Please insert your MegaOS install CD into drive 1...");
            Console.ReadKey(true);
            if (!Directory.Exists(@"1:\") && !bypassCheck) {
                CheckSystem();
            }
            CopyFiles(2);

        }

        public static void Installer(string user, string password, string pcname) {
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

            core.Log("Files copied successfully!", LogType.OK, false);
            // users
            core.Log("Creating users....", LogType.Info, false);
            Kernel.user = new UserManager();
            Kernel.user.AddUser(user, password, "admin");
            // registry
            core.Log("Checking registry", LogType.Info, false);
            Kernel.registry = new Registry();
            core.Log("Writing registry...", LogType.Info);
            File.WriteAllLines(Kernel.registry.registry, defaultRegistry);
            CMDChangeLineInFile ch = new CMDChangeLineInFile("ch", "ch");
            string[] args = { Kernel.registry.registry, "3", $"pcname={pcname}" };
            ch.execute(args);
            // complete
            core.Log("Installation complete! Press any key to reboot...", LogType.OK, false);
            Console.ReadKey(true);
            Cosmos.System.Power.Reboot();
        }

        public static string[] defaultRegistry = {
            "[MEGAOS]",
            @"version=1.1",
            @"versionname=Nova",
            @"pcname=megaos",
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
                            File.Copy(sourceFile, destinationPath, true);
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
