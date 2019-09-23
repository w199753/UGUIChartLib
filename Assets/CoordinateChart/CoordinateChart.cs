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

public class CoordinateChart : ChartBase
{
    [SerializeField]
    private bool m_isShowXAxis = true;
    public bool IsShowXAxis
    {
        get { return m_isShowXAxis; }
    }

    [SerializeField]
    private bool m_isShowYAxis = true;
    public bool IsShowYAxis
    {
        get { return m_isShowYAxis; }
    }

    [SerializeField]
    private bool m_isShowGrid = true;
    public bool IsShowGrid
    {
        get { return m_isShowGrid; }
    }

    [SerializeField]
    private bool m_isShowScale = true;//刻度
    public bool IsShowScale
    {
        get { return m_isShowScale; }
    }

    [SerializeField, Range(1, 100)]
    private int m_baseUnit = 10;
    public int BaseUnit
    {
        get { return m_baseUnit; }
    }

    [SerializeField]
    private Color m_axisColor = Color.white;
    public Color AxisColor
    {
        get { return m_axisColor; }
    }

    [SerializeField]
    private bool m_isShowArrow = true;
    public bool IsShowArrow
    {
        get { return m_isShowArrow; }
    }

    [SerializeField]
    private float m_arrowSize = 1;
    public float ArrowSize
    {
        get { return m_arrowSize; }
    }

    [SerializeField]
    private float m_lineWidth = 1;
    public float LineWidth
    {
        get { return m_lineWidth; }
    }

