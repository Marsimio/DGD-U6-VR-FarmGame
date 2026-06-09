using UnityEngine;

[CreateAssetMenu(fileName = "SeedData", menuName = "Scriptable Objects/SeedData")]
public class SeedData : ScriptableObject
{
    public string seedName;
    public GameObject cropPrefab;
}
