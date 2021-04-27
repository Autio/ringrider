using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public List<GameObject> CharacterPrefabs = new List<GameObject>();
    public List<Character> Characters = new List<Character>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void LoadCharacters()
    {
        foreach(GameObject c in CharacterPrefabs)
        {
            Characters.Add(c.GetComponent<CharacterTemplate>().character);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
