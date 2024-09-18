using UnityEngine;
using UnityEngine.UI;

public class ActionProgressViewer : MonoBehaviour
{
    private Slider _progressBar;
    private Slider _prefabProgressBar;
    private ChangingObject _presenter;

    private void Awake()
    {
        _presenter = GetComponent<ChangingObject>();
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

    private void OnActionProgress(float value)
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
