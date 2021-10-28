using System;
using Shared;
using UnityEngine;


public class KillZone : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(Tags.Ball))
        {
            var ballScript = col.gameObject.GetComponent<Ball>();
            ballScript.Crashed();
        }

        if (col.gameObject.CompareTag(Tags.PowerUp))
        {
            Destroy(col.gameObject);
        }
    }
}