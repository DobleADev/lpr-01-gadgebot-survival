using UnityEngine;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GenericSaveLoadManager
{
    // Define the imported JavaScript functions for WebGL only
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SaveToLocalStorage(string key, string value);

    [DllImport("__Internal")]
    private static extern string LoadFromLocalStorage(string key);
#endif

    public void SaveGameData(
        string key,
        string savePath,
    GadgebotSurvivalSaveData data
    )
    {
        string json = JsonUtility.ToJson(data);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // Windows/Editor: Use binary serialization
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
#elif UNITY_WEBGL
        // WebGL: Use browser's localStorage via JavaScript plugin
        SaveToLocalStorage(key, json);
        // Debug.Log("Game data saved to WebGL localStorage.");
#else
        // Other platforms: Use PlayerPrefs
        // PlayerPrefs.SetString(key, json);
        // PlayerPrefs.Save();
        // Debug.Log("Game data saved using PlayerPrefs.");
#endif
    }

    public GadgebotSurvivalSaveData LoadGameData(
        string key,
        string savePath)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // Windows/Editor: Load from binary file
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Open);
            GadgebotSurvivalSaveData data = formatter.Deserialize(stream) as GadgebotSurvivalSaveData;
            stream.Close();
            // Debug.Log("Game data loaded from binary file on Windows.");
            return data;
        }
        else
        {
            // Debug.Log("No game data file found.");
            return null;
        }
#elif UNITY_WEBGL
        // WebGL: Load from browser's localStorage
        string json = LoadFromLocalStorage(key);
        if (!string.IsNullOrEmpty(json))
        {
            // Debug.Log("Game data loaded from WebGL localStorage.");
            return JsonUtility.FromJson<GadgebotSurvivalSaveData>(json);
        }
        else
        {
            // Debug.Log("No game data found in localStorage.");
            return null;
        }
#else
        // Other platforms: Load from PlayerPrefs
        // json = PlayerPrefs.GetString(key);
#endif
    }
}