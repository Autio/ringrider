using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : Singleton<PlayerCharacterController>
{
    public Transform playerCharacter;
    List<GameObject> CharacterPrefabs = new List<GameObject>();
    public List<GameCharacterScriptableObject> PlayerCharacterScriptableObjects = new List<GameCharacterScriptableObject>();

    public void UpdateCharacter(int id)
    {
        // Update the colour of the actvie player object
        Color newColor = PlayerCharacterScriptableObjects[id].color;
        playerCharacter.GetComponent<SpriteRenderer>().color = newColor;

    }

}
