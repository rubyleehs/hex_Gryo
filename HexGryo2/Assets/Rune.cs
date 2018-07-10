using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour {

    public int runeID;
    public HexCell currentCell;
    public HexCell nextCell;
    public bool InChain = false;
    public int IsRotating = 0;

    public Color32 currentColor;

    public void StartMoveSetup()
    {
        nextCell = currentCell.FindRuneNextCell();
        currentCell.rune = null;
        nextCell.rune = this;

    }

    public void FinishMoveSetup()
    {
        this.transform.position = nextCell.transform.position;
        nextCell.rune = this;
        currentCell = nextCell;
        nextCell = currentCell.FindRuneNextCell();
    }

    public void RemoveFromGrid()
    {
        if(nextCell == null)
        {
            Debug.Log(this.transform.position);
        }
        InChain = false;
        currentCell.rune = null;
        nextCell.rune = null;

        runeID = 0;
        nextCell = null;
        this.gameObject.SetActive(false);
    }

    public void RuneRotationIDChange(int rot)
    {
        IsRotating = rot;
        if (runeID != 1)
        {
            IsRotating = rot;
            if (runeID % 2 == 0)
            {
                runeID ++;
            }
            else runeID--;
        }
    }
}
