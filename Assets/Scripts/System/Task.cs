using System;

public class Task
{
    public int Priority { get; private set; }
    public SelectableObject Target { get; private set; }

    public event Action<Task> Completed;

    public Task(int priority, SelectableObject target)
    {
        Priority = priority;
        Target = target;
    }

    public void Complete()
    {
        Completed?.Invoke(this);
    }

    public int CompareTo(object obj)
    {
        if (obj is Task task)
            return Priority.CompareTo(task.Priority);
        else
            throw new ArgumentException("Incorrect parameter value");
    }
}
