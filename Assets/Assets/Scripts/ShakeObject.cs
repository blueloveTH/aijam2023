using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;

    private Vector3 originalPosition;
    private float currentShakeDuration = 0f;

    public int shakeID;

    private void Start()
    {
        return;
        originalPosition = transform.position;
        if (shakeID == 1)
        {
            StartShake();
        }

    }

    private void Update()
    {
        return;
        if (Enemy_01.canHit == false)
        {
            if (currentShakeDuration > 0)
            {
                transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
                currentShakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                currentShakeDuration = 0f;
                transform.position = originalPosition;
            }
        }

        if (shakeID == 1)
        {
            if (currentShakeDuration > 0)
            {
                transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
                currentShakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                currentShakeDuration = 0f;
                transform.position = originalPosition;
            }
        }
    }



    public void StartShake()
    {
        currentShakeDuration = shakeDuration;
    }
}
