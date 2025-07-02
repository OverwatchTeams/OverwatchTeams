using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] private float targetAspect = 16f / 9f;
    private int lastScreenWidth;
    private int lastScreenHeight;
    [SerializeField] private float checkInterval = 0.5f;
    private float resizeTimer = 0f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            Application.runInBackground = true;
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ApplyFixedAspect(Screen.width);
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            resizeTimer = checkInterval;
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }

        if (resizeTimer > 0)
        {
            resizeTimer -= Time.unscaledDeltaTime;
            if (resizeTimer <= 0f)
            {
                ApplyFixedAspect(lastScreenWidth);
            }
        }
    }

    void ApplyFixedAspect(int width)
    {
        int height = Mathf.RoundToInt(width / targetAspect);
        Screen.SetResolution(width, height, false); // false = 창모드
    }
}
