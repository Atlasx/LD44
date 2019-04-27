using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrackObject : MonoBehaviour
{
    public GameObject TrackTarget;
    public Vector3 Offset;

    public void LateUpdate()
    {
        transform.position = TrackTarget.transform.position + Offset;
    }
}
