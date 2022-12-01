using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SutureModel : MonoBehaviour
{
    [SerializeField] private List<GameObject> _wounds = null;
    [SerializeField] private GameObject _modelDisplayPrefab = null;
    [SerializeField] private VerticalLayoutGroup _layout = null;
    [SerializeField] private CanvasGroup _group = null;
    [SerializeField] private float _fadeInSpeed = 1.0f;
    [SerializeField] private float _moveOutSpeed = 1.0f;

    private List<GameObject> _children = new List<GameObject>();
    private int _lastActiveChild = 0;
    private OperationData _currentOperation = null;
    private Coroutine _currentCoroutine = null;

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void ChangeMeshes(OperationData operation)
    {
        _children.Clear();
        _lastActiveChild = 0;
        _layout.enabled = true;
        _currentOperation = operation;

        // Instantiate them all
        CreateDisplayModels(operation.allModels);

        // Display them
        FadeInModel();
    }

    public void CreateDisplayModels(List<ModelData> models)
    {
        int count = 0;
        foreach (var model in models)
        {
            GameObject gameobject = Instantiate(_modelDisplayPrefab, transform);
            _children.Add(gameobject);
            MeshFilter filter = gameobject.GetComponentInChildren<MeshFilter>();
            if (filter != null)
            {
                filter.mesh = model.meshModel;
                _wounds[count].SetActive(true);

                // Set back the alpha to one
                SpriteRenderer r = _wounds[count].GetComponent<SpriteRenderer>();
                r.color = new Color(255, 255, 255, 255);

                // Start particules systemes
                ParticleSystem particules = _wounds[_lastActiveChild].GetComponentInChildren<ParticleSystem>();
                particules?.Play();

                count++;
            }
        }

        for (int i = count; i < _wounds.Count; i++)
        {
            _wounds[count].SetActive(false);
        }
    }

    public void FadeInModel()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        while (_group.alpha < 1.0f)
        {
            _group.alpha += Mathf.Lerp(0, 1, _fadeInSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void MarkModelCompleted()
    {
        // Desactivate the layout
        _layout.enabled = false;

        StartCoroutine(MoveOutModel());
    }

    public IEnumerator MoveOutModel()
    {
        // Always the first one
        GameObject go = _children[_lastActiveChild];
        SpriteRenderer r = _wounds[_lastActiveChild].GetComponent<SpriteRenderer>();

        // Stop particules systemes
        ParticleSystem particules = _wounds[_lastActiveChild].GetComponentInChildren<ParticleSystem>();
        particules?.Stop();

        _lastActiveChild++;
        
        Vector3 destination = new Vector3(-400, go.transform.localPosition.y, go.transform.localPosition.z);

        Color finalColor = new Color(255, 255, 255, 0);
        while (go.transform.localPosition.x > -300)
        {
            r.color = Color.LerpUnclamped(r.color, finalColor, _moveOutSpeed * 6 * Time.deltaTime);
            go.transform.localPosition = Vector3.LerpUnclamped(go.transform.localPosition, destination, _moveOutSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(go);

        
        //_layout.enabled = true;
        //yield return null;
        //_layout.enabled = false;
    }
}
