public class Task
{
    public SelectableObject Target { get; private set; }
    public TypeOfTask TypeOfTask { get; private set; }
    public int Priority { get; private set; }

    public Task(int priority, TypeOfTask typeOfTask, SelectableObject target)
    {
        Priority = priority;
        TypeOfTask = typeOfTask;
        Target = target;
    }
}
