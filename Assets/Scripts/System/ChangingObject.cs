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

    public virtual void StopAction()
    {
        StopCoroutine(CurrentAction);
        CurrentAction = null;
    }

    public void ActionFinish()
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
