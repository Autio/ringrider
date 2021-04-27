using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/GameCharacterScriptableObject", order = 1)]
public class GameCharacterScriptableObject : ScriptableObject
{

    public string characterName;
    public int cost;
    public GameObject prefab;
    public Color color;

}