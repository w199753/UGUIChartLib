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
public class Test : MonoBehaviour
{
    public RadarChart rd;
    public LineChart line;
    public PieChart pie;
    public BarChart bar;

    public Text tt;
    // Use this for initialization
    void Start()
    {
        if (line)
        {
            line.AddKeyPosition(new Vector2(5, 5) * 100).AddKeyPosition(new Vector2(6, 6) * 100);
            line.CopyValueToBG(line.KeyPosList, "lineChartBg");
        }

        if (pie)
        {
            pie.AddValue(new PieInfo("输出", Color.white, 1)).
                AddValue(new PieInfo("防御", Color.yellow, 15)).
                AddValue(new PieInfo("治疗", Color.green, 49)).
                AddValue(new PieInfo("拆塔", Color.blue, 35));
        }
        if(bar)
        {
            BarInfo bb1 = new BarInfo("123", 2);
            bb1.SetGroupItem(new PieInfo("小弟", Color.yellow, 500));
            bb1.SetGroupItem(new PieInfo("大哥", Color.blue, 200));

            BarInfo bb2 = new BarInfo("1234", 2);
            bb2.SetGroupItem(new PieInfo("小弟", Color.yellow, 500));
            bb2.SetGroupItem(new PieInfo("大哥", Color.blue, 200));

            bar.AddValue(bb1).AddValue(bb2);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
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

