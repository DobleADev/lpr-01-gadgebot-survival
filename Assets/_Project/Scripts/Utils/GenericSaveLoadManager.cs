using UnityEngine;
using System.Runtime.InteropServices;

public class GenericSaveLoadManager
{
    // Define the imported JavaScript functions for WebGL only
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SaveToLocalStorage(string key, string value);

    [DllImport("__Internal")]
    private static extern string LoadFromLocalStorage(string key);
#endif

    public void SaveGameData(string key, GadgebotSurvivalSaveData data)
    {
        string json = JsonUtility.ToJson(data);

#if UNITY_WEBGL
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

    public GadgebotSurvivalSaveData LoadGameData(string key)
    {
        string json = null;

#if UNITY_WEBGL
        // WebGL: Load from browser's localStorage
        json = LoadFromLocalStorage(key);
#else
        // Other platforms: Load from PlayerPrefs
        // json = PlayerPrefs.GetString(key);
#endif

        if (!string.IsNullOrEmpty(json))
        {
            return JsonUtility.FromJson<GadgebotSurvivalSaveData>(json);
        }

        // Debug.Log("No game data found.");
        return null; // Return null if no data exists
    }
}