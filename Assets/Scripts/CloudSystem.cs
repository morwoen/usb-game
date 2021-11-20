using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSystem : MonoBehaviour
{
  [SerializeField]
  private Transform destroyPosition;
  [SerializeField]
  private float spawnInterval = 0.5f;
  [SerializeField]
  private Vector2 scaleRange;
  [SerializeField]
  private Vector2 speedRange;
  [SerializeField]
  private List<GameObject> prefabs;

  private Camera cam;

  private void Start() {
    cam = FindObjectOfType<Camera>();
    InvokeRepeating("SpawnCloud", 0, spawnInterval);
  }

  private void SpawnCloud() {
    int cloudIndex = Random.Range(0, prefabs.Count);
    Cloud cloud = Instantiate(prefabs[cloudIndex], transform).GetComponent<Cloud>();
    cloud.speed = Random.Range(speedRange.x, speedRange.y);
    cloud.destroyAt = destroyPosition.position.x;

    float scale = Random.Range(scaleRange.x, scaleRange.y);
    cloud.transform.localScale = new Vector3(scale, scale, 1);

    // calculated here since the camera is not init in the start
    float maxY = MapGenerator.levels * (MapGenerator.ceilingHeight + 1) + cam.orthographicSize;
    cloud.transform.position = new Vector3(transform.position.x, Random.Range(transform.position.y, maxY));
  }
}
