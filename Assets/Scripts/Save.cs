using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save 
{
    public int gamePlays;
    public int coins;
    public int highScore;
    public Character activeCharacter;
    public List<Character> unlockedCharacters = new List<Character>();
}


public class Character 
{
    public string name;
    public int cost;
    public GameObject prefab;
}