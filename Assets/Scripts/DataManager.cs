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
    progress = FindObjectOfType<PlayerController>().progress;
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
}
