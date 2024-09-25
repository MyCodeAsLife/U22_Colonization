using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChangingObject : SelectableObject
{
    protected SingleReactiveProperty<float> ActionProgress = new();
    protected ActionProgressViewer ActionProgressViewer;
    protected Coroutine CurrentAction;
    protected float DurationOfAction;

    public event Action ActionStarted;
    public event Action ActionFinished;

    protected override void Awake()
    {
        base.Awake();

        if (TryGetComponent<ActionProgressViewer>(out ActionProgressViewer) == false)
            ActionProgressViewer = transform.AddComponent<ActionProgressViewer>();
    }

    public void SubscribeToActionProgress(Action<float> func) => ActionProgress.Change += func;
    public void UnsubscribeToActionProgress(Action<float> func) => ActionProgress.Change -= func;
    public float GetActionProgress() => ActionProgress.Value;

    protected virtual void StopAction()
    {
        Debug.Log("Stop Action");

        if (CurrentAction != null)
            StopCoroutine(CurrentAction);

        CurrentAction = null;
    }

    protected void ActionStart()
    {
        ActionStarted?.Invoke();
    }

    protected virtual void ActionFinish()
    {
        ActionFinished?.Invoke();
        CurrentAction = null;
    }

    protected IEnumerator PerformingAnAction(Action action)
    {
        ActionStarted?.Invoke();
        float timer = 0;

        while (timer < DurationOfAction)
        {
            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
            ActionProgress.Value = timer / DurationOfAction;
        }

        action();
        ActionFinish();
    }
}
