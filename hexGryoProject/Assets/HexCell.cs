using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
    NW, NE, E, SE, SW, W
}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction)//This only aplies if its a new thing you made, for ints and stuff you need to overide it; i think.
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }
}

public class HexCell : MonoBehaviour {

    public Vector2Int axialCordinates;
    public Rune rune;
    public int runeID;
    public bool CanAcceptRune = true;

    [SerializeField]
    public HexCell[] cellNeighbours;
    public RuneGrid runeGrid;


    public void Start()
    {
        
    }
    public HexCell GetNeighbour(HexDirection dir)
    {
        return cellNeighbours[(int)dir];
    }

    public void SetNeighbour(HexDirection direction, HexCell cell)
    {
        cellNeighbours[(int)direction] = cell;
        cell.cellNeighbours[(int)direction.Opposite()] = this;//notice the opposite(). this is possible due to HexDirExtensions;
    }

    public void CheckAllAdjRuneChain()
    {
        for (int i = 0; i < 3; i++)
        {
            CheckDirRuneChain(i);
        }

    }

    public bool CheckDirRuneChain(HexDirection direction)
    {
        return (this.runeID == cellNeighbours[(int)direction].runeID && this.runeID == cellNeighbours[(int)direction.Opposite()].runeID);
    }

    public void CheckDirRuneChain(int direction)
    {
        if (axialCordinates.y != 0 || direction == 2 || direction == 5)
        {
            if (this.runeID == cellNeighbours[direction].runeID && this.runeID == cellNeighbours[(direction + 3) % 6].runeID)
            {
                this.rune.InChain = true;
                cellNeighbours[direction].rune.InChain = true;
                cellNeighbours[(direction + 3) % 6].rune.InChain = true;
            }
        }
    }

    public void UpdateRunes()
    {
        if (rune != null && rune.currentCell == rune.nextCell)
        {
            rune.nextCell = FindRuneNextPos();
            if (rune.currentCell != rune.nextCell)
            {
                rune.currentCell.CanAcceptRune = true;
                rune.nextCell.CanAcceptRune = false;

            }
        }
    }

    public void UpdateRunesAbove()
    {
        for (int i = 0; i < 2; i++)
        {
            if(cellNeighbours[i] != null)
            {
                UpdateRunes();
            }
        }
    }


    public HexCell FindRuneNextPos()
    {
        if (axialCordinates.y == 0) return this.GetComponent<HexCell>();

        if (cellNeighbours[3].CanAcceptRune && cellNeighbours[4].CanAcceptRune)
        {
            if (cellNeighbours[3].cellNeighbours[4] != null && cellNeighbours[3].cellNeighbours[4].CanAcceptRune)
            {
                return cellNeighbours[3].cellNeighbours[4];
            }
            else
            {
                return cellNeighbours[(int)Random.Range(3, 5)];
            }
        }
        else if (cellNeighbours[3].CanAcceptRune)
        {
            return cellNeighbours[3];
        }
        else if (cellNeighbours[4].CanAcceptRune)
        {
            return cellNeighbours[4];
        }
        else return this;
    }
}
