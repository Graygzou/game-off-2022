using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SutureModel : MonoBehaviour
{
    [SerializeField] private Mesh _currentMesh = null;
    [SerializeField] private MeshFilter _displaySpot = null;

    public void ChangeMesh(Mesh m)
    {
        // Do a transition here
        _currentMesh = m;
        _displaySpot.mesh = m;
    }


    public void FadeInModel()
    {

    }
}
