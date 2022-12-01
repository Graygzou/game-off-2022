using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Button _retryButton = null;
    [SerializeField] private Button _mainMenuButton = null;

    // Start is called before the first frame update
    public void Start()
    {
        _retryButton?.onClick.RemoveAllListeners();
        _retryButton?.onClick.AddListener(OnRetryClicked);

        _mainMenuButton?.onClick.RemoveAllListeners();
        _mainMenuButton?.onClick.AddListener(OnReturnMainMenuClicked);
    }

    public void OnRetryClicked()
    {
        // Just retry the current level without going through the whole story
        GameManager.Instance.SwitchToContext(GameContext.Operation);
    }

    public void OnReturnMainMenuClicked()
    {
        GameManager.Instance.SwitchToContext(GameContext.MainMenu);
    }
}
