using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject linePrefab;

    [Header("Parameters")]
    [SerializeField, Min(0)] private float minDistance = 0.1f;

    private LineRenderer _lineRenderer;
    private List<Vector3> _fingerPosition;
    private Vector3 _currentFingerPosition;
    private bool _isFirstTouch = true;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (TakeTouchRaycast(out _currentFingerPosition))
            {
                if (!_isFirstTouch) UpdateLine();
                else CreateLine();
            }            
        }
        else if (!_isFirstTouch) _isFirstTouch = true;
    }

    private void CreateLine()
    {
        _fingerPosition.Clear();
        _fingerPosition.Add(_currentFingerPosition);
        _fingerPosition.Add(_currentFingerPosition);

        _lineRenderer = Instantiate(linePrefab).GetComponent<LineRenderer>();
        _lineRenderer.SetPosition(0, _currentFingerPosition);
        _lineRenderer.SetPosition(1, _currentFingerPosition);

        _isFirstTouch = false;
    }

    private void UpdateLine()
    {
        if(Vector3.Distance(_currentFingerPosition, _fingerPosition[_fingerPosition.Count - 1]) < minDistance)
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
        Ray ray = mainCamera.ScreenPointToRay(touch.position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            position = hit.point;
            return true;
        }

        return false;
    }

}
