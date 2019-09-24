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
public abstract class ChartBase : BaseMeshEffect {

    protected float Kx, Ky;
    protected float width, height;

    protected DrawAttribute drawAttribute = new DrawAttribute();

    protected static List<UIVertex> tempVertexTriangleStream = new List<UIVertex>();


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

    protected void DrawSimpleTriangle(VertexHelper vh,DrawAttribute atb)
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
}
