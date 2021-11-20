using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Progress", menuName = "Missions/Progress")]
public class Progress : ScriptableObject, ISerializationCallbackReceiver
{
  private MissionDatabase database;

  public List<MissionData> missionDatas = new List<MissionData>();
  public float musicVolume = 1;
  public float soundVolume = 1;

  private void OnEnable() {
#if UNITY_EDITOR
    database = (MissionDatabase)AssetDatabase.LoadAssetAtPath("Assets/Resources/MissionDatabase.asset", typeof(MissionDatabase));
#else
    database = Resources.Load<MissionDatabase>("MissionDatabase");
#endif
  }

  public void CompleteMission(Mission mission) {
    MissionData missionData = GetMissionData(mission);
    if (missionData != null) {
      missionData.completed = true;
    } else {
      missionDatas.Add(new MissionData(database.objToId[mission], mission, true));
    }
  }

  public bool IsCompleted(Mission mission) {
    return GetMissionData(mission)?.completed == true;
  }

  private MissionData GetMissionData(Mission mission) {
    foreach (var missionData in missionDatas) {
      if (missionData.mission.Equals(mission)) {
        return missionData;
      }
    }
    return null;
  }

  public void OnAfterDeserialize() {
    foreach (var itemRef in missionDatas) {
      itemRef.mission = database.idToObj[itemRef.id];
    }
  }

  public void Save() {
    string data = JsonUtility.ToJson(this, true);
    var bf = new BinaryFormatter();
    var file = File.Create(string.Concat(Application.persistentDataPath, name));
    bf.Serialize(file, data);
    file.Close();
  }

  public void Load() {
    var path = string.Concat(Application.persistentDataPath, name);
    if (File.Exists(path)) {
      var bf = new BinaryFormatter();
      var file = File.Open(path, FileMode.Open);
      JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
      file.Close();
    }
  }

  internal void Clear() {
    missionDatas.Clear();
  }

  public void OnBeforeSerialize() {
  }

  [Serializable]
  public class MissionData
  {
    public int id;
    public Mission mission;
    public bool completed;

    public MissionData(int id, Mission item, bool completed) {
      this.id = id;
      this.mission = item;
      this.completed = completed;
    }
  }
}
