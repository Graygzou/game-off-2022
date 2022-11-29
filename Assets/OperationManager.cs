using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationManager : MonoBehaviour
{
    [SerializeField] private Texture2D _needleCrosshair;
    [SerializeField] private SutureModel _sutureModel = null;
    [SerializeField] private SutureCanvas _operationCanvas = null;
    [SerializeField] private Slider _time = null;

    private LevelData _currentLevel = null;
    private int _currentOperationIndex = 0;

    private ScoreValidator _scoreValidator;
    private bool _started = false;
    private bool _finished = false;
    private float _levelSpeed = 1.0f;
    private float _mistakeDamage = 1.0f;

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
        _currentLevel = null;
        _currentOperationIndex = 0;
    }

    void Start()
    {
        Vector2 cursorOffset = new Vector2(_needleCrosshair.width / 2, _needleCrosshair.height / 2);
        Cursor.SetCursor(_needleCrosshair, cursorOffset, CursorMode.Auto);

        _currentLevel = GameManager.Instance.GetCurrentLevel();
        _currentOperationIndex = 0;
        StartOperation(_currentLevel.operationSequences[_currentOperationIndex]);
    }


    public void StartOperation(ModelData level)
    {
        _levelSpeed = level.lifeReducingSpeed;
        _mistakeDamage = level.mistakeDamage;

        // Update UI
        _sutureModel.ChangeMesh(level.meshModel);

        // Validator
        SetupValidator(level.meshModel, level.simplificationUsed, level.maxScoreTolerance);
        
        _started = true;
    }

    public void SetupValidator(Mesh model, float meshSimplification, float scoreTolerance)
    {
        _scoreValidator = new ScoreValidator(model, meshSimplification, scoreTolerance);
        _scoreValidator.OnModelValidated += OnModelValided;
        _scoreValidator.OnModelRejected += OnModelRejected;
        _operationCanvas.Initialize(_scoreValidator);
    }

    // Update is called once per frame
    void Update()
    {
        if (_time.value <= 0)
        {
            _started = false;
            _finished = true;
            GameManager.Instance.GameOver();
        }

        // Make the timer gradually reduce
        if (_started)
        {
            _time.value -= Time.deltaTime * _levelSpeed;
        }
    }

    public void OnModelValided()
    {
        if (_currentOperationIndex >= _currentLevel.operationSequences.Count)
        {
            Debug.Log("Level finished !");
            _onLevelFinished?.Invoke(_scoreValidator.GetCumulatedScore());
            return;
        }

        Debug.Log("Yeah next one !");
        _currentOperationIndex++;
        StartOperation(_currentLevel.operationSequences[_currentOperationIndex]);
    }

    public void OnModelRejected()
    {
        Debug.Log("Wrong!");
        // TODO: Screen shake Screen
        _time.value -= _mistakeDamage;
    }
}
