using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Sight Settings")]
    public float sightRadius = 10f; //�þ߹���
    [Range(0, 360)]
    public float sightAngle = 90f; //�þ߰�
    //����� ���� ���ʴ� 88���̴�.
    //���� 2���̹Ƿ� 176����� �� �� �ִ�.
    //�׳� 180�� �ص� ����
    //���⼭�� �ľ��� �����ϱ� ���� 90���� ��

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private List<Transform> visibleTargets = new List<Transform>(); //�������� ��
    public Transform nearestTarget; //���� ����� ��

    private float distanceToTarget = 0f; //���� ����� ������ �Ÿ�

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
            FindVisibleTargets(); //�������� �� Ž��
        }
    }
    void FindVisibleTargets()
    {
        distanceToTarget = 0f;
        nearestTarget = null;
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, sightRadius, targetMask); //��ü������� �÷��̾ Ž��

        //Ž���� ���� �˻�
        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            //�������� ����
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //Vector3.Angle : �ι��� ������ ���� ����
            //������ 2�� �� ������ Angle�� 180������ �ۿ� �ȳ���
            if(Vector3.Angle(transform.forward, dirToTarget) < sightAngle / 2)
            {
                //�Ÿ��� ����
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                //�÷��̾�� �� ���̿� ��ֹ��� ���ٸ�
                if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target); //Ÿ���� ����
                    //�Ÿ�üũ
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

        Vector3 viewAngleA = DirFromAngle(-sightAngle / 2, false); //���� ������
        Vector3 viewAngleB = DirFromAngle(sightAngle / 2, false); //������ ������

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
