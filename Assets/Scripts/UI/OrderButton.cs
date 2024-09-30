using UnityEngine;

public class OrderButton : MonoBehaviour
{
    [SerializeField] protected Price objectPrice;

    protected MainBase _currentBuilding;

    public void SetCustomer(MainBase building)
    {
        _currentBuilding = building;
    }

    protected bool CheckPriceAvailability(AmountOfResources amount, AmountOfResources price)
    {
        return amount.Food >= price.Food && amount.Timber >= price.Timber && amount.Marble >= price.Marble;
    }
}
