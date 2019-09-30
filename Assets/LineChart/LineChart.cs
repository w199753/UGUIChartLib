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
using UnityEngine.Profiling;
using UnityEngine.UI;

public enum PointItemMode
{
    None,
    Square,
    Triangle,
    Diamond,
    Circle,
    Star
}
public class LineChart : ChartBase
{


    [SerializeField]
    private List<Vector3> keyPos = new List<Vector3>();
    public List<Vector3> KeyPosList
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
    private PointItemMode m_itemMode = PointItemMode.Square;
    public PointItemMode ItemMode
    {
        get { return m_itemMode; }
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

        float len = 0;

        for (int i = 0; i < keyPos.Count - 1; i++)
        {
            drawAttribute.SetPosition(
                CacheUnit.SetVector(keyPos[i].x, keyPos[i].y),
                CacheUnit.SetVector(keyPos[i + 1].x, keyPos[i + 1].y),
                CacheUnit.SetVector(keyPos[i + 1].x, keyPos[i + 1].y - LineWidth),
                CacheUnit.SetVector(keyPos[i].x, keyPos[i].y - LineWidth));
            drawAttribute.SetColor(
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor));
            DrawSimpleQuad(vh, drawAttribute);
        }

        switch (ItemMode)
        {
            case PointItemMode.Square:
                len = lineWidth;
                //画点
                for (int i = 0; i < keyPos.Count; i++)
                {
                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(-len, +len) + keyPos[i],
                        CacheUnit.SetVector(+len, +len) + keyPos[i],
                        CacheUnit.SetVector(+len, -len) + keyPos[i],
                        CacheUnit.SetVector(-len, -len) + keyPos[i]
                        );
                    drawAttribute.SetColor(
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor));
                    DrawSimpleQuad(vh, drawAttribute);
                }
                break;

            case PointItemMode.Triangle:
                len = lineWidth * 3;
                for (int i = 0; i < keyPos.Count; i++)
                {
                    //模拟一个等边三角形
                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(0, (1.732f - 1) * len * 0.5f) + keyPos[i],
                        CacheUnit.SetVector(0, (1.732f - 1) * len * 0.5f) + keyPos[i],
                        CacheUnit.SetVector(len / 2, (-1.732f + 1) * len * 0.5f) + keyPos[i],
                        CacheUnit.SetVector(-len / 2, (-1.732f + 1) * len * 0.5f) + keyPos[i]
                        );
                    drawAttribute.SetColor(
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor));
                    DrawSimpleQuad(vh, drawAttribute);
                }
                break;

            case PointItemMode.Diamond:
                len = lineWidth*1.5f;
                for (int i = 0; i < keyPos.Count; i++)
                {
                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(len, 0) + keyPos[i],
                        CacheUnit.SetVector(0, -len) + keyPos[i],
                        CacheUnit.SetVector(-len, 0) + keyPos[i],
                        CacheUnit.SetVector(0, len) + keyPos[i]
                        );
                    drawAttribute.SetColor(
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor));
                    DrawSimpleQuad(vh, drawAttribute);
                }
                break;

            case PointItemMode.Star:
                len = lineWidth * 2f;
                float width = len * Mathf.Cos(54 * Mathf.Deg2Rad);
                float y = width * Mathf.Tan(36 * Mathf.Deg2Rad);
                float buttomHeight = len * Mathf.Sin(54 * Mathf.Deg2Rad);
                for (int i = 0; i < keyPos.Count; i++)
                {
                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(len * Mathf.Cos(90 * Mathf.Deg2Rad), len * Mathf.Sin(90 * Mathf.Deg2Rad)) + keyPos[i],
                        CacheUnit.SetVector(len * Mathf.Cos(306 * Mathf.Deg2Rad), len * Mathf.Sin(306 * Mathf.Deg2Rad)) + keyPos[i],
                        CacheUnit.SetVector(0, -(buttomHeight - y)) + keyPos[i],
                        CacheUnit.SetVector(len * Mathf.Cos(234 * Mathf.Deg2Rad), len * Mathf.Sin(234 * Mathf.Deg2Rad)) + keyPos[i]
                        );
                    drawAttribute.SetColor(
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor));
                    DrawSimpleQuad(vh, drawAttribute);

                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(len * Mathf.Cos(18 * Mathf.Deg2Rad), len * Mathf.Sin(18 * Mathf.Deg2Rad)) + keyPos[i],
                        CacheUnit.SetVector(0, -(buttomHeight - y)) + keyPos[i],
                        CacheUnit.SetVector(0, -(buttomHeight - y)) + keyPos[i],
                        CacheUnit.SetVector(len * Mathf.Cos(162 * Mathf.Deg2Rad), len * Mathf.Sin(162 * Mathf.Deg2Rad)) + keyPos[i]
                        );
                    drawAttribute.SetColor(
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                        ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor));
                    DrawSimpleQuad(vh, drawAttribute);
                }
                break;

            case PointItemMode.Circle:
                for (int i = 0; i < keyPos.Count; i++)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        float cos = Mathf.Cos(j) * lineWidth, sin = Mathf.Sin(j) * lineWidth;
                        float nextCos = Mathf.Cos(j + 0.0628f * 15f) * lineWidth, nextSin = Mathf.Sin(j + 0.0628f * 15f) * lineWidth;

                        drawAttribute.SetPosition(
                            CacheUnit.SetVector(0,0) + keyPos[i],
                            CacheUnit.SetVector(nextCos, nextSin) + keyPos[i],
                            CacheUnit.SetVector(cos, sin) + keyPos[i],
                            CacheUnit.SetVector(0,0) + keyPos[i]
                            );
                        drawAttribute.SetColor(
                            ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                            ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                            ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor),
                            ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor));
                        DrawSimpleQuad(vh, drawAttribute);
                    }
                }
                break;

            case PointItemMode.None:
                break;
            default:
                break;
        }

    }

    public void CopyValueToBG(List<Vector3> pos, string targetName)
    {
        GameObject go = GameObject.Find(targetName);
        if (go == null)
        {
            Debug.Log("please check targetName");
            return;
        }
        go.GetComponent<LineChartBG>().keyPos = new List<Vector3>(pos);
    }

    public LineChart AddKeyPosition(Vector3 v)
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
