﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int CurrentHealth;
    public int MaxHealth;
    public int RegenRate;
    public float InvincibilityDuration;

    public bool RandomStartingHealth;
    public int MinStartHealth;
    public int MaxStartHealth;

    public ParticleSystem bloodSplatter;

    public GameObject healthManagerPrefab;
    public Canvas healthCanvas;

    [HideInInspector]
    public HealthUI healthUI;
    [HideInInspector]
    public UITrackObject uiTracker;

    private float invcTimer = 0f;

    // Hurts the entity
    public void Harm(int hp)
    {
        if (invcTimer <= 0f)
        {
            CurrentHealth -= hp;

            healthUI.SetHealth(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0)
            {
                // Death
                CurrentHealth = 0;
                Kill();
            }

            invcTimer = InvincibilityDuration;

            SpawnBlood();
        }
    }

    private void SpawnBlood()
    {
        Instantiate(bloodSplatter, transform.position, Quaternion.identity);
    }

    // Heals the entity
    public void Heal(int hp)
    {

    }

    // Heals the entity over a few ticks
    public void HealOverTime(int hp)
    {

    }

    // Kills the entity
    public void Kill()
    {

    }

    public void Start()
    {
        healthCanvas = GameObject.FindWithTag("HealthCanvas").GetComponent<Canvas>();
        GameObject hm = Instantiate(healthManagerPrefab, healthCanvas.transform);
        healthUI = hm.GetComponent<HealthUI>();
        uiTracker = hm.GetComponent<UITrackObject>();

        Debug.Assert(uiTracker != null, "Failed to find UI Tracker component!");
        Debug.Assert(healthUI != null, "Health Manager Prefab needs a Health UI component");

        if (RandomStartingHealth)
        {
            MaxHealth = Random.Range(MinStartHealth, MaxStartHealth);
            CurrentHealth = MaxHealth;
        }

        // Set our tracking target and current health
        uiTracker.TrackTarget = gameObject;
        healthUI.SetHealth(CurrentHealth, MaxHealth);
    }

    public void Update()
    {
        if (invcTimer > 0f)
        {
            invcTimer -= Time.deltaTime;
        }
    }
}