using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS.CMD
{
    public class Command
    {
        public readonly string name;
        public readonly string desc;
        
        public Command(string name, string desc) {
            this.name = name;
            this.desc = desc;
            
        }

        public virtual string execute(string[] args) { return ""; }
    }
}
