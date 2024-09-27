using TMPro;
using UnityEngine;

[RequireComponent(typeof(MainBaseAI))]
public class DisplayResourceUI : MonoBehaviour                          // Устаревшая система, в данный момент неработает, удалить? Это верхняя белая панель отображения ресурсов
{
    [SerializeField] private TMP_Text _displayNumberOfFood;
    [SerializeField] private TMP_Text _displayNumberOfTimber;
    [SerializeField] private TMP_Text _displayNumberOfMarble;

    private MainBaseAI _model;

    public void Selection()                     // При выделении объекта, подцеплять его Store
    {
        var downPanel = FindFirstObjectByType<DownPanelUI>();
        _displayNumberOfFood = downPanel.DisplayNumberOfFood;
        _displayNumberOfTimber = downPanel.DisplayNumberOfTimber;
        _displayNumberOfMarble = downPanel.DisplayNumberOfMarble;
        BindEvents();
    }

    public void UnSelection()                   // При развыделении объекта, отцеплять его Store и стирать все надписи
    {
        _displayNumberOfFood.text = "";
        _displayNumberOfTimber.text = "";
        _displayNumberOfMarble.text = "";
        UnbindEvents();
        _displayNumberOfFood = null;
        _displayNumberOfTimber = null;
        _displayNumberOfMarble = null;
    }

    private void OnEnable()
    {
        _model = GetComponent<MainBaseAI>();
        //_model.FoodQuantityChanged += OnChangeNumberOfFood;
        //_model.TimberQuantityChanged += OnChangeNumberOfTimber;
        //_model.MarbleQuantityChanged += OnChangeNumberOfMarble;

        //_model.Store.FoodQuantityChanged += OnChangeNumberOfFood;
        //_model.Store.TimberQuantityChanged += OnChangeNumberOfTimber;
        //_model.Store.MarbleQuantityChanged += OnChangeNumberOfMarble;

        //BindEvents();
    }

    private void OnDisable()
    {
        //_model.FoodQuantityChanged -= OnChangeNumberOfFood;
        //_model.TimberQuantityChanged -= OnChangeNumberOfTimber;
        //_model.MarbleQuantityChanged -= OnChangeNumberOfMarble;

        //_model.Store.FoodQuantityChanged -= OnChangeNumberOfFood;
        //_model.Store.TimberQuantityChanged -= OnChangeNumberOfTimber;
        //_model.Store.MarbleQuantityChanged -= OnChangeNumberOfMarble;

        //UnbindEvents();
    }

    private void OnChangeNumberOfFood(int number) => _displayNumberOfFood.text = "Food: " + number.ToString();
    private void OnChangeNumberOfTimber(int number) => _displayNumberOfTimber.text = "Timber: " + number.ToString();
    private void OnChangeNumberOfMarble(int number) => _displayNumberOfMarble.text = "Marble: " + number.ToString();

    private void BindEvents()
    {
        _model.Store.FoodQuantityChanged += OnChangeNumberOfFood;
        _model.Store.TimberQuantityChanged += OnChangeNumberOfTimber;
        _model.Store.MarbleQuantityChanged += OnChangeNumberOfMarble;
    }

    private void UnbindEvents()
    {
        _model.Store.FoodQuantityChanged -= OnChangeNumberOfFood;
        _model.Store.TimberQuantityChanged -= OnChangeNumberOfTimber;
        _model.Store.MarbleQuantityChanged -= OnChangeNumberOfMarble;
    }
}
