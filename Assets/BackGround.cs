using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public Material materialBG;

    public float scrollSpd = 0.2f;

    private void Update()
    {
        Vector2 direction = Vector2.up;
        materialBG.mainTextureOffset += direction * scrollSpd * Time.deltaTime;
    }
}
