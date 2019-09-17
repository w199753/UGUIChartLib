/*
	author：@
	Last modified data:
	funtion todo:
        shoud set pivot at new Vector2(0 0);

        add and set Keypos to BG:
        line.AddKeyPosition(new Vector2(5, 5)).Add(new Vector2(6, 6));
        line.CopyValueToBG(line.KeyPosList,"lineChartBg");
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LineChart : BaseMeshEffect
{
    private static List<UIVertex> tempVertexTriangleStream = new List<UIVertex>();

    [SerializeField]
    private List<Vector2> keyPos = new List<Vector2>();
    public List<Vector2> KeyPosList
    {
        get { return keyPos; }
    }


    [SerializeField, Range(0.1f, 1)]
    private float lineWidth = 1;
    public float LineWidth
    {
        get { return lineWidth * 0.2f; }
        set { lineWidth = value; /*graphic.SetVerticesDirty();*/ }
    }

    [SerializeField]
    private Color pointColor = Color.white;
    public Color PointColor
    {
        get { return pointColor; }
        set { pointColor = value; graphic.SetVerticesDirty(); }
    }


    [SerializeField]
    private Color lineColor = Color.white;
    public Color LineColor
    {
        get { return lineColor; }
        set { lineColor = value; graphic.SetVerticesDirty(); }
    }
    float x, y;
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
        UIVertex[] tmpVertex = new UIVertex[4];
        vh.GetUIVertexStream(tempVertexTriangleStream);
        vh.Clear();

        #region 画圆测试
        //test=================画圆
        /*
        List<UIVertex> vertex = new List<UIVertex>();
        UIVertex zz = new UIVertex();
        zz.position = new Vector3(0, 0, 0);
        float R = 100;
        for (float i = 0; i < 2*Mathf.PI; i+=0.2f)
        {
            UIVertex tt = new UIVertex();
            tt.position = new Vector3(0, 0, 0);
            tt.color = Color.white;
            vertex.Add(tt);

            tt.position = new Vector3(R * Mathf.Cos(i + 0.2f), R * Mathf.Sin(i + 0.2f));
            tt.color = Color.white;
            vertex.Add(tt);

            tt.position = new Vector3(R * Mathf.Cos(i), R * Mathf.Sin(i));
            tt.color = Color.white;
            vertex.Add(tt);
        }
        vh.AddUIVertexTriangleStream(vertex);*/
        #endregion
        float[] result = ChartUntils.GetKeyArrayMaxxAndMaxy(keyPos);
        float maxx = result[0], maxy = result[1];

        float Kx = x / maxx, Ky = y / maxy;

        for (int i = 0; i < keyPos.Count - 1; i++)
        {
            tmpVertex[0].position = new Vector2(keyPos[i].x * Kx, keyPos[i].y * Ky);
            tmpVertex[0].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor);

            tmpVertex[1].position = new Vector2(keyPos[i + 1].x * Kx, keyPos[i + 1].y * Ky);
            tmpVertex[1].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor);

            tmpVertex[2].position = new Vector2(keyPos[i + 1].x * Kx, (keyPos[i + 1].y - LineWidth) * Ky);
            tmpVertex[2].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor);

            tmpVertex[3].position = new Vector2(keyPos[i].x * Kx, (keyPos[i].y - LineWidth) * Ky);
            tmpVertex[3].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, LineColor);

            vh.AddUIVertexQuad(tmpVertex);
        }

        float newLineWidth = LineWidth;
        //画点
        for (int i = 0; i < keyPos.Count; i++)
        {
            tmpVertex[0].position = new Vector3((keyPos[i].x * Kx - newLineWidth - Kx * Ky / 1000f), (keyPos[i].y * Ky + newLineWidth + Kx * Ky / 1000f), 0);
            tmpVertex[0].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor);

            tmpVertex[1].position = new Vector3((keyPos[i].x * Kx + newLineWidth + Kx * Ky / 1000f), (keyPos[i].y * Ky + newLineWidth + Kx * Ky / 1000f), 0);
            tmpVertex[1].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor);

            tmpVertex[2].position = new Vector3((keyPos[i].x * Kx + newLineWidth + Kx * Ky / 1000f), (keyPos[i].y * Ky - newLineWidth - Kx * Ky / 1000f), 0);
            tmpVertex[2].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor);

            tmpVertex[3].position = new Vector3((keyPos[i].x * Kx - newLineWidth - Kx * Ky / 1000f), (keyPos[i].y * Ky - newLineWidth - Kx * Ky / 1000f), 0);
            tmpVertex[3].color = ChartUntils.GetMultipliedColor(tempVertexTriangleStream[0].color, PointColor);

            vh.AddUIVertexQuad(tmpVertex);
        }
    }

    public void CopyValueToBG(List<Vector2> pos, string targetName)
    {
        GameObject go = GameObject.Find(targetName);
        if (go == null)
        {
            Debug.Log("please check targetName");
            return;
        }
        go.GetComponent<LineChartBG>().keyPos = new List<Vector2>(pos);
    }

    public LineChart AddKeyPosition(Vector2 v)
    {
        this.keyPos.Add(v);
        return this;
    }

    public void SortValueByAscendOrder()
    {
        keyPos.Sort(
            (a, b) => { return a.x.CompareTo(a.x); }
            );
        keyPos.Sort(
            (a, b) => { return a.y.CompareTo(a.y); }
            );
    }

    public void SortValueByDescendOrder()
    {
        keyPos.Sort(
            (a, b) => { return -a.x.CompareTo(a.x); }
            );
        keyPos.Sort(
            (a, b) => { return -a.y.CompareTo(a.y); }
            );
    }


}
