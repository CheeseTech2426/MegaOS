using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS {
    internal class FileTools {
        public static bool Create(string path) {
            try {
                if (FileExists(path)) {
                    GL.WriteLine($"Failed to create file {path}. Reason: File already exists!");
                    return false;
                }
                File.Create(path);
                return true;
            } catch (Exception e) {
                GL.WriteLine($"Fileman: {e.Message}");
                return false;
            }
        }

        public static bool Delete(string path) {
            try {
                if (!FileExists(path)) {
                    GL.WriteLine($"Failed to delete file {path}. Reason: File does not exist!");
                    return false;
                }
                File.Delete(path);
                return true;
            } catch (Exception e) {
                GL.WriteLine($"Fileman: {e.Message}");
                return false;
            }
        }

        public static bool CreateDir(string path) {
            try {
                if (DirExists(path)) {
                    GL.WriteLine($"Failed to create directory {path}. Reason: Directory already exists!");
                    return false;
                }
                Directory.CreateDirectory(path);
                return true;
            } catch (Exception e) {
                GL.WriteLine($"Fileman: {e.Message}");
                return false;
            }
        }

        public static bool DeleteDir(string path) {
            try {
                if (!DirExists(path)) {
                    GL.WriteLine($"Failed to delete directory {path}. Reason: Directory does not exist!");
                    return false;
                }
                Directory.Delete(path);
                return true;
            } catch (Exception e) {
                GL.WriteLine($"Fileman: {e.Message}");
                return false;
            }
        }


        public static bool ChangeLine(string filePath, int lineNumber, string newContent) {
            try {
                string[] lines = File.ReadAllLines(filePath);
                if (lineNumber < 0 || lineNumber >= lines.Length) {
                    GL.WriteLine("Error: Line number is out of range.");
                    return false;
                }
                lines[lineNumber] = newContent;
                File.WriteAllLines(filePath, lines);
                return true;
            } catch (Exception ex) {
                GL.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public static void RemoveLine(string filePath, string lineToRemove) {
            try {
                string tempFile = Path.GetTempFileName();

                using (StreamReader reader = new StreamReader(filePath))
                using (StreamWriter writer = new StreamWriter(tempFile)) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        if (line != lineToRemove)
                            writer.WriteLine(line);
                    }
                }

                File.Delete(filePath);
                File.Move(tempFile, filePath);
            } catch (Exception ex) {
                Console.WriteLine($"Error removing line from file: {ex.Message}");
            }
        }

        public static string GetLine(string filePath, int lineNumber) {
            try {
                using (StreamReader reader = new StreamReader(filePath)) {
                    for (int currentLine = 1; currentLine < lineNumber; currentLine++) {
                        string line = reader.ReadLine();
                        if (line == null) {
                            // If the specified line number is beyond the end of the file,
                            // return null or throw an exception, depending on your requirements
                            return null;
                        }
                    }

                    // Read the line at the specified line number
                    return reader.ReadLine();
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return null;
            }
        }

        public static bool FileExists(string path) {
            if (File.Exists(path)) {
                return true;
            }
            return false;
        }

        public static bool DirExists(string path) {
            if (Directory.Exists(path)) {
                return true;
            }
            return false;
        }
    }
}
