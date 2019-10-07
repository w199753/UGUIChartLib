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
        void OnEnable()
        {
            Debug.Log(1231);
        }
    }
}

