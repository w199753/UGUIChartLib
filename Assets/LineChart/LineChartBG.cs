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

public class LineChartBG : ChartBase
{


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

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        ModifyVertices(vh);
    }

    private void ModifyVertices(VertexHelper vh)
    {
        tempVertexTriangleStream.Clear();
        vh.GetUIVertexStream(tempVertexTriangleStream);
        vh.Clear();

        float[] result = ChartUntils.GetKeyArrayMaxxAndMaxy(keyPos);
        float maxx = result[0], maxy = result[1];


        for (int i = 0; i < keyPos.Count - 1; i++)
        {
            quadattribute.SetPosition(
                CacheUnit.SetVector(keyPos[i].x, keyPos[i].y),
                CacheUnit.SetVector(keyPos[i + 1].x, keyPos[i + 1].y),
                CacheUnit.SetVector(keyPos[i + 1].x, 0),
                CacheUnit.SetVector(keyPos[i].x, 0));
            quadattribute.SetColor(
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, upBorderColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, upBorderColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, buttomBorderColor),
                ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, buttomBorderColor));
            quadattribute.SetUV(
                CacheUnit.SetVector(keyPos[i].x / maxx, keyPos[i].y / maxy),
                CacheUnit.SetVector(keyPos[i + 1].x / maxx, keyPos[i + 1].y / maxy),
                CacheUnit.SetVector(keyPos[i + 1].x / maxx, 0),
                CacheUnit.SetVector(keyPos[i].x / maxx, 0)
                );
            dd.SetItem(vh, quadattribute);
        }

    }

}

