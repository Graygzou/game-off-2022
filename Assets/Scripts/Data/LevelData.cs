using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level", order = 1)]
public class LevelData : ScriptableObject
{
    public List<ModelData> operationSequences = new List<ModelData>();
}