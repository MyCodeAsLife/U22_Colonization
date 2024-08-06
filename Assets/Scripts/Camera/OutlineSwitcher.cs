using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class OutlineSwitcher : MonoBehaviour
{
    //public bool IsActive;
    //public GameObject Object_Hit;

    private int _selectionMask;
    //private OutlineViewer _selectedObjectComponent;
    private Outline _outline;

    private void Start()
    {
        _selectionMask = LayerMask.NameToLayer("Interactable");
    }

    private void LateUpdate()
    {
        var mainCamera = FindCamera();

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition).origin,
            mainCamera.ScreenPointToRay(Input.mousePosition).direction, out hit, 100, int.MaxValue ^ _selectionMask))
        {
            Debug.Log(hit.collider.gameObject.name);        //----------

            if (/*_selectedObjectComponent == null*/ _outline == null)
            {
                SelectObject(hit);
            }
            else if (hit.transform.TryGetComponent(out Outline otherComponent)
                     && otherComponent != /*_selectedObjectComponent*/_outline)
            {
                Destroy(/*_selectedObjectComponent*/_outline);
                SelectObject(hit);
            }
        }
    }

    private void SelectObject(RaycastHit hit)
    {
        hit.transform.AddComponent<Outline>();
        /*_selectedObjectComponent*/
        _outline = hit.transform.GetComponent<Outline>();
        _outline.enabled = true;
    }

    private Camera FindCamera()             // ”празднить
    {
        if (GetComponent<Camera>())
        {
            return GetComponent<Camera>();
        }

        return Camera.main;
    }
}
