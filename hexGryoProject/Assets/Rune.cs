using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour {

    public int runeID;
    public RuneGrid runeGrid;
    public HexCell currentCell;

    public HexCell nextCell;
    public bool InChain = false;

    public void RemoveFromGrid()
    {
        InChain = false;
        currentCell.runeID = 0;
        currentCell.CanAcceptRune = true;
        runeID = 0;
        nextCell = null;
        this.gameObject.SetActive(false);
    }

}
