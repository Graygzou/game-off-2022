using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image _dialogBackground = null;
    [SerializeField] private DialogBox _leftDialogueBox = null;
    [SerializeField] private DialogBox _rightDialogueBox = null;
    [SerializeField] private Button[] _skipButtons = null;

    [Header("Data")]
    [SerializeField] private DialogueData[] _tips = null;

    private int _currentActiveTips = 0;
    private DialogueData _currentData = null;
    private List<Speech> _remainingSpeech = new List<Speech>();

    public static bool IsTipsActive = false;

    private Action _onOnboardingDone = null;
    public event Action OnOnboardingDone
    {
        add
        {
            _onOnboardingDone -= value;
            _onOnboardingDone += value;
        }
        remove
        {
            _onOnboardingDone -= value;
        }
    }

    private Action _onStoryEnded = null;
    public event Action OnStoryEnded
    {
        add
        {
            _onStoryEnded -= value;
            _onStoryEnded += value;
        }
        remove
        {
            _onStoryEnded -= value;
        }
    }

    public void Start()
    {
        foreach (var button in _skipButtons)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(DisplayNextSpeech);
        }
    }

    public void CloseDialogueBox()
    {
        _currentActiveTips += _currentData.IsStory ? 0 : 1;
    }

    public void FastForwardDialogue()
    {
    }

    public void SkipDialogue()
    {
        // TODO ?
    }

    public void DisplaySpecificTips(int index)
    {
        _currentActiveTips = index;
        _currentData = _tips[_currentActiveTips];
        OpenTipsBox();
        _currentActiveTips++;
    }

    public void DisplayNextTips()
    {
        if (_currentActiveTips >= _tips.Length)
        {
            Debug.Log("No more Tips to show !");
            _onOnboardingDone?.Invoke();
            return;
        }

        _currentData = _tips[_currentActiveTips];
        OpenTipsBox();
        _currentActiveTips++;
    }

    private void OpenTipsBox()
    {
        IsTipsActive = true;
        _remainingSpeech.AddRange(_currentData.Speeches);
        DisplayNextSpeech();
    }

    private void DisplayNextSpeech()
    {
        if (_remainingSpeech.Count <= 0)
        {
            IsTipsActive = false;
            // Should display them after
            return;
        }

        Speech nextSpeech = _remainingSpeech[0];
        _remainingSpeech.RemoveAt(0);

        _leftDialogueBox.EnableBox(nextSpeech.isPreviewOnLeft, nextSpeech);
        _rightDialogueBox.EnableBox(!nextSpeech.isPreviewOnLeft, nextSpeech);
    }
}
