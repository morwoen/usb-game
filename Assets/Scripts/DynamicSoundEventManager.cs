using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DynamicSoundEventManager : MonoBehaviour
{
  [SerializeField]
  List<string> events;

  Dictionary<string, StudioEventEmitter> eventEmitters = new Dictionary<string, StudioEventEmitter>();

  private void Awake() {
    events.ForEach(soundEvent => {
      StudioEventEmitter eventEmitter = gameObject.AddComponent<StudioEventEmitter>();
      eventEmitter.Event = soundEvent;
      eventEmitters.Add(soundEvent, eventEmitter);
    });
  }

  public void PlayEvent(string key) {
    eventEmitters[key].Play();
  }
  
  public void StopEvent(string key) {
    eventEmitters[key].Stop();
  }
}
