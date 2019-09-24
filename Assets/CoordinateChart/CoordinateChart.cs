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
    private bool m_isShowXGrid = true;
    public bool IsShowXGrid
    {
        get { return m_isShowXGrid; }
    }

    [SerializeField]
    private bool m_isShowYGrid = true;
    public bool IsShowYGrid
    {
        get { return m_isShowYGrid; }
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

    [SerializeField, Range(5f, 15f)]
    private float m_arrowSize = 1;
    public float ArrowSize
    {
        get { return m_arrowSize; }
    }

    [SerializeField, Range(1f, 5f)]
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
        ModifyVertices(vh);
    }

    float xLength, yLength;
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

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
            drawAttribute.SetPosition(
                CacheUnit.SetVector(0, -LineWidth),
                CacheUnit.SetVector(0, 0),
                CacheUnit.SetVector(xLength, 0),
                CacheUnit.SetVector(xLength, -LineWidth));
            drawAttribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
            DrawSimpleQuad(vh, drawAttribute);
        }

        if (IsShowArrow)
        {
            //draw x arrow
            drawAttribute.SetPosition(
                CacheUnit.SetVector(xLength, LineWidth + ArrowSize - LineWidth / 2f),
                CacheUnit.SetVector(xLength + 2 * ArrowSize, -LineWidth / 2f),
                CacheUnit.SetVector(xLength + 2 * ArrowSize, -LineWidth / 2f),
                CacheUnit.SetVector(xLength, -LineWidth - ArrowSize - LineWidth / 2f));
            drawAttribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
            DrawSimpleQuad(vh, drawAttribute);

            //draw y arrow
            drawAttribute.SetPosition(
                CacheUnit.SetVector(-ArrowSize - LineWidth / 2 - LineWidth, yLength),
                CacheUnit.SetVector(-LineWidth / 2, yLength + ArrowSize * 2),
                CacheUnit.SetVector(-LineWidth / 2, yLength + ArrowSize * 2),
                CacheUnit.SetVector(ArrowSize + LineWidth / 2, yLength));
            drawAttribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
            DrawSimpleQuad(vh, drawAttribute);
        }

        if (IsShowYAxis)
        {
            //draw y axis
            drawAttribute.SetPosition(
                CacheUnit.SetVector(-LineWidth, -LineWidth),
                CacheUnit.SetVector(-LineWidth, yLength),
                CacheUnit.SetVector(0, yLength),
                CacheUnit.SetVector(0, -LineWidth));
            drawAttribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
            DrawSimpleQuad(vh, drawAttribute);
        }

        if (IsShowScale)
        {
            //draw x sacle
            for (int i = BaseUnit; i <= xLength; i += BaseUnit)
            {
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(i - LineWidth / 2f, 0),
                    CacheUnit.SetVector(i - LineWidth / 2f, 10),
                    CacheUnit.SetVector(i + LineWidth / 2f, 10),
                    CacheUnit.SetVector(i + LineWidth / 2f, 0));
                drawAttribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
                DrawSimpleQuad(vh, drawAttribute);
            }
            //draw y scale
            for (int i = BaseUnit; i < yLength; i += BaseUnit)
            {
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(0, i + LineWidth / 2f),
                    CacheUnit.SetVector(10, i + LineWidth / 2f),
                    CacheUnit.SetVector(10, i - LineWidth / 2f),
                    CacheUnit.SetVector(0, i - LineWidth / 2f));
                drawAttribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
                DrawSimpleQuad(vh, drawAttribute);
            }
        }

        //draw grid
        if (IsShowYGrid)
        {
            //draw x grid
            for (int i = BaseUnit; i < yLength; i += BaseUnit)
            {
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(0, i + LineWidth / 2f),
                    CacheUnit.SetVector(xLength, i + LineWidth / 2f),
                    CacheUnit.SetVector(xLength, i - LineWidth / 2f),
                    CacheUnit.SetVector(0, i - LineWidth / 2f));
                drawAttribute.SetColor(GridColor, GridColor, GridColor, GridColor);
                DrawSimpleQuad(vh, drawAttribute);
            }
        }
        if (IsShowXGrid)
        {
            //draw y grid
            for (int i = BaseUnit; i <= xLength; i += BaseUnit)
            {
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(i - LineWidth / 2f, 0),
                    CacheUnit.SetVector(i - LineWidth / 2f, yLength),
                    CacheUnit.SetVector(i + LineWidth / 2f, yLength),
                    CacheUnit.SetVector(i + LineWidth / 2f, 0));
                drawAttribute.SetColor(GridColor, GridColor, GridColor, GridColor);
                DrawSimpleQuad(vh, drawAttribute);
            }
        }

    }

}


