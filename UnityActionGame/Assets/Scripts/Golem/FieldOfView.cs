using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Sight Settings")]
    public float sightRadius = 10f; //시야범위
    [Range(0, 360)]
    public float sightAngle = 90f; //시야각
    //사람의 눈은 한쪽당 88도이다.
    //눈이 2개이므로 176도라고 볼 수 있다.
    //그냥 180도 해도 무관
    //여기서는 파악을 쉽게하기 위해 90도를 씀

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private List<Transform> visibleTargets = new List<Transform>(); //보여지는 적
    public Transform nearestTarget; //가장 가까운 적

    private float distanceToTarget = 0f; //가장 가까운 적과의 거리

    public List<Transform> VisibleTargets => visibleTargets;
    public Transform NearestTarget => nearestTarget;
    public float DistanceToTarget => distanceToTarget;

    void Start()
    {
        targetMask = LayerMask.GetMask(Define.Layers.player);
        obstacleMask = LayerMask.GetMask(Define.Layers.opstacle);

        StartCoroutine(FindTargetsWithDelay());
    }
    IEnumerator FindTargetsWithDelay()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            FindVisibleTargets(); //보여지는 적 탐지
        }
    }
    void FindVisibleTargets()
    {
        distanceToTarget = 0f;
        nearestTarget = null;
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, sightRadius, targetMask); //구체모양으로 플레이어를 탐지

        //탐지된 적을 검사
        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            //적으로의 방향
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //Vector3.Angle : 두방향 사이의 각을 구함
            //나누기 2를 한 이유는 Angle이 180도까지 밖에 안나옴
            if(Vector3.Angle(transform.forward, dirToTarget) < sightAngle / 2)
            {
                //거리를 구함
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                //플레이어와 골렘 사이에 장애물이 없다면
                if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target); //타겟을 넣음
                    //거리체크
                    if(nearestTarget == null || (distanceToTarget > dstToTarget))
                    {
                        nearestTarget = target;
                    }
                    distanceToTarget = dstToTarget;
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3
            (Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        Vector3 viewAngleA = DirFromAngle(-sightAngle / 2, false); //왼쪽 꼭짓점
        Vector3 viewAngleB = DirFromAngle(sightAngle / 2, false); //오른쪽 꼭짓점

        float x = Mathf.Sin(sightAngle / 2 * Mathf.Deg2Rad) * sightRadius;
        float y = Mathf.Cos(sightAngle / 2 * Mathf.Deg2Rad) * sightRadius;

        Gizmos.DrawLine(transform.position, 
            transform.position + viewAngleA * sightRadius);
        Gizmos.DrawLine(transform.position,
            transform.position + viewAngleB * sightRadius);

        Gizmos.color = Color.red;
        foreach(Transform visibleTarget in visibleTargets)
        {
            Gizmos.DrawLine(transform.position, visibleTarget.position);
        }
    }
}
