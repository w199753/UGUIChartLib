/*
	author：@
	Last modified data:
	funtion todo:
        shoud set pivot at new Vector2(0 0);
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineChartBG : BaseMeshEffect
{
    private static List<UIVertex> tempVertexTriangleStream = new List<UIVertex>();


    public List<Vector2> keyPos;

    [SerializeField]
    private Color upBorderColor = Color.white;
    public Color UpBorderColor
    {
        get { return upBorderColor; }
        set { upBorderColor = value; graphic.SetVerticesDirty(); }
    }

    [SerializeField]
    private Color buttomBorderColor = Color.white;
    public Color ButtomBorderColor
    {
        get { return buttomBorderColor; }
        set { buttomBorderColor = value; graphic.SetVerticesDirty(); }
    }

    [Range(-1, 1)]
    public float alphaScale;

    float x;
    float y;
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;
        x = graphic.rectTransform.sizeDelta.x;
        y = graphic.rectTransform.sizeDelta.y;

        ModifyVertices(vh);
    }

    private void ModifyVertices(VertexHelper vh)
    {
        tempVertexTriangleStream.Clear();
        vh.GetUIVertexStream(tempVertexTriangleStream);
        vh.Clear();
        /* UIVertex[] tmpVertex = new UIVertex[4];
         tmpVertex[0].position = new Vector2(0, 0);
         tmpVertex[1].position = new Vector2(0, 1);
         tmpVertex[2].position = new Vector2(1,1);
         tmpVertex[3].position = new Vector2(1, 0);
         vh.AddUIVertexQuad(tmpVertex);*/

        UIVertex[] tmpVertex = new UIVertex[4];

        float[] result = ChartUntils.GetKeyArrayMaxxAndMaxy(keyPos);
        float maxx = result[0], maxy = result[1];

        float Kx = x / maxx, Ky = y / maxy;
        for (int i = 0; i < keyPos.Count - 1; i++)
        {
            tmpVertex[0].position = new Vector2(keyPos[i].x * Kx, keyPos[i].y * Ky);
            tmpVertex[0].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, upBorderColor);
            tmpVertex[0].uv0 = new Vector2(keyPos[i].x / maxx, keyPos[i].y / maxy);

            tmpVertex[1].position = new Vector2(keyPos[i + 1].x * Kx, keyPos[i + 1].y * Ky);
            tmpVertex[1].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, upBorderColor);
            tmpVertex[1].uv0 = new Vector2(keyPos[i + 1].x / maxx, keyPos[i + 1].y / maxy);

            tmpVertex[2].position = new Vector2(keyPos[i + 1].x * Kx, 0);
            tmpVertex[2].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, buttomBorderColor);
            tmpVertex[2].uv0 = new Vector2(keyPos[i + 1].x / maxx, 0);

            tmpVertex[3].position = new Vector2(keyPos[i].x * Kx, 0);
            tmpVertex[3].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, buttomBorderColor);
            tmpVertex[3].uv0 = new Vector2(keyPos[i].x / maxx, 0);

            vh.AddUIVertexQuad(tmpVertex);
        }

    }

    /*  private void ModifyVertices(List<UIVertex> vertices)
      {
          UIVertex tmpVertex = vertices[0];
          vertices.Clear();

          for (int i = 0; i < keyPos.Count-1; i++)
          {
              tmpVertex.position = new Vector2(keyPos[i].x, keyPos[i].y);
              vertices.Add(tmpVertex);
              tmpVertex.position = new Vector2(keyPos[i+1].x, 0);
              vertices.Add(tmpVertex);
              tmpVertex.position = new Vector2(keyPos[i].x,0);
              vertices.Add(tmpVertex);
              tmpVertex.position = new Vector2(keyPos[i].x, keyPos[i].y);
              vertices.Add(tmpVertex);
              tmpVertex.position = new Vector2(keyPos[i+1].x, keyPos[i+1].y);
              vertices.Add(tmpVertex);
              tmpVertex.position = new Vector2(keyPos[i+1].x, 0);
              vertices.Add(tmpVertex);
          }
      }*/


}
