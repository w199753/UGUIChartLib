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

public class CacheUnit
{
    public static List<float> cacheSinList = new List<float>();
    public static List<float> cacheCosList = new List<float>();


    private static Vector3 tmpV = new Vector3();

    public static void CacheItemAsAngle(float parametersCount, float delta)
    {
        cacheSinList.Clear();
        cacheCosList.Clear();
        float i = 0;

        for (int j = 0; j <= parametersCount; j++)
        {
            float preRadian = i * Mathf.Deg2Rad;
            cacheCosList.Add(Mathf.Cos(preRadian));
            cacheSinList.Add(Mathf.Sin(preRadian));
            i += delta;
        }
    }

    public static void CacheItemAsRadian(float parametersCount, List<PieInfo> PieInfoList)
    {
        cacheCosList.Clear();
        cacheSinList.Clear();
    }

    public static Vector3 SetVector(float x, float y, float z = 0)
    {
        tmpV.x = x;
        tmpV.y = y;
        tmpV.z = z;
        return tmpV;
    }
}

public enum ColorMode
{
    Single,
    Sector
}
public enum ChartDirType
{
    Vertical,
    Horizontal
}
public enum ChartRandererType
{
    BarGroup,
    SingleBar
}

public abstract class ChartBase : BaseMeshEffect
{

    protected float Kx, Ky;
    protected float width, height;

    protected DrawAttribute drawAttribute = new DrawAttribute();

    protected static List<UIVertex> tempVertexTriangleStream = new List<UIVertex>();

    public bool isShowBorder = false;
    public Color borderColor = Color.white;
    public float borderWidth = 5f;

    private UIVertex[] tmpVertexStream = new UIVertex[4];
    protected void DrawSimpleQuad(VertexHelper vh, DrawAttribute atb)
    {
        for (int i = 0; i < 4; i++)
        {
            tmpVertexStream[i].color = atb.Color[i];
            tmpVertexStream[i].position = atb.Pos[i];
            tmpVertexStream[i].uv0 = atb.UV[i];
        }
        vh.AddUIVertexQuad(tmpVertexStream);
    }

    protected void DrawSimpleTriangle(VertexHelper vh, DrawAttribute atb)
    {
        for (int i = 0; i < 3; i++)
        {
            tmpVertexStream[i].color = atb.Color[i];
            tmpVertexStream[i].position = atb.Pos[i];
            tmpVertexStream[i].uv0 = atb.UV[i];
        }
        List<UIVertex> tmpStream = new List<UIVertex>(tmpVertexStream);
        vh.AddUIVertexTriangleStream(tmpStream);
    }


    /*
     * v2-------v3
     * |         |
     * |         |
     * v1-------v4
     */
    protected void DrawSimpleRectBorder(VertexHelper vh, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        //draw up border
        drawAttribute.SetPosition(
            CacheUnit.SetVector(v2.x, v2.y + borderWidth),
            CacheUnit.SetVector(v3.x, v3.y + borderWidth),
            CacheUnit.SetVector(v3.x, v3.y),
            CacheUnit.SetVector(v2.x, v2.y)
            );
        drawAttribute.SetColor(borderColor, borderColor, borderColor, borderColor);
        DrawSimpleQuad(vh, drawAttribute);

        //draw right border
        drawAttribute.SetPosition(
            CacheUnit.SetVector(v3.x+ borderWidth, v3.y+ borderWidth),
            CacheUnit.SetVector(v4.x+ borderWidth, v4.y - borderWidth),
            CacheUnit.SetVector(v4.x , v4.y - borderWidth),
            CacheUnit.SetVector(v3.x , v3.y + borderWidth)
            );
        drawAttribute.SetColor(borderColor, borderColor, borderColor, borderColor);
        DrawSimpleQuad(vh, drawAttribute);

        //draw buttom border
        drawAttribute.SetPosition(
            CacheUnit.SetVector(v1.x, v1.y - borderWidth),
            CacheUnit.SetVector(v4.x, v4.y - borderWidth),
            CacheUnit.SetVector(v4.x, v4.y ),
            CacheUnit.SetVector(v1.x, v1.y)
            );
        drawAttribute.SetColor(borderColor, borderColor, borderColor, borderColor);
        DrawSimpleQuad(vh, drawAttribute);

        //draw left border
        drawAttribute.SetPosition(
             CacheUnit.SetVector(v2.x - borderWidth, v2.y + borderWidth),
             CacheUnit.SetVector(v1.x - borderWidth, v1.y - borderWidth),
             CacheUnit.SetVector(v1.x, v1.y - borderWidth),
             CacheUnit.SetVector(v2.x, v2.y + borderWidth)
             );
        drawAttribute.SetColor(borderColor, borderColor, borderColor, borderColor);
        DrawSimpleQuad(vh, drawAttribute);

    }


    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        width = graphic.rectTransform.sizeDelta.x;
        height = graphic.rectTransform.sizeDelta.y;

        Kx = width * 0.5f;
        Ky = height * 0.5f;//scale ratio
    }
}

public struct DrawAttribute
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
    public void Clear()
    {
        Pos[0] =  Pos[1] =  Pos[2] =  Pos[3] = CacheUnit.SetVector(0, 0, 0); 
        UV[0] =  UV[1] =  UV[2] =  UV[3] = CacheUnit.SetVector(0, 0, 0);
        Color[0] = Color[1] = Color[2] = Color[3] = UnityEngine.Color.white;

    }
}
