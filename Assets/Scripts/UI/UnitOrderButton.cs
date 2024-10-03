public class UnitOrder : OrderButton
{
    public void TryBuy()
    {
        AmountOfResources price = CurrentBuilding.GetPriceOf(ObjectPrice);

        if (IsEnoughResources(CurrentBuilding.Store.AmountOfResources, price) && (CurrentBuilding.TaskManager.IsBuildingPlanned == false))
        {
            CurrentBuilding.Store.SubtractResources(price);
            CurrentBuilding.CreateCollectorBot();
        }
    }
}
