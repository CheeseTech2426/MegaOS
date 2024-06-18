using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaOS.CMD;

namespace MegaOS {
    internal class MusicPlayer {
        public void Play(string file) {
            if (!File.Exists(file)) {
                Beep.Warning();
                GL.WriteLine("File doesn't exist!"); return; }
            if (!file.EndsWith(".snd")) {
                Beep.Warning();
                GL.WriteLine("File is not a sound file!");
                return;
            }

            string lines = File.ReadAllText(file);
            string[] notes = lines.Split(';');
            foreach (string note in notes) {
                string[] parts = note.Split(',');
                if (int.TryParse(parts[0], out int freq)) {
                    if (int.TryParse(parts[1], out int dur)) {
                        Cosmos.System.PCSpeaker.Beep((uint)freq, (uint)dur);
                    } else {
                        continue;
                    }
                } else {
                    continue;
                }
            }
            return;
        }
    }

    public class CMDPlayer : Command {
        public CMDPlayer(string n, string d) : base(n, d) { }
        public override string execute(string[] args) {
            MusicPlayer music = new MusicPlayer();
            music.Play(args[0]);
            return "";
        }
    }

    public class Beep {
        static MusicPlayer music = new MusicPlayer();

        public static void Startup() {
            music.Play(Kernel.registry.GetValue("SOUND", "startup"));
            return;
        }

        public static void Shutdown() {
            music.Play(Kernel.registry.GetValue("SOUND", "shutdown"));
            return;
        }

        public static void Warning() {
            music.Play(Kernel.registry.GetValue("SOUND", "warning"));
            return ;
        }

        public static void Default() {
            music.Play(Kernel.registry.GetValue("SOUND", "beep"));
            return;
        }

        public static void Error() {
            music.Play(Kernel.registry.GetValue("SOUND", "error"));
            return;
        }

        public static void Crash() {
            music.Play(Kernel.registry.GetValue("SOUND", "crash"));
        }
    }
}
