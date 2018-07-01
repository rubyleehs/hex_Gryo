using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 5f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public static Vector3[] corners =
    {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
        };
}

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;

    public float cellPadding;

    public GameObject cellPrefab;
    public HexCell[,] hexCells;

    private void Awake()
    {
        hexCells = new HexCell[width , height];

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
        Vector3 cellPos = new Vector3(x * (HexMetrics.innerRadius+cellPadding) * 2f, y * (HexMetrics.outerRadius +cellPadding) * 1.5f , 0);
        cellPos.x += (HexMetrics.innerRadius+cellPadding) * (y % 2);


        HexCell cell = hexCells[x,y] = Instantiate(cellPrefab).GetComponent<HexCell>();
        cell.axialCordinates = FromOffsetToAxialCoords(x, y);

        cell.transform.SetParent(this.transform, false);
        cell.transform.localPosition = cellPos;
        cell.transform.localScale = Vector3.one * HexMetrics.outerRadius*2;
    }


    public static Vector2Int FromOffsetToAxialCoords(int x, int y)
    {
        return new Vector2Int(x - y / 2, y);
    }

    public static Vector2Int FromAxialToOffsetCoords(int x, int y)
    {
        return new Vector2Int(x + y *2, y);
    }

    public List<HexCell> GetSurroundingCells(int radius, Vector2Int center)//returns in axial coords;
    {
        List<HexCell> surroundingCells = new List<HexCell>();

        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                int x = center.x + dx;
                int y = center.y + dy;
                
                if (y >= 0 && y < height && x + y / 2 >= 0 && x + y / 2 < width && Mathf.Abs(dx + dy)<= radius)
                {
                    surroundingCells.Add(hexCells[x + y/2 , y]);
                }
            }
        }

        return surroundingCells;
    }


    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector2 mousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(0);
            

            Transform tileClicked = FromWorldPosToTile(mousePos.x, mousePos.y);

            //if (tileClicked == null) return;

            List<HexCell> surroundCells = GetSurroundingCells(2, tileClicked.GetComponent<HexCell>().axialCordinates);

            for (int i = 0; i < surroundCells.Count; i++)
            {
               // if (surroundCells[i] == null) return;

               surroundCells[i].GetComponent<SpriteRenderer>().color = Color.red;
            }

            tileClicked.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    public Transform FromWorldPosToTile(float x, float y)
    {
        int _y = Mathf.RoundToInt(y / ((HexMetrics.outerRadius + cellPadding) * 1.5f));
        int _x = Mathf.RoundToInt((x - (HexMetrics.innerRadius + cellPadding) * (_y % 2)) / ((HexMetrics.innerRadius + cellPadding) * 2f));
        
        if(_y >= 0 && _y < height && _x >= 0 && _x < width)
        {
            return hexCells[_x  , _y].transform;
        }
        else
        {
            return null;
        }
    }
}