using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour
{
    public float speed = 1.0f;
    Vector3 direct = Vector3.down;
    public GameObject player;

    public GameObject prefabsExplosion;
    // Start is called before the first frame update
    void Start()
    {
        int rndNim =  Random.Range(0, 10);
        if(rndNim < 3)
        {
            GameObject target = GameObject.Find("Player");
            direct = target.transform.position - transform.position;
            direct.Normalize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + direct * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            GameObject gameManager = GameObject.Find("GameManager");
            ScoreManager scoreManager = gameManager.GetComponent<ScoreManager>();
            AudioSource audioManager = gameManager.GetComponent<AudioSource>();

            scoreManager.nowScore++;
            scoreManager.nowScoreUI.text ="Score : " + scoreManager.nowScore;

            if(scoreManager.nowScore > scoreManager.bestScore)
            {
                scoreManager.bestScore = scoreManager.nowScore;
                scoreManager.bestScoreUI.text = "Best Score : " + scoreManager.bestScore;
                PlayerPrefs.SetInt("bestScore", scoreManager.bestScore);
            }

            if (prefabsExplosion != null)
            {
                GameObject explosionObj = Instantiate(prefabsExplosion);
                explosionObj.transform.position = transform.position;
            }
            else
            {
                IndustrialDroneExplosionVFX.Spawn(transform.position);
            }
            
            Destroy(collision.gameObject);
            Destroy(gameObject);

        }
    }
}
