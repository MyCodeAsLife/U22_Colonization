using System.Collections;
using UnityEngine;
using TMPro;

public class DownPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayNumberOfFood;
    [SerializeField] private TMP_Text _displayNumberOfTimber;
    [SerializeField] private TMP_Text _displayNumberOfMarble;
    //private Image _imagePanel;                                                          // В этом нет смысла
    private Vector3 _showPosition;
    private Vector3 _hidePosition;
    private Coroutine _swimming;
    private BuildingPlacer _buildingPlacer;
    private GameObject _buildingPlacerPrefab;
    //private IList<OrderButton> _orderButtons;
    private OrderButton[] _orderButtons;

    //private MainBaseAI _selectBuilding;
    private SelectableObject _selectedObject;

    public TMP_Text DisplayNumberOfFood => _displayNumberOfFood;
    public TMP_Text DisplayNumberOfTimber => _displayNumberOfTimber;
    public TMP_Text DisplayNumberOfMarble => _displayNumberOfMarble;

    private void Awake()                                // +++++++++
    {
        _orderButtons = GetComponentsInChildren<OrderButton>();
    }

    private void Start()
    {
        _showPosition = Vector3.zero;
        _hidePosition = new Vector3(0, -150f, 0);
        //_imagePanel = GetComponent<Image>();
        //_imagePanel.rectTransform.anchoredPosition = new Vector3(0, -150f, 0);
        transform.position = _hidePosition;                                      // Magic
        //StartCoroutine(SwimmingPanel(_showPosition));                                                                 // +++++++++++++
        //OnChangeNumberOfFood(0);
        //OnChangeNumberOfTimber(0);
        //OnChangeNumberOfMarble(0);
    }

    public void LinkBase(MainBaseAI mainBase)
    {
        _selectedObject = mainBase;
        //_selectBuilding = mainBase;
        ChangeResourcesNumber(mainBase);
        mainBase.Store.FoodQuantityChanged += OnChangeNumberOfFood;
        mainBase.Store.TimberQuantityChanged += OnChangeNumberOfTimber;
        mainBase.Store.MarbleQuantityChanged += OnChangeNumberOfMarble;

        //Debug.Log("LinkBase");                                                                  // +++++++++++++

        for (int i = 0; i < _orderButtons.Length; i++)
            _orderButtons[i].SetCustomer(mainBase);

        ShowPanel();
    }

    public void UnLinkBase(MainBaseAI mainBase)
    {
        HidePanel();
        mainBase.Store.FoodQuantityChanged -= OnChangeNumberOfFood;
        mainBase.Store.TimberQuantityChanged -= OnChangeNumberOfTimber;
        mainBase.Store.MarbleQuantityChanged -= OnChangeNumberOfMarble;
        //_selectBuilding = null;
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

    private void ChangeResourcesNumber(MainBaseAI mainBase)
    {
        AmountOfResources amountOfResources = mainBase.Store.GetAmountOfResources();

        OnChangeNumberOfFood(amountOfResources.Food);
        OnChangeNumberOfTimber(amountOfResources.Timber);
        OnChangeNumberOfMarble(amountOfResources.Marble);
    }

    private IEnumerator SwimmingPanel(Vector3 targetPosition)
    {
        float speed = 0.1f;                                                       //+++++++++++
        var delay = new WaitForFixedUpdate();
        bool isWork = true;

        while (isWork)
        {
            yield return delay;

            transform.position = Vector3.Lerp(transform.position, targetPosition, speed);                                 // Рабочий вариант
            float distance = Vector3.Distance(transform.position, targetPosition);
            //_imagePanel.rectTransform.anchoredPosition = Vector3.Lerp(transform.position, Vector3.zero, speed);       // Рабочий вариант
            //float distance = Vector3.Distance(_imagePanel.rectTransform.anchoredPosition, Vector3.zero);

            if (distance < 0.1f)
            {
                transform.position = targetPosition;
                //_imagePanel.rectTransform.anchoredPosition = Vector3.zero;
                isWork = false;
            }
        }

        _swimming = null;
    }
}
