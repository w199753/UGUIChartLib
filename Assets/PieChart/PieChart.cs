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

    [SerializeField]
    private float separationDegree = 0f;
    /// <summary>
    /// 分离度
    /// </summary>
    public float SeparationDegree
    {
        get { return separationDegree; }
    }

    [SerializeField, Range(0, 1)]
    private float innerRatio = 1;
    /// <summary>
    /// 内部圆环缩放比例
    /// </summary>
    public float InnerRaio
    {
        get { return innerRatio; }
    }


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

            float preRadian = i;
            float nextRadian = i + 0.0628f * delta * (value / delta);
            Vector3 preV = new Vector3(Mathf.Cos(preRadian), Mathf.Sin(preRadian));
            Vector3 nextV = new Vector3(Mathf.Cos(nextRadian), Mathf.Sin(nextRadian));
            Vector3 separationVector = (preV + nextV).normalized * 0.5f;

            while (count < value)
            {
                float cos = Mathf.Cos(i), sin = Mathf.Sin(i);
                float nextCos = Mathf.Cos(i + 0.0628f * delta), nextSin = Mathf.Sin(i + 0.0628f * delta);

                drawAttribute.SetPosition(
                    CacheUnit.SetVector(nextCos * ratioX, nextSin * ratioY) + separationVector * separationDegree,
                    CacheUnit.SetVector(nextCos * (Kx), nextSin * (Ky)) + separationVector * separationDegree,
                    CacheUnit.SetVector(cos * (Kx), sin * (Ky)) + separationVector * separationDegree,
                    CacheUnit.SetVector(cos * ratioX, sin * ratioY) + separationVector * separationDegree);
                drawAttribute.SetColor(
                    pieInfoList[j].color, pieInfoList[j].color, pieInfoList[j].color, pieInfoList[j].color);
                DrawSimpleQuad(vh, drawAttribute);

                i += 0.0628f * delta;
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
/// 图标中的基本信息
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

[Serializable]
public struct BarInfo
{
    public string name;
    public int groupCount;
    public List<PieInfo> attributeInfoList;

    public BarInfo(string name, int groupCount, float itemDelta = 10f)
    {
        this.name = name;
        this.groupCount = groupCount;
        attributeInfoList = new List<PieInfo>();
        index = 0;
    }

    private int index;
    public BarInfo SetGroupItem(string name, int value, Color color)
    {
        if (index < groupCount)
        {
            attributeInfoList.Add(new PieInfo(name, color, value));
            index++;
        }
        return this;
    }

    public BarInfo SetGroupItem(PieInfo info)
    {
        if (index < groupCount)
        {
            attributeInfoList.Add(info);
            index++;
        }
        return this;
    }

    public void SetGroupItem(List<PieInfo> infoList)
    {
        foreach (var item in infoList)
        {
            if (index < groupCount)
            {
                attributeInfoList.Add(item);
                index++;
            }
        }
    }



}
