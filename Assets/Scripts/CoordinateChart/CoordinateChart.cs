/*
 * UGUIChartLib
 * Copyright © 2019 w199753. 
 * feedback:http://15384855139@163.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace ChartLib
{

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
        float centerX, centerY;
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            RectTransform trans = graphic.rectTransform;
            xLength = width - width * trans.pivot.x;
            yLength = height - height * trans.pivot.y;

            centerX = -width * trans.pivot.x;
            centerY = -height * trans.pivot.y;
        }

        private void ModifyVertices(VertexHelper vh)
        {
            vh.Clear();

            if (IsShowXAxis)
            {
                //draw x
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(0 + centerX, -LineWidth),
                    CacheUnit.SetVector(0 + centerX, 0),
                    CacheUnit.SetVector(width + centerX, 0),
                    CacheUnit.SetVector(width + centerX, -LineWidth));
                drawAttribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
                DrawSimpleQuad(vh, drawAttribute);
            }

            if (IsShowArrow)
            {
                //draw x arrow
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(width + centerX, LineWidth + ArrowSize - LineWidth / 2f),
                    CacheUnit.SetVector(width + centerX + 2 * ArrowSize, -LineWidth / 2f),
                    CacheUnit.SetVector(width + centerX + 2 * ArrowSize, -LineWidth / 2f),
                    CacheUnit.SetVector(width + centerX, -LineWidth - ArrowSize - LineWidth / 2f));
                drawAttribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
                DrawSimpleQuad(vh, drawAttribute);

                //draw y arrow
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(-ArrowSize - LineWidth / 2 - LineWidth, height + centerY),
                    CacheUnit.SetVector(-LineWidth / 2, height + centerY + ArrowSize * 2),
                    CacheUnit.SetVector(-LineWidth / 2, height + centerY + ArrowSize * 2),
                    CacheUnit.SetVector(ArrowSize + LineWidth / 2, height + centerY));
                drawAttribute.SetColor(AxisColor, AxisColor, AxisColor, AxisColor);
                DrawSimpleQuad(vh, drawAttribute);
            }

            if (IsShowYAxis)
            {
                //draw y axis
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(-LineWidth, -LineWidth + centerY),
                    CacheUnit.SetVector(-LineWidth, height + centerY),
                    CacheUnit.SetVector(0, height + centerY),
                    CacheUnit.SetVector(0, -LineWidth + centerY));
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
                //draw x grid      need to draw based on pivot,so need to parts
                for (float i = -centerY; i <= height; i+=BaseUnit)
                {
                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(0 + centerX, i + LineWidth / 2f + centerY),
                        CacheUnit.SetVector(width + centerX, i + LineWidth / 2f + centerY),
                        CacheUnit.SetVector(width + centerX, i - LineWidth / 2f + centerY),
                        CacheUnit.SetVector(0 + centerX, i - LineWidth / 2f + centerY));
                    drawAttribute.SetColor(GridColor, GridColor, GridColor, GridColor);
                    DrawSimpleQuad(vh, drawAttribute);
                }
                for (float i = -centerY; i >=0; i-=BaseUnit)
                {
                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(0 + centerX, i + LineWidth / 2f + centerY),
                        CacheUnit.SetVector(width + centerX, i + LineWidth / 2f + centerY),
                        CacheUnit.SetVector(width + centerX, i - LineWidth / 2f + centerY),
                        CacheUnit.SetVector(0 + centerX, i - LineWidth / 2f + centerY));
                    drawAttribute.SetColor(GridColor, GridColor, GridColor, GridColor);
                    DrawSimpleQuad(vh, drawAttribute);
                }
            }
            if (IsShowXGrid)
            {
                //draw y grid      need to draw based on pivot,so need to parts
                for (float i = -centerX; i<=width; i += BaseUnit)
                {
                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(i - LineWidth / 2f + centerX, 0 + centerY),
                        CacheUnit.SetVector(i - LineWidth / 2f + centerX, height + centerY),
                        CacheUnit.SetVector(i + LineWidth / 2f + centerX, height + centerY),
                        CacheUnit.SetVector(i + LineWidth / 2f + centerX, 0 + centerY));
                    drawAttribute.SetColor(GridColor, GridColor, GridColor, GridColor);
                    DrawSimpleQuad(vh, drawAttribute);
                }
                for (float i = -centerX; i >=0; i -= BaseUnit)
                {
                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(i - LineWidth / 2f + centerX, 0 + centerY),
                        CacheUnit.SetVector(i - LineWidth / 2f + centerX, height + centerY),
                        CacheUnit.SetVector(i + LineWidth / 2f + centerX, height + centerY),
                        CacheUnit.SetVector(i + LineWidth / 2f + centerX, 0 + centerY));
                    drawAttribute.SetColor(GridColor, GridColor, GridColor, GridColor);
                    DrawSimpleQuad(vh, drawAttribute);
                }
            }

        }

    }
}



