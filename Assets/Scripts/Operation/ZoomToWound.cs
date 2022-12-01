using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomToWound : MonoBehaviour
{
    // Will be used for translate
    [SerializeField] private Transform _patientTransform = null;
    // Will be used for scaling
    [SerializeField] private Transform _patientModelTransform = null;

    [SerializeField] private List<Vector3> _patientZones = null;
    [SerializeField] private float _zoomScale = 1.0f;
    [SerializeField] private float _zoomSpeed = 1.0f;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private PatientZone _zoneIdToReach = PatientZone.Face;
#endif // UNITY_EDITOR

    // History of the prefab
    private Vector3 _startingPosition = Vector3.zero;
    private Vector3 _startingScale = Vector3.one;

    private bool _isMoving = false;
    private Vector3 _positionToReach = Vector3.zero;
    private Vector3 _scaleToReach = Vector3.zero;

    private Action _onZoomDone = null;
    public event Action OnZoomDone
    {
        add
        {
            _onZoomDone -= value;
            _onZoomDone += value;
        }
        remove
        {
            _onZoomDone -= value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _isMoving = false;
        _startingPosition = _patientTransform.position;
        _startingScale = _patientModelTransform.localScale;
        _positionToReach = Vector3.zero;
        _scaleToReach = Vector3.zero;
    }

    // Update is called once per frame
    public void ZoomIn(PatientZone zoneToReach)
    {
        Debug.Log($"Zoom to zone {zoneToReach}!");
        // Should center the zone to reach and zoom close to it
        _positionToReach = _patientZones[(int)zoneToReach];
        _scaleToReach = Vector3.one * _zoomScale;
        _isMoving = _patientTransform.position != _positionToReach;

#if UNITY_EDITOR
        Debug.Log("_positionToReach " + _positionToReach);
        Debug.Log("_scaleToReach " + _scaleToReach);
        if (_patientTransform.position == _positionToReach)
        {
            Debug.Log("Already at the position !");
        }
#endif // UNITY_EDITOR
    }

    public bool ZoomOut()
    {
        _positionToReach = _startingPosition;
        _scaleToReach = _startingScale;
        _isMoving = _patientTransform.position != _positionToReach;

        return _isMoving;
    }

    public void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            ZoomIn(_zoneIdToReach);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ZoomOut();
        }
#endif // UNITY_EDITOR

        if (_isMoving)
        {
            if (_patientTransform.position.x - _positionToReach.x < Mathf.Epsilon &&
                _patientTransform.position.y - _positionToReach.y < Mathf.Epsilon)
            {
                float step = _zoomSpeed * Time.deltaTime; // calculate distance to move
                _patientTransform.position = Vector3.LerpUnclamped(_patientTransform.position, _positionToReach, step);
                _patientModelTransform.localScale = Vector3.LerpUnclamped(_patientModelTransform.localScale, _scaleToReach, step);
            }
            else
            {
                _onZoomDone?.Invoke();
                _isMoving = false;
            }
        }
    }

    internal void MoveActivePatient()
    {
        _patientTransform.transform.Translate(Vector3.left);
    }

    internal void MoveInactivePatient()
    {
        _patientTransform.transform.Translate(Vector3.right);
    }
}
