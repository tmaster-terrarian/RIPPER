using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public int health;
        public int armor;
        public int maxHealth;
        public int maxArmor;
    }

    public PlayerData playerData = new PlayerData();

    public void Write(string fileName)
    {
        string content = JsonUtility.ToJson(playerData, true);

        File.WriteAllText(Application.persistentDataPath + "/Saves/" + fileName, content);
    }

    public void Read(string fileName)
    {
        string content = File.ReadAllText(Application.persistentDataPath + "/Saves/" + fileName);

        playerData = JsonUtility.FromJson<PlayerData>(content);
    }
}
