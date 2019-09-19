/*
	author：@
	Last modified data:
	funtion todo:
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PieChart : BaseMeshEffect
{

    [SerializeField]
    private List<PieInfo> pieInfoList = new List<PieInfo>();
    public List<PieInfo> PieInfoList
    {
        get { return pieInfoList; }
    }

    [SerializeField,Range(0,1)]
    private float innerRatio=1;
    public float InnerRaio { get { return innerRatio; } }


    private float width, height;
    private Vector2 m_pos = new Vector2();


    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || pieInfoList.Count == 0)
        {
            return;
        }
        int res = 0;
        foreach (var item in pieInfoList)
        {
            res += item.value;
        }
        if (res != 100)
        {
            Debug.LogError("the total value exceeds the limit");
            return;
        } 
        width = graphic.rectTransform.sizeDelta.x;
        height = graphic.rectTransform.sizeDelta.y;

        ModifyVertices(vh);

    }


    float Kx = 0, Ky = 0;
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        width = graphic.rectTransform.sizeDelta.x;
        height = graphic.rectTransform.sizeDelta.y;

        Kx = width * 0.5f;
        Ky = height * 0.5f;//scale ratio
    }

    private Vector2 SetVector2(float x,float y)
    {
        m_pos.x = x; m_pos.y = y;
        return m_pos;
    }

    UIVertex vertex = new UIVertex();
    UIVertex[] tmpVertexStream = new UIVertex[4];
    private void ModifyVertices(VertexHelper vh)
    {
        vh.Clear();

        float i = 0;
        float ratioX = (Kx - InnerRaio * Kx);
        float ratioY = (Ky - InnerRaio * Ky);
        for (int j = 0; j < pieInfoList.Count; j++)
        {
            float value = pieInfoList[j].value;

            float delta = value * 0.05f;//节约mesh资源,,,数值越大越节省，同时效果也越差.简易数值最好为整数，否则效果出错

            if (value < 5) delta = 1;

            float count = 0;
            while (count < value)
            {
                float cos = Mathf.Cos(i),sin=Mathf.Sin(i);
                float nextCos = Mathf.Cos(i + 0.0628f * delta), nextSin = Mathf.Sin(i + 0.0628f * delta);

                vertex.position = SetVector2(nextCos * ratioX, nextSin * ratioY);
                vertex.color = pieInfoList[j].color;
                tmpVertexStream[0] = vertex;

                vertex.position = SetVector2(nextCos * Kx, nextSin * Ky);
                vertex.color = pieInfoList[j].color;
                tmpVertexStream[1] = vertex;

                vertex.position = SetVector2(cos * Kx, sin * Ky);
                vertex.color = pieInfoList[j].color;
                tmpVertexStream[2] = vertex;

                vertex.position = SetVector2(cos * ratioX, sin * ratioY);
                vertex.color = pieInfoList[j].color;
                tmpVertexStream[3] = vertex;

                vh.AddUIVertexQuad(tmpVertexStream);
                i += 0.0628f *delta;
                //if (i > 2 * Mathf.PI) break;
                count += delta;
                //count+=1;
            }

        }

    }

    public PieChart AddValue(PieInfo info)
    {
        this.pieInfoList.Add(info);
        return this;
    }

}

/// <summary>
/// 饼状图的基本信息，比例为百分比
/// </summary>
[Serializable]
public struct PieInfo
{
    public string name;
    public Color color;
    public int value;

    public PieInfo(string name, Color color, int value)
    {
        this.name = name;
        this.color = color;
        this.value = value;
    }
}
