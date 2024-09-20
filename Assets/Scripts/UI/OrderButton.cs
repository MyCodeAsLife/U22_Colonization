using UnityEngine;

public class OrderButton : MonoBehaviour
{
    [SerializeField] protected SelectableObject Prefab;

    protected MainBaseAI _currentBuilding;                                    // Или у каждого строения сделать ссылку на пренадлежность базе

    //public virtual void TryBuy() { }

    public void SetCustomer(MainBaseAI building)
    {
        //Debug.Log("SetCustomer");                                                     //++++++++++++++++++++
        _currentBuilding = building;
    }

    protected bool CheckPriceAvailability(AmountOfResources amount, AmountOfResources price)
    {
        //Debug.Log($"amount.food:{amount.Food} >= price.food:{price.Food} = {amount.Food >= price.Food}");       //+++++++++++
        //Debug.Log($"amount.timber:{amount.Timber} >= price.timber:{price.Timber} = {amount.Timber >= price.Timber}");       //+++++++++++
        //Debug.Log($"amount.marble:{amount.Marble} >= price.marble:{price.Marble} = {amount.Marble >= price.Marble}");       //+++++++++++

        return amount.Food >= price.Food && amount.Timber >= price.Timber && amount.Marble >= price.Marble;
    }
}
