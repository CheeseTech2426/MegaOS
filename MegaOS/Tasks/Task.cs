using System;

public class Task {
    public int Id { get; private set; }
    public string Name { get; private set; }
    private Action<object[]> action;
    private object[] arguments;
    public bool isRunning;
    private bool isCompleted;
    public bool isMarkedForDeletion;
    public bool keepRunning;

    public Task(int id, string name, Action<object[]> action, object[] arguments, bool keepRunning = false) {
        Id = id;
        Name = name;
        this.action = action;
        this.arguments = arguments;
        isRunning = false;
        isCompleted = false;
        isMarkedForDeletion = false;
        this.keepRunning = keepRunning;
    }

    public Task Reset() {
        return new Task(Id, Name, action, arguments, keepRunning);
    }

    public void Start() {
        if (!isRunning) {
            isRunning = true;
            action(arguments);
            if (!keepRunning) {
                isRunning = false;
                isCompleted = true;
            }
        } else {
            throw new InvalidOperationException($"Task {Id} is already running.");
        }
    }

    public void Stop() {
        isRunning = false;
        isCompleted = true;

    }

    public void MarkToBeRemoved() {
        isMarkedForDeletion = true;
    }

    public bool IsRunning => isRunning;
    public bool IsCompleted => isCompleted;
    public bool IsMarkedForDeletion => isMarkedForDeletion;
}
