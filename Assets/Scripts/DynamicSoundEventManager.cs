using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DynamicSoundEventManager : MonoBehaviour
{
  [EventRef]
  [SerializeField]
  private List<string> events;

  private Dictionary<string, StudioEventEmitter> eventEmitters = new Dictionary<string, StudioEventEmitter>();
  private bool init = false;

  private void Awake() {
    InitEmitters();
  }

  private void InitEmitters() {
    if (init) return;
    init = true;
    events.ForEach(soundEvent => {
      StudioEventEmitter eventEmitter = gameObject.AddComponent<StudioEventEmitter>();
      eventEmitter.Event = soundEvent;
      eventEmitters.Add(soundEvent, eventEmitter);
    });
  }

  public bool IsPlaying(string key) {
    InitEmitters();
    return eventEmitters[key].IsPlaying();
  }

  public void PlayEvent(string key) {
    InitEmitters();
    eventEmitters[key].Play();
  }
  
  public void StopEvent(string key) {
    InitEmitters();
    eventEmitters[key].Stop();
  }
}
