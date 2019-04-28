using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

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
    public float AIUpdateFreq;
    public float InternalGravity;
    public float InternalDrag;
    public float DamagedBounceHeight;

    [Header("Wander Settings")]
    public float WanderDistance;
    public float MoveSpeed;
    public float WanderMinTimer;
    public float WanderMaxTimer;
    public float MaxWanderDistance;
    public float ArrivedDistance;

    [Header("Chase Settings")]
    public float AlertDistance;
    public float GiveUpChaseDistance;
    public float ChaseSpeed;
    public float AttackDistance;
    public float AttackDistanceTolerance;

    [Header("Flee Settings")]
    public float SafeDistance;
    public float FleeSpeed;
    public float FleeHealth;

    private float wanderTimer = 0f;
    private Vector3 wanderTarget = Vector3.zero;

    private float aiUpdateDuration;
    private float aiUpdateTimer = 0f;

    private Vector2 velocity = Vector2.zero;
    private Vector2 internalVelocity = Vector2.zero;
    private bool internalRest = true;

    private Animator anim;
    private Rigidbody2D rb;
    private Health health;
    private Weapon activeWeapon;
    private GameObject childSprite;

    private PlayerController pController;

    public void Start()
    {
        childSprite = transform.GetChild(0)?.gameObject;
        anim = childSprite.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();

        activeWeapon = childSprite.GetComponentInChildren<Weapon>();
        pController = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();

        aiUpdateDuration = 1f / AIUpdateFreq;

        // Subscribe to our health messages
        UnityAction healthAction = HealthAction;
        health.Subscribe(GetInstanceID(), healthAction);
    }

    public void HealthAction()
    {
        Vector2 fromPlayer = (transform.position - pController.transform.position).normalized;
        internalRest = false;
        internalVelocity += Vector2.up * Mathf.Sqrt(2f * InternalGravity * DamagedBounceHeight);
        velocity += fromPlayer * 3f;
    }

    public void Update()
    {
        UpdateTimers();
        AnimationUpdate();

        if (aiUpdateTimer >= aiUpdateDuration)
        {
            AIUpdate();
            // Reset timer
            aiUpdateTimer = 0f;
        }

        WeaponFlip();
    }

    public void FixedUpdate()
    {
        switch (currentState)
        {
            case OrcState.Idle:
                WanderFixedUpdate();
                break;
            case OrcState.Chase:
                ChaseFixedUpdate();
                break;
            case OrcState.Flee:
                FleeFixedUpdate();
                break;
        }

        // Add Drag
        velocity *= Drag;

        // Apply velocity
        rb.velocity = velocity;

        // Update internal child velocity
        if (!internalRest)
        {
            internalVelocity *= InternalDrag;
            internalVelocity += InternalGravity * Vector2.down * Time.deltaTime;

            if (childSprite.transform.localPosition.y <= 0f && Vector3.Dot(internalVelocity, Vector2.down) > 0.707f)
            {
                internalVelocity = Vector2.zero;
                childSprite.transform.localPosition = Vector3.zero;
                internalRest = true;
            }

            // Move our child
            childSprite.transform.localPosition += new Vector3(internalVelocity.x, internalVelocity.y, 0f) * Time.deltaTime;
        }
    }

    private void WeaponFlip()
    {
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            // Left or Right
            if (velocity.x < 0f)
            {
                // Left
                activeWeapon.SetOrientation(WeaponOrientation.Left);
            }
            else
            {
                // Right
                activeWeapon.SetOrientation(WeaponOrientation.Right);
            }
        }
        else
        {
            // Up or Down
            if (velocity.y < 0f)
            {
                //Down
                activeWeapon.SetOrientation(WeaponOrientation.Left);
            }
            else
            {
                // Up
                activeWeapon.SetOrientation(WeaponOrientation.Right);
            }
        }
    }

    private void AnimationUpdate()
    {
        anim.SetFloat("MoveX", velocity.x);
        anim.SetFloat("MoveY", velocity.y);
        anim.SetBool("Idle", velocity.sqrMagnitude > 0.1f);
    }

    private void FleeFixedUpdate()
    {
        Vector2 awayFromPlayer = (transform.position - pController.transform.position).normalized;
        velocity += awayFromPlayer * FleeSpeed * Time.deltaTime;
    }

    private void ChaseFixedUpdate()
    {
        float sqrDistToPlayer = (pController.transform.position - transform.position).sqrMagnitude;
        float sqrMaxAttackDistTol = AttackDistance + AttackDistanceTolerance;
        float sqrMinAttackDistTol = AttackDistance - AttackDistanceTolerance;
        sqrMaxAttackDistTol *= sqrMaxAttackDistTol;
        sqrMinAttackDistTol *= sqrMinAttackDistTol;

        Vector2 moveDir = Vector2.zero;

        if (sqrDistToPlayer < sqrMaxAttackDistTol && sqrDistToPlayer > sqrMinAttackDistTol)
        {
            // Prereqs met for attacking
            activeWeapon?.Use();
        }

        if (sqrDistToPlayer < sqrMinAttackDistTol)
        {
            // Move away from the player
            Vector2 awayFromPlayer = (transform.position - pController.transform.position).normalized;
            moveDir = awayFromPlayer;
        }

        if (sqrDistToPlayer > sqrMaxAttackDistTol)
        {
            // Give chase to the player
            Vector2 toPlayer = (pController.transform.position - transform.position).normalized;
            moveDir = toPlayer;
        }

        // Apply our target movement
        velocity += moveDir * ChaseSpeed * Time.deltaTime;
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

    private void AIUpdate()
    {
        // Useful data for state checks
        float sqrDistToPlayer = (pController.transform.position - transform.position).sqrMagnitude;

        // Check all state changes
        switch (currentState)
        {
            case OrcState.Idle:
                {
                    // Player is within alert range
                    if (sqrDistToPlayer < AlertDistance * AlertDistance)
                    {
                        currentState = OrcState.Chase;
                    }
                    break;
                }
            case OrcState.Chase:
                {
                    // Player is out of range
                    if (sqrDistToPlayer > GiveUpChaseDistance * GiveUpChaseDistance)
                    {
                        currentState = OrcState.Idle;
                    }

                    // Health is too low, flee
                    if (health.CurrentHealth < FleeHealth)
                    {
                        currentState = OrcState.Flee;
                    }
                    break;
                }
            case OrcState.Flee:
                {
                    // AI has fled far enough
                    if (sqrDistToPlayer > SafeDistance * SafeDistance)
                    {
                        currentState = OrcState.Idle;
                    }
                    break;
                }
        }

        // Any global state checks
    }

    private void UpdateTimers()
    {
        aiUpdateTimer += Time.deltaTime;

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
}
