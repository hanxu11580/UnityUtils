using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 灵活网格布局
/// </summary>
public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        None,
        RowMajor,
        ColumnMajor,
        FixedRow,
        FixedColumn
    }
    public FitType fitType;
    // 行
    public int rows;
    // 列
    public int columns;
    // 一个格子有多大
    public Vector2 cellSize;
    // 间隔
    public Vector2 spacing;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        // 确定网格Size
        float sqrRt = Mathf.Sqrt(transform.childCount);
        rows = Mathf.CeilToInt(sqrRt);
        columns = Mathf.CeilToInt(sqrRt);
        //根据优先级进行调整
        if(fitType == FitType.RowMajor)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns); 
        }else if(fitType == FitType.ColumnMajor)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }else if(fitType == FitType.FixedRow)
        {
            rows = 1;
            columns = transform.childCount;
        }
        else if(fitType == FitType.FixedColumn)
        {
            rows = transform.childCount;
            columns = 1;
        }


        // 布局宽高
        float parentWid = rectTransform.rect.width;
        float parentHei = rectTransform.rect.height;
        // 进行分割得到单个cell尺寸
        cellSize.x = (parentWid - spacing.x - padding.left - padding.right) / (float)columns;
        cellSize.y = (parentHei - spacing.y - padding.top - padding.bottom) / (float)rows;

        /*
         * 想象成一个横纵都是以0开始的网格
         * 下面计算的就是算当前这个子物体在哪个网格的坐标
         */
        int rowIdx;
        int columnIdx;
        for (int i = 0,cCount = transform.childCount; i < cCount; i++)
        {
            rowIdx = i / columns;
            columnIdx = i % columns; 
            var item = rectChildren[i];
            var xPos = (cellSize.x + spacing.x) * columnIdx;
            var yPos = (cellSize.y + spacing.y) * rowIdx;
            //然后边缘位置，直接进行平移
            xPos += padding.left;
            yPos += padding.top;
            // axis = 0水平 1垂直
            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }

    }

    public override void CalculateLayoutInputVertical()
    {
        
    }

    public override void SetLayoutHorizontal()
    {
        
    }

    public override void SetLayoutVertical()
    {
        
    }
}
