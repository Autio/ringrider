using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTemplate : MonoBehaviour
{
    public GameCharacterScriptableObject characterData;
    public Character character;

    private void Awake() 
    {
        character.name = characterData.characterName;
        character.cost = characterData.cost;    
        character.color = characterData.color;
        character.prefab = characterData.prefab;
                
    }


}
