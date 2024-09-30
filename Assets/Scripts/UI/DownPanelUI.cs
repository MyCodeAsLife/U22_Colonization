using System.Collections;
using UnityEngine;
using TMPro;

public class DownPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayNumberOfFood;
    [SerializeField] private TMP_Text _displayNumberOfTimber;
    [SerializeField] private TMP_Text _displayNumberOfMarble;

    private GameObject _buildingPlacerPrefab;
    private SelectableObject _selectedObject;
    private BuildingPlacer _buildingPlacer;
    private OrderButton[] _orderButtons;
    private Vector3 _showPosition;
    private Vector3 _hidePosition;
    private float _swimmingSpeed;
    private Coroutine _swimming;

    public TMP_Text DisplayNumberOfFood => _displayNumberOfFood;
    public TMP_Text DisplayNumberOfTimber => _displayNumberOfTimber;
    public TMP_Text DisplayNumberOfMarble => _displayNumberOfMarble;

    private void Awake()
    {
        _orderButtons = GetComponentsInChildren<OrderButton>();
    }

    private void Start()
    {
        _swimmingSpeed = 0.1f;
        _showPosition = Vector3.zero;
        _hidePosition = new Vector3(0, -150f, 0);
        transform.position = _hidePosition;
    }

    public void LinkBase(MainBase mainBase)
    {
        _selectedObject = mainBase;
        ChangeResourcesNumber(mainBase);
        mainBase.Store.FoodQuantityChanged += OnChangeNumberOfFood;
        mainBase.Store.TimberQuantityChanged += OnChangeNumberOfTimber;
        mainBase.Store.MarbleQuantityChanged += OnChangeNumberOfMarble;

        for (int i = 0; i < _orderButtons.Length; i++)
            _orderButtons[i].SetCustomer(mainBase);

        ShowPanel();
    }

    public void UnLinkBase(MainBase mainBase)
    {
        HidePanel();
        mainBase.Store.FoodQuantityChanged -= OnChangeNumberOfFood;
        mainBase.Store.TimberQuantityChanged -= OnChangeNumberOfTimber;
        mainBase.Store.MarbleQuantityChanged -= OnChangeNumberOfMarble;
        _selectedObject = null;
    }

    private void OnChangeNumberOfFood(int number) => _displayNumberOfFood.text = "Food: " + number.ToString();
    private void OnChangeNumberOfTimber(int number) => _displayNumberOfTimber.text = "Timber: " + number.ToString();
    private void OnChangeNumberOfMarble(int number) => _displayNumberOfMarble.text = "Marble: " + number.ToString();

    private void ShowPanel()
    {
        if (_swimming != null)
            StopCoroutine(_swimming);

        _swimming = StartCoroutine(SwimmingPanel(_showPosition));
    }

    private void HidePanel()
    {
        if (_swimming != null)
            StopCoroutine(_swimming);

        _swimming = StartCoroutine(SwimmingPanel(_hidePosition));
    }

    private void ChangeResourcesNumber(MainBase mainBase)
    {
        AmountOfResources amountOfResources = mainBase.Store.AmountOfResources;

        OnChangeNumberOfFood(amountOfResources.Food);
        OnChangeNumberOfTimber(amountOfResources.Timber);
        OnChangeNumberOfMarble(amountOfResources.Marble);
    }

    private IEnumerator SwimmingPanel(Vector3 targetPosition)
    {
        const float PermissibleFault = 0.1f;
        var delay = new WaitForFixedUpdate();
        bool isWork = true;

        while (isWork)
        {
            yield return delay;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _swimmingSpeed);

            if (Vector3Extensions.IsEnoughClose(transform.position, targetPosition, PermissibleFault))
            {
                transform.position = targetPosition;
                isWork = false;
            }
        }

        _swimming = null;
    }
}
