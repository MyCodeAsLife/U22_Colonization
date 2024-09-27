public class Task
{
    public int Priority { get; private set; }
    public TypesOfTasks TypeOfTask { get; private set; }
    public SelectableObject Target { get; private set; }

    public Task(int priority, TypesOfTasks typeOfTask, SelectableObject target)
    {
        Priority = priority;
        TypeOfTask = typeOfTask;
        Target = target;
    }
}
