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


[Serializable]
public struct BGAttribute
{
    public float lineCurde;
    public Color upBorderColor;
    public Color buttomBorderColor;

    public BGAttribute(float lineCurde, Color upBorderColor, Color buttomBorderColor)
    {
        this.lineCurde = lineCurde;
        this.upBorderColor = upBorderColor;
        this.buttomBorderColor = buttomBorderColor;
    }
}

public class CacheUnit
{
    public static List<float> cacheSinList = new List<float>();
    public static List<float> cacheCosList = new List<float>();

    public static void CacheItemAsAngle(float parametersCount,float delta)
    {
        cacheSinList.Clear();
        cacheCosList.Clear();
        float i = 0;
        

        //float nextRadin = (i + angle) * Mathf.PI / 180f;
        for (int j = 0; j <= parametersCount; j++)
        {
            float preRadian = i * Mathf.Deg2Rad;
            cacheCosList.Add(Mathf.Cos(preRadian));
            cacheSinList.Add(Mathf.Sin(preRadian));
            i += delta;
        }
    }

    public static void CacheItemAsRadian(float parametersCount,float delta)
    {

    }
}

public class RadarChartBGPlus : BaseMeshEffect
{
    private enum BGColorMode
    {
        Circle,
        Sector
    }

    [SerializeField]
    public List<BGAttribute> bgInfoList = new List<BGAttribute>();

    [SerializeField]
    int parametersCount;//数据项个数

    [SerializeField]
    private BGColorMode colorMode = BGColorMode.Circle;

    [SerializeField]
    int circleCount = 3;

    [SerializeField, Range(1f, 2f)]
    private float guideLineWidth = 1.5f;
    [SerializeField]
    private Color lineColor = Color.white;

    [SerializeField, Range(0.1f, 0.5f)]
    float lineCurde = 0.1f;//线条的粗度
    public float LineCurde
    {
        get { return lineCurde; }
        set { lineCurde = value; graphic.SetVerticesDirty(); }
    }

    [SerializeField, Range(10, 50)]
    private float lineDleta = 10;


    private float width, height;
    private Vector2 m_pos = new Vector2();

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }


        ModifyVertices(vh);
    }

    float Kx = 0, Ky = 0;
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        width = graphic.rectTransform.sizeDelta.x;
        height = graphic.rectTransform.sizeDelta.y;

        Kx = width * 0.5f;
        Ky = height * 0.5f;//scale ratio
    }
    private Vector2 SetVector2(float x, float y)
    {
        m_pos.x = x; m_pos.y = y;
        return m_pos;
    }

    UIVertex vertex = new UIVertex();
    UIVertex[] tmpVertexStream = new UIVertex[4];

    static int tmpCount = 0;
    private void ModifyVertices(VertexHelper vh)
    {
        vh.Clear();

        float angle = 360f / parametersCount;
        //判断是否需要缓存
        if (tmpCount != parametersCount)
        {
            CacheUnit.CacheItemAsAngle(parametersCount,angle);
        } 
        tmpCount = parametersCount;

        float innerX = (Kx - LineCurde * Kx), innerY = (Ky - LineCurde * Ky);
        float outerX = Kx, outerY = Ky;
 
        for (int z = 0; z < circleCount; z++)
        {
            float delta = lineDleta * z;
            for (int j = 0; j < parametersCount; j++)
            {
                //设置颜色模式
                int sequence = -1;
                if (colorMode == BGColorMode.Circle)
                    sequence = z;
                else
                    sequence = j;

                float cos = CacheUnit.cacheCosList[j];
                float sin = CacheUnit.cacheSinList[j];
                float nextCos = CacheUnit.cacheCosList[j + 1];
                float nextSin = CacheUnit.cacheSinList[j + 1];


                //上边    顺序：左下角  左上角    右上角   右下角
                vertex.position = SetVector2(nextCos * (innerX - delta), nextSin * (innerY - delta));
                vertex.color = bgInfoList[sequence].upBorderColor;
                tmpVertexStream[0] = vertex;

                vertex.position = SetVector2(nextCos * (outerX - delta), nextSin * (outerY - delta));
                vertex.color = bgInfoList[sequence].buttomBorderColor;
                tmpVertexStream[1] = vertex;

                vertex.position = SetVector2(cos * (outerX - delta), sin * (outerY - delta));
                vertex.color = bgInfoList[sequence].buttomBorderColor;
                tmpVertexStream[2] = vertex;

                vertex.position = SetVector2(cos * (innerX - delta), sin * (innerY - delta));
                vertex.color = bgInfoList[sequence].upBorderColor;
                tmpVertexStream[3] = vertex;

                vh.AddUIVertexQuad(tmpVertexStream);
            }
        }

        //绘制辅助线
        for (int j = 0; j < parametersCount; j++)
        {
            float cos = CacheUnit.cacheCosList[j];
            float sin = CacheUnit.cacheSinList[j];

            float K = (sin * (Ky)) / (cos * (outerX));//计算斜率，增加线条宽度

            K = Mathf.Abs(K);

            if (K > 500f)//垂直
            {
                vertex.position = SetVector2(-guideLineWidth, guideLineWidth);
                vertex.color = lineColor;
                tmpVertexStream[0] = vertex;

                vertex.position = SetVector2(cos * (outerX) - guideLineWidth, sin * (Ky) + guideLineWidth);
                vertex.color = lineColor;
                tmpVertexStream[1] = vertex;

                vertex.position = SetVector2(cos * (outerX) + guideLineWidth, sin * (outerY) - guideLineWidth);
                vertex.color = lineColor;
                tmpVertexStream[2] = vertex;

                vertex.position = SetVector2(guideLineWidth, -guideLineWidth);
                vertex.color = lineColor;
                tmpVertexStream[3] = vertex;

                vh.AddUIVertexQuad(tmpVertexStream);
                continue;
            }
            vertex.position = SetVector2(-guideLineWidth, guideLineWidth + K);
            vertex.color = lineColor;
            tmpVertexStream[0] = vertex;

            vertex.position = SetVector2(cos * (outerX) + guideLineWidth, sin * (Ky) + guideLineWidth + K);
            vertex.color = lineColor;
            tmpVertexStream[1] = vertex;

            vertex.position = SetVector2(cos * (outerX) + guideLineWidth, sin * (outerY) - guideLineWidth - K);
            vertex.color = lineColor;
            tmpVertexStream[2] = vertex;

            vertex.position = SetVector2(-guideLineWidth, -guideLineWidth - K);
            vertex.color = lineColor;
            tmpVertexStream[3] = vertex;

            vh.AddUIVertexQuad(tmpVertexStream);
        }

    }

}
