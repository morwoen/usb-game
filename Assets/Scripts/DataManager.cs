using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
  private static DataManager _instance;
  private static DataManager Instance
  {
    get
    {
      if (_instance) return _instance;

      var oldInstance = FindObjectOfType<DataManager>();
      if (oldInstance) {
        _instance = oldInstance;
        return _instance;
      }

      var go = new GameObject();
      go.name = "[DataManager]";
      _instance = go.AddComponent<DataManager>();

      return _instance;
    }
  }

  private Progress progress;

  private bool loaded = false;

  private void Awake() {
    DontDestroyOnLoad(this);
  }

  private void OnEnable() {
    progress = FindObjectOfType<ProgressHolder>().progress;
  }

  private void LoadData() {
    if (loaded) return;
    progress.Load();
    loaded = true;
  }

  private void ClearData() {
    progress.Clear();
  }

  private void OnApplicationQuit() {
    progress.Save();

    progress.Clear();
  }

  public static void Load() {
    Instance.LoadData();
  }

  public static void Clear() {
    Load();
    Instance.ClearData();
  }

  public static bool IsCompleted(Mission mission) {
    Load();
    return Instance.progress.IsCompleted(mission);
  }

  public static void CompleteMission(Mission mission) {
    Load();
    Instance.progress.CompleteMission(mission);
  }

  public static float GetSoundVolume() {
    Load();
    return Instance.progress.soundVolume;
  }

  public static float GetMusicVolume() {
    Load();
    return Instance.progress.musicVolume;
  }

  public static void SetSoundVolume(float value) {
    Load();
    Instance.progress.soundVolume = value;
  }

  public static void SetMusicVolume(float value) {
    Load();
    Instance.progress.musicVolume = value;
  }
}
