using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class OrcController : MonoBehaviour
{
    public enum OrcState
    {
        Idle,
        Chase,
        Flee
    }

    public OrcState currentState = OrcState.Idle;

    [Header("General Settings")]
    public float Drag;

    [Header("Wander Settings")]
    public float WanderDistance;
    public float MoveSpeed;
    public float WanderMinTimer;
    public float WanderMaxTimer;
    public float MaxWanderDistance;
    public float ArrivedDistance;

    [Header("Chase Settings")]
    public float GiveUpDistance;
    public float ChaseSpeed;

    [Header("Flee Settings")]
    public float SafeDistance;
    public float FleeSpeed;

    private float wanderTimer = 0f;
    private Vector3 wanderTarget = Vector3.zero;

    private Vector2 velocity = Vector2.zero;

    private Animator anim;
    private Rigidbody2D rb;

    public void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        AnimationUpdate();

        // State-specific updates
        switch (currentState)
        {
            case OrcState.Idle:
                wanderTimer -= Time.deltaTime;
                break;
            case OrcState.Chase:

                break;
            case OrcState.Flee:

                break;
        }
    }

    public void FixedUpdate()
    {
        switch (currentState)
        {
            case OrcState.Idle:
                WanderFixedUpdate();
                break;
            case OrcState.Chase:

                break;
            case OrcState.Flee:

                break;
        }

        // Add Drag
        velocity *= Drag;

        // Apply velocity
        rb.velocity = velocity;
    }

    private void AnimationUpdate()
    {
        anim.SetFloat("MoveX", velocity.x);
        anim.SetFloat("MoveY", velocity.y);
        anim.SetBool("Idle", velocity.sqrMagnitude > 0.1f);
    }

    private void WanderFixedUpdate()
    {
        if (wanderTimer <= 0f)
        {
            // Wait time is up, pick a new spot to wander to
            Vector2 wanderDir = new Vector2(Random.value * 2f - 1f, Random.value * 2f - 1f).normalized;
            float wanderDist = Random.Range(1f, MaxWanderDistance);
            wanderDir *= wanderDist;
            wanderTarget = transform.position + new Vector3(wanderDir.x, wanderDir.y, 0f);
            wanderTimer = Random.Range(WanderMinTimer, WanderMaxTimer);
        }

        Vector3 toTarget = wanderTarget - transform.position;
        if (toTarget.sqrMagnitude < ArrivedDistance)
        {
            // Already arrived, stay put
            velocity *= 0.5f;
        }
        else
        {
            // Move towards our target
            velocity += new Vector2(toTarget.x, toTarget.y).normalized * MoveSpeed * Time.deltaTime;
        }

    }
}
