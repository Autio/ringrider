using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

public class SaveController : Singleton<SaveController>
{
    
    public void SaveGame(Save save)
    {

        //Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

    }

    // public Save CreateSaveGameObject(Save save)
    // {

    //     save.gamePlays = gamePlays;
    //     save.coins = coins;
    //     save.highScore = highScore;
    //     return save;
    // }

    public Save LoadGame(){
        Debug.Log("Loading game");
        if(File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // Clear stuff if needs clearing
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            Save loadedFile = new Save();
            
            loadedFile.gamePlays = save.gamePlays;
            loadedFile.coins = save.coins;
            loadedFile.highScore = save.highScore;
            loadedFile.unlockedCharacters = save.unlockedCharacters;
            loadedFile.activeCharacter = save.activeCharacter;

            Debug.Log(save.highScore);
            Debug.Log("Game Loaded");

            return loadedFile;

        }

        return null;
    }

}
