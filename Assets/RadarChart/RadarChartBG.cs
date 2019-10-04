/*
 * UGUIChartLib
 * Copyright © 2019 w199753. 
 * feedback:http://15384855139@163.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartLib
{

    public class RadarChartBG : BaseMeshEffect
    {
        static List<UIVertex> tempVertexTriangleStream = new List<UIVertex>();
        static List<UIVertex> tempInnerVertices = new List<UIVertex>();
        static List<UIVertex> tempOuterVertices = new List<UIVertex>();


        [SerializeField]
        int parametersCount;

        [SerializeField, Range(1, 20)]
        int cricleCount = 1;
        public int CricleCount
        {
            get { return cricleCount; }
            set { cricleCount = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField, Range(0, 1f)]
        float cricleDisInterval = 0f;
        public float CricleDisInterval
        {
            get { return cricleDisInterval; }
            set { cricleDisInterval = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField, Range(0, 360f)]
        float startAngleDegree = 0f;
        public float StartAngleDegree
        {
            get { return startAngleDegree; }
            set { startAngleDegree = value; graphic.SetVerticesDirty(); }
        }

        private float defaultCurde = 1f;
        [SerializeField, Range(0, 1f)]
        float lineCurde;
        public float LineCurde
        {
            get { return lineCurde; }
            set { lineCurde = value; graphic.SetVerticesDirty(); }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;
            vh.GetUIVertexStream(tempVertexTriangleStream);

            ModifyVertices(tempVertexTriangleStream);

            vh.Clear();

            vh.AddUIVertexTriangleStream(tempVertexTriangleStream);

            tempVertexTriangleStream.Clear();
        }

        private void ModifyVertices(List<UIVertex> vertices)
        {
            if (parametersCount == 0) return;

            if (ChartUntils.NeedsToUpdateCaches(parametersCount, startAngleDegree))
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

            UIVertex startVertex = vertices[0];
            vertices.Clear();
            for (int j = 0; j < cricleCount; j++)
            {
                for (int i = 0; i < parametersCount; i++)
                {
                    float parmeter = 1f;
                    float cosine = ChartUntils.cacheCosines[i];
                    float sine = ChartUntils.cacheSines[i];

                    UIVertex outertmp = startVertex;
                    float outerParmeter = parmeter * (defaultCurde - j * cricleDisInterval * 0.15f);
                    outertmp.position = centerPosition + (xUnit * cosine + yUnit * sine) * outerParmeter;
                    outertmp.uv0 = centerUv + (uUnit * cosine + vUnit * sine) * outerParmeter;
                    tempOuterVertices.Add(outertmp);

                    UIVertex innertmp = startVertex;
                    float innerParmeter = parmeter * ((defaultCurde - lineCurde * 0.1f) - j * cricleDisInterval * 0.15f);
                    innertmp.position = centerPosition + (xUnit * cosine + yUnit * sine) * innerParmeter;
                    innertmp.uv0 = centerUv + (uUnit * cosine + vUnit * sine) * innerParmeter;
                    tempInnerVertices.Add(innertmp);

                }

                if (parametersCount > 0)
                {
                    tempOuterVertices.Add(tempOuterVertices[0]);
                    tempInnerVertices.Add(tempInnerVertices[0]);
                }

                if (defaultCurde != 0f)
                {
                    for (int i = 0; i < parametersCount; i++)
                    {
                        vertices.Add(tempInnerVertices[i]);
                        vertices.Add(tempOuterVertices[i]);
                        vertices.Add(tempOuterVertices[i + 1]);

                    }
                }
                if ((defaultCurde - lineCurde * 0.1f) != 0f)
                {
                    for (int i = 0; i < parametersCount; i++)
                    {
                        vertices.Add(tempOuterVertices[i + 1]);
                        vertices.Add(tempInnerVertices[i + 1]);
                        vertices.Add(tempInnerVertices[i]);
                    }
                }
                tempOuterVertices.Clear();
                tempInnerVertices.Clear();
            }
        }

        public void SetParameterCount(int c)
        {
            cricleCount = c;
        }

    }

}

