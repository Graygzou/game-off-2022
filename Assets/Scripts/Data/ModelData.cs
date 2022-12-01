using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Models", order = 1)]
public class ModelData : ScriptableObject
{
    [Header("Level specific")]
    public int operationIndex = 0;
    public float simplificationUsed = 0.1F;
    public float maxScoreTolerance = 100F;
    public Mesh meshModel = null;
}