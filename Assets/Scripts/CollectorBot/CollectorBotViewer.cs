using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CollectorBotAI))]
public class CollectorBotViewer : MonoBehaviour
{
    private Slider _progressBar;
    private Slider _prefabProgressBar;
    private CollectorBotAI _presenter;

    private void Awake()
    {
        _presenter = GetComponent<CollectorBotAI>();
        _prefabProgressBar = Resources.Load<Slider>("Prefabs/ProgressBar");
        _progressBar = Instantiate<Slider>(_prefabProgressBar, transform);
    }

    private void OnDisable()
    {
        _presenter.ActionStarted -= OnStartAction;
        _presenter.ActionFinished -= OnFinishAction;
        _presenter.UnsubscribeToActionProgress(OnActionProgress);
    }

    private void Start()
    {
        _progressBar.gameObject.SetActive(false);
        _presenter.ActionStarted += OnStartAction;
        _presenter.ActionFinished += OnFinishAction;
        _presenter.SubscribeToActionProgress(OnActionProgress);
    }

    public void OnActionProgress(float value)
    {
        _progressBar.value = value;
    }

    private void OnStartAction()
    {
        _progressBar.gameObject.SetActive(true);
    }

    private void OnFinishAction()
    {
        _progressBar.gameObject.SetActive(false);
    }
}
