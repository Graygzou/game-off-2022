using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup = null;
    [SerializeField] private Image _characterEyes = null;
    [SerializeField] private Image _characterMouth = null;
    [SerializeField] private TextMeshProUGUI _characterSpeech = null;

    public void Awake()
    {
        _canvasGroup.alpha = 0;
        _characterSpeech.text = string.Empty;
    }

    public void EnableBox(bool toggle, Speech nextSpeech)
    {
        _canvasGroup.alpha = toggle ? 1 : 0;
        _canvasGroup.interactable = toggle;
        _canvasGroup.blocksRaycasts = toggle;
        if (toggle)
        {
            SetPreview(nextSpeech.mouthSprite, nextSpeech.eyesSprite);
            SetText(nextSpeech.characterText);
        }
    }

    public void SetText(string text)
    {
        _characterSpeech.text = text;
    }

    public void SetPreview(Sprite mouth, Sprite eyes)
    {
        if (mouth != null)
        {
            _characterMouth.sprite = mouth;
        }

        if (eyes != null)
        {
            _characterEyes.sprite = eyes;
        }
    }
}