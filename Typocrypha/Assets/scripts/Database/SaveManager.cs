using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;


/* Multiple Save Files -
 * 
 * Need to add a system that can load from multiple save
 * slots. Slots can just be 'numbers' that have different
 * paths in the 'NewGame' and 'LoadGame' functions. In
 * those functions we need to dynamically change file
 * names but that should be about it.
 */

/* Documentation:
 * 
 * The GameData class holds the exact things to be stored
 * in the .dat file, all save data should be specified in
 * -1-.
 * 
 * The games data is tracked locally in -2-, this section
 * should be updated as things change.
 * 
 * -3- holds the initial values for all saved things before
 * the player has done anything
 * 
 * -4- loads the data from the save file into the locally
 * tracked game data to be accessed by other parts of the
 * game.
 * 
 * -5- stores the local data into a GameData object to be
 * written to the save file
 * 
 * --WHEN ADDING NEW PARAMETERS--
 * make sure you update each section, -1- -2- -3- -4- -5-
 * with the new parameters that are being saved. 
 * 
 */

public class SaveManager : MonoBehaviour {

    /*
     * this class contains what is stored in the actual
     * .dat file, when saving things are loaded into a
     * GameData object then serialized
     */  
    [Serializable]
    public class GameData {
        //------------1------------//
        public int vol1;
        public int vol2;
        public int vol3;
        //------------1------------//
    }

    public static SaveManager manager;

    //------------2------------//
    public int volume1;
    public int volume2;
    public int volume3;
    //------------2------------//

    void Awake() {
        if(manager == null) {
            manager = this;
            DontDestroyOnLoad(gameObject);

            Reset();
        } else if(this != manager) {
            Destroy(gameObject);
        }
    }

    //Resets all values to new game values
    void Reset() {
        //------------3------------//
        volume1 = 100;
        volume2 = 100;
        volume3 = 100;
        //------------3------------//
    }

    //creates a new save file
    void NewGame() {
        Reset();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(Application.persistentDataPath + "/savefile.dat", FileMode.Create);

        GameData data = GetData();

        bf.Serialize(file, data);
        file.Close();
    }

    //save the current game data into the save file
    public void SaveGame() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/savefile.dat", FileMode.Open);

        GameData data = GetData();

        bf.Serialize(file, data);
        file.Close();
    }

    //load the parameters saved into the save file
    public void LoadGame() {
        if (File.Exists(Application.persistentDataPath + "/savefile.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savefile.dat", FileMode.Open);

            GameData data = (GameData)bf.Deserialize(file);
            file.Close();

            //------------4------------//
            volume1 = data.vol1;
            volume2 = data.vol2;
            volume3 = data.vol3;
            //------------4------------//
        }
    }

    //store local data into a GameData object
    public GameData GetData() {
        GameData data = new GameData();
        
        //-----------5------------//
        data.vol1 = volume1;
        data.vol2 = volume2;
        data.vol3 = volume3;
        //-----------5------------//

        return data;
    } 
}
