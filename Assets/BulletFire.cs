using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFire : MonoBehaviour
{
    public GameObject BulletObject;
    public GameObject BulletFireObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isFire = Input.GetButtonDown("Jump");
        if (isFire)
        {
            GameObject bullet = Instantiate(BulletObject);
            bullet.transform.position = BulletFireObject.transform.position;
        }
    }
}
