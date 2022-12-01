using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class SutureCanvasCreation : MonoBehaviour
{
    [SerializeField] private Camera _operationCamera = null;
    [SerializeField] private LineRenderer _sutureNeedleRenderer = null;
    [SerializeField] private string _meshName = null;
    [SerializeField] private float _simplifyFactor = 0.2f;
    

    private LineRenderer _currentSuture = null;
    private int pointCount = 0;
    private Mesh _outputMesh = null;    

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_currentSuture != null)
            {
                // Start over
                EraseSuture();
            }
            
            StartFirstStich();
        }
        else if (Input.GetMouseButton(0))
        {
            // Nothing ?
            Suture();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveMesh();
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
        Destroy(_currentSuture.gameObject);
        _currentSuture = null;
        pointCount = 0;
    }

    public void SaveMesh()
    {
        GetComponent<MeshFilter>().sharedMesh = new Mesh();
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        // Simplify it
        _currentSuture.Simplify(_simplifyFactor);
        Debug.Log($"position count is {_currentSuture.positionCount}");
        _currentSuture.BakeMesh(mesh);

        SaveMesh(mesh, _meshName, false, false);
    }

    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }
}
#endif // UNITY_EDITOR