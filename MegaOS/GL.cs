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
        public static ConsoleColor DialogTop;

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
            DialogTop = ConsoleColor.Blue;
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

        public static List<int> DrawMenuBarOptions(string[] menu, int idx, int x = 0, int y = 1, bool restore = false) {
            List<int> xpos = new List<int>();
            (int x1, int y1) = (0,0);
            if(restore) (x1, y1) = GetCursor();
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
            if (restore) SetCursor(x1, y1);
            return xpos;
        }

        public static void DrawTitleBar(string title) {
            (int x, int y) = GetCursor();
            Write(new string(' ', Width), ConsoleColor.Green, ConsoleColor.Green);
            SetCursor((Width / 2) - title.Length, 0);
            Write(title, ConsoleColor.White, ConsoleColor.Green);
            SetCursor(x, y);
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



        public static void DrawPointC(int x, int y, char c,ConsoleColor color) {
            (int l, int t) = GetCursor();
            SetCursor(x, y);
            SetBGColor(color);
            Write(c.ToString());
            SetCursor(l, t);
            RestoreBG();
        }

        public static string DrawDialogWithTextField(int pw, int ph, string title,ConsoleColor bg,ConsoleColor fg, string message, string initialInput = "", bool hide = false) {
            (int l, int t) = Console.GetCursorPosition();
            int w = Width;
            int h = Height;
            bool dialog = true;
            int cx = w / 2;
            int cy = h / 2;
            int x1 = Math.Max(0, cx - pw);
            int x2 = Math.Min(Width - 1, cx + pw);
            int y1 = Math.Max(0, cy - ph);
            int y2 = Math.Min(Height - 1, cy + ph);
            int dashes = Math.Max(1, pw - title.Length + 1);
            int m = pw % 2;

            DrawLineH(DialogTop, x1, y1, x2);
            for (int i = y1 + 1; i < y2; i++) {
                DrawLineH(bg, x1, i, x2);
            }

            // window top
            DrawDialogTop(x1, x2, y1, cx, title);

            int messageX = cx - message.Length / 2;
            int messageY = cy - 3;

            // Check if the message fits within the dialog, adjust if necessary
            if (messageX < x1) {
                messageX = x1;
            } else if (messageX + message.Length > x2) {
                messageX = x2 - message.Length;
            }

            // message
            Write(messageX, messageY, message, fg, bg);

            DrawLineH(bg, x1 + (cx - 2), y2 - 1, x2 - (cx - 2));


            // Input field rendering
            int inputFieldWidth = pw - 4; // Width excluding borders
            int inputFieldX = cx - inputFieldWidth / 2; // Center the text box horizontally
            int inputFieldY = messageY + 2; // Shift the text box below the message

            // Render the input field
            DrawTextBoxBorder(inputFieldX, inputFieldY - 1, inputFieldX + inputFieldWidth, inputFieldY + 1,ConsoleColor.Black);
            Write(inputFieldX + 1, inputFieldY - 1, new string(' ', inputFieldWidth),ConsoleColor.White, fg); // Clear previous content

            int okButtonX = cx - " OK ".Length / 2;
            int okButtonY = Math.Min(Height - 1, y2 - 2);

            Write(okButtonX, okButtonY, " OK ",ConsoleColor.White,ConsoleColor.Black);

            // Set cursor position inside the input field initially
            Console.SetCursorPosition(inputFieldX + initialInput.Length / 2 + 1, inputFieldY);
            while (dialog) {
                UpdateTextBox(inputFieldX, inputFieldY - 1, inputFieldX + inputFieldWidth, inputFieldY + 1,ConsoleColor.White);
                ConsoleKeyInfo key = Console.ReadKey(true);
                string hidden = "";
                if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsSymbol(key.KeyChar) || char.IsWhiteSpace(key.KeyChar)) {
                    // Append the typed character to the input 
                    if (!hide) {
                        if (initialInput.Length < inputFieldWidth - 2)
                            initialInput += key.KeyChar;
                        // Render the updated input field
                        Write(inputFieldX + 1, inputFieldY, initialInput, fg,ConsoleColor.Black);
                    } else {
                        if (initialInput.Length < inputFieldWidth - 2)
                            initialInput += key.KeyChar;

                        hidden = new string('*', initialInput.Length);
                        // Render the updated input field
                        Write(inputFieldX + 1, inputFieldY, hidden, fg,ConsoleColor.Black);
                    }
                } else if (key.Key == ConsoleKey.Backspace && initialInput.Length > 0) {
                    if (!hide) {
                        // Remove the last character when backspace is pressed
                        initialInput = initialInput.Substring(0, initialInput.Length - 1);
                        Write(inputFieldX + 1, inputFieldY, new string(' ', inputFieldWidth - 1),ConsoleColor.White,ConsoleColor.White);
                        // Render the updated input field
                        Write(inputFieldX + 1, inputFieldY, initialInput, fg,ConsoleColor.White);
                    } else {
                        initialInput = initialInput.Substring(0, initialInput.Length - 1);
                        Write(inputFieldX + 1, inputFieldY, new string(' ', inputFieldWidth - 1),ConsoleColor.White,ConsoleColor.White);
                        hidden = new string('*', initialInput.Length);
                        Write(inputFieldX + 1, inputFieldY, hidden, fg,ConsoleColor.White);
                    }
                } else if (key.Key == ConsoleKey.Enter) {
                    dialog = false;
                    for (int i = y1; i < y2; i++) {
                        DrawLineH(Background, x1, i, x2);
                    }
                }

            }
            Console.SetCursorPosition(l, t);
            return initialInput;
        }

        public static void DrawDialog(int pw, int ph, string title,ConsoleColor bg,ConsoleColor fg, string message, string buttonText = "  ENTER  ") {
            int w = Width;
            int h = Height;
            bool dialog = true;
            int cx = w / 2;
            int cy = h / 2;
            int x1 = Math.Max(0, cx - pw);
            int x2 = Math.Min(Width - 1, cx + pw);
            int y1 = Math.Max(0, cy - ph);
            int y2 = Math.Min(Height - 1, cy + ph);
            int dashes = Math.Max(1, pw - title.Length + 1);
            int m = pw % 0;

            DrawLineH(DialogTop, x1, y1, x2);
            for (int i2 = y1 + 1; i2 < y2; i2++) {
                DrawLineH(bg, x1, i2, x2);
            }

            DrawDialogTop(x1, x2, y1, cx, title);

            int messageX = cx - message.Length / 2;
            int messageY = cy;

            messageX = Math.Max(x1, messageX);

            messageX = x1 + 1;

            int i = 0;
            for (int x = x1 + 1; x < x2 - 1; x++) {
                if (string.IsNullOrEmpty(message) || i >= message.Length) break;
                if (x >= x2) {
                    x = x1 + 1;
                    messageY++;
                    DrawPointC(x, messageY, message[i], bg);
                } else {
                    DrawPointC(x, messageY, message[i], bg);
                }
                i++;
            }

            DrawLineH(bg, x1 + (cx - 2), y2 - 1, x2 - (cx - 2));

            int okButtonX = cx - buttonText.Length / 2;
            int okButtonY = Math.Min(Height - 1, y2 - 2);

            Write(okButtonX, okButtonY, buttonText,ConsoleColor.White,ConsoleColor.Black);

            while (dialog) {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) {
                    for (int j = y1; j < y2; j++) {
                        DrawLineH(ConsoleColor.Black, x1, j, x2);
                    }
                    dialog = false;
                }
            }
        }

        private static void DrawTextBoxBorder(int x1, int y1, int x2, int y2,ConsoleColor color) {
            for (int i = y1; i < y2; i++)
                DrawLineH(color, x1, i, x2);


            for (int i = x1; i < x2; i++) {
                DrawPointC(i, y1, '-', color);
                DrawPointC(i, y2, '-', color);
            }

            DrawPointC(x1, y1, '+', color);
            DrawPointC(x2, y1, '+', color);
            DrawPointC(x1, y2, '+', color);
            DrawPointC(x2, y2, '+', color);
            DrawPointC(x1, y1 + 1, '|', color);
            DrawPointC(x2, y1 + 1, '|', color);

        }

        static void DrawDialogTop(int x1, int x2, int y, int cx, string title) {
            for (int i = x1; i < x2; i++) {
                DrawPointC(i, y, '-', DialogTop);
            }

            string t = $"[{title}]";
            int idx = 0;
            for (int i = cx - t.Length + 5; i < cx + t.Length + 5; i++) {
                DrawPointC(i, y, t[idx], DialogTop);
                idx++;
            }
        }

        static void UpdateTextBox(int x1, int y1, int x2, int y2,ConsoleColor color) {
            DrawLineH(color, x1, y2, x2);

            for (int i = x1; i < x2; i++) {
                DrawPointC(i, y1, '-', color);
                DrawPointC(i, y2, '-', color);
            }

            DrawPointC(x1, y1, '+', color);
            DrawPointC(x2, y1, '+', color);
            DrawPointC(x1, y2, '+', color);
            DrawPointC(x2, y2, '+', color);
            DrawPointC(x1, y1 + 1, '|', color);
            DrawPointC(x2, y1 + 1, '|', color);
        }
    }
}
