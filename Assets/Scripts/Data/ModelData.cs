using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Models", order = 1)]
public class ModelData : ScriptableObject
{
    [Header("Level specific")]
    public int levelIndex = 0;
    public float simplificationUsed = 0.1F;
    public float maxScoreTolerance = 100F;
    public Mesh meshModel = null;

    [Header("Gameplay specific")]
    public float lifeReducingSpeed = 1.0f;
    public float mistakeDamage = 1.5f;

    // Could maybe be generated
    [Header("Display infos")]
    public string causeOfInjury = string.Empty;
    public string nameOfTheCat = string.Empty;
    public int ageOfTheCat = 1;
}