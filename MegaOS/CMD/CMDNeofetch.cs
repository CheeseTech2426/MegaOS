using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS.CMD {
    internal class CMDNeofetch : Command {
        public CMDNeofetch(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            GL.Clear();
            GL.SetCursor(0, 2);
            GL.Write("   __  ___              ____  ____\r\n  /  |/  /__ ___ ____ _/ __ \\/ __/\r\n / /|_/ / -_) _ `/ _ `/ /_/ /\\ \\  \r\n/_/  /_/\\__/\\_, /\\_,_/\\____/___/  \r\n           /___/                  ");
            int x = 35;
            GL.SetCursor(x, 2);
            GL.Write($"{Kernel.userLoggedIn}@{Kernel.pcname}", ConsoleColor.Red);
            GL.SetCursor(x, 3);
            GL.Write(new string('-', $"{Kernel.userLoggedIn}@{Kernel.pcname}".Length), ConsoleColor.Red);
            GL.SetCursor(x, 4);
            GL.Write("OS: ", ConsoleColor.Green);
            GL.Write(CoreServices.fullVersionString); 
            GL.SetCursor(x, 5);
            GL.Write("Host: ", ConsoleColor.Green);
            GL.Write(GetComputer(Cosmos.Core.CPU.GetCPUBrandString()));
            GL.SetCursor(x, 6);
            GL.Write("Uptime: ", ConsoleColor.Green);
            string mins = uptime()[0];
            string secs = uptime()[1];
            GL.Write($"{mins}:{secs} minutes");
            GL.SetCursor(x, 7);
            GL.Write("Shell: ", ConsoleColor.Green);
            GL.Write("MegaOS Shell v1.0 Beta");
            GL.SetCursor(x, 8);
            GL.Write("Resolution: ", ConsoleColor.Green);
            GL.Write($"{GL.Width}x{GL.Height}");
            GL.SetCursor(x, 9);
            GL.Write("CPU: ", ConsoleColor.Green);
            GL.Write(GetCPU());
            GL.SetCursor(x, 10);
            GL.Write("Memory: ", ConsoleColor.Green);
            GL.Write($"{Memory()[0]}MB/{Memory()[1]}MB");
            GL.SetCursor(0, 11);
            return "";

        }

        private string GetCPU() {
            string cpu = Cosmos.Core.CPU.GetCPUBrandString();
            cpu.Replace("(TM)", "(R)");
            return cpu;
        }

        private string[] uptime() {
            int uptime = Convert.ToInt32(Cosmos.Core.CPU.GetCPUUptime() / 1000 / 1000 / 1000 / 5); //Get system uptime and convert it to seconds
            int sec = uptime % 60;
            int mins = (uptime - sec) / 60;
            string secs = sec.ToString();
            if (secs.Length != 2) //If seconds is not double digit, add a 0 to front to keep M:SS format.
            {
                secs = "0" + secs;
            }
            return new string[2] { mins.ToString(), secs };
        }

        private string[] Memory() {
            uint max = Cosmos.Core.CPU.GetAmountOfRAM();
            ulong available = Cosmos.Core.GCImplementation.GetAvailableRAM();
            ulong used = max - available;
            return new string[2] { used.ToString(), max.ToString() };
        }

        private string GetComputer(string cpu) {
            if (cpu.ToLower().Contains("i3")) { 
                return ("Intel Core i3 Machine"); } 
            else if (cpu.ToLower().Contains("i5")) {
                return ("Intel Core i5 Machine"); } 
            else if (cpu.ToLower().Contains("i7")) {
                return ("Intel Core i7 Machine"); } 
            else if (cpu.ToLower().Contains("i9")) { 
                return ("Intel Core i9 Machine"); } 
            else if (cpu.ToLower().Contains("pentium"))
                { return ("Intel Pentium Machine"); }
            else if (cpu.ToLower().Contains("celeron")) 
                { return ("Intel Celeron Machine"); } 
            else if (cpu.ToLower().Contains("xeon")) {
                return ("Intel Xeon Machine"); } 
            else if (cpu.ToLower().Contains("ryzen 3")) 
                { return ("AMD Ryzen 3 Machine"); } 
            else if (cpu.ToLower().Contains("ryzen 5"))
                { return ("AMD Ryzen 5 Machine"); } 
            else if (cpu.ToLower().Contains("ryzen 7"))
                { return ("AMD Ryzen 7 Machine"); } 
            else if (cpu.ToLower().Contains("ryzen 9")) 
                { return ("AMD Ryzen 9 Machine"); 
            } else if (cpu.ToLower().Contains("epyc"))
                { return ("AMD EPYC Machine"); } 
            else if (cpu.ToLower().Contains("threadripper"))
                { return ("AMD Threadripper Machine"); }
            else { return ("Other/Unknown Machine"); }
        }
    }

    /* 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
}
