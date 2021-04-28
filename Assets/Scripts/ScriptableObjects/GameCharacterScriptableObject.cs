using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/GameCharacterScriptableObject", order = 1)]
public class GameCharacterScriptableObject : ScriptableObject
{
    public int id;
    public string characterName;
    public int cost;
    public Transform prefab;
    public Color color;

}