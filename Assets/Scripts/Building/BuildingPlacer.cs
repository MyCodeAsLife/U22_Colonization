using System;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    private SingleReactiveProperty<float> _cellSize = new();       //++++++++
    //[SerializeField] private float _tempCellSize;                                   //+++++++++

    public float CellSize { get { return _cellSize.Value; } }

    private void Awake()
    {
        _cellSize.Value = 2f;
        //_tempCellSize = 2f;
    }

    //private void LateUpdate()
    //{
    //    _cellSize.Value = _tempCellSize;
    //}

    public void SubscribeOnCellSizeChanged(Action<float> fun)
    {
        _cellSize.Change += fun;
    }

    public void UnSubscribeOnCellSizeChanged(Action<float> fun)
    {
        _cellSize.Change -= fun;
    }
}
