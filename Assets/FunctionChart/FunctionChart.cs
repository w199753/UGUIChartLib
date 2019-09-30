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
using UnityEngine.Profiling;

public class FunctionChart : ChartBase
{
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
        public LinearFun(float a, float b, float c)
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

    [Serializable]
    public struct InverseFun
    {
        public float K;
    }

    /// <summary>
    /// 自定义函数图像
    /// </summary>
    public struct CustomFun
    {

    }

    private enum FunctionType
    {
        SinFun,
        CosFun,
        LinearFun,
        InverseFun
    }

    [SerializeField]
    private FunctionType funType = FunctionType.SinFun;


    [SerializeField]
    private float lineWidth = 2f;
    public float LineWidth
    {
        get { return lineWidth; }
    }

    [SerializeField, Range(1f, 15f)]
    private float lineSmooth = 1f;
    public float LineSmooth
    {
        get { return lineSmooth; }
    }

    [SerializeField]
    private Color m_lineColor = Color.white;
    public Color LineColor
    {
        get { return m_lineColor; }
    }

    [SerializeField, Range(1f, 100f)]
    private float m_baseUnit = 1f;
    public float BaseUnit
    {
        get { return m_baseUnit; }
    }

    public LinearFun linearFun = new LinearFun(0.1f, 0.1f, 0.1f);
    public CosineFun cosFun = new CosineFun();
    public SinFun sinFun = new SinFun();
    public InverseFun inverseFun = new InverseFun();

    public override void ModifyMesh(VertexHelper vh)
    {
        ModifyVertices(vh);
    }

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
            case FunctionType.InverseFun:
                DrawInverseFunChart(vh, funType);
                break;
            default:
                break;
        }
    }

    void DrawSinFunChart(VertexHelper vh, FunctionType type)
    {
        var startPos = GetResult(-width / 2.0f, type) * sinFun.A;
        for (var x = -width / 2.0f + 1; x < width / 2.0f; x += lineSmooth)
        {
            var endPos = GetResult(x, type) * sinFun.A;
            DrawSimpleQuad(vh, GetQuad(startPos + CacheUnit.SetVector(0, sinFun.B), endPos + CacheUnit.SetVector(0, sinFun.B), m_lineColor, lineWidth));
            startPos = endPos;
        }
    }

    void DrawCosFunChart(VertexHelper vh, FunctionType type)
    {
        var startPos = GetResult(-width / 2.0f, type) * cosFun.A;
        for (var x = -width / 2.0f + 1; x < width / 2.0f; x += lineSmooth)
        {
            var endPos = GetResult(x, type) * cosFun.A;
            DrawSimpleQuad(vh, GetQuad(startPos + CacheUnit.SetVector(0, cosFun.B), endPos + CacheUnit.SetVector(0, cosFun.B), m_lineColor, lineWidth));
            startPos = endPos;
        }
    }

    void DrawLinearFunChart(VertexHelper vh, FunctionType type)
    {
        var startPos = GetResult(-width / 2.0f, type);
        for (var x = -width / 2.0f + 1; x < width / 2.0f; x += lineSmooth)
        {
            var endPos = GetResult(x, type);
            DrawSimpleQuad(vh, GetQuad(startPos, endPos, m_lineColor, lineWidth));
            startPos = endPos;
        }
    }

    void DrawInverseFunChart(VertexHelper vh, FunctionType type)
    {
        var startPos = GetResult(-width / 2.0f, type);
        float delta;
        int count = 0;
        for (var x = -width / 2.0f + 1; x < -0.1f; x += lineSmooth * (delta * 0.2f))
        {
            delta = Mathf.Abs(x);
            count++;
            if (count > 1000) break;//最多绘制两千次,防超
            var endPos = GetResult(x, type);
            DrawSimpleQuad(vh, GetQuad(startPos, endPos, m_lineColor, lineWidth));
            startPos = endPos;
        }
        startPos = GetResult(0 + 0.5f, type);
        count = 0;
        for (var x = 0 + 0.1f; x < width / 2.0f; x += lineSmooth * (delta * 0.2f))
        {
            delta = Mathf.Abs(x);
            count++;
            if (count > 1000) break;
            var endPos = GetResult(x, type);
            DrawSimpleQuad(vh, GetQuad(startPos, endPos, m_lineColor, lineWidth));
            startPos = endPos;
        }
    }


    /// <summary>
    /// 通过函数获得结果的坐标点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private Vector3 GetResult(float x, FunctionType type)
    {
        switch (type)
        {
            case FunctionType.SinFun:
                return CacheUnit.SetVector(x, Mathf.Sin(x * Mathf.Deg2Rad) * 100)*BaseUnit;
            case FunctionType.CosFun:
                return CacheUnit.SetVector(x, Mathf.Cos(x * Mathf.Deg2Rad) * 100) * BaseUnit;
            case FunctionType.LinearFun:
                //y=(-Ax-C)/B
                return CacheUnit.SetVector(x, (-linearFun.A * x - linearFun.C) / linearFun.B) * BaseUnit;
            case FunctionType.InverseFun:
                return CacheUnit.SetVector(x, inverseFun.K / x) * BaseUnit;
            default:
                return default(Vector3);
        }

    }


    /// <summary>
    /// 获得需要绘制的四边形属性
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="color0"></param>
    /// <param name="lineWidth"></param>
    /// <returns></returns>
    private DrawAttribute GetQuad(Vector3 startPos, Vector3 endPos, Color color0, float lineWidth = 2.0f)
    {
        var dis = Vector2.Distance(startPos, endPos);
        if (dis == 0) dis = 0.001f;
        var y = lineWidth * 0.5f * (endPos.x - startPos.x) / dis;
        var x = lineWidth * 0.5f * (endPos.y - startPos.y) / dis;
        if (y <= 0) y = -y;
        else x = -x;

        drawAttribute.SetPosition(
            CacheUnit.SetVector(startPos.x + x, startPos.y + y),
            CacheUnit.SetVector(endPos.x + x, endPos.y + y),
            CacheUnit.SetVector(endPos.x - x, endPos.y - y),
            CacheUnit.SetVector(startPos.x - x, startPos.y - y)
            );
        drawAttribute.SetColor(color0, color0, color0, color0);
        return drawAttribute;
    }

}
