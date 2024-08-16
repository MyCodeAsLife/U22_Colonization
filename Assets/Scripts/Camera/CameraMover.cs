using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private Vector3 _startPoint;
    private Plane _plane;

    private void Start()
    {
        _plane = new Plane(Vector3.up, Vector3.zero);               // Вместо plane использовать Карту? или расчитывать расстояние по лучу из PlayerControlSystem (
    }

    private void LateUpdate()                                       // Забирать луч из PlayerControlSystem
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float distance;
        _plane.Raycast(ray, out distance);
        Vector3 point = ray.GetPoint(distance);

        if (Input.GetMouseButtonDown(2))                            // Вынести Input Actions в отдельный контроллер и через него привязыватся к нажатиям клавиш.
        {
            _startPoint = point;
        }

        if (Input.GetMouseButton(2))                                // Перемещять мыш с зажатым колесиком
        {
            Vector3 offset = point - _startPoint;
            transform.position -= offset;
        }

        if (Input.mouseScrollDelta.y != 0)                          // Колесико крутится вперед 1, крутится назад -1
            transform.Translate(0f, 0f, Input.mouseScrollDelta.y * 3);
    }
}
