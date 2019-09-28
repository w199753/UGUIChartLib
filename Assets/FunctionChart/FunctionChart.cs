/*
	author：@
	Last modified data:
	funtion todo:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FunctionChart : ChartBase {
    [Serializable]
    //Ax+By+C=0;     y=(-Ax-C)/B
    public struct LinearFun
    {
        [Range(0.1f, 5f)]
        public float A;
        [Range(0.1f, 5f)]
        public float B;
        [Range(0.1f, 5f)]
        public float C;

        public LinearFun(float a,float b, float c)
        {
            A = a;
            B = b;
            C = c;
        }
    }

    [Serializable]
    //Acos(x)+B
    public struct CosineFun
    {
        [Range(0.1f, 5f)]
        public float A;
        public float B;
    }

    [Serializable]
    //Asin(x)+B
    public struct SinFun
    {
        [Range(0.1f, 5f)]
        public float A;
        public float B;
    }

    private enum FunctionType
    {
        SinFun,
        CosFun,
        LinearFun
    }

    [SerializeField]
    private FunctionType funType = FunctionType.SinFun;


    [SerializeField]
    private float lineWidth = 2f;
    public float LineWidth
    {
        get { return lineWidth; }
    }

    [SerializeField,Range(5f,15f)]
    private float lineSmooth = 10f;
    public float LineSmooth
    {
        get { return lineSmooth; }
    }

    public LinearFun linearFun = new LinearFun(0.1f,0.1f,0.1f);
    public CosineFun cosFun = new CosineFun();
    public SinFun sinFun = new SinFun();

    public override void ModifyMesh(VertexHelper vh)
    {
        ModifyVertices(vh);
    }

    float centerX, centerY;
    private void ModifyVertices(VertexHelper vh)
    {
        vh.Clear();

        switch (funType)
        {
            case FunctionType.SinFun:
                DrawSinFunChart(vh, funType);
                break;
            case FunctionType.CosFun:
                DrawCosFunChart(vh, funType);
                break;
            case FunctionType.LinearFun:
                DrawLinearFunChart(vh, funType);
                break;
            default:
                break;
        }


    }

    void DrawSinFunChart(VertexHelper vh, FunctionType type)
    {
        var startPos = GetResult(-width / 2.0f,type) * sinFun.A;
        for (var x = -width / 2.0f + 1; x < width / 2.0f; x += lineSmooth)
        {
            var endPos = GetResult(x,type) * sinFun.A;
            vh.AddUIVertexQuad(GetQuad(startPos + new Vector2(0, sinFun.B), endPos + new Vector2(0, sinFun.B), Color.white, lineWidth));
            startPos = endPos;
        }
    }

    void DrawCosFunChart(VertexHelper vh, FunctionType type)
    {
        var startPos = GetResult(-width / 2.0f, type) * cosFun.A;
        for (var x = -width / 2.0f + 1; x < width / 2.0f; x += lineSmooth)
        {
            var endPos = GetResult(x, type) * cosFun.A;
            vh.AddUIVertexQuad(GetQuad(startPos + new Vector2(0, cosFun.B), endPos + new Vector2(0, cosFun.B), Color.white, lineWidth));
            startPos = endPos;
        }
    }

    void DrawLinearFunChart(VertexHelper vh, FunctionType type)
    {
        var startPos = GetResult(-width / 2.0f, type);
        for (var x = -width / 2.0f + 1; x < width / 2.0f; x += lineSmooth)
        {
            var endPos = GetResult(x, type) ;
            vh.AddUIVertexQuad(GetQuad(startPos, endPos, Color.white, lineWidth));
            startPos = endPos;
        }
    }

    Vector2 GetResult(float x, FunctionType type)
    {
        switch (type)
        {
            case FunctionType.SinFun:
                return new Vector2(x, Mathf.Sin(x * Mathf.Deg2Rad) * 100);
            case FunctionType.CosFun:
                return new Vector2(x, Mathf.Cos(x * Mathf.Deg2Rad) * 100);
            case FunctionType.LinearFun:
                //y=(-Ax-C)/B
                return new Vector2(x, (-linearFun.A*x-linearFun.C)/linearFun.B )*10;
            default:
                return default(Vector2);
        }
        
    }

    private UIVertex[] GetQuad(Vector2 startPos, Vector2 endPos, Color color0, float lineWidth = 2.0f)
    {
        var dis = Vector2.Distance(startPos, endPos);
        if (dis == 0) dis = 0.001f;
        var y = lineWidth * 0.5f * (endPos.x - startPos.x) / dis;
        var x = lineWidth * 0.5f * (endPos.y - startPos.y) / dis;
        if (y <= 0) y = -y;
        else x = -x;
        var vertex = new UIVertex[4];
        vertex[0].position = new Vector3(startPos.x + x, startPos.y + y);
        //vertex[0].color = Color.red;
        vertex[1].position = new Vector3(endPos.x + x, endPos.y + y);
        vertex[2].position = new Vector3(endPos.x - x, endPos.y - y);
        vertex[3].position = new Vector3(startPos.x - x, startPos.y - y);
        for ( var i = 0 ; i < vertex.Length ; i++ ) vertex[i].color = color0;
        return vertex;
    }

}
