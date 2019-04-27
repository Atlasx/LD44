using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDuration : MonoBehaviour
{
    public float Duration;

    private float elapsed = 0f;

    public void Start()
    {
        elapsed = 0f;
    }

    public void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > Duration)
        {
            Destroy(gameObject);
        }
    }
}
