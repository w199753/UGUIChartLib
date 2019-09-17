/*
	author：@
	Last modified data:
	funtion todo:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public RadarChart rd;
    public LineChart line;
    public PieChart pie;
	// Use this for initialization
	void Start () {
        line.AddKeyPosition(new Vector2(5, 5)).AddKeyPosition(new Vector2(6, 6));
        line.CopyValueToBG(line.KeyPosList,"lineChartBg");

        pie.AddValue(new PieInfo("输出", Color.white, 10)).
            AddValue(new PieInfo("防御", Color.yellow, 15)).
            AddValue(new PieInfo("治疗", Color.green, 40)).
            AddValue(new PieInfo("拆塔",Color.blue,35));

           
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.A))
        {
            rd.SetParameter(0, 0.5f);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            rd.SetParameter(1, 0.6f);
        }
        if (Input.GetKey(KeyCode.C))
        {
            rd.StartAngleDegree = 30;
        }
    }
}
