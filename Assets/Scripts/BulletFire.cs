using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletFire : MonoBehaviour

{

    public GameObject BulletObject;

    public GameObject BulletFireObject;

    //public GameObject GuidedBulletObject;

    //public Transform target;

    //public float speed = 5;

    //public float rotationSpeed = 5f;

    // Start is called before the first frame update

    void Start()

    {



    }



    // Update is called once per frame

    void Update()
    {

        bool isFire = Input.GetButtonDown("Jump");
        bool isTargetedFire = Input.GetKeyDown(KeyCode.X);
        if (isFire)
        {
            BulletSpawn(BulletType.NORMAL);


        }
        else if (isTargetedFire)
        {
            BulletSpawn(BulletType.TAGETED);
        }

    }

    public void BulletSpawn(BulletType bulletYype)
    {
        GameObject bullet = Instantiate(BulletObject);

        bullet.transform.position = BulletFireObject.transform.position;
        bullet.GetComponent<Bullet>().type = bulletYype;
    }
}

