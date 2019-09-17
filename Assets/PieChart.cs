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

    private static List<UIVertex> tempVertexTriangleStream = new List<UIVertex>();

    [SerializeField]
    private List<PieInfo> pieInfoList = new List<PieInfo>();
    public List<PieInfo> PieInfoList
    {
        get { return pieInfoList; }
    }


    float x, y;
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
        x = graphic.rectTransform.sizeDelta.x;
        y = graphic.rectTransform.sizeDelta.y;

        ModifyVertices(vh);

    }

    private void ModifyVertices(VertexHelper vh)
    {
        UIVertex vertex = new UIVertex();
        vh.Clear();

        float i = 0;
        float Kx = x *0.5f, Ky = y *0.5f;//scale ratio

        for (int j = 0; j < pieInfoList.Count; j++)
        {
            float value = pieInfoList[j].value;

            float delta = value * 0.1f;//节约mesh资源

            if (value < 5)delta = 1;

            float count = 0;
            while (count < value)
            {
                vertex.position = new Vector3(0, 0, 0);
                vertex.color = pieInfoList[j].color;
                tempVertexTriangleStream.Add(vertex);

                vertex.position = new Vector3(Mathf.Cos(i + 0.0628f * delta) * Kx, Mathf.Sin(i + 0.0628f * delta) * Ky);
                vertex.color = pieInfoList[j].color;
                tempVertexTriangleStream.Add(vertex);

                vertex.position = new Vector3(Mathf.Cos(i) * Kx, Mathf.Sin(i) * Ky);
                vertex.color = pieInfoList[j].color;
                tempVertexTriangleStream.Add(vertex);


                i += 0.0628f * delta;
                if (i > 2 * Mathf.PI) break;
                count += delta;
            }

        }

        vh.AddUIVertexTriangleStream(tempVertexTriangleStream);
        tempVertexTriangleStream.Clear();
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
