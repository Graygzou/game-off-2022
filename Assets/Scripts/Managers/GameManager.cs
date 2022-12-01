using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Texture2D _needleCrosshair = null;
    [SerializeField] private SoundManager _soundManager = null;
    [SerializeField] private FadeManager _fadeManager = null;

    [SerializeField] private LevelData[] _levels = null;

    private GameContext _currentContext = GameContext.MainMenu;
    private int _currentLevelIndex = 0;
    private int _currentStoryIndex = 0;
    private LevelData _currentLevel = null;
    private DialogueManager _dialogManagerFound = null;
    private OperationManager _operationManagerFound = null;

    public LevelData GetCurrentLevel()
    {
        return _currentLevel;
    }

    public override void Awake()
    {
        base.Awake();
        Reset();
    }

    public void Reset()
    {
        _currentLevelIndex = 0;
        _currentStoryIndex = 0;
        _currentLevel = null;
    }

    public void Start()
    {
        // Scene Management
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitCursor();

        _soundManager.StartBackgroundMusic(_currentContext);
    }

    public void InitCursor()
    {
        Vector2 hotSpot = new Vector2(0, _needleCrosshair.height);
#if UNITY_WEBGL
        Cursor.SetCursor(_needleCrosshair, hotSpot, CursorMode.ForceSoftware);
#else
        Cursor.SetCursor(_needleCrosshair, hotSpot, CursorMode.Auto);
#endif
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartGame()
    {
#if UNITY_EDITOR
        Debug.Log("The game will start");
#endif

        // Story telling
        SwitchToContext(GameContext.Story);
    }

    public void SwitchToContext(GameContext newcontext)
    {
        _currentContext = newcontext;

        _fadeManager.StartFadout();
        SceneManager.LoadScene((int)_currentContext);
        _soundManager.StartBackgroundMusic(_currentContext);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameContext context = (GameContext)scene.buildIndex;
        switch (context)
        {
            case GameContext.MainMenu:
                // Reset everything
                Reset();
                break;

            case GameContext.Credits:
                break;

            case GameContext.Story:
                // Get the DialogueManager somewhere in the scene
                GameObject dialogueManagerObject = GameObject.FindGameObjectWithTag("DialogueManager");
                _dialogManagerFound = dialogueManagerObject.GetComponent<DialogueManager>();
                _dialogManagerFound.OnStoryEnded += OnDialogEnded;
                _dialogManagerFound.OnNoStoriesLeft += OnNoStoryLeft;
                _dialogManagerFound.DisplayNextDialogue(_currentStoryIndex);
                break;

            case GameContext.Operation:
                GameObject operationManager = GameObject.FindGameObjectWithTag("OperationManager");
                _operationManagerFound = operationManager.GetComponent<OperationManager>();
                _operationManagerFound.OnLevelFailed += GameOver;
                _operationManagerFound.OnLevelFinished += OnLevelFinished;
                break;

            case GameContext.GameOver:
                break;

            default:
                break;
        }
        if (context == GameContext.Story)
        {

        }
        else if (context == GameContext.Story)
        {
            
        }
        else if (context == GameContext.Operation)
        {
            
        }
    }

    public void GameOver()
    {
        SwitchToContext(GameContext.GameOver);
    }

    public void OnDialogEnded()
    {
        _dialogManagerFound.OnStoryEnded -= OnDialogEnded;
        _currentStoryIndex++;
        
        if (_currentLevelIndex < _levels.Length)
        {
            // Jump to the next level
            _currentLevel = _levels[_currentLevelIndex];
            SwitchToContext(GameContext.Operation);
        }
        else
        {
            // Game finished
            SwitchToContext(GameContext.Congrats);
        }
    }

    public void OnNoStoryLeft()
    {
        // End of the game for now
        SwitchToContext(GameContext.Congrats);
    }

    // UI specific
    public void ShowCredits()
    {
        SwitchToContext(GameContext.Credits);
    }

    public void OnLevelFinished(int cumulatedScore)
    {
        SwitchToContext(GameContext.Story);
    }
}
