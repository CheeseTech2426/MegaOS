using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS.CMD {
    internal class CMDShutdown : Command {
        public CMDShutdown(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            DateTime now = DateTime.Now;
            int seconds = 5;
            GL.WriteLine("Your computer will shutdown after 5 seconds. Press space key to cancel.");
            while ((DateTime.Now - now).TotalSeconds < seconds) {
                Cosmos.System.KeyEvent k;
                if (Cosmos.System.KeyboardManager.TryReadKey(out k)) {
                    if (k.Key == Cosmos.System.ConsoleKeyEx.Spacebar) return "Cancelled.";
                }
            }
            Beep.Shutdown();
            Cosmos.System.Power.Shutdown();
            return "";
        }
    }

    internal class CMDReboot : Command {
        public CMDReboot(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            DateTime now = DateTime.Now;
            int seconds = 5;
            GL.WriteLine("Your computer will restart after 5 seconds. Press space key to cancel.");
            while ((DateTime.Now - now).TotalSeconds < seconds) {
                Cosmos.System.KeyEvent k;
                if (Cosmos.System.KeyboardManager.TryReadKey(out k)) {
                    if (k.Key == Cosmos.System.ConsoleKeyEx.Spacebar) return "Cancelled.";
                }
            }
            Beep.Shutdown();
            Cosmos.System.Power.Reboot();
            return "";
        }
    }
}
