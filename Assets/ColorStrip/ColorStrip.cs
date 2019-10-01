/*
	author：@
	Last modified data:
	funtion todo:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ColorStrip : ChartBase
{

    [SerializeField]
    private List<Color> m_colorList = new List<Color>();
    public List<Color> ColorList
    {
        get { return m_colorList; }
    }

    [SerializeField]
    private ChartDirType m_dirType = ChartDirType.Horizontal;
    public ChartDirType DirType
    {
        get { return m_dirType; }
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
        float delta = 0;
        int count = ColorList.Count;

        if (isShowBorder)
        {
            DrawSimpleRectBorder(vh,
                    CacheUnit.SetVector(centerX, centerY),
                    CacheUnit.SetVector(centerX, centerY + height),
                    CacheUnit.SetVector(centerX + width, centerY + height),
                    CacheUnit.SetVector(centerX + width, centerY)
                );
        }


        for (int i = 0; i < count - 1; i++)
        {
            if (DirType == ChartDirType.Horizontal)
            {
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(centerX + delta, centerY),
                    CacheUnit.SetVector(centerX + delta, centerY + height),
                    CacheUnit.SetVector(centerX + delta + (width) / (float)(count - 1), centerY + height),
                    CacheUnit.SetVector(centerX + delta + (width) / (float)(count - 1), centerY)
                    );
                drawAttribute.SetColor(m_colorList[i], m_colorList[i], m_colorList[i + 1], m_colorList[i + 1]);
                DrawSimpleQuad(vh, drawAttribute);

                delta += (width) / (float)(count - 1);
            }
            else
            {
                drawAttribute.SetPosition(
                    CacheUnit.SetVector(centerX, centerY + delta),
                    CacheUnit.SetVector(centerX, centerY + delta + (height) / (float)(count - 1)),
                    CacheUnit.SetVector(centerX + width, centerY + delta + (height) / (float)(count - 1)),
                    CacheUnit.SetVector(centerX + width, centerY + delta)
                    );
                drawAttribute.SetColor(m_colorList[i], m_colorList[i + 1], m_colorList[i + 1], m_colorList[i]);
                DrawSimpleQuad(vh, drawAttribute);

                delta += (height) / (float)(count - 1);
            }

        }

    }


}
