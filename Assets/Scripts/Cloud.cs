using UnityEngine;

public class Cloud : MonoBehaviour
{
  public float speed = 1;
  public float destroyAt = 40f;

  private void Update() {
    transform.Translate(new Vector3(speed * Time.deltaTime, 0));

    if (destroyAt < transform.position.x) {
      Destroy(gameObject);
    }
  }
}
