using UnityEngine;

public class Rotator : MonoBehaviour
{
    private float _angle;

    private void Start()
    {
        _angle = 0.5f;
    }

    private void LateUpdate()
    {
        transform.RotateAround(transform.position, Vector3.up, _angle);
    }
}
