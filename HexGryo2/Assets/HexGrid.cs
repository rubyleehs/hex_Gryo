using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 5f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public static Vector2[] corners =
    {
        new Vector2(-innerRadius, -0.5f * outerRadius),
        new Vector2(-innerRadius, 0.5f * outerRadius),
        new Vector2(0f, outerRadius),
        new Vector2(innerRadius, 0.5f * outerRadius),
        new Vector2(innerRadius, -0.5f * outerRadius),
        new Vector2(0f, -outerRadius),      
    };
}

    public class HexGrid : MonoBehaviour
{

    public int width = 6;
    public int height = 6;

    public float cellPadding;

    public GameObject cellPrefab;
    public HexCell[,] hexCells;
    public Transform cellParent;

    private Camera mainCam;
    private LineRenderer lineRenderer;
    private Vector2 mousePos;
    private Vector3[] linePoints = new Vector3[24];
    private RuneGrid runeGrid;

    private void Awake()
    {
        runeGrid = this.GetComponent<RuneGrid>();
        lineRenderer = this.GetComponent<LineRenderer>();
        hexCells = new HexCell[width, height];
        mainCam = Camera.main;
        mainCam.transform.position =  new Vector3(width * (HexMetrics.innerRadius + cellPadding - 0.5f), height * (HexMetrics.outerRadius + cellPadding - 0.5f) * 0.75f, -10);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, y);
            }
        }
    }


    public void CreateCell(int x, int y)
    {
        Vector3 cellPos = new Vector3(x * (HexMetrics.innerRadius + cellPadding) * 2f, y * (HexMetrics.outerRadius + cellPadding) * 1.5f, 0);
        cellPos.x += (HexMetrics.innerRadius + cellPadding) * (y % 2);


        HexCell cell = hexCells[x, y] = Instantiate(cellPrefab).GetComponent<HexCell>();
        cell.arrayCoords = new Vector2Int(x, y);
        //cell.runeGrid = runeGrid;
        //cell.axialCordinates = HexMetrics.FromOffsetToAxialCoords(x, y);
        //cell.runeID = 0;

        cell.cellNeighbours = new HexCell[6];

        if (x > 0)
        {
            cell.SetNeighbour(HexDirection.W, hexCells[x - 1, y]);
        }
        if (y > 0)
        {
            if (y % 2 == 1)
            {
                cell.SetNeighbour(HexDirection.SW, hexCells[x, y - 1]);
                if (x + 1 < width)
                {
                    cell.SetNeighbour(HexDirection.SE, hexCells[x + 1, y - 1]);
                }
            }
            else
            {
                if (x > 0)
                {
                    cell.SetNeighbour(HexDirection.SW, hexCells[x - 1, y - 1]);
                }
                cell.SetNeighbour(HexDirection.SE, hexCells[x, y - 1]);
            }
        }


        cell.transform.SetParent(cellParent, false);
        cell.transform.localPosition = cellPos;
        cell.transform.localScale = Vector3.one * HexMetrics.outerRadius * 2;
    }

    public Transform FromWorldPosToTile(float x, float y)
    {
        int _y = Mathf.RoundToInt(y / ((HexMetrics.outerRadius + cellPadding) * 1.5f));
        int _x = Mathf.RoundToInt((x - (HexMetrics.innerRadius + cellPadding) * (_y % 2)) / ((HexMetrics.innerRadius + cellPadding) * 2f));

        if (_y >= 0 && _y < height && _x >= 0 && _x < width)
        {
            return hexCells[_x, _y].transform;
        }
        else
        {
            return null;
        }
    }

    public void ShowSelectionPoints(HexCell cell)
    {
        if(cell == null)
        {
            lineRenderer.SetPositions(new Vector3[24]);
        }

        for (int i = 0; i < 6; i++)
        {
            Vector2 cellNeigbourPos = cell.cellNeighbours[i].transform.position;
            linePoints[i * 4] = cellNeigbourPos + HexMetrics.corners[i];
            linePoints[i * 4 + 1] = cellNeigbourPos + HexMetrics.corners[(i + 1) % 6];
            linePoints[i * 4 + 2] = cellNeigbourPos + HexMetrics.corners[(i + 2) % 6];
            linePoints[i * 4 + 3] = cellNeigbourPos + HexMetrics.corners[(i + 3) % 6];
        }

        lineRenderer.SetPositions(linePoints);
    }
}
