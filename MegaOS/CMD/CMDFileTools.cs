using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MegaOS.CMD {
    internal class CMDFileTools {
        public static bool IsInMegaOSFolder(string path) {
            string megaOSFolder = Path.Combine(Kernel.path, "MegaOS");
            return path.StartsWith(megaOSFolder, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetFileType(string file) {
            if (file == null) return null;
            file = file.ToLower();
            if (file.EndsWith(".txt")) return "Text File";
            if (file.EndsWith(".mreg")) return "MegaOS Registry File";
            if (file.EndsWith(".sys")) return "System File";
            if (file.EndsWith(".snd")) return "Sound File";
            if (file.EndsWith(".mtxt")) return "MegaOS Text File";
            if (file.EndsWith(".efi")) return "Extensible Firmware Interface File";
            return Path.GetExtension(file) + " File";
        }
    }



    public class CMDListFiles : Command {
        public CMDListFiles(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            string filePath = Kernel.path;
            if (args.Length != 0) {
                filePath = Path.Combine(Kernel.path, args[0]);
            }

            string[] files = Directory.GetFiles(filePath);
            string[] dirs = Directory.GetDirectories(filePath);
            GL.WriteLine($"Directories ({dirs.Length}):", ConsoleColor.Green);
            foreach (string dir in dirs) {
                GL.WriteLine($"[{dir}]", ConsoleColor.Yellow);
            }
            GL.WriteLine($"Files ({files.Length}):", ConsoleColor.Green);
           
            foreach (string file in files) {
                string fileType = CMDFileTools.GetFileType(file);
                int filePadding = 40 - file.Length;
                GL.Write($"{file}", ConsoleColor.Yellow);
                GL.Write(new string(' ', filePadding));
                GL.WriteLine(fileType, ConsoleColor.Green);
            }

            return "";
        }
    }

    public class CMDDelete : Command {
        public CMDDelete(string n, string d) : base(n, d) { }

        public override string execute(string[] args) {
            string file = args[0];
            string r = "";
            if (CMDFileTools.IsInMegaOSFolder(Kernel.path)) {
                GL.WriteLine();
                GL.Write("Admin password: ");
               
            }
            if (File.Exists(Path.Combine(Kernel.path, file))) {
                File.Delete(Path.Combine(Kernel.path, file));
                r = "File deleted successfully!";
            } else {
                Beep.Warning();
                r = $"File '{args[0]}' does not exist!";
            }
            return r;
        }
    }

    public class CMDChangeDir : Command {
        public CMDChangeDir(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            string res = "";
            if (args.Length != 0) {
                if (args[0] == "") { }
                string newPath = Path.Combine(Kernel.path, args[0]);
                if (Directory.Exists(newPath)) {
                    Kernel.path = newPath;
                    res = "";
                } else {
                    Beep.Warning();
                    res = $"Directory {args[0]} doesn't exist!";
                }
            } else {
                string path = Kernel.path;
                if (path.EndsWith(@"\")) {
                    path = path.Substring(0, path.Length - 1);
                }
                int lastIndex = path.LastIndexOf(@"\");
                if (lastIndex <= 1)
                {
                    Beep.Warning();
                    res = "Already at root: " + path;
                } else if (lastIndex >= 0) {
                    string newPath = path.Substring(0, lastIndex + 1);
                    Kernel.path = newPath;
                    res = "";
                } else {
                    Beep.Warning();
                    res = "Path does not contain a backslash.";
                }
            }
            return res;
        }
    }

    public class CMDCreate : Command {
        public CMDCreate(string n, string d) : base(n, d) { }

        public override string execute(string[] args) {
            string file = args[0];
            string r = "";
            try {
                if (!File.Exists(Path.Combine(Kernel.path, file))) {
                    File.Create(Path.Combine(Kernel.path, file));
                    r = "File created successfully!";
                } else {
                    Beep.Warning();
                    r = $"File '{args[0]}' already exists!";
                }
            } catch(Exception e) {
                Beep.Warning();
                return $"An error occured whilst creating file {file} at {Path.Combine(Kernel.path, file)}! Error: {e.Message}";
            }
            return r;
        }
    }


    public class CMDDeleteDir : Command {
        public CMDDeleteDir(string n, string d) : base(n, d) { }

        public override string execute(string[] args) {
            string file = args[0];
            string r = "";
            if (Directory.Exists(Path.Combine(Kernel.path, file))) {
                Directory.Delete(Path.Combine(Kernel.path, file), true);
                r = "Directory deleted successfully!";
            } else {
                Beep.Warning();
                r = $"Directory '{args[0]}' does not exist!";
            }
            return r;
        }
    }

    public class CMDCreateDir : Command {
        public CMDCreateDir(string n, string d) : base(n, d) { }

        public override string execute(string[] args) {
            string file = args[0];
            string r = "";
            if (!Directory.Exists(Path.Combine(Kernel.path, file))) {
                Directory.Delete(Path.Combine(Kernel.path, file));
                r = "Directory created successfully!";
            } else {
                Beep.Warning();
                r = $"Directory '{args[0]}' already exists!";
            }
            return r;
        }
    }

    public class CMDRead : Command {
        public CMDRead(string n, string d) : base(n, d) { }

        public override string execute(string[] args) {
            string r = "";
            try {
                string[] d = System.IO.File.ReadAllLines(Kernel.path + @"\" + args[0]);
                foreach (string line in d) {
                    GL.WriteLine(line);
                }
                return r;
            } catch (Exception e) {
                Beep.Warning();
                return e.Message;
            }
        }
    }

    public class CMDCopy : Command {
        public CMDCopy(string n, string d) : base(n, d) { }

        public override string execute(string[] args) {
            string r = "";
            try {
                string file = args[0];
                string dest = args[1];
                File.Copy(file, dest, true);
                return r;
            } catch (Exception e) {
                Beep.Warning();
                return e.Message;
            }
        }
    }

    public class CMDMove : Command {
        public CMDMove(string n, string d) : base(n, d) { }

        public override string execute(string[] args) {
            string r = "";
            try {
                string file = args[0];
                string dest = args[1];
                File.Copy(file, dest, true);
                FileTools.Delete(file);
                return r;
            } catch (Exception e) {
                Beep.Warning();
                return e.Message;
            }
        }
    }

    public class CMDChangeLineInFile : Command {
        public CMDChangeLineInFile(string n, string d) : base(n, d) { }

        public override string execute(string[] args) {
            string r = "";
            try {
                string file = args[0];
                int line;
                if (int.TryParse(args[1], out line)) {
                    FileTools.ChangeLine(file, line, args[2]);
                } else {
                    Beep.Warning();
                    r = "Not a valid line number!";
                }
                return r;
            } catch (Exception e) {
                Beep.Warning();
                return e.Message;
            }
        }
    }

    public class CMDWrite : Command {
        public CMDWrite(string n, string d) : base(n, d) { }

        public override string execute(string[] args) {
            try {
                string file = args[0];
                string r = "";
                if (Directory.Exists(Path.Combine(Kernel.path, file))) {
                    File.WriteAllText(Path.Combine(Kernel.path, file), args[1]);
                } else {
                    Beep.Warning();
                    r = $"File '{args[0]}' does not exist!";
                }
                return r;
            } catch (Exception e) {
                Beep.Warning();
                return e.Message;
            }
        }
    }
}
