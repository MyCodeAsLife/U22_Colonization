using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private Vector3 _startPoint;
    private Plane _plane;

    private void Start()
    {
        _plane = new Plane(Vector3.up, Vector3.zero);               // ������ plane ������������ �����? ��� ����������� ���������� �� ���� �� PlayerControlSystem (
    }

    private void LateUpdate()                                       // �������� ��� �� PlayerControlSystem
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float distance;
        _plane.Raycast(ray, out distance);
        Vector3 point = ray.GetPoint(distance);

        if (Input.GetMouseButtonDown(2))                            // ������� Input Actions � ��������� ���������� � ����� ���� ������������ � �������� ������.
        {
            _startPoint = point;
        }

        if (Input.GetMouseButton(2))                                // ���������� ��� � ������� ���������
        {
            Vector3 offset = point - _startPoint;
            transform.position -= offset;
        }

        if (Input.mouseScrollDelta.y != 0)                          // �������� �������� ������ 1, �������� ����� -1
            transform.Translate(0f, 0f, Input.mouseScrollDelta.y * 3);
    }
}
