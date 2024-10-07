public interface ITaskManager
{
    public bool IsBuildingPlanned { get; }

    public void AddCollectorBot(CollectorBot collectorBot);
    public void RemoveCollectorBot(CollectorBot collectorBot);
    public void ScheduleConstruction(BuildingUnderConstruction building);
}
