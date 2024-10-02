using UnityEngine;

public class OrderButton : MonoBehaviour
{
    [SerializeField] protected Price ObjectPrice;

    protected MainBase CurrentBuilding;

    public void SetCustomer(MainBase building)
    {
        CurrentBuilding = building;
    }

    protected bool IsEnoughResources(AmountOfResources amount, AmountOfResources price)
    {
        return amount.Food >= price.Food && amount.Timber >= price.Timber && amount.Marble >= price.Marble;
    }
}
