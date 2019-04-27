using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject TrackTarget;

    [Range(0f, 10f)]
    public float TrackingSharpness = 0.5f;
    public float ZDepth = -10f;


    public void LateUpdate()
    {
        Vector3 newPos = Vector3.Lerp(transform.position, TrackTarget.transform.position, TrackingSharpness * Time.deltaTime);
        newPos.z = ZDepth;

        transform.position = newPos;
    }
}
