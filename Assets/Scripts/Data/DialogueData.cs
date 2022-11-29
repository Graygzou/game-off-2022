using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Dialogues", order = 1)]
public class DialogueData : ScriptableObject
{
    public bool IsStory = true;
    public List<Speech> Speeches = new List<Speech>();
    public Sprite DialogueBackground = null;
}