using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image _dialogBackground = null;
    [SerializeField] private DialogBox _leftDialogueBox = null;
    [SerializeField] private DialogBox _rightDialogueBox = null;
    [SerializeField] private Button[] _skipButtons = null;

    [Header("Data")]
    [SerializeField] private DialogueData[] _stories = null;
    [SerializeField] private DialogueData[] _tips = null;

    private int _currentActiveStory = 0;
    private int _currentActiveTips = 0;
    private DialogueData _currentData = null;
    private List<Speech> _remainingSpeech = null;

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
        _currentActiveStory += _currentData.IsStory ? 1 : 0;
        _currentActiveTips += _currentData.IsStory ? 0 : 1;
    }

    public void FastForwardDialogue()
    {
    }

    public void SkipDialogue()
    {
        _currentActiveStory++;
    }

    public void DisplayNextDialogue()
    {
        if (_currentActiveStory >= _stories.Length)
        {
            Debug.LogError("No more stories to tell !");
            return;
        }

        _currentData = _stories[_currentActiveStory];
        if (_currentData.Speeches.Count <= 0)
        {
            Debug.LogError("No speech in this dialog !");
            return;
        }

        _remainingSpeech = new List<Speech>(_currentData.Speeches.Count);
        OpenDialogueBox();
    }

    private void OpenDialogueBox()
    {
        if (_currentData.DialogueBackground != null)
        {
            _dialogBackground.sprite = _currentData.DialogueBackground;
        }

        _remainingSpeech.AddRange(_currentData.Speeches);
        DisplayNextSpeech();
    }

    private void DisplayNextSpeech()
    {
        if (_remainingSpeech.Count <= 0)
        {
            Debug.Log("Story is ended !");
            if (_onStoryEnded != null)
            {
                _onStoryEnded();
            }

            // In all cases stop here to avoid error.
            return;
        }

        Speech nextSpeech = _remainingSpeech[0];
        _remainingSpeech.RemoveAt(0);

        _leftDialogueBox.EnableBox(nextSpeech.isPreviewOnLeft, nextSpeech);
        _rightDialogueBox.EnableBox(!nextSpeech.isPreviewOnLeft, nextSpeech);
    }
}
