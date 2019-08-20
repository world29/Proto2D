using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGauge : MonoBehaviour
{
    public GameObject jumpIcon;

    public int initialJumpCount = 2;
    public int maxJumpCount = 3;
    public float timeToAutoIncrement = 3;

    private int jumpCount;
    private float autoIncrementTimer;

    private void Start()
    {
        jumpCount = initialJumpCount;
    }

    void Update()
    {
        autoIncrementTimer += Time.deltaTime;
        if (autoIncrementTimer > timeToAutoIncrement)
        {
            IncrementJumpCount();

            autoIncrementTimer = 0;
        }
    }

    public int GetJumpCount()
    {
        return jumpCount;
    }

    public void IncrementJumpCount()
    {
        int prev = jumpCount;

        jumpCount = Mathf.Min(jumpCount + 1, maxJumpCount);

        if (prev != jumpCount)
        {
            UpdateUI();
        }
    }

    public void DecrementJumpCount()
    {
        int prev = jumpCount;

        jumpCount = Mathf.Max(jumpCount - 1, 0);

        if (prev != jumpCount)
        {
            UpdateUI();
        }
    }

    public void SetJumpCount(int count)
    {
        count = Mathf.Clamp(count, 0, maxJumpCount);
        
        if (count != jumpCount)
        {
            jumpCount = count;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        for (int i = 0;i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i< jumpCount; i ++)
        {
            Instantiate(jumpIcon, transform);
        }
    }
}
