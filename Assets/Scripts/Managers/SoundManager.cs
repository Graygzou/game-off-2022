using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioSource _musicSource = null;
    [SerializeField] private GameObject _sfxPrefab = null;

    [Header("Settings")]
    [SerializeField] private AudioClip _mainMenuMusic = null;
    [SerializeField] private AudioClip _StoryTellingMusic = null;
    [SerializeField] private AudioClip[] _roundsMusic = null;
    [SerializeField] private AudioClip _deathMusic = null;

    private List<GameObject> gos = null;

    public void Awake()
    {
        gos = new List<GameObject>();
        _musicSource.playOnAwake = false;
    }

    public void OnDestroy()
    {
        foreach (var item in gos)
        {
            Destroy(item);
        }
    }

    public void StartBackgroundMusic(GameContext context)
    {
        switch (context)
        {
            case GameContext.MainMenu:
                _musicSource.clip = _mainMenuMusic;
                break;
            case GameContext.Story:
                _musicSource.clip = _StoryTellingMusic;
                break;
            case GameContext.Operation:
                _musicSource.clip = _roundsMusic[0];
                break;
            case GameContext.GameOver:
                _musicSource.clip = _deathMusic;
                break;
            default:
                return;
        }

        _musicSource.Play();
    }

    public void AddSound(AudioClip audioClip)
    {
        // Play the audioClip oneshot
        GameObject go = GameObject.Instantiate(_sfxPrefab, transform);
        gos.Add(go);
        AudioSource audio = go.GetComponent<AudioSource>();
        audio.clip = audioClip;
        audio.Play();
    }

    public void UpdateMusicPitch(float pitch)
    {
        // Speed up the music when timing is near the end
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
