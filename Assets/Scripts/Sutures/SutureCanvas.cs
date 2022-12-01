using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SutureCanvas : MonoBehaviour
{
    [SerializeField] private Camera _operationCamera = null;
    [SerializeField] private LineRenderer _sutureNeedleRenderer = null;

    private bool _isInitialized = false;
    private ScoreValidator _scoreValidator = null;
    private LineRenderer _currentSuture = null;
    private int pointCount = 0;

    public void Initialize(ScoreValidator scoreValidator)
    {
        _isInitialized = true;
        _scoreValidator = scoreValidator;
        _scoreValidator.OnModelValidated += EraseSuture;
        _scoreValidator.OnModelRejected += EraseSuture;
    }

    public void OnDestroy()
    {
        _scoreValidator.OnModelValidated -= EraseSuture;
        _scoreValidator.OnModelRejected -= EraseSuture;
    }

    // Update is called once per frame
    public void Update()
    {
        if (!_isInitialized || (GameManager.Instance.IsOnBoarding && TipsManager.IsTipsActive))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_currentSuture != null)
            {
                EraseSuture();
            }
            
            StartFirstStich();
        }
        else if (Input.GetMouseButton(0))
        {
            // Nothing ?
            Suture();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(_currentSuture != null);
            if (_currentSuture != null)
            {
                _scoreValidator.ComputeScore(_currentSuture);
            }
        }
    }

    private void StartFirstStich()
    {
        _currentSuture = Instantiate(_sutureNeedleRenderer);

        Vector2 mousePosition = _operationCamera.ScreenToWorldPoint(Input.mousePosition);
        _currentSuture.SetPosition(pointCount, mousePosition);
        pointCount++;
        _currentSuture.SetPosition(pointCount, mousePosition);
        pointCount++;
    }

    private void Suture()
    {
        _currentSuture.positionCount = pointCount + 1;
        Vector2 mousePosition = _operationCamera.ScreenToWorldPoint(Input.mousePosition);
        _currentSuture.SetPosition(pointCount , mousePosition);
        pointCount++;
    }

    private void EraseSuture()
    {
        _scoreValidator.ResetScore();
        Destroy(_currentSuture.gameObject);
        _currentSuture = null;
        pointCount = 0;
    }
}
