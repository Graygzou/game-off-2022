using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperationManager : MonoBehaviour
{
    [SerializeField] private SutureModel _sutureModel = null;
    [SerializeField] private SutureCanvas _operationCanvas = null;
    [SerializeField] private ZoomToWound _zoomManager = null;
    [SerializeField] private Slider _time = null;
    [SerializeField] private GameObject _newOperationText = null;
    [SerializeField] private TipsManager _tipsManager = null;

    [Header("Others")]
    [SerializeField] private AudioClip _hurtSFX = null;


    [SerializeField] private LevelData _currentLevel = null;
    private int _currentOperationIndex = -1;
    private int _currentModelIndex = 0;

    private ScoreValidator _scoreValidator;
    private bool _started = false;
    private bool _finished = false;
    private float _levelSpeed = 1.0f;
    private float _mistakeDamage = 1.0f;

    private Action _onLevelFailed = null;
    public event Action OnLevelFailed
    {
        add
        {
            _onLevelFailed -= value;
            _onLevelFailed += value;
        }
        remove
        {
            _onLevelFailed -= value;
        }
    }

    private Action<int> _onLevelFinished = null;
    public event Action<int> OnLevelFinished
    {
        add
        {
            _onLevelFinished -= value;
            _onLevelFinished += value;
        }
        remove
        {
            _onLevelFinished -= value;
        }
    }

    public void Awake()
    {
        _started = false;
        _finished = true;
        //_currentLevel = null;
        _currentOperationIndex = -1;
    }

    void Start()
    {
        _finished = false;

        if (_currentLevel == null)
        {
            _currentLevel = GameManager.Instance.GetCurrentLevel();
        }

        // We're on the onboarding. So trigger it
        _tipsManager.DisplayNextTips();
        _tipsManager.OnOnboardingDone += () => GameManager.Instance.SetOnBoarding(false);
        GameManager.Instance.SetOnBoarding(true);

        MoveToNextOperation();
    }


    public void StartOperation(OperationData level)
    {
        _levelSpeed = level.lifeReducingSpeed;
        _mistakeDamage = level.mistakeDamage;

        // Update UI
        _sutureModel.ChangeMeshes(level);

        // Set validator for first mesh
        _currentModelIndex = 0;
        ModelData model = _currentLevel.operationSequences[_currentOperationIndex].allModels[_currentModelIndex];
        SetupValidator(model);
        _currentModelIndex++;
    }

    public void OnDestroy()
    {
        _scoreValidator.OnModelValidated -= OnModelValidated;
        _scoreValidator.OnModelValidated -= _sutureModel.MarkModelCompleted;
        _scoreValidator.OnModelRejected -= OnModelRejected;
    }

    public void SetupValidator(ModelData model)
    {
        Mesh mesh = model.meshModel;
        float meshSimplification = model.simplificationUsed;
        float scoreTolerance = model.maxScoreTolerance;

        _scoreValidator = new ScoreValidator(mesh, meshSimplification, scoreTolerance);
        _scoreValidator.OnModelValidated += OnModelValidated;
        _scoreValidator.OnModelValidated += _sutureModel.MarkModelCompleted;
        _scoreValidator.OnModelRejected += OnModelRejected;
        _operationCanvas.Initialize(_scoreValidator);
    }

    public void UpdateValidator(ModelData model)
    {
        Mesh mesh = model.meshModel;
        float meshSimplification = model.simplificationUsed;
        float scoreTolerance = model.maxScoreTolerance;

        _scoreValidator.UpdateModel(mesh, meshSimplification, scoreTolerance);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsOnBoarding)
        {
            return;
        }

        if (!_finished && _time.value <= 0)
        {
            _started = false;
            _finished = true;
            _onLevelFailed?.Invoke();
        }

        // Make the timer gradually reduce
        if (_started)
        {
            _time.value -= Time.deltaTime * _levelSpeed;
        }
    }

    public void OnModelValidated()
    {
        Debug.Log("Model validated !");
        if (GameManager.Instance.IsOnBoarding && _currentModelIndex == 1)
        {
            _tipsManager.DisplaySpecificTips(2);
        }

        int totalModelCount = _currentLevel.operationSequences[_currentOperationIndex].allModels.Count;
        if (_currentModelIndex < totalModelCount)
        {
            Debug.Log("Yeah next model !");
            MoveToNextModel();
            return;
        }

        if (_currentOperationIndex + 1 < _currentLevel.operationSequences.Count)
        {
            Debug.Log("Yeah next operation !");
            MoveToNextOperation();
            return;
        }

        Debug.Log("Level finished !");
        _onLevelFinished?.Invoke(_scoreValidator.GetCumulatedScore());
    }

    public void MoveToNextOperation()
    {
        _currentOperationIndex++;

        StartCoroutine(OperationTeaser());
    }

    public IEnumerator OperationTeaser()
    {
        _newOperationText.SetActive(true);
        yield return new WaitForSeconds(3);
        _newOperationText.SetActive(false);

        // First zoom out if required
        _zoomManager.OnZoomDone += ZoomInToDoOperation;
        if (_zoomManager.ZoomOut() == false)
        {
            // Call it right away
            ZoomInToDoOperation();
        }
    }

    public void ZoomInToDoOperation()
    {
        _zoomManager.OnZoomDone -= ZoomInToDoOperation;

        // Then zoom in
        OperationData data = _currentLevel.operationSequences[_currentOperationIndex];
        _zoomManager.OnZoomDone += PrepareOperation;
        _zoomManager.ZoomIn(data.zone);
    }

    public void PrepareOperation()
    {
        _zoomManager.OnZoomDone -= PrepareOperation;

        // Display wounds here

        // Start the reel operation now !
        StartOperation(_currentLevel.operationSequences[_currentOperationIndex]);

        // In case it's not done yet.
        _started = true;
    }

    public void MoveToNextModel()
    {
        ModelData model = _currentLevel.operationSequences[_currentOperationIndex].allModels[_currentModelIndex];
        UpdateValidator(model);
        _currentModelIndex++;
    }

    public void OnModelRejected()
    {
        if (GameManager.Instance.IsOnBoarding)
        {
            _tipsManager.DisplayNextTips();
            return;
        }

        Debug.Log("Wrong!");
        GameManager.Instance.AddSound(_hurtSFX);
        // TODO: Screen shake Screen
        _time.value -= _mistakeDamage;
    }
}
