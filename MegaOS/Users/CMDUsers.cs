using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS.CMD {
    internal class CMDUsers : Command {
        public CMDUsers(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            switch (args[0]) {
                case "list":
                    Kernel.user.ListUsers();
                    break;
                case "create":
                    Kernel.user.AddUser(args[1], args[2], args[3]);
                    break;
                case "load":
                    Kernel.user.LoadUsers();
                    break;
                case "save":
                    Kernel.user.SaveUsers();
                    break;
                case "cat":
                    CMDManager cm = new CMDManager();
                    cm.Process(@"cat MegaOS\usr\users.sys");
                    break;
                default:
                    GL.WriteLine("Usage:\n- list - Lists all the users\n- create [username] [password] [permission] - Creates an user\n- load - Loads users\n- save - Saves users");
                    break;
            }
            return "";
        }
    }

    public class CMDLogoff : Command {
        public CMDLogoff(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            Kernel.user.Login();
            return "";
        }
    }
}
