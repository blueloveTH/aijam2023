using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTime : MonoBehaviour
{
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0.125f;
            }
            else
            {
                Time.timeScale = 1f; // 恢复正常时间流逝速度
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.Translate(1, 0, 0);
        }
    }
}
