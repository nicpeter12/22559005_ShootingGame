using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMove : MonoBehaviour
{
   public float moveSpeed = 5f;

 void Update()
 {
    float h = Input.GetAxisRaw("Horizontal");
    float v = Input.GetAxisRaw("Vertical");
    Vector3 direction = new Vector3(h, 0, v).normalized;
    transform.Translate(direction * moveSpeed * Time.deltaTime);
 }
}
