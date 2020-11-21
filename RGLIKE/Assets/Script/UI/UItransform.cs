using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UItransform : MonoBehaviour
{
    private int count;
    private int index;
	public struct UItransformData
	{
        public RectTransform rtt;
        public Vector2 base_viewSize;
        public Vector2 minSize;

        public Vector2 base_viewPosition;
        public Vector2 minPos;

        public void init(RectTransform rttf)
		{
            rtt = rttf;

            base_viewSize = Camera.main.ScreenToViewportPoint(rtt.sizeDelta);
            minSize = rtt.sizeDelta / 3;

            base_viewPosition = Camera.main.ScreenToViewportPoint(rtt.anchoredPosition);
            minPos = rtt.anchoredPosition / 3;
        }

        public void reSize()
		{
            Vector2 v = Camera.main.ViewportToScreenPoint(base_viewSize);
            v.x = Mathf.Max(v.x, minSize.x);
            v.y = Mathf.Max(v.y, minSize.y);
            rtt.sizeDelta = v;
           
            v = Camera.main.ViewportToScreenPoint(base_viewPosition);
            v.x = Mathf.Max(v.x, minPos.x);
            v.y = Mathf.Max(v.y, minPos.y);
            rtt.anchoredPosition = v;
        }
    }
    public UItransformData[] uiTransformData;

    public void createUItransformData(int count)
	{
        if (uiTransformData != null)
            return;

        this.count = count;
        uiTransformData = new UItransformData[this.count];
        index = 0;
    }

    public void initUItransformData(RectTransform rttf)
	{
        ref UItransformData uit = ref uiTransformData[index];
        uit.init(rttf);
        index++;

        if (index > count)
            print("ui transform count over");
    }

    public void reSizeUItransformData()
	{
        for (int i = 0; i < count; i++) 
		{
            ref UItransformData uit = ref uiTransformData[i];
            uit.reSize();
        }
    }
}
