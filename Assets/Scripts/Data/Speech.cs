using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Speech
{
    [Header("UI stuff")]
    [SerializeField] public Sprite mouthSprite;
    [SerializeField] public Sprite eyesSprite;
    [SerializeField] public string characterName;

    [Header("Gameplay stuff")]
    [SerializeField] public bool isPreviewOnLeft;
    [SerializeField] public string characterText;
    [SerializeField] public AudioClip characterSound;
}
