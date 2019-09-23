/*
	author：@
	Last modified data:
	funtion todo:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
public class Test : MonoBehaviour {
    public RadarChart rd;
    public LineChart line;
    public PieChart pie;

    public Text tt;
	// Use this for initialization
	void Start () {
        line.AddKeyPosition(new Vector2(5, 5)*100).AddKeyPosition(new Vector2(6, 6)*100);
        line.CopyValueToBG(line.KeyPosList,"lineChartBg");

        pie.AddValue(new PieInfo("输出", Color.white, 1)).
            AddValue(new PieInfo("防御", Color.yellow, 15)).
            AddValue(new PieInfo("治疗", Color.green, 49)).
            AddValue(new PieInfo("拆塔",Color.blue,35));

        print(Mathf.PI / 180+" "+Mathf.Deg2Rad);
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

