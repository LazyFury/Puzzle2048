using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class FPSCounter : MonoBehaviour
{

    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] Color Error;
    [SerializeField] Color Warning;
    [SerializeField] Color Success;
    int frameCount = 0;
    [SerializeField] float updateInterval = 0.5f;
    [SerializeField] bool AllowPoint = false;
    float time;

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        time += Time.deltaTime;

        if (time >= updateInterval)
        {
            float fps = Mathf.Ceil(frameCount / time * 100) / 100;
            text.text = AllowPoint ? fps.ToString("0.0") : fps.ToString("0") + " FPS";

            if (fps <= 10)
            {
                text.color = Error;
            }
            else
            if (fps <= 25)
            {
                text.color = Warning;
            }
            else
            {
                text.color = Success;
            }

            frameCount = 0;
            time -= updateInterval;
        }
    }
}
