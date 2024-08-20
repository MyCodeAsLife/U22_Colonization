using System.Collections;
using TMPro;
using UnityEngine;

public class BuildingPanelUI : MonoBehaviour
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
    //private MainBaseAI _selectBuilding;

    private void Awake()
    {

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

    public void TryBuy()    // Запрос к выделенному зданию на наличие необходимых ресурсов
    {

    }

    public void LinkBase(MainBaseAI mainBase)
    {
        //_selectBuilding = mainBase;
        ChangeResourcesNumber(mainBase);
        mainBase.FoodQuantityChanged += OnChangeNumberOfFood;
        mainBase.TimberQuantityChanged += OnChangeNumberOfTimber;
        mainBase.MarbleQuantityChanged += OnChangeNumberOfMarble;
        ShowPanel();
    }

    public void UnLinkBase(MainBaseAI mainBase)
    {
        HidePanel();
        mainBase.FoodQuantityChanged -= OnChangeNumberOfFood;
        mainBase.TimberQuantityChanged -= OnChangeNumberOfTimber;
        mainBase.MarbleQuantityChanged -= OnChangeNumberOfMarble;
        //_selectBuilding = null;
    }

    public void ShowPanel()                                             // private???
    {
        if (_swimming != null)
            StopCoroutine(_swimming);

        _swimming = StartCoroutine(SwimmingPanel(_showPosition));
    }
    public void HidePanel()                                             // private???
    {
        if (_swimming != null)
            StopCoroutine(_swimming);

        _swimming = StartCoroutine(SwimmingPanel(_hidePosition));
    }

    private void OnChangeNumberOfFood(int number) => _displayNumberOfFood.text = "Food: " + number.ToString();
    private void OnChangeNumberOfTimber(int number) => _displayNumberOfTimber.text = "Timber: " + number.ToString();
    private void OnChangeNumberOfMarble(int number) => _displayNumberOfMarble.text = "Marble: " + number.ToString();

    private void ChangeResourcesNumber(MainBaseAI mainBase)
    {
        OnChangeNumberOfFood(mainBase.NumberOfFood);
        OnChangeNumberOfTimber(mainBase.NumberOfTimber);
        OnChangeNumberOfMarble(mainBase.NumberOfMarble);
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
