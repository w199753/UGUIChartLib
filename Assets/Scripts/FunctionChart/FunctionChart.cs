/*
 * UGUIChartLib
 * Copyright © 2019 w199753. 
 * feedback:http://15384855139@163.com
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Profiling;


namespace ChartLib
{
    public enum FunctionType
    {
        SinFun,
        CosFun,
        LinearFun,
        InverseFun,
        CustomFun
    }
    public class FunctionChart : ChartBase
    {

        [Serializable]
        //Ax+By+C=0;     y=(-Ax-C)/B
        public struct LinearFun
        {
            [Range(-500f, 500f), SerializeField]
            public float A;
            [Range(-500f, 500f), SerializeField]
            public float B;
            [Range(-500f, 500f), SerializeField]
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
            [Range(-50f, 50f), SerializeField]
            public float A;
            [Range(-500f, 500f), SerializeField]
            public float B;
        }

        [Serializable]
        //Asin(x)+B
        public struct SinFun
        {
            [Range(-50f, 50f), SerializeField]
            public float A;
            [Range(-500f, 500f), SerializeField]
            public float B;
        }

        [Serializable]
        public struct InverseFun
        {
            [Range(-500f, 500f), SerializeField]
            public float K;
        }

        /// <summary>
        /// 自定义函数图像
        /// </summary>
        [Serializable]
        public struct CustomFun
        {
            [SerializeField]
            public string formula;
        }


        [SerializeField,HideInInspector]
        private FunctionType m_funType = FunctionType.SinFun;
        public FunctionType FunType
        {
            get { return m_funType; }
            set { m_funType = value; }
        }

        [SerializeField, HideInInspector]
        private float m_lineWidth = 2f;
        public float LineWidth
        {
            get { return m_lineWidth; }
        }

        [SerializeField, Range(1f, 15f), HideInInspector]
        private float m_lineSmooth = 1f;
        public float LineSmooth
        {
            get { return m_lineSmooth; }
        }

        [SerializeField, HideInInspector]
        private Color m_lineColor = Color.white;
        public Color LineColor
        {
            get { return m_lineColor; }
        }

        [SerializeField, Range(1f, 100f), HideInInspector]
        private float m_baseUnit = 1f;
        public float BaseUnit
        {
            get { return m_baseUnit; }
        }

        [SerializeField,HideInInspector]
        public LinearFun linearFun = new LinearFun(0.1f, 0.1f, 0.1f);

        [SerializeField, HideInInspector]
        public CosineFun cosFun = new CosineFun();

        [SerializeField, HideInInspector]
        public SinFun sinFun = new SinFun();

        [SerializeField, HideInInspector]
        public InverseFun inverseFun = new InverseFun();

        [SerializeField, HideInInspector]
        public CustomFun customFun = new CustomFun();

        public override void ModifyMesh(VertexHelper vh)
        {
            ModifyVertices(vh);
        }


        float centerX, centerY;
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            RectTransform trans = graphic.rectTransform;

            centerX = width * trans.pivot.x;
            centerY = height * trans.pivot.y;

            //print(Screen.height);
        }
        private void ModifyVertices(VertexHelper vh)
        {
            vh.Clear();

            switch (m_funType)
            {
                case FunctionType.SinFun:
                    DrawSinFunChart(vh, m_funType);
                    break;
                case FunctionType.CosFun:
                    DrawCosFunChart(vh, m_funType);
                    break;
                case FunctionType.LinearFun:
                    DrawLinearFunChart(vh, m_funType);
                    break;
                case FunctionType.InverseFun:
                    DrawInverseFunChart(vh, m_funType);
                    break;
                case FunctionType.CustomFun:
                    DrawCustomFunChart(vh, m_funType);
                   /* for (int i = 0; i < 10; i++)
                    {
                        GetResult(i, m_funType);
                    }*/

                    break;
                default:
                    break;
            }
        }

        void DrawSinFunChart(VertexHelper vh, FunctionType type)
        {
            float startX = -width / 2.0f;

            var startPos = SetCullingArea(ref startX, type) * sinFun.A;

            for (var x = startX; x < width / 2.0f; x += m_lineSmooth)
            {
                var endPos = GetResult(x, type) * sinFun.A;
                if (GetCullingArea(endPos)) continue;

                DrawSimpleQuad(vh, GetQuad(startPos + CacheUnit.SetVector(0, sinFun.B), endPos + CacheUnit.SetVector(0, sinFun.B), m_lineColor, m_lineWidth));
                startPos = endPos;
            }
        }

        void DrawCosFunChart(VertexHelper vh, FunctionType type)
        {
            float startX = -width / 2.0f;

            var startPos = SetCullingArea(ref startX, type) * cosFun.A;

            for (var x = startX; x < width / 2.0f; x += m_lineSmooth)
            {
                var endPos = GetResult(x, type) * cosFun.A;
                if (GetCullingArea(endPos)) continue;

                DrawSimpleQuad(vh, GetQuad(startPos + CacheUnit.SetVector(0, cosFun.B), endPos + CacheUnit.SetVector(0, cosFun.B), m_lineColor, m_lineWidth));
                startPos = endPos;
            }
        }

        void DrawLinearFunChart(VertexHelper vh, FunctionType type)
        {
            float startX = -width / 2.0f;

            var startPos = SetCullingArea(ref startX, type);

            for (var x = startX; x < width / 2.0f; x += m_lineSmooth)
            {
                var endPos = GetResult(x, type);
                if (GetCullingArea(endPos)) continue;

                DrawSimpleQuad(vh, GetQuad(startPos, endPos, m_lineColor, m_lineWidth));
                startPos = endPos;
            }
        }

        void DrawInverseFunChart(VertexHelper vh, FunctionType type)
        {
            float startX = -width / 2.0f;
            float delta;
            int count = 0;

            var startPos = SetCullingArea(ref startX, type);

            for (var x = startX; x < -0.1f; x += m_lineSmooth * (delta * 0.2f))
            {
                delta = Mathf.Abs(x);
                count++;
                if (count > 600) break;//最多绘制两千次,防无限绘制y方向
                var endPos = GetResult(x, type);
                if (GetCullingArea(endPos)) continue;

                DrawSimpleQuad(vh, GetQuad(startPos, endPos, m_lineColor, m_lineWidth));
                startPos = endPos;
            }

            startX = 0 + 0.1f;
            startPos = SetCullingArea(ref startX, type);
            count = 0;

            for (var x = startX; x < width / 2.0f; x += m_lineSmooth* (delta * 0.2f))
            {
                delta = Mathf.Abs(x);
                count++;
                if (count > 600) break;
                var endPos = GetResult(x, type);

                if (GetCullingArea(endPos)) continue;

                DrawSimpleQuad(vh, GetQuad(startPos, endPos, m_lineColor, m_lineWidth));
                startPos = endPos;
            }
        }

        void DrawCustomFunChart(VertexHelper vh, FunctionType type)
        {
            //Profiler.BeginSample("123123123");
            float startX = -width / 2.0f;
            var startPos = SetCullingArea(ref startX, type);

            //print(height - centerY + " " + -centerY);
            //print(width - centerX + " " + -centerX);
            for (var x = startX; x < width / 2.0f; x += m_lineSmooth * 0.2f)
            {
                var endPos = GetResult(x, type);
                if (GetCullingArea(endPos)) continue;
                //print(startPos.y+" "+ endPos.y+"....."+startX);
                DrawSimpleQuad(vh, GetQuad(startPos, endPos, m_lineColor, m_lineWidth));
                startPos = endPos;
            }
            //Profiler.EndSample();
        }

        private Vector3 SetCullingArea(ref float startX, FunctionType type)
        {
            float add = 0.1f;
            Vector2 tmpStartPos = GetResult(startX, type);
            while (tmpStartPos.y > height - centerY || tmpStartPos.y < -centerY)
            {
                if (add >= width / 2.0f - 1) break;//防死循环
                tmpStartPos = GetResult(startX + add, type);

                add += 0.1f;
            }
            //print(tmpStartPos + "......................"+add+"................."+startX);
            startX += (add - 0.1f);
            return tmpStartPos;
        }

        private bool GetCullingArea(Vector2 pos)
        {
            if (pos.y > height - centerY || pos.y < -centerY)
                return true;
            if (pos.x > width - centerX || pos.x < -centerX)
                return true;
            return false;
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
                    return CacheUnit.SetVector(x, Mathf.Sin(x * Mathf.Deg2Rad) * 100) * BaseUnit;
                case FunctionType.CosFun:
                    return CacheUnit.SetVector(x, Mathf.Cos(x * Mathf.Deg2Rad) * 100) * BaseUnit;
                case FunctionType.LinearFun:
                    //y=(-Ax-C)/B
                    return CacheUnit.SetVector(x, (-linearFun.A * x - linearFun.C) / linearFun.B) * BaseUnit;
                case FunctionType.InverseFun:
                    return CacheUnit.SetVector(x, inverseFun.K / x) * BaseUnit;
                case FunctionType.CustomFun:
                    //exp = new Expression();
                    exp.SetFormula(customFun.formula);

                    return CacheUnit.SetVector(x, exp.GetReslut(x)) * BaseUnit;
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


        private Expression exp = new Expression();

    }
}
