using Cosmos.System.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS.CMD {
    internal class CMDDiskUtility : Command{
        public CMDDiskUtility(string n,string d) : base(n,d) { }
        public override string execute(string[] args) {
            DiskUtility du = new DiskUtility();
            List<Disk> disks = Kernel.vfs.GetDisks();
            int id;
            switch (args[0]) {
                case "disks":
                    du.GetDisks();
                    break;
                case "mount":
                    if (int.TryParse(args[1], out id)) {
                        du.MountDrive(disks[id]);
                        break;
                    } else {
                        Beep.Warning();
                        return $"Diskutil: Error mounting disk! ...{args[0]} '{args[1]}' is not a number!";
                    }
                case "format":
                    if (int.TryParse(args[2], out id)) {
                        du.FormatDisk(disks[id]);
                        break;
                    } else {
                        Beep.Warning();
                        return $"Diskutil: Error formatting disk! Reason: ...{args[0]} '{args[1]}' is not a number!";
                    }
                case "partitions":
                    try {
                        switch (args[1]) {
                            case "get":
                                if (int.TryParse(args[2], out id)) {
                                    du.GetPartitions(disks[id]);
                                    break;
                                } else {
                                    Beep.Warning();
                                    return $"Diskutil: Error getting partitions! '{args[2]}' is not a number!";
                                }
                            case "format":
                                if (int.TryParse(args[2], out id)) {
                                    int parId;
                                    if (int.TryParse(args[3], out parId)) {
                                        du.FormatPartition(disks[id], parId, false);
                                        break;
                                    } else {
                                        Beep.Warning();
                                        return $"Diskutil: Error formatting partition! Reason: ...{args[2]} '{args[3]}' is not a number!";
                                    }
                                } else {
                                    Beep.Warning();
                                    return $"Diskutil: Error formatting partition! Reason: ...{args[1]} '{args[2]}' is not a number!";
                                }
                            case "delete":
                                if (int.TryParse(args[2], out id)) {
                                    int parId;
                                    if (int.TryParse(args[3], out parId)) {
                                        du.DeletePartition(disks[id], parId);
                                        break;
                                    } else {
                                        Beep.Warning();
                                        return $"Diskutil: Error deleting partition! Reason: ...{args[2]} '{args[3]}' is not a number!";
                                    }
                                } else {
                                    Beep.Warning();
                                    return $"Diskutil: Error deleting partition! Reason: ...{args[1]} '{args[2]}' is not a number!";
                                }
                            case "create":
                                if (int.TryParse(args[2], out id)) {
                                    int parId;
                                    if (int.TryParse(args[3], out parId)) {
                                        du.CreatePartition(disks[id], parId);
                                        break;
                                    } else {
                                        Beep.Warning();
                                        return $"Diskutil: Error deleting partition! Reason: ...{args[2]} '{args[3]}' is not a number!";
                                    }
                                } else {
                                    Beep.Warning();
                                    return $"Diskutil: Error deleting partition! Reason: ...{args[1]} '{args[2]}' is not a number!";
                                }
                            case "createfs":
                                break;
                            default:
                                Beep.Warning();
                                return $"Invalid argument {args[1]}! Valid arguments are create, delete, get and format!";

                        }
                    } catch (Exception e) {
                        Beep.Warning();
                        return $"Diskutil: An error occured! Reason: {e.Message}";
                    }
                    break;
                default:
                    Beep.Warning();
                    return $"Invalid argument {args[0]}!";
            }
            return "";
        }
    }

    public class DiskUtility {

        public void GetPartitions(Disk disk, int x = 0) {
            try {
                List<ManagedPartition> partitions = disk.Partitions;
                int i = 0;
                foreach (ManagedPartition partition in partitions) {
                    if (x > 0) GL.SetCursor(x, GL.GetCursor().y);
                    GL.WriteLine($"Partition {i}:", ConsoleColor.Yellow);
                    if (x > 0) GL.SetCursor(x, GL.GetCursor().y);
                    GL.WriteLine($"    Host: {partition.Host}", ConsoleColor.Blue);
                    if (x > 0) GL.SetCursor(x, GL.GetCursor().y);
                    GL.WriteLine($"    Root path: {partition.RootPath}", ConsoleColor.Blue);
                    if (x > 0) GL.SetCursor(x, GL.GetCursor().y);
                    GL.WriteLine($"    Has filesystem: {partition.HasFileSystem}", ConsoleColor.Blue);
                    if (x > 0) GL.SetCursor(x, GL.GetCursor().y);
                    i++;
                }
            } catch (Exception e) {
                Beep.Warning();
                GL.WriteLine( $"Error getting partitions! Reason: {e.Message}");
                return;
            }
            GL.WriteLine( "Successfully got partitions!");
        }

        public void GetDisks() {
            List<Disk> disks = Kernel.vfs.GetDisks();
            int idx = 0;
            try {
                foreach (Disk d in disks) {
                    GL.WriteLine($"Disk {idx}:");
                    d.DisplayInformation();
                    idx++;
                }
            } catch (Exception e) {
                Beep.Warning();
                GL.WriteLine($"Error getting disks! Reason: {e.Message}") ;
                return;
            }
            GL.WriteLine("Successfully got disks!");
        }

        public void CreatePartition(Disk disk, int size) {
            GL.WriteLine($"Creating partition with size {size}...");
            try {
                disk.CreatePartition(size);
            } catch(Exception e) {
                Beep.Warning();
                GL.WriteLine($"An error occured whilst creating partition! Error: {e.Message}");
                return;
            }

            GL.WriteLine("Successfully created partition!");
        }

        public void DeletePartition(Disk disk, int idx) {
            try {
                GL.Write("Are you sure? This process CANNOT be undone! (y/N) ");
                string input = Console.ReadLine().ToLower();
                if (input == "yes" || input == "y") {
                    GL.WriteLine("Deleting partition... This may take a while!");
                    disk.DeletePartition(idx);
                } else {
                    Beep.Warning();
                    GL.WriteLine("Operation cancelled!");
                    return;
                }
                
            } catch (Exception e) {
                Beep.Warning();
                GL.WriteLine($"Error deleting partition! The partition might be unbootable or corrupt! Reason: {e.Message}") ;
                return;
            }
            GL.WriteLine("Successfully deleted partition!");
        }

        public void FormatPartition(Disk disk, int idx, bool quick) {
            try {
                GL.Write("Are you sure? This process CANNOT be undone! (y/N) ");
                string input = Console.ReadLine().ToLower();
                if (input == "yes" || input == "y") {
                    GL.WriteLine("Formatting... This may take a while!");
                    disk.FormatPartition(idx, "FAT32", quick);
                } else {
                    Beep.Warning();
                    GL.WriteLine("Operation cancelled!");
                    return;
                }
                
            } catch (Exception e) {
                Beep.Warning();
                GL.WriteLine($"Error formating partition! The partition might be unbootable or corrupt! Reason: {e.Message}");
                return;
            }
            GL.WriteLine("Successfully formatted partition!") ;
        }

        public void MountDrive(Disk disk) {
            GL.WriteLine("Mounting drive...");
            try {
                disk.Mount();
            } catch (Exception e) {
                Beep.Warning();
                GL.WriteLine( $"Error mounting drive! Reason: {e.Message}");
            }
            GL.WriteLine("Successfully mounted disk!");
        }

        public void FormatDisk(Disk disk) {
            GL.Write("WARNING! All the data on the disk WILL be erased! Continue? (y/N) ");
            string input = Console.ReadLine().ToLower();
            if(input == "y" || input == "yes") {
                GL.WriteLine("Formatting... This may take a while. Do not turn your computer off!");
                try {
                    disk.Clear();
                    GL.WriteLine("Creating partition...");
                    CreatePartition(disk, 512);
                    FormatPartition(disk, 0, false);
                    GL.WriteLine("Mounting...");
                    disk.MountPartition(0);

                } catch (Exception e) {
                    Beep.Warning();
                    GL.WriteLine($"An error occured whilst formatting disk! Error: {e.Message}");
                    return;
                }
            } else {
                Beep.Warning();
                GL.WriteLine("Operation cancelled.");
                return;
            }

            GL.WriteLine("Successfully formatted disk.");
        }
    }
}
