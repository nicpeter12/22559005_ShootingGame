using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { NORMAL, TAGETED}
public class Bullet : MonoBehaviour
{

    [Header("유도탄 설정")]
    public float speed = 10f;          // 총알의 이동 속도
    public float driftSpeed = 3f;      // 좌우로 타겟을 쫓아가는 속도 (낮을수록 살짝만 이동함)
    public string targetTag = "Monster"; // 추적할 타겟의 태그

    public BulletType type;

    private Transform target;

    void Start()
    {
        // 총알이 생성될 때 가장 가까운 몬스터를 찾습니다.
        FindNearestTarget();
    }

    void Update()
    {
        // 1. 회전 없이 무조건 월드 기준 위쪽(Up)으로 전진합니다.
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);

        // 타겟이 없으면 그대로 직진만 하고 끝냅니다.
        if (target == null || type == BulletType.NORMAL) return;

        // 2. 타겟이 이미 총알보다 아래로 내려갔다면 추적을 포기하고 직진합니다. (어색함 방지)
        if (target.position.y < transform.position.y)
        {
            target = null;
            return;
        }

        // 3. 타겟의 X 좌표(좌우)를 향해 부드럽게 이동합니다.
        Vector3 currentPos = transform.position;
        float newX = Mathf.MoveTowards(currentPos.x, target.position.x, driftSpeed * Time.deltaTime);

        // Y(위아래)는 그대로 두고, X(좌우) 위치만 갱신합니다.
        transform.position = new Vector3(newX, currentPos.y, currentPos.z);
    }

    // 가장 가까운 타겟을 찾는 함수
    void FindNearestTarget()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag(targetTag);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestMonster = null;

        foreach (GameObject monster in monsters)
        {
            // 총알보다 아래에 있는 몬스터는 처음부터 타겟으로 잡지 않습니다.
            if (monster.transform.position.y < transform.position.y) continue;

            float distance = Vector3.Distance(transform.position, monster.transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestMonster = monster;
            }
        }

        if (nearestMonster != null)
        {
            target = nearestMonster.transform;
        }
    }
}

