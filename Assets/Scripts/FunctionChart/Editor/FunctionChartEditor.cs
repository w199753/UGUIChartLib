/*
	author：@
	Last modified data:
	funtion todo:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChartLib
{
    [CustomEditor(typeof(FunctionChart))]
    public class FunctionChartEditor : Editor
    {
        private FunctionChart m_functionChart;
        private SerializedObject obj;

        private SerializedProperty lineWidth;
        private SerializedProperty lineSmooth;
        private SerializedProperty lineColor;
        private SerializedProperty baseUnit;
        private SerializedProperty funType;

        private SerializedProperty customFun;
        private SerializedProperty customFunFormula;

        private SerializedProperty lineFun;
        private SerializedProperty lineFunA;
        private SerializedProperty lineFunB;
        private SerializedProperty lineFunC;

        private SerializedProperty sinFun;
        private SerializedProperty sinFunA;
        private SerializedProperty sinFunB;

        private SerializedProperty cosFun;
        private SerializedProperty cosFunA;
        private SerializedProperty cosFunB;

        private SerializedProperty inverseFun;
        private SerializedProperty inverseFunK;

        GUIStyle style = new GUIStyle();
        void OnEnable()
        {
            m_functionChart = target as FunctionChart;
            obj = new SerializedObject(target);

            lineWidth = obj.FindProperty("m_lineWidth");
            lineSmooth = obj.FindProperty("m_lineSmooth");
            baseUnit = obj.FindProperty("m_baseUnit");
            lineColor = obj.FindProperty("m_lineColor");
            funType = obj.FindProperty("m_funType");

            customFun = obj.FindProperty("customFun");
            customFunFormula = customFun.FindPropertyRelative("formula");

            lineFun = obj.FindProperty("linearFun");
            lineFunA = lineFun.FindPropertyRelative("A");
            lineFunB = lineFun.FindPropertyRelative("B");
            lineFunC = lineFun.FindPropertyRelative("C");

            sinFun = obj.FindProperty("sinFun");
            sinFunA = sinFun.FindPropertyRelative("A");
            sinFunB = sinFun.FindPropertyRelative("B");

            cosFun = obj.FindProperty("cosFun");
            cosFunA = cosFun.FindPropertyRelative("A");
            cosFunB = cosFun.FindPropertyRelative("B");

            inverseFun = obj.FindProperty("inverseFun");
            inverseFunK = inverseFun.FindPropertyRelative("K");

        }
        public override void OnInspectorGUI()
        {
            obj.Update();
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(lineWidth);
            EditorGUILayout.PropertyField(lineSmooth);
            EditorGUILayout.PropertyField(lineColor);
            EditorGUILayout.PropertyField(baseUnit);
            EditorGUILayout.PropertyField(funType);

            style.fontStyle = FontStyle.Bold;

            switch (m_functionChart.FunType)
            {
                case FunctionType.SinFun:
                    EditorGUILayout.LabelField("Asin(x)+B",style);
                    EditorGUILayout.PropertyField(sinFunA);
                    EditorGUILayout.PropertyField(sinFunB);
                    break;
                case FunctionType.CosFun:
                    EditorGUILayout.LabelField("Acos(x)+B", style);
                    EditorGUILayout.PropertyField(cosFunA);
                    EditorGUILayout.PropertyField(cosFunB);
                    break;
                case FunctionType.LinearFun:
                    EditorGUILayout.LabelField("(-Ax-C)/B", style);
                    EditorGUILayout.PropertyField(lineFunA);
                    EditorGUILayout.PropertyField(lineFunB);
                    EditorGUILayout.PropertyField(lineFunC);
                    break;
                case FunctionType.InverseFun:
                    EditorGUILayout.LabelField("K/x", style);
                    EditorGUILayout.PropertyField(inverseFunK);
                    break;
                case FunctionType.CustomFun:
                    EditorGUILayout.LabelField("Custom formula:", style);
                    EditorGUILayout.PropertyField(customFunFormula);
                    break;
                default:
                    break;
            }
            EditorUtility.SetDirty(target);
            obj.ApplyModifiedProperties();

        }
    }
}

