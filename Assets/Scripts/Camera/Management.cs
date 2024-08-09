using UnityEngine;
using UnityEngine.UI;

public class Management : MonoBehaviour
{
    private int _selectionMask;
    private SelectableObject _hovered;

    private void Start()
    {
        _selectionMask = LayerMask.NameToLayer("Interactable") /*& LayerMask.NameToLayer("Resource")*/;
        //Debug.Log(_selectionMask);
    }

    private void LateUpdate()
    {
        RaycastHit hit = new RaycastHit();                                          // ������� ������� � Start ��������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                // ������� ������� � Start ��������
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);                 // ��������� ���� �� ������, ������ 100

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100/*, _selectionMask*/ /*^ int.MaxValue*/))    // ������������ ��������� ������������ �����
        {
            //Debug.Log(hit.collider.gameObject.layer);
            //Debug.Log(hit.collider.transform.name);        //----------

            if (hit.collider.TryGetComponent<SelectableObject>(out SelectableObject obj))
            {
                Debug.Log(obj.transform.name);

                if (_hovered != obj)
                {
                    _hovered?.UnSelect();
                    _hovered = obj;
                    _hovered.Select();
                }
            }
            else
            {
                _hovered?.UnSelect();
                _hovered = null;
            }

        }
    }
}
