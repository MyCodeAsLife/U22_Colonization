using UnityEngine;

public class OrderButton : MonoBehaviour
{
    [SerializeField] protected Price objectPrice;

    protected MainBaseAI _currentBuilding;

    public void SetCustomer(MainBaseAI building)
    {
        _currentBuilding = building;
    }

    protected bool CheckPriceAvailability(AmountOfResources amount, AmountOfResources price)
    {
        return amount.Food >= price.Food && amount.Timber >= price.Timber && amount.Marble >= price.Marble;
    }
}
