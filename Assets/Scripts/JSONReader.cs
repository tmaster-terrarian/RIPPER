using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset textJson;

    [System.Serializable]
    public class PlayerStats
    {
        public int health;
        public int armor;
    }

    [System.Serializable]
    public class PlayerStatsList
    {
        public PlayerStats[] playerStats;
    }

    public PlayerStatsList playerStatsList = new PlayerStatsList();

    void Start()
    {
        Load(textJson.text);
    }

    public void Load(string json)
    {
        playerStatsList = JsonUtility.FromJson<PlayerStatsList>(json);
    }
}
