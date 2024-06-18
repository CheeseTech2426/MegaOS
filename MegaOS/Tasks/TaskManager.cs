using System;
using System.Collections.Generic;

namespace MegaOS
{
    public class TaskManager
    {
        private Dictionary<int, Task> tasks = new Dictionary<int, Task>();
        private int nextTaskId = 1;

        public int CreateTask(string name, Action<object[]> taskAction, object[] arguments, bool keepRunning = false)
        {
            if (taskAction == null || arguments == null) { taskAction = empty; arguments = new object[1] { "" }; }
            var task = new Task(nextTaskId, name, taskAction, arguments, keepRunning);
            tasks[nextTaskId] = task;
            nextTaskId++;
            return task.Id;
        }

        private void empty(object[] arr) { }

        public void StartTask(int taskId)
        {
            if (tasks.TryGetValue(taskId, out var task))
            {
                task.Start();
                //Console.WriteLine($"Task {taskId} ({task.Name}) started.");
            }
            else
            {
                Console.WriteLine($"Task {taskId} not found.");
            }
        }

        public void StopTask(int taskId)
        {
            if (tasks.TryGetValue(taskId, out var task))
            {
                task.Stop();
                //Console.WriteLine($"Task {taskId} ({task.Name}) stopped.");
            }
            else
            {
                Console.WriteLine($"Task {taskId} not found.");
            }
        }

        public void ListTasks()
        {
            Console.WriteLine("Current tasks:");
            foreach (var task in tasks.Values)
            {
                Console.WriteLine($"- {task.Id}: {task.Name} (Running: {task.IsRunning}, Completed: {task.IsCompleted})");
            }
        }

        public void CleanUpTasks()
        {
            var tasksToRemove = new List<int>();

            foreach (var task in tasks.Values)
            {
                if (task.IsCompleted || task.IsMarkedForDeletion)
                {
                    tasksToRemove.Add(task.Id);
                }
            }

            foreach (var taskId in tasksToRemove)
            {
                tasks.Remove(taskId);
                Console.WriteLine($"Task {taskId} removed.");
            }
        }

        public void CheckAndRestartTasks() {
            try {
                foreach (var task in tasks.Values) {
                    if (task.IsCompleted && task.keepRunning) {
                        var newTask = task.Reset();
                        tasks[task.Id] = newTask;
                        StartTask(newTask.Id);
                    }
                }
            } catch {
                return;
            }
        }

        public void RunAllTasks()
        {
            if (tasks.Count < 1 || tasks.Count == 0) return;
            foreach (var task in tasks.Values)
            {
                if (!task.IsCompleted && !task.IsMarkedForDeletion)
                {
                    task.Start();
                }
            }
        }

        public bool isTaskRunning(string name) {
            foreach (Task task in tasks.Values) {
                if(task.Name == name && task.IsRunning) return true;
            }
            return false;
        }

        public Dictionary<int, Task> GetTasks() {
            return tasks;
        }

        public void EndAllTasks() {
            nextTaskId = 1;
            foreach (Task task in Kernel.taskman.GetTasks().Values) {
                task.MarkToBeRemoved();
            }
            CleanUpTasks();
        }

        public void EndAllTasks(string name) {
            nextTaskId = 1;
            foreach (Task task in Kernel.taskman.GetTasks().Values) {
                if (task.Name.ToLower() == name.ToLower())
                    task.Stop();
            }
            CleanUpTasks();
        }
    }
}
