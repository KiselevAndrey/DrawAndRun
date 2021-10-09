using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GraphicRaycasterRaycaster : MonoBehaviour
{
    private GraphicRaycaster _raycaster;
    private PointerEventData _pointerEventData;
    private EventSystem _eventSystem;

    void Start()
    {
        _raycaster = GetComponent<GraphicRaycaster>();
        _eventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            _raycaster.Raycast(_pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.worldPosition);
            }
        }
    }
}