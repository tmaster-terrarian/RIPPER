using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    [System.Serializable]
    public class SettingsData
    {
        public float mouseSensitivity = 3.5f;
        public bool invertY = false;
        public bool invertX = false;
        public float fov = 100;
        public bool screenShake = true;
        public float masterVolume = 100;
        public float musicVolume = 100;
        public float soundVolume = 80;
        public bool hudEnabled = true;
        public bool bloomEnabled = true;
        public float contrast = 10;
        public int fpsTarget = -1;
        public bool showFps = false;
    }

    public SettingsData settingsData = new SettingsData();

    public void Write(string fileName)
    {
        string content = JsonUtility.ToJson(settingsData, true);

        File.WriteAllText(Application.persistentDataPath + "/" + fileName, content);
    }

    public void Read(string fileName)
    {
        string content = File.ReadAllText(Application.persistentDataPath + "/" + fileName);

        settingsData = JsonUtility.FromJson<SettingsData>(content);
    }
}
