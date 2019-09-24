/*
	author：@
	Last modified data:
	funtion todo:
        shoud set pivot at new Vector2(0 0);

        add and set Keypos to BG:
        line.AddKeyPosition(new Vector2(5, 5)).Add(new Vector2(6, 6));
        line.CopyValueToBG(line.KeyPosList,"lineChartBg");
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LineChart : ChartBase
{
    

    [SerializeField]
    private List<Vector2> keyPos = new List<Vector2>();
    public List<Vector2> KeyPosList
    {
        get { return keyPos; }
    }


    [SerializeField, Range(5f, 10)]
    private float lineWidth = 5f;
    public float LineWidth
    {
        get { return lineWidth; }
        set { lineWidth = value; /*graphic.SetVerticesDirty();*/ }
    }

    [SerializeField]
    private Color pointColor = Color.white;
    public Color PointColor
    {
        get { return pointColor; }
        set { pointColor = value; graphic.SetVerticesDirty(); }
    }


    [SerializeField]
    private Color lineColor = Color.white;
    public Color LineColor
    {
        get { return lineColor; }
        set { lineColor = value; graphic.SetVerticesDirty(); }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        ModifyVertices(vh);
    }



    private void ModifyVertices(VertexHelper vh)
    {
        tempVertexTriangleStream.Clear();
        vh.GetUIVertexStream(tempVertexTriangleStream);
        vh.Clear();

        for (int i = 0; i < keyPos.Count - 1; i++)
        {
            drawAttribute.SetPosition(
                CacheUnit.SetVector(keyPos[i].x, keyPos[i].y), 
                CacheUnit.SetVector(keyPos[i + 1].x, keyPos[i + 1].y),
                CacheUnit.SetVector(keyPos[i + 1].x, keyPos[i + 1].y - LineWidth), 
                CacheUnit.SetVector(keyPos[i].x, (keyPos[i].y - LineWidth)));
            drawAttribute.SetColor(
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor), 
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor));
            DrawSimpleQuad(vh, drawAttribute);
        }

        //画点
        for (int i = 0; i < keyPos.Count; i++)
        {
            drawAttribute.SetPosition(
                CacheUnit.SetVector(keyPos[i].x - LineWidth, keyPos[i].y + LineWidth), 
                CacheUnit.SetVector(keyPos[i].x + LineWidth, keyPos[i].y + LineWidth),
                CacheUnit.SetVector(keyPos[i].x + LineWidth, keyPos[i].y - LineWidth),
                CacheUnit.SetVector(keyPos[i].x - LineWidth, keyPos[i].y - LineWidth));
            drawAttribute.SetColor(
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor), 
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor));
            DrawSimpleQuad(vh, drawAttribute);

        }
    }

    public void CopyValueToBG(List<Vector2> pos, string targetName)
    {
        GameObject go = GameObject.Find(targetName);
        if (go == null)
        {
            Debug.Log("please check targetName");
            return;
        }
        go.GetComponent<LineChartBG>().keyPos = new List<Vector2>(pos);
    }

    public LineChart AddKeyPosition(Vector2 v)
    {
        this.keyPos.Add(v);
        return this;
    }

    public void SortValueByAscendOrder()
    {
        keyPos.Sort(
            (a, b) => { return a.x.CompareTo(a.x); }
            );
        keyPos.Sort(
            (a, b) => { return a.y.CompareTo(a.y); }
            );
    }

    public void SortValueByDescendOrder()
    {
        keyPos.Sort(
            (a, b) => { return -a.x.CompareTo(a.x); }
            );
        keyPos.Sort(
            (a, b) => { return -a.y.CompareTo(a.y); }
            );
    }


}
