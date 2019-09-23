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
public abstract class ChartBase : BaseMeshEffect {

    protected float Kx, Ky;
    protected float width, height;

    protected QuadAttribute quadattribute = new QuadAttribute();
    protected DrawUnit dd = new DrawUnit();

    protected static List<UIVertex> tempVertexTriangleStream = new List<UIVertex>();

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        width = graphic.rectTransform.sizeDelta.x;
        height = graphic.rectTransform.sizeDelta.y;

        Kx = width * 0.5f;
        Ky = height * 0.5f;//scale ratio
    }




}
