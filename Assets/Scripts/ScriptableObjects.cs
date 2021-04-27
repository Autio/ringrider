using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterScriptableObject", order = 1)]
public class CharacterScriptableObject : ScriptableObject
{

    public string characterName;
    public int cost;
    public GameObject prefab;
    public Color color;

}