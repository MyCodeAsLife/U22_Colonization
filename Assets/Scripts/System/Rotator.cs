using UnityEngine;

public class Rotator : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.RotateAround(transform.position, Vector3.up, 0.5f);
    }
}
