using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent navMeshAgent;
    float pathFindTimer = .5f;
    [SerializeField] GameObject target;
    float health = 100;

    [SerializeField] float pathFindUpdateTime = .5f;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        pathFindTimer -= Time.deltaTime;
        if (pathFindTimer <= 0)
        {
            pathFindTimer = pathFindUpdateTime;
            if (target != null)
            {
                navMeshAgent.SetDestination(target.transform.position);
            }
        }
    }
}
