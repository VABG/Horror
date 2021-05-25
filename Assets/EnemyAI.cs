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

    // Basic navmesh stuff
    [SerializeField] Transform attackCenter;
    UnityEngine.AI.NavMeshAgent navMeshAgent;
    float pathFindTimer = .5f;
    [SerializeField] float pathFindUpdateTime = .5f;
    [SerializeField] GameObject target;

    // Path
    [SerializeField] GameObject pathParent;
    Transform[] path;
    int currentPathPoint = 0;

    // AI Looking
    [SerializeField] GameObject head;
    [SerializeField] LayerMask viewMask;
    [SerializeField] float viewAngle = 25;
    [SerializeField] float losePlayerTime = 3.0f;
    float sawLastTimer = 3;

    // Other
    AIState state = AIState.Idle;
    [SerializeField] float health = 30;
    [SerializeField] FootstepSounds sounds;
    [SerializeField] AudioSource mouthAudio;
    //Attacking
    float attackTimer = 0;
    [SerializeField] float attackDelay = 1;

    public void Damage(float damage, Vector3 position, Vector3 force)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Pathfinding stuff
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

        sounds.UpdateFootstep(navMeshAgent.velocity.magnitude);

        switch (state)
        {
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
        // Timer
        sawLastTimer += Time.deltaTime;
        if (sawLastTimer >= losePlayerTime && state == AIState.Attacking)
        {
            SetPathState();
        }

        // Direction to target
        Vector3 direction = target.transform.position - head.transform.position;

        // Angle to target
        float angularDifference = Vector3.Angle(direction.normalized, head.transform.forward);        
        if (angularDifference > viewAngle) return;
        
        //Raycast
        if (Physics.Raycast(new Ray(head.transform.position, direction.normalized), out RaycastHit h, 20, viewMask))
        {
            Debug.DrawLine(head.transform.position, h.point);

            FirstPersonController fp = h.transform.GetComponent<FirstPersonController>();
            if (fp != null)
            {
                if (sawLastTimer >= losePlayerTime) mouthAudio.Play();
                sawLastTimer = 0;
                state = AIState.Attacking;
                navMeshAgent.stoppingDistance = 1.0f;
            }
        }        
    }

    private void UpdateAttacking()
    {        
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0 && (target.transform.position - transform.position).magnitude < 1.5)
        {
            //Attack!
            RaycastHit[] rhits = Physics.SphereCastAll(attackCenter.position, .5f, Vector3.up, viewMask);
            for (int i = 0; i < rhits.Length; i++)
            {
                IDamagable dmg = rhits[i].transform.GetComponent<IDamagable>();
                if (dmg != null && dmg != this)
                {
                    dmg.Damage(20.0f, Vector3.zero, transform.forward * 50);
                }
            }
            attackTimer = attackDelay;
        }

        pathFindTimer -= Time.deltaTime;
        if (pathFindTimer <= 0)
        {
            pathFindTimer = pathFindUpdateTime;
            if (target != null)
            {
                navMeshAgent.SetDestination(target.transform.position);
            }
            else
            {
                state = AIState.Path;
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
        navMeshAgent.stoppingDistance = .5f;
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
