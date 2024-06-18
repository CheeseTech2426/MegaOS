using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS {
    public static class GL {
        public static ConsoleColor Foreground;
        public static ConsoleColor Background;
        public static ConsoleColor Bar;
        public static ConsoleColor BarHighlight;
        public static ConsoleColor BarDropdownMenu;

        private static ConsoleColor _foreground;
        private static ConsoleColor _background;

        public static readonly int Width = Console.WindowWidth;
        public static readonly int Height = Console.WindowHeight;

        public static void Setup() {
            Foreground = ConsoleColor.White;
            Background = ConsoleColor.Black;
            Bar = ConsoleColor.Blue;
            BarHighlight = ConsoleColor.White;
            BarDropdownMenu = ConsoleColor.Blue;
        }

        public static void Write(string t = "") {
            Console.Write(t);
        }

        public static void Write(string t, ConsoleColor fg) {
            SetFGColor(fg);
            Write(t);
            RestoreFG();
        }

        public static void Write(string t, ConsoleColor fg, ConsoleColor bg) {
            SetBGColor(bg);
            SetFGColor(fg);
            Write(t);
            RestoreBG();
            RestoreFG();
        }

        public static void Write(int x, int y, string t, ConsoleColor fg, ConsoleColor bg) {
            (int x1, int y1) = GetCursor();
            SetCursor(x, y);
            SetBGColor(bg);
            SetFGColor(fg);
            Write(t);
            RestoreBG();
            RestoreFG();
            SetCursor(x1, y1);
        }

        public static void WriteLine(string t, ConsoleColor fg) {
            SetFGColor(fg);
            WriteLine(t);
            RestoreFG();
        }

        public static void WriteLine(string t, ConsoleColor fg, ConsoleColor bg) {
            SetBGColor(bg);
            SetFGColor(fg);
            WriteLine(t);
            RestoreBG();
            RestoreFG();
        }

        public static void WriteLine(string t = "") {
            Write($"{t}\n");
        }

        public static void Clear() {
            Console.Clear();
        }

        public static int PrintDOS() {
            Write($"{Kernel.userLoggedIn}@{Kernel.pcname}:", ConsoleColor.Green);
            Write($"{Kernel.path}", ConsoleColor.Blue);
            if (Kernel.userLoggedIn.ToLower() == "admin") {
                Write("# ",ConsoleColor.White);
            } else {
                Write("$ ",ConsoleColor.White);
            }
            return $"{Kernel.userLoggedIn}@{Kernel.pcname}:{Kernel.path}# ".Length;
        }

        public static void DrawBar() {
            (int x, int y) = GetCursor();
            SetCursor(0, 0);
            Write(new string(' ', Width), ConsoleColor.Blue, ConsoleColor.Blue);
            string time = DateTime.Now.ToString("HH:mm:ss ddd dd.MM.yyyy");
            Write(Width - (time.Length + 2), 0, time, ConsoleColor.Yellow, ConsoleColor.Blue);
            Write(2, 0, CoreServices.fullVersionString, ConsoleColor.Magenta, ConsoleColor.Blue);
            SetCursor(x, y);
        }

        public static void DrawLineH(ConsoleColor color, int x1, int y1, int x2) {
            (int x, int y) = GetCursor();

            SetCursor(x1, y1);
            SetBGColor(color);
            for (int i = x1; i < x2; i++) {
                Write(" ");
            }
            SetCursor(x, y);
            RestoreBG();
        }

        public static List<int> DrawMenuBarOptions(string[] menu, int idx, int x = 0, int y = 1) {
            List<int> xpos = new List<int>();
            SetCursor(x, y);
            int i = 0;
            foreach (string s in menu) {
                if (i == idx) {
                    Write($" {s} ", ConsoleColor.White, ConsoleColor.White);
                } else {
                    Write($" {s} ", ConsoleColor.White, ConsoleColor.Blue);
                }
                xpos.Add(GetCursor().x - $" {s} ".Length);
                i++;
            }
            return xpos;
        }

        public static void FillScreen(ConsoleColor color) {
            SetBGColor(color);
            Clear();
            RestoreBG();
        }

        public static void DrawDropdownMenu(string[] menu, int idx, int x) {
            SetCursor(0, 1);
            for(int i = 2; i < menu.Length + 2; i++) {
                
                if (i - 2 == idx) {
                    SetCursor(x, i);
                    Write(new string(' ', 20));
                    SetCursor(x, i);
                    Write($" {menu[i - 2]} " + new string(' ', 20 - $" {menu[i - 2]} ".Length), ConsoleColor.White, ConsoleColor.White);
                } else {
                    SetCursor(x, i);
                    Write(new string(' ', 20));
                    SetCursor(x, i);
                    Write($" {menu[i - 2]} " + new string(' ', 20 - $" {menu[i - 2]} ".Length), ConsoleColor.White, ConsoleColor.Blue);
                }
            }
        }

        public static void CloseDropdownMenu(string[] menu, int x) {
            SetCursor(0, 1);
            for (int i = 2; i < menu.Length + 2; i++) {
                SetCursor(x, i);
                Write(new string(' ', 20));
            }
        }

        public static int GetLongest(string[] array) {
            if (array == null || array.Length == 0) {
                return 0; // Return 0 if the array is null or empty
            }

            int maxLength = array[0].Length; // Initialize maxLength with the length of the first element

            for (int i = 1; i < array.Length; i++) {
                if (array[i].Length > maxLength) {
                    maxLength = array[i].Length; // Update maxLength if a longer element is found
                }
            }

            return maxLength;
        }

        public static void SetFGColor(ConsoleColor fg) {
            _foreground = Console.ForegroundColor;
            Console.ForegroundColor = fg;
            Foreground = fg;
        }

        public static void RestoreFG() {
            Console.ForegroundColor = _foreground;
            Foreground = _foreground;
        }

        public static void SetBGColor(ConsoleColor fg) {
            _background = Console.BackgroundColor;
            Console.BackgroundColor = fg;
            Background = fg;
        }

        public static void RestoreBG() {
            Console.BackgroundColor = _background;
            Background = _background;
        }

        public static (int x,int y) GetCursor() {
            return Console.GetCursorPosition();
        }

        public static void SetCursor(int x,int y) {
            Console.SetCursorPosition(x, y);
        }
    }
}
