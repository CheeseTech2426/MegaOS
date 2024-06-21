using Cosmos.Debug.Kernel.Plugs.Asm;
using MegaOS.CMD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS.CMD
{
    internal class CMDManager {
        public List<Command> commands;
        public List<string> args;
        public CMDManager() {
            commands = new List<Command>();
            commands.Add(new CMDClear("cls", "Clears the screen"));
            commands.Add(new CMDHelp("help", "Lists all the commands"));
            commands.Add(new CMDChangeDir("cd", "Usage: cd [path] - Change the current directory"));
            commands.Add(new CMDListFiles("ls", "Usage: ls [path] - List files in a directory"));
            commands.Add(new CMDCreate("touch", "Usage: touch [name] - Creates an empty file"));
            commands.Add(new CMDDelete("rm", "Usage: rm [file] - Deletes a specified file"));
            commands.Add(new CMDCreateDir("mkdir", "Usage: mkdir [name] - Creates a directory"));
            commands.Add(new CMDDeleteDir("rmdir", "Usage: rmdir [name] - Deletes a directory"));
            commands.Add(new CMDRead("cat", "Usage: cat [file] - Reads a file"));
            commands.Add(new CMDWrite("write", "Usage: write [file] [text] - Writes to a file"));
            commands.Add(new CMDCopy("cp", "Usage: cp [target] [destination] - Copies a file"));
            commands.Add(new CMDMove("mv", "Usage: cp [target] [destination] - Moves a file"));
            commands.Add(new CMDChangeLineInFile("ch", "Usage: ch [file] [line #] [new line] - Changes specific line in a file"));
            commands.Add(new CMDEdit("edit", "MegaOS text editor"));
            commands.Add(new CMDNeofetch("neofetch", "A neofetch clone"));
            commands.Add(new CMDShutdown("shutdown", "Turns the computer off"));
            commands.Add(new CMDReboot("reboot", "Restarts the computer"));
            commands.Add(new CMDUsers("users", "User Manager"));
            commands.Add(new CMDPlayer("player", "SND file player"));
            commands.Add(new CMDLogoff("logout", "End the current user's session"));
            commands.Add(new CMDSystemMaintenance("sys", "The system maintenance utility"));
            commands.Add(new CMDDiskUtility("diskutil", "The MegaOS default disk utility"));
            commands.Add(new CMDTask("taskman", "Task Manager"));
            commands.Add(new CMDShell("shell", "Launches the text based Shell (beta)"));
        }

        public string Process(string input) {
            string[] split = input.Split(' ');
            string label = split[0];
            args = new List<string>();
            int ctr = 0;
            foreach (string s in split) {
                if (ctr != 0) {
                    args.Add(s);
                }
                ++ctr;
            }
            foreach(Command cmd in commands) {
                if(cmd.name == label) {
                    string s = cmd.execute(args.ToArray());
                    GL.DrawBar();
                    if(s == "") {
                        return "";
                    } else {
                        return s;
                    }
                }
            }
            Beep.Warning();
            return $"Invalid command '{label}'!";
        }
    }

    public class CMDClear : Command {
        public CMDClear(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            GL.Clear();
            GL.DrawBar();
            return "";
        }
    }
    public class CMDShell : Command {
        public CMDShell(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            Shell.Shell shell = new Shell.Shell();
            return "";
        }
    }

    public class CMDHelp : Command {
        public CMDHelp(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            CMDManager cm = new CMDManager();
            foreach(Command cmd in cm.commands) {
                GL.WriteLine($"- {cmd.name}: {cmd.desc}");
            }
            return "";
        }
    }
    public class CMDSystemMaintenance : Command {
        public CMDSystemMaintenance(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            CoreServices core = new CoreServices();
            switch (args[0]) {
                case "sfa":
                    core.CheckSystem();
                    break;
                case "checkreg":
                    Kernel.registry.CheckRegistry();
                    break;
                case "startsetup":
                    OOBE oobe = new OOBE();
                    oobe.Run(false);
                    break;
                case "dtrace":
                    core.Log("dtrace", LogType.PanicGraphical);
                    break;
                case "network":
                    NetworkTest network = new NetworkTest();
                    network.Run();
                    break;
                default:
                    GL.WriteLine("Usage");
                    GL.WriteLine("- sfa - Checks system file integrity. Requires MegaOS Install CD");
                    GL.WriteLine("- checkreg - Checks registry for any errors or corruptions");
                    GL.WriteLine("- dtrace - Simulates a kernel panic");
                    break;
            }
            return "";
        }
    }
}


