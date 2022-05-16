using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
public class SaveAreaUI : MonoBehaviour
{

    VerticalLayoutGroup vertical;

    // Start is called before the first frame update
    void Start()
    {
        vertical = GetComponent<VerticalLayoutGroup>();
        var safeAreaRect = Screen.safeArea;
        float scaleRatio = (GetComponent<RectTransform>()?.rect.width ?? 480) / Screen.width;
        var left = safeAreaRect.xMin * scaleRatio;
        var right = (Screen.width - safeAreaRect.xMax) * scaleRatio;
        var top = safeAreaRect.yMin * scaleRatio;
        var bottom = (Screen.height - safeAreaRect.yMax) * scaleRatio;
        vertical.padding.top = (int)top;
        vertical.padding.bottom = (int)bottom;
        vertical.padding.left = (int)left;
        vertical.padding.right = (int)right;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
