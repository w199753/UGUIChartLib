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
public class NewTest : BaseMeshEffect
{

    [Header("水平渲染")]
    [Tooltip("是否启用水平渲染?")]
    public bool horizontal = false;
    public Color32 left = Color.white;
    public Color32 right = Color.white;

    [Space(10)]
    [Header("垂直渲染")]
    [Tooltip("是否启用垂直渲染?")]
    public bool vertical = false;
    public Color32 top = Color.white;
    public Color32 bottom = Color.white;

    private void HorAndVerColor(VertexHelper vh)
    {
        //获取UIVertex
        int count = vh.currentVertCount;
        if (count == 0) return;
        List<UIVertex> vertexs = new List<UIVertex>();
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = new UIVertex();
            vh.PopulateUIVertex(ref vertex, i);
            vertexs.Add(vertex);
        }
        //获取开头和结尾vh.x/vh.y坐标
        float rightX = vertexs[0].position.x;
        float leftX = vertexs[0].position.x;
        float topY = vertexs[0].position.y;
        float bottomY = vertexs[0].position.y;
        for (int i = 1; i < count; i++)
        {
            float x = vertexs[i].position.x;
            if (x > rightX)
            {
                rightX = x;
            }
            else if (x < leftX)
            {
                leftX = x;
            }
            float y = vertexs[i].position.y;
            if (y > topY)
            {
                topY = y;
            }
            else if (y < bottomY)
            {
                bottomY = y;
            }
        }
        float hor = rightX - leftX;
        float ver = topY - bottomY;
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = vertexs[i];  //颜色渐变实现
            //水平
            Color32 colorX = Color32.Lerp(left, right, (vertex.position.x - leftX) / hor);
            //垂直
            Color32 colorY = Color32.Lerp(bottom, top, (vertex.position.y - bottomY) / ver);
            vertex.color = ColorPlus(colorX, colorY);
            vh.SetUIVertex(vertex, i);
        }
    }
          private Color32 ColorPlus(Color x, Color y)
    {
        return x * y;
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;
        HorAndVerColor(vh);
    }
    private void HorizontalColor(VertexHelper vh)
    {
        //获取UIVertex
        int count = vh.currentVertCount;
        if (count == 0) return;
        List<UIVertex> vertexs = new List<UIVertex>();
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = new UIVertex();
            vh.PopulateUIVertex(ref vertex, i);
            vertexs.Add(vertex);
        }
        float rightX = vertexs[0].position.x;
        float leftX = vertexs[0].position.x;
        for (int i = 1; i < count; i++)
        {
            float x = vertexs[i].position.x;
            if (x > rightX)
            {
                rightX = x;
            }
            else if (x < leftX)
            {
                leftX = x;
            }
        }
        float hor = rightX - leftX;
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = vertexs[i];  //颜色渐变实现
            Color32 color = Color32.Lerp(left, right, (vertex.position.x - leftX) / hor);
            vertex.color = color;
            vh.SetUIVertex(vertex, i);
        }
    }

}
