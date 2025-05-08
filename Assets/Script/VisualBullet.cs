using UnityEngine;

public class VisualBullet : MonoBehaviour
{
  public float speed = 30f;
  public float lifetime = 4f;
  private Vector3 dir;
  private float timer;

  public void Init(Vector3 direction)
  {
    dir = direction.normalized;
    timer = 0f;
  }

  void Update()
  {
    transform.position += dir * speed * Time.deltaTime;
    timer += Time.deltaTime;
    if (timer >= lifetime)
      Destroy(gameObject);
  }
}
