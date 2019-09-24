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
    private ChartRandererType m_chartRandererType = ChartRandererType.Horizontal;
    public ChartRandererType barRandererType
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

    }
    private void ModifyVertices(VertexHelper vh)
    {
        vh.Clear();
        float groupDelta = 0;
        for (int i = 0; i < BarInfoList.Count; i++)
        {
            float itemDelta = 0;

            for (int j = 0; j < BarInfoList[i].groupCount; j++)
            {
                PieInfo tmpInfo = BarInfoList[i].attributeInfoList[j];
                Color tmpColor = new Color();
                if (BarColorMode == ColorMode.Single)
                {
                    tmpColor = BarInfoList[i].attributeInfoList[0].color;
                }
                else if (BarColorMode == ColorMode.Sector)
                {
                    tmpColor = tmpInfo.color;
                }
                if (barRandererType == ChartRandererType.Horizontal)
                {
                    drawAttribute.SetPosition(
                        CacheUnit.SetVector(groupDelta + itemDelta, 0),
                        CacheUnit.SetVector(groupDelta + itemDelta, tmpInfo.value),
                        CacheUnit.SetVector(groupDelta + itemDelta + BarWidth, tmpInfo.value),
                        CacheUnit.SetVector(groupDelta + itemDelta + BarWidth, 0)
                        );
                    drawAttribute.SetColor(tmpColor, tmpColor, tmpColor, tmpColor);
                    DrawSimpleQuad(vh, drawAttribute);
                    itemDelta += m_barItemDelta + BarWidth;
                }
                else if (barRandererType == ChartRandererType.Vertical)
                {
                    drawAttribute.SetPosition(
                       CacheUnit.SetVector(0, groupDelta + itemDelta),
                       CacheUnit.SetVector(0, groupDelta + itemDelta + BarWidth),
                       CacheUnit.SetVector(tmpInfo.value, groupDelta + itemDelta + BarWidth),
                       CacheUnit.SetVector(tmpInfo.value, groupDelta + itemDelta)
                       );
                    drawAttribute.SetColor(tmpColor, tmpColor, tmpColor, tmpColor);
                    DrawSimpleQuad(vh, drawAttribute);
                    itemDelta += m_barItemDelta + BarWidth;
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




