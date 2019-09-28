/*
	author：@
	Last modified data:
	funtion todo:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BarChart : ChartBase
{

    [SerializeField]
    private List<BarInfo> m_barInfoList = new List<BarInfo>();
    public List<BarInfo> BarInfoList
    {
        get { return m_barInfoList; }
    }

    [SerializeField]
    private float m_groupDelta = 100f;
    public float GroupDelta
    {
        get { return m_groupDelta; }
    }

    [SerializeField]
    private float m_barItemDelta = 10;
    public float BarItemDelta
    {
        get { return m_barItemDelta; }
    }


    [SerializeField]
    private ColorMode m_colorMode = ColorMode.Sector;
    public ColorMode BarColorMode
    {
        get { return m_colorMode; }
    }

    [SerializeField]
    private float m_barWidth = 20f;
    public float BarWidth
    {
        get { return m_barWidth; }
    }

    [SerializeField]
    private ChartDirType m_chartDirType = ChartDirType.Horizontal;
    public ChartDirType barDirType
    {
        get { return m_chartDirType; }
    }

    [SerializeField]
    private ChartRandererType m_chartRandererType = ChartRandererType.BarGroup;
    public ChartRandererType chartRandererType
    {
        get { return m_chartRandererType; }
    }


    public override void ModifyMesh(VertexHelper vh)
    {
        ModifyVertices(vh);
    }

    float centerX, centerY;
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        RectTransform trans = graphic.rectTransform;

        centerX = -width * trans.pivot.x;
        centerY = -height * trans.pivot.y;
    }


    private void ModifyVertices(VertexHelper vh)
    {
        vh.Clear();
        float groupDelta = 0;
        Color tmpColor = new Color();
        for (int i = 0; i < BarInfoList.Count; i++)
        {
            float itemDelta = 0;

            for (int j = 0; j < BarInfoList[i].groupCount; j++)
            {
                PieInfo tmpInfo = BarInfoList[i].attributeInfoList[j];
                if (BarColorMode == ColorMode.Single)
                {
                    tmpColor = BarInfoList[i].attributeInfoList[0].color;
                }
                else if (BarColorMode == ColorMode.Sector)
                {
                    tmpColor = tmpInfo.color;
                }

                if (m_chartRandererType == ChartRandererType.BarGroup)
                {
                    if (barDirType == ChartDirType.Horizontal)
                    {
                        if (isShowBorder)
                        {
                            DrawSimpleRectBorder(vh,
                                CacheUnit.SetVector(groupDelta + itemDelta + centerX, 0 + centerY),
                                CacheUnit.SetVector(groupDelta + itemDelta + centerX, tmpInfo.value + centerY),
                                CacheUnit.SetVector(groupDelta + itemDelta + BarWidth + centerX, tmpInfo.value + centerY),
                                CacheUnit.SetVector(groupDelta + itemDelta + BarWidth + centerX, 0 + centerY)
                                );
                        }
                        drawAttribute.SetPosition(
                            CacheUnit.SetVector(groupDelta + itemDelta + centerX, 0 + centerY),
                            CacheUnit.SetVector(groupDelta + itemDelta + centerX, tmpInfo.value + centerY),
                            CacheUnit.SetVector(groupDelta + itemDelta + BarWidth + centerX, tmpInfo.value + centerY),
                            CacheUnit.SetVector(groupDelta + itemDelta + BarWidth + centerX, 0 + centerY)
                            );
                    }
                    else if (barDirType == ChartDirType.Vertical)
                    {
                        if (isShowBorder)
                        {
                            DrawSimpleRectBorder(vh,
                               CacheUnit.SetVector(0 + centerX, groupDelta + itemDelta + centerY),
                               CacheUnit.SetVector(0 + centerX, groupDelta + itemDelta + BarWidth + centerY),
                               CacheUnit.SetVector(tmpInfo.value + centerX, groupDelta + itemDelta + BarWidth + centerY),
                               CacheUnit.SetVector(tmpInfo.value + centerX, groupDelta + itemDelta + centerY)
                                );
                        }
                        drawAttribute.SetPosition(
                           CacheUnit.SetVector(0 + centerX, groupDelta + itemDelta + centerY),
                           CacheUnit.SetVector(0 + centerX, groupDelta + itemDelta + BarWidth + centerY),
                           CacheUnit.SetVector(tmpInfo.value + centerX, groupDelta + itemDelta + BarWidth + centerY),
                           CacheUnit.SetVector(tmpInfo.value + centerX, groupDelta + itemDelta + centerY)
                           );
                    }
                    drawAttribute.SetColor(tmpColor, tmpColor, tmpColor, tmpColor);
                    DrawSimpleQuad(vh, drawAttribute);
                    itemDelta += m_barItemDelta + BarWidth;
                }
                else if (m_chartRandererType == ChartRandererType.SingleBar)//单条显示
                {
                    m_colorMode = ColorMode.Sector;
                    if (barDirType == ChartDirType.Horizontal)
                    {
                        if(isShowBorder)
                        {
                            DrawSimpleRectBorder(vh, CacheUnit.SetVector(groupDelta + centerX, 0 + centerY + itemDelta),
                                CacheUnit.SetVector(groupDelta + centerX, tmpInfo.value + centerY + itemDelta),
                                CacheUnit.SetVector(groupDelta + BarWidth + centerX, tmpInfo.value + centerY + itemDelta),
                                CacheUnit.SetVector(groupDelta + BarWidth + centerX, 0 + centerY + itemDelta));
                        }

                        drawAttribute.SetPosition(
                            CacheUnit.SetVector(groupDelta + centerX, 0 + centerY + itemDelta),
                            CacheUnit.SetVector(groupDelta + centerX, tmpInfo.value + centerY + itemDelta),
                            CacheUnit.SetVector(groupDelta + BarWidth + centerX, tmpInfo.value + centerY + itemDelta),
                            CacheUnit.SetVector(groupDelta + BarWidth + centerX, 0 + centerY + itemDelta)
                            );

                    }
                    else if (barDirType == ChartDirType.Vertical)
                    {
                        if(isShowBorder)
                        {
                            DrawSimpleRectBorder(vh,
                               CacheUnit.SetVector(0 + centerX + itemDelta, groupDelta + centerY),
                               CacheUnit.SetVector(0 + centerX + itemDelta, groupDelta + BarWidth + centerY),
                               CacheUnit.SetVector(tmpInfo.value + centerX + itemDelta, groupDelta + BarWidth + centerY),
                               CacheUnit.SetVector(tmpInfo.value + centerX + itemDelta, groupDelta + centerY));
                        }

                        drawAttribute.SetPosition(
                           CacheUnit.SetVector(0 + centerX + itemDelta, groupDelta + centerY),
                           CacheUnit.SetVector(0 + centerX + itemDelta, groupDelta + BarWidth + centerY),
                           CacheUnit.SetVector(tmpInfo.value + centerX + itemDelta, groupDelta + BarWidth + centerY),
                           CacheUnit.SetVector(tmpInfo.value + centerX + itemDelta, groupDelta + centerY)
                           );
                    }
                    drawAttribute.SetColor(tmpColor, tmpColor, tmpColor, tmpColor);
                    DrawSimpleQuad(vh, drawAttribute);
                    itemDelta += tmpInfo.value;
                }
            }
            groupDelta += BarWidth + m_groupDelta;//设置组间距
        }

    }
    public BarChart AddValue(BarInfo info)
    {
        m_barInfoList.Add(info);
        return this;
    }
}




