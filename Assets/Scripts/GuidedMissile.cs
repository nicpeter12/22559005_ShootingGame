using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    public Transform target;       // 유도할 대상
    public float speed = 10f;       // 미사일 속도
    public float rotationSpeed = 5f; // 회전 속도 (유도 성능)

    void FixedUpdate()
    {
        if (target == null)
        {
            // 타겟이 없으면 직진
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        // 1. 타겟 방향 계산
        Vector3 direction = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 2. 부드럽게 타겟 방향으로 회전
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);

        // 3. 이동
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == target)
        {
            // 충돌 시 처리
            Destroy(gameObject);
        }
    }
}
