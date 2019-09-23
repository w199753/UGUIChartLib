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

public class RadarChart : BaseMeshEffect
{
    static List<UIVertex> tempVertexTriangleStream = new List<UIVertex>();
    static List<UIVertex> tempInnerVertices = new List<UIVertex>();
    static List<UIVertex> tempOuterVertices = new List<UIVertex>();


    [SerializeField]
    float[] parameters;

    [SerializeField, Range(0, 360f)]
    float startAngleDegree = 0f;
    public float StartAngleDegree
    {
        get { return startAngleDegree; }
        set { startAngleDegree = value; graphic.SetVerticesDirty(); }
    }

    [SerializeField]
    Color outerColor = Color.white;
    public Color OuterColor
    {
        get { return outerColor; }
        set { outerColor = value; graphic.SetVerticesDirty(); }
    }

    [SerializeField, Range(0, 1f)]
    float outerRatio = 1f;
    public float OuterRatio
    {
        get { return outerRatio; }
        set { outerRatio = value; graphic.SetVerticesDirty(); }
    }

    [SerializeField]
    Color innerColor = Color.clear;
    public Color InnerColor
    {
        get { return innerColor; }
        set { innerColor = value; graphic.SetVerticesDirty(); }
    }

    [SerializeField, Range(0, 1f)]
    float innerRatio = 0f;
    public float InnerRatio
    {
        get { return innerRatio; }
        set { innerRatio = value; graphic.SetVerticesDirty(); }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;
        vh.GetUIVertexStream(tempVertexTriangleStream);

        ModifyVertices(tempVertexTriangleStream);
        //print(vh.currentVertCount + " ");
        vh.Clear();

        vh.AddUIVertexTriangleStream(tempVertexTriangleStream);

        tempVertexTriangleStream.Clear();
    }

    private void ModifyVertices(List<UIVertex> vertices)
    {
        //print(vertices.Count);
        if (parameters == null) return;

        if (ChartUntils.NeedsToUpdateCaches(parameters.Length, startAngleDegree))
        {
            ChartUntils.CacheSinesAndCosines(StartAngleDegree);
        }

        //1 get center point
        Vector3 centerPosition = (vertices[0].position + vertices[2].position) / 2f;

        //2 set xUnit and yUnit
        Vector3 xUnit = (centerPosition.x - vertices[0].position.x) * Vector3.right;
        Vector3 yUnit = (centerPosition.y - vertices[0].position.y) * Vector3.up;

        //3 set uv
        Vector2 centerUv = (vertices[0].uv0 + vertices[2].uv0) / 2f;
        Vector2 uUnit = (centerUv.x - vertices[0].uv0.x) * Vector3.right;
        Vector2 vUnit = (centerUv.y - vertices[0].uv0.y) * Vector3.up;

        //3 set color gradual change(any vertex,the result is same)
        Color outerMultipliedColor = ChartUntils.GetMultipliedColor(vertices[0].color, outerColor);
        Color innerMultipledColor = ChartUntils.GetMultipliedColor(vertices[0].color, innerColor);

        for (int i = 0; i < parameters.Length; i++)
        {
            float parmeter = parameters[i];
            float cosine = ChartUntils.cacheCosines[i];
            float sine = ChartUntils.cacheSines[i];

            UIVertex outertmp = vertices[0];
            float outerParmeter = parmeter * outerRatio;
            outertmp.position = centerPosition + (xUnit * cosine + yUnit * sine) * outerParmeter;
            outertmp.uv0 = centerUv + (uUnit * cosine + vUnit * sine) * outerParmeter;
            outertmp.color = outerMultipliedColor;
            tempOuterVertices.Add(outertmp);



            UIVertex innertmp = vertices[0];
            float innerParmeter = parmeter * innerRatio;
            innertmp.position = centerPosition + (xUnit * cosine + yUnit * sine) * innerParmeter;
            innertmp.uv0 = centerUv + (uUnit * cosine + vUnit * sine) * innerParmeter;
            innertmp.color = innerMultipledColor;
            tempInnerVertices.Add(innertmp);

        }

        if (parameters.Length > 0)
        {
            tempOuterVertices.Add(tempOuterVertices[0]);
            tempInnerVertices.Add(tempInnerVertices[0]);
        }

        vertices.Clear();
        if (outerRatio != 0f)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                vertices.Add(tempInnerVertices[i]);
                vertices.Add(tempOuterVertices[i]);
                vertices.Add(tempOuterVertices[i + 1]);

            }
        }
        if (innerRatio != 0f)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                vertices.Add(tempOuterVertices[i + 1]);
                vertices.Add(tempInnerVertices[i + 1]);
                vertices.Add(tempInnerVertices[i]);
            }
        }
        tempOuterVertices.Clear();
        tempInnerVertices.Clear();
    }

    public void SetParameter(int index, float value)
    {
        if (index >= parameters.Length) return;
        parameters[index] = value;
        graphic.SetVerticesDirty();
    }
    public void SetParameters(float[] values) { parameters = values; graphic.SetVerticesDirty(); }
    public float GetParameter(int index) { return index >= parameters.Length ? default(float) : parameters[index]; }
    public IEnumerable<float> GetParameters() { return parameters; }

}

public class ChartUntils
{
    public static float? cacheStartAngleDegree = null;
    public static List<float> cacheSines = new List<float>();
    public static List<float> cacheCosines = new List<float>();
    
    public static int parameterCount;
    public static bool NeedsToUpdateCaches(int count, float startAngleDegree)
    {
        parameterCount = count;
        return !cacheStartAngleDegree.HasValue ||
 cacheStartAngleDegree.Value != startAngleDegree ||
 cacheCosines.Count != count;
    }
    public static void CacheSinesAndCosines(float startAngleDegree)
    {
        cacheSines.Clear();
        cacheCosines.Clear();

        float startAngleRadian = (90f - startAngleDegree) / 180f * (float)Math.PI;
        float unitRadian = -2f * (float)Math.PI / (float)parameterCount;

        for (int i = 0; i < parameterCount; i++)
        {
            float radian = startAngleRadian + (float)i * unitRadian;
            cacheSines.Add(Mathf.Sin(radian));
            cacheCosines.Add(Mathf.Cos(radian));
        }
        cacheStartAngleDegree = startAngleDegree;
    }
    public static Color32 GetMultipliedColor(Color32 color1, Color32 color2)
    {
        return new Color32(
            (Byte)(color1.r * color2.r / 255),
            (Byte)(color1.g * color2.g / 255),
            (Byte)(color1.b * color2.b / 255),
            (Byte)(color1.a * color2.a / 255));
    }

    public static float[] GetKeyArrayMaxxAndMaxy(List<Vector2> keyArray)
    {
        float maxx = -9999;
        float maxy = -9999;
        foreach (var item in keyArray)
        {
            if (item.x > maxx)
            {
                maxx = item.x;
            }
            if (item.y > maxy)
                maxy = item.y;
        }
        return new float[2] { maxx, maxy };
    }
}


