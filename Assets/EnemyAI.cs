using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
    Idle,
    Path,
    Attacking
}

public class EnemyAI : MonoBehaviour, IDamagable
{
    AIState state = AIState.Idle;
    UnityEngine.AI.NavMeshAgent navMeshAgent;
    float pathFindTimer = .5f;
    [SerializeField] GameObject head;
    [SerializeField] GameObject target;
    [SerializeField]float health = 30;

    [SerializeField] float pathFindUpdateTime = .5f;
    [SerializeField]GameObject pathParent;
    [SerializeField] LayerMask viewMask;
    [SerializeField] float viewAngle = 25;
    [SerializeField] float losePlayerTime = 3.0f;
    [SerializeField] FootstepSounds sounds;
    float sawLastTimer = 0;

    Transform[] path;
    int currentPathPoint = 0;
    public void Damage(float damage, Vector3 position, Vector3 force)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (pathParent != null)
        {
            path = pathParent.GetComponentsInChildren<Transform>();
            SetPathState();
        }        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLooking();

        sounds.UpdateFootstep(navMeshAgent.velocity.magnitude / 10);
        switch (state){
            case AIState.Idle:                
                break;
            case AIState.Attacking:
                UpdateAttacking();
                break;
            case AIState.Path:
                UpdatePath();
                break;
        }
    }

    private void UpdateLooking()
    {

        sawLastTimer += Time.deltaTime;
        Vector3 direction = target.transform.position - head.transform.position;
        float angularDifference = Vector3.Angle(direction.normalized, head.transform.forward);
        if (sawLastTimer >= losePlayerTime && state == AIState.Attacking)
        {
            SetPathState();
        }

        if (angularDifference > viewAngle) return;
        
        if (Physics.Raycast(new Ray(head.transform.position, direction.normalized), out RaycastHit h, 20, viewMask))
        {
            Debug.DrawRay(head.transform.position, direction.normalized * h.distance);
            FirstPersonController fp = h.transform.GetComponent<FirstPersonController>();
            if (fp != null)
            {
                sawLastTimer = 0;
                state = AIState.Attacking;
            }
        }
    }

    private void UpdateAttacking()
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

    private void UpdatePath()
    {
        if (navMeshAgent.remainingDistance < 1)
        {
            NextPathPoint();
            navMeshAgent.SetDestination(path[currentPathPoint].position);
        }
        
    }

    private void NextPathPoint()
    {
        currentPathPoint++;
        if (currentPathPoint == path.Length)
        {
            currentPathPoint = 0;
        }
    }

    private void SetPathState()
    {
        state = AIState.Path;
        float d = 500000;
        int closest = 0;
        for (int i = 0; i < path.Length; i++)
        {
            float dist = (transform.position - path[i].position).magnitude;
            if (dist < d)
            {
                closest = i;
                d = dist;
            }
        }
        currentPathPoint = closest;
        navMeshAgent.SetDestination(path[currentPathPoint].position);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
