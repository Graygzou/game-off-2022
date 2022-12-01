using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button _startButton = null;
    [SerializeField] private Button _creditsButton = null;
    [SerializeField] private AudioClip _selectionCip = null;

    void Start()
    {
        _startButton.onClick.RemoveAllListeners();
        _startButton.onClick.AddListener(OnStartClicked);

        _creditsButton.onClick.RemoveAllListeners();
        _creditsButton.onClick.AddListener(OnCreditClicked);
    }

    private void OnStartClicked()
    {
        GameManager.Instance.AddSound(_selectionCip);
        GameManager.Instance.StartGame();
    }

    private void OnCreditClicked()
    {
        GameManager.Instance.AddSound(_selectionCip);
        GameManager.Instance.SwitchToContext(GameContext.Credits);
    }
}
