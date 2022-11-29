using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreValidator
{
    private int _cumulatedScore = 0;

    private float _scoreLine = 0.0f;
    private float _scoreMesh = 0.0f;
    private bool _scoreComputed = false;

    private Mesh _modelMesh = null;
    private float _simplificationUsed;
    private float _maxScoreTolerance;

    private Action _onModelValidated = null;
    public event Action OnModelValidated
    {
        add
        {
            _onModelValidated -= value;
            _onModelValidated += value;
        }
        remove
        {
            _onModelValidated -= value;
        }
    }

    private Action _onModelRejected = null;
    public event Action OnModelRejected
    {
        add
        {
            _onModelRejected -= value;
            _onModelRejected += value;
        }
        remove
        {
            _onModelRejected -= value;
        }
    }

    public ScoreValidator(Mesh modelMesh, float simplificationUsed, float maxScoreTolerance)
    {
        this._modelMesh = modelMesh;
        this._simplificationUsed = simplificationUsed;
        this._maxScoreTolerance = maxScoreTolerance;
    }

    public void ResetScore()
    {
        _scoreLine = 0.0f;
        _scoreMesh = 0.0f;
        _scoreComputed = false;
    }

    // Update is called once per frame
    public void ComputeScore(LineRenderer currentSuture)
    {
        if (_scoreComputed)
        {
            // Score already computed
            return;
        }

        // simplified it
        currentSuture.Simplify(_simplificationUsed);
        Debug.Log("Mesh vertices is = " + _modelMesh.vertices.Length);
        Debug.Log("Line renderer count is =  " + currentSuture.positionCount);

        ScoreWithLineRenderer(currentSuture, _modelMesh);
        ScoreWithMesh(currentSuture, _modelMesh);
        _scoreComputed = true;
    }

    public void ScoreWithLineRenderer(LineRenderer currentSuture, Mesh mesh)
    {
        int minBound = Mathf.Min(mesh.vertices.Length, currentSuture.positionCount);

        Vector3 firstMeshPoint = mesh.vertices[0];
        for (int i = 0; i < minBound; i++)
        {
            // Always offset by the starting point
            Vector3 offsetPositionModel = mesh.vertices[i] - firstMeshPoint;
            Vector3 offsetPositionSuture = currentSuture.GetPosition(i) - currentSuture.GetPosition(0);
            Debug.Log($"MODEL: Comparing distance of {mesh.vertices[i]} becoming {offsetPositionModel}");
            Debug.Log($"DRAW: Comparing distance of {currentSuture.GetPosition(i)} becoming {offsetPositionSuture}");
            _scoreLine += Vector3.Distance(offsetPositionModel, offsetPositionSuture);
        }

        Debug.Log("Line Score is " + _scoreLine);
    }

    internal int GetCumulatedScore()
    {
        return _cumulatedScore;
    }


    // This one is the best one with the right simplification
    public void ScoreWithMesh(LineRenderer currentSuture, Mesh mesh)
    {
        Mesh m = new Mesh();
        currentSuture.BakeMesh(m);
        Debug.Log("Generated Mesh vertices count is =  " + m.vertices.Length);

        int minBound = Mathf.Min(mesh.vertices.Length, m.vertices.Length);

        Vector3 firstMeshPoint = mesh.vertices[0];
        for (int i = 0; i < minBound; i++)
        {
            Vector3 offsetPositionModel = mesh.vertices[i] - firstMeshPoint;
            Vector3 offsetPositionSuture = m.vertices[i] - m.vertices[0];
            Debug.Log($"MODEL: Comparing distance of {mesh.vertices[i]} becoming {offsetPositionModel}");
            Debug.Log($"DRAW: Comparing distance of {m.vertices[i]} becoming {offsetPositionSuture}");
            _scoreMesh += Vector3.Distance(offsetPositionModel, offsetPositionSuture);
        }

        Debug.Log("Mesh Score is " + _scoreMesh);
        if (_scoreMesh <= _maxScoreTolerance)
        {
            _onModelValidated?.Invoke();
        }
        else
        {
            _onModelRejected?.Invoke();
        }
    }
}
