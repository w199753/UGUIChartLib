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


public class PieChart : ChartBase
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


    public override void ModifyMesh(VertexHelper vh)
    {
        if (pieInfoList.Count == 0)
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
        ModifyVertices(vh);
    }


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

                quadattribute.SetPosition(
                    CacheUnit.SetVector(nextCos * ratioX, nextSin * ratioY),
                    CacheUnit.SetVector(nextCos * Kx, nextSin * Ky),
                    CacheUnit.SetVector(cos * Kx, sin * Ky),
                    CacheUnit.SetVector(cos * ratioX, sin * ratioY));
                quadattribute.SetColor(
                    pieInfoList[j].color,pieInfoList[j].color,pieInfoList[j].color,pieInfoList[j].color);
                dd.SetItem(vh, quadattribute);

                i += 0.0628f *delta;
                count += delta;
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
