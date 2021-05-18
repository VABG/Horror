using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamagable
{
    UnityEngine.AI.NavMeshAgent navMeshAgent;
    float pathFindTimer = .5f;
    [SerializeField] GameObject target;
    [SerializeField]float health = 30;

    [SerializeField] float pathFindUpdateTime = .5f;

    public void Damage(float damage, Vector3 position, Vector3 force)
    {
        health -= damage;
        if (health <= 0) Die();
    }

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

    void Die()
    {
        Destroy(gameObject);
    }
}
