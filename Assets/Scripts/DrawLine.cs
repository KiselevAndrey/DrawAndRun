using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private enum RayCastSystem { Square_2D, Box_3D }

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform lineParent;

    [Header("Parameters")]
    [SerializeField, Min(0)] private float minDistance = 0.1f;
    [SerializeField] private RayCastSystem rayCastSystem;

    public static System.Action<List<Vector2>> OnEndTouch;

    private GameObject _line;
    private LineRenderer _lineRenderer;
    private List<Vector2> _fingerPosition = new List<Vector2>();
    private Vector3 _currentFingerPosition;
    private bool _isFirstTouch = true;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (TakeTouchRaycast(out _currentFingerPosition, lineParent))
            {
                if (!_isFirstTouch) UpdateLine();
                else CreateLine();
            }
        }
        else if (!_isFirstTouch)
        {
            _isFirstTouch = true;
            OnEndTouch?.Invoke(_fingerPosition);
            Destroy(_line);            
        }
    }

    private void CreateLine()
    {
        _fingerPosition.Clear();
        _fingerPosition.Add(_currentFingerPosition);
        _fingerPosition.Add(_currentFingerPosition);

        _line = Instantiate(linePrefab, lineParent);
        _lineRenderer = _line.GetComponent<LineRenderer>();
        _lineRenderer.SetPosition(0, _currentFingerPosition);
        _lineRenderer.SetPosition(1, _currentFingerPosition);

        _isFirstTouch = false;
    }

    private void UpdateLine()
    {
        if(Vector2.Distance(_currentFingerPosition, _fingerPosition[_fingerPosition.Count - 1]) > minDistance)
        {
            _fingerPosition.Add(_currentFingerPosition);
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _currentFingerPosition);
        }
    }


    private bool TakeTouchRaycast(out Vector3 position)
    {
        position = Vector3.zero;
        Touch touch = Input.GetTouch(0);
        switch (rayCastSystem)
        {
            case RayCastSystem.Square_2D:
                RaycastHit2D hit2D = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(touch.position), Vector2.zero);

                if (hit2D.collider != null)
                {
                    position = hit2D.point;
                    return true;
                }
                break;

            case RayCastSystem.Box_3D:
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    position = hit.point;
                    return true;
                }
                break;
        }

        return false;
    }

    private bool TakeTouchRaycast(out Vector3 position, Transform parent)
    {
        bool result = TakeTouchRaycast(out position);
        if (result)
        {
            position -= parent.position;
        }

        return result;
    }

}
