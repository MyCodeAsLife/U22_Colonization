using UnityEngine;

public class Barack : Building
{
    [SerializeField] private int BoolShet;                                  // ++++++++++

    protected override void Start()
    {
        base.Start();
        //SelfPrice.Food = 3;
        //SelfPrice.Timber = 6;
        //SelfPrice.Marble = 8;
    }
}
