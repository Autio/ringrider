using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterController : Singleton<GameCharacterController>
{
    List<GameObject> CharacterPrefabs = new List<GameObject>();
    public List<GameCharacterScriptableObject> CharacterScriptableObjects = new List<GameCharacterScriptableObject>();

    public void UpdateCharacter(int id)
    {
        // Update the colour of the actvie player object
        Color newColor = CharacterScriptableObjects[id].color;
    }
 
    // Start is called before the first frame update
    void Start()
    {

    }

    public void LoadCharacters()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
