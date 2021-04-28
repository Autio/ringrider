using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save 
{
    public int gamePlays;
    public int coins;
    public int highScore;
    public int activeCharacter = 0;
    // IDs of the characters that have been unlocked
    public List<int> unlockedCharacters = new List<int>();

}