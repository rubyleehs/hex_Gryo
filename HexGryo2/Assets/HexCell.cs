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

    [SerializeField]
    public HexCell[] cellNeighbours;

    public Vector2Int arrayCoords;
    public Rune rune;

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
        return (this.rune.runeID == cellNeighbours[(int)direction].rune.runeID && rune.runeID == cellNeighbours[(int)direction.Opposite()].rune.runeID);
    }

    public void CheckDirRuneChain(int direction)
    {
        if (cellNeighbours[direction]!= null && cellNeighbours[(direction + 3) % 6] != null && cellNeighbours[direction].rune != null && cellNeighbours[(direction + 3) % 6].rune != null)
        {
            if(rune == null)
            {
                Debug.Log(arrayCoords);
            }
            if (rune.runeID == cellNeighbours[direction].rune.runeID && rune.runeID == cellNeighbours[(direction + 3) % 6].rune.runeID) 
            {
                this.rune.InChain = true;
                cellNeighbours[direction].rune.InChain = true;
                cellNeighbours[(direction + 3) % 6].rune.InChain = true;
            }
        }
    }

    public HexCell FindRuneNextCell()
    {
        if(arrayCoords.y == 0)
        {
            return this;
        }
        int score = 1;
        if (cellNeighbours[3] != null && cellNeighbours[3].rune == null)
        {
            score *= 3;
        }
        if (cellNeighbours[4] != null && cellNeighbours[4].rune == null)
        {
            score *= 4;
        }
        if(score == 12)
        {
            if (arrayCoords.y > 1 && cellNeighbours[3].cellNeighbours[4].rune == null)
            {
                return cellNeighbours[3].cellNeighbours[4];
            }
            else
            {
                int temp = Random.Range(3, 5);
                rune.RuneRotationIDChange(7 - (temp * 2));
                return cellNeighbours[temp];
            }
        }
        else if(score != 1)
        {
            rune.RuneRotationIDChange(7 - (score * 2));
            return cellNeighbours[score];
        }
        else
        {
            return this;
        }
    }
}
