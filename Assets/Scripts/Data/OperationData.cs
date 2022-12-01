using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Operation", order = 1)]
public class OperationData : ScriptableObject
{
    [Header("Level specific")]
    public List<ModelData> allModels = new List<ModelData>();
    public PatientZone zone = PatientZone.Face;

    [Header("Gameplay specific")]
    public float mistakeDamage = 0.05f;
    public float lifeReducingSpeed = 0.01f;

    // Could maybe be generated
    [Header("Display infos")]
    public string causeOfInjury = string.Empty;
    public string nameOfTheCat = string.Empty;
    public int ageOfTheCat = 1;
}