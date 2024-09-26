//using System;

public class Task
{
    public int Priority { get; private set; }
    public TypesOfTasks TypeOfTask { get; private set; }
    public SelectableObject Target { get; private set; }

    //public event Action<Task> Completed;

    public Task(int priority, TypesOfTasks typeOfTask, SelectableObject target)
    {
        Priority = priority;
        TypeOfTask = typeOfTask;
        Target = target;
    }

    //public void Complete()
    //{
    //    Completed?.Invoke(this);
    //}
}