    [SerializeField]
    private Color m_gridColor = new Color(1, 1, 1, 0.3f);
    public Color GridColor
    {
        get { return m_gridColor; }
    }


    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        ModifyVertices(vh);
    }

    float xLength, yLength;
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        width = graphic.rectTransform.sizeDelta.x;
        height = graphic.rectTransform.sizeDelta.y;

        RectTransform trans = graphic.rectTransform;
        xLength = width - width * trans.pivot.x;
        yLength = height - height * trans.pivot.y;
        //print(width + " " + height+" "+xLength+" "+yLength);
    }

    private void ModifyVertices(VertexHelper vh)
    {
        vh.Clear();

        if (IsShowXAxis)
        {
            //draw x
            quadattribute.SetPosition(
                CacheUnit.SetVector(0, -LineWidth),
                CacheUnit.SetVector(0, 0),
                CacheUnit.SetVector(xLength, 0),
                CacheUnit.SetVector(xLength, -LineWidth));
            quadattribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
            dd.SetItem(vh, quadattribute);
        }

        if (IsShowArrow)
        {
            //draw x arrow
            quadattribute.SetPosition(
                CacheUnit.SetVector(xLength, LineWidth + ArrowSize - LineWidth / 2f),
                CacheUnit.SetVector(xLength + 2 * ArrowSize, -LineWidth / 2f),
                CacheUnit.SetVector(xLength + 2 * ArrowSize, -LineWidth / 2f),
                CacheUnit.SetVector(xLength, -LineWidth - ArrowSize - LineWidth / 2f));
            quadattribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
            dd.SetItem(vh, quadattribute);

            //draw y arrow
            quadattribute.SetPosition(
                CacheUnit.SetVector(-ArrowSize - LineWidth / 2 - LineWidth, yLength),
                CacheUnit.SetVector(-LineWidth / 2, yLength + ArrowSize * 2),
                CacheUnit.SetVector(-LineWidth / 2, yLength + ArrowSize * 2), 
                CacheUnit.SetVector(ArrowSize + LineWidth / 2, yLength));
            quadattribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
            dd.SetItem(vh, quadattribute);
        }

        if (IsShowYAxis)
        {
            //draw y axis
            quadattribute.SetPosition(
                CacheUnit.SetVector(-LineWidth, -LineWidth), 
                CacheUnit.SetVector(-LineWidth, yLength),
                CacheUnit.SetVector(0, yLength), 
                CacheUnit.SetVector(0, -LineWidth));
            quadattribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
            dd.SetItem(vh, quadattribute);
        }

        if (IsShowScale)
        {
            //draw x sacle
            for (int i = BaseUnit; i <= xLength; i += BaseUnit)
            {
                quadattribute.SetPosition(
                    CacheUnit.SetVector(i - LineWidth / 2f, 0), 
                    CacheUnit.SetVector(i - LineWidth / 2f, 10),
                    CacheUnit.SetVector(i + LineWidth / 2f, 10), 
                    CacheUnit.SetVector(i + LineWidth / 2f, 0));
                quadattribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
                dd.SetItem(vh, quadattribute);
            }
            //draw y scale
            for (int i = BaseUnit; i < yLength; i += BaseUnit)
            {
                quadattribute.SetPosition(
                    CacheUnit.SetVector(0, i + LineWidth / 2f), 
                    CacheUnit.SetVector(10, i + LineWidth / 2f),
                    CacheUnit.SetVector(10, i - LineWidth / 2f), 
                    CacheUnit.SetVector(0, i - LineWidth / 2f));
                quadattribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
                dd.SetItem(vh, quadattribute);
            }
        }

        //draw grid
        if (IsShowGrid)
        {

            for (int i = BaseUnit; i <= xLength; i += BaseUnit)
            {
                quadattribute.SetPosition(
                    CacheUnit.SetVector(i - LineWidth / 2f, 0), 
                    CacheUnit.SetVector(i - LineWidth / 2f, yLength),
                    CacheUnit.SetVector(i + LineWidth / 2f, yLength), 
                    CacheUnit.SetVector(i + LineWidth / 2f, 0));
                quadattribute.SetColor(GridColor, GridColor, GridColor, GridColor);
                dd.SetItem(vh, quadattribute);
            }
            for (int i = BaseUnit; i < yLength; i += BaseUnit)
            {
                quadattribute.SetPosition(
                    CacheUnit.SetVector(0, i + LineWidth / 2f),
                    CacheUnit.SetVector(xLength, i + LineWidth / 2f),
                    CacheUnit.SetVector(xLength, i - LineWidth / 2f), 
                    CacheUnit.SetVector(0, i - LineWidth / 2f));
                quadattribute.SetColor(GridColor, GridColor, GridColor, GridColor);
                dd.SetItem(vh, quadattribute);
            }
        }

    }


}
public struct QuadAttribute
{
    private Vector3[] pos;
    private Color[] color;
    private Vector3[] uv;
    public Vector3[] Pos
    {
        get { if (pos == null) pos = new Vector3[4]; return pos; }
    }
    public Color[] Color
    {
        get { if (color == null) color = new Color[4]; return color; }
    }
    public Vector3[] UV
    {
        get { if (uv == null) uv = new Vector3[4]; return uv; }
    }

    //private void Init

    public void SetPosition(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        Pos[0] = v1; Pos[1] = v2; Pos[2] = v3; Pos[3] = v4;
    }
    public void SetUV(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        UV[0] = v1; UV[1] = v2; UV[2] = v3; UV[3] = v4;
    }
    public void SetColor(Color c1, Color c2, Color c3, Color c4)
    {
        Color[0] = c1; Color[1] = c2; Color[2] = c3; Color[3] = c4;
    }
}
public class DrawUnit
{
    VertexHelper vh;
    UIVertex[] tmpVertexStream = new UIVertex[4];
    public void SetItem(VertexHelper vh, QuadAttribute atb)
    {
        for (int i = 0; i < 4; i++)
        {
            tmpVertexStream[i].color = atb.Color[i];
            tmpVertexStream[i].position = atb.Pos[i];
            tmpVertexStream[i].uv0 = atb.UV[i];
        }
        vh.AddUIVertexQuad(tmpVertexStream);
    }
}
