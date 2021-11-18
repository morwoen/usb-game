using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission Database", menuName = "Missions/Database")]
public class MissionDatabase : ScriptableObject, ISerializationCallbackReceiver
{
  public Mission[] missions;

  public Dictionary<Mission, int> objToId = new Dictionary<Mission, int>();
  public Dictionary<int, Mission> idToObj = new Dictionary<int, Mission>();

  public void OnAfterDeserialize() {
    objToId = new Dictionary<Mission, int>();
    idToObj = new Dictionary<int, Mission>();
    for (int i = 0; i < missions.Length; i++) {
      objToId.Add(missions[i], i);
      idToObj.Add(i, missions[i]);
    }
  }

  public void OnBeforeSerialize() {

  }
}