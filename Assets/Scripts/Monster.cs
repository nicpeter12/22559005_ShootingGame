using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float speed = 1.0f;
    Vector3 direct = Vector3.down;

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
        GameObject explosionObj = Instantiate(prefabsExplosion);
        explosionObj.transform.position = transform.position;
        Destroy(collision.gameObject);

        Destroy(gameObject);
    }
}
