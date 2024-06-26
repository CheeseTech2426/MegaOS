﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace MegaOS.Shell
{
    internal class Shell
    {

        string[] menu = { "Applications", "Power", "Run" };
        string[] appsMenu = { "Calculator", "Task Manager", "DOS" };
        int idx = 0;
        int appIdx = 0;
        int ShellID;
        int HelperID;

        bool isAppMenu = false;

        List<int> xpos;

        public Shell()
        {
            GL.Clear();
            xpos = new List<int>();
            Run();
        }

        private void Run()
        {
            Kernel.taskman.EndAllTasks();
            Refresh();
            ShellID = Kernel.taskman.CreateTask("Shell", Refresh, new object[1] { "shell" }, false);
            Kernel.taskman.StartTask(ShellID);
        }


        private void Refresh(object[] args = null)
        {
            GL.SetCursor(0, 0);
            GL.FillScreen(ConsoleColor.Black);
            GL.Write(new string(' ', 80), ConsoleColor.White, ConsoleColor.Green);
            GL.Write(80 / 2 - "Shell".Length, 0, "Shell", ConsoleColor.White, ConsoleColor.Green);
            GL.SetCursor(0, 1);
            GL.Write(new string(' ', 80), ConsoleColor.White, ConsoleColor.Blue);
            xpos = GL.DrawMenuBarOptions(menu, 0);
            while (Kernel.taskman.isTaskRunning("Shell"))
            {
                Kernel.taskman.CheckAndRestartTasks();
                (int x, int y) = GL.GetCursor();
                GL.SetCursor(0, 15);
                Kernel.taskman.ListTasks();
                GL.SetCursor(x, y);
                Cosmos.System.KeyEvent k;
                if (Cosmos.System.KeyboardManager.TryReadKey(out k))
                {
                    if (k.Modifiers == ConsoleModifiers.Control)
                    {
                        if (k.Modifiers == ConsoleModifiers.Alt)
                        {
                            if (k.Key == Cosmos.System.ConsoleKeyEx.Delete)
                            {
                                Kernel.taskman.StopTask(ShellID);
                            }
                        }
                    }

                    if (!isAppMenu && k.Modifiers == ConsoleModifiers.Alt)
                    {
                        if (k.Key == Cosmos.System.ConsoleKeyEx.LeftArrow && idx > 0)
                        {
                            idx--;
                            GL.DrawMenuBarOptions(menu, idx);
                        }
                        if (k.Key == Cosmos.System.ConsoleKeyEx.RightArrow && idx < menu.Length - 1)
                        {
                            idx++;
                            GL.DrawMenuBarOptions(menu, idx);
                        }
                        if (k.Key == Cosmos.System.ConsoleKeyEx.Enter)
                        {
                            Execute(menu[idx]);
                        }
                    }
                    else if (isAppMenu)
                    {
                        if (k.Key == Cosmos.System.ConsoleKeyEx.UpArrow && appIdx > 0)
                        {
                            appIdx--;
                            GL.DrawDropdownMenu(appsMenu, appIdx, xpos[0]);
                        }
                        if (k.Key == Cosmos.System.ConsoleKeyEx.DownArrow && appIdx < appsMenu.Length - 1)
                        {
                            appIdx++;
                            GL.DrawDropdownMenu(appsMenu, appIdx, xpos[0]);
                        }
                        if (k.Key == Cosmos.System.ConsoleKeyEx.Enter)
                        {
                            Execute(appsMenu[appIdx]);
                        }
                        if (k.Key == Cosmos.System.ConsoleKeyEx.Escape)
                        {
                            Kernel.taskman.StopTask(HelperID);
                            Kernel.taskman.GetTasks()[HelperID].Stop();
                            Kernel.taskman.EndAllTasks("Shell Applications Helper");
                            appIdx = 0;
                            GL.CloseDropdownMenu(appsMenu, xpos[idx]);
                            isAppMenu = false;
                        }
                    }
                    else
                    {

                    }
                }
            }
            GL.Clear();
        }


        public void Execute(string instrucion)
        {
            switch (instrucion.ToLower())
            {
                case "applications":
                    isAppMenu = true;
                    HelperID = Kernel.taskman.CreateTask("Shell Applications Helper", null, new object[1] { "SAH" }, true);
                    Kernel.taskman.StartTask(HelperID);
                    GL.DrawDropdownMenu(appsMenu, appIdx, xpos[0]);
                    break;
                case "task manager":
                case "dos":
                    Kernel.taskman.EndAllTasks("Shell Applications Helper");
                    Kernel.taskman.StopTask(ShellID);
                    break;
                case "calculator":
                    break;
                case "run":
                case "power":
                    break;
                default:
                    GL.SetCursor(0, 0);
                    GL.WriteLine("                      Crash");
                    GL.WriteLine(" This program has performed an illegal operation");
                    GL.WriteLine($" and will now shutdown. Process {ShellID} will now");
                    GL.WriteLine($" be terminated.");
                    Console.ReadKey();
                    Kernel.taskman.StopTask(ShellID);
                    break;
            }
        }

    }
}
