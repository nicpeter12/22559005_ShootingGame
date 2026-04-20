using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject Player; 

 void Update()
 {
    float h = Input.GetAxisRaw("Horizontal");
    //float v = Input.GetAxisRaw("Vertical");
    Vector3 direction = new Vector3(h, 0, 0).normalized;
    transform.Translate(direction * moveSpeed * Time.deltaTime);
 }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            Player.SetActive(false);
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
