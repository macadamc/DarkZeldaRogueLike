using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShadyPixel.SaveLoad
{
    [System.Serializable]
    public class SaveFile
    {
        public List<string> progressionKeyNames;
        public List<string> progressionValues;

    }

    public class SaveLoadManager : MonoBehaviour
    {

        public void SaveGame()
        {
            string savePath = Application.persistentDataPath + "/save.dat";

            SaveFile saveFile = new SaveFile();
            saveFile.progressionKeyNames = GameManager.GM.progressionManager.gameProgression.progressionKeyNames;
            saveFile.progressionValues = GameManager.GM.progressionManager.gameProgression.progressionValues;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(savePath);
            bf.Serialize(file, saveFile);
            file.Close();

            Debug.Log("Game saved.");
        }

        public void LoadGame()
        {
            string savePath = Application.persistentDataPath + "/save.dat";

            if (File.Exists(savePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(savePath, FileMode.Open);

                SaveFile saveFile = (SaveFile)bf.Deserialize(file);

                GameManager.GM.progressionManager.gameProgression.progressionKeyNames = saveFile.progressionKeyNames;
                GameManager.GM.progressionManager.gameProgression.progressionValues = saveFile.progressionValues;

                file.Close();

                Debug.Log("Game loaded.");
            }
        }

        public void DeleteSave()
        {
            string savePath = Application.persistentDataPath + "/save.dat";

            if (File.Exists(savePath))
            {
                File.Delete(savePath);

                Debug.Log("Save deleted.");
            }
        }

    }
}