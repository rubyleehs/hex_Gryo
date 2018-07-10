using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneGrid : MonoBehaviour {

    public Sprite[] runeSprites;
    public GameObject rune;
    public HexGrid hexGrid;

    public List<Rune> runesList= new List<Rune>();
    public List<HexCell> recheckList = new List<HexCell>();
    public float movePeriod;

    private float moveTime = 0;

    bool ToCheckChain = false;
    bool AllowClick= true;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame

    private void Update()
    {
        if (ToCheckChain)
        {
            moveTime += Time.deltaTime;
        }

        AllowClick = true;
        for (int i = 0; i < runesList.Count; i++)
        {
            if (runesList[i].currentCell != runesList[i].nextCell)
            {
                AllowClick = false;
                ToCheckChain = true;
                if (runesList[i].transform.position == runesList[i].nextCell.transform.position)
                {
                    runesList[i].currentCell.rune = null;
                    runesList[i].currentCell.runeID = 0;
                    runesList[i].currentCell.CanAcceptRune = true;
                    runesList[i].currentCell = runesList[i].nextCell;
                    runesList[i].currentCell.CanAcceptRune = false;
                    runesList[i].currentCell.runeID = runesList[i].runeID;
                    runesList[i].currentCell.rune = runesList[i];

                    runesList[i].currentCell.UpdateRunes();
                    if (runesList[i].currentCell != runesList[i].nextCell)
                    {
                        recheckList.Add(runesList[i].currentCell);
                        runesList[i].currentCell.CanAcceptRune = true;
                        runesList[i].nextCell.CanAcceptRune = false;
                    }
                }
                runesList[i].transform.position = Vector3.Lerp(runesList[i].currentCell.transform.position, runesList[i].nextCell.transform.position, moveTime / movePeriod);
            }

        }
        for (int i = 0; i < recheckList.Count; i++)
        {
            recheckList[i].UpdateRunesAbove();
        }
        for (int i = recheckList.Count -1; i >= 0; i--)
        {
            recheckList[i].UpdateRunesAbove();
            recheckList.RemoveAt(i);
        }

        if (AllowClick && ToCheckChain)
        {
            moveTime = 0;
            CheckChains();
        }
        if(moveTime >= movePeriod)
        {
            moveTime -= movePeriod;
        }


        if (Input.GetButtonDown("Fire1")) //&& AllowClick)
        {
            Vector2 mousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(0);

            SummonRandomRune(mousePos);
        }
        if ((Input.GetButtonDown("Fire2")))
        {
            Vector2 mousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(0);
            Transform nearestTile = hexGrid.FromWorldPosToTile(mousePos.x, mousePos.y);
            nearestTile.GetComponent<HexCell>().runeID = -1;
            nearestTile.GetComponent<HexCell>().CanAcceptRune = false;
            nearestTile.gameObject.SetActive(false);
        }

    }

    public void CheckChains()
    {
        ToCheckChain = false;
        for (int i = 0; i < runesList.Count; i++)
        {
            runesList[i].currentCell.CheckAllAdjRuneChain();
        }
        for (int i = 0; i < runesList.Count; i++)
        {
            if (runesList[i].InChain)
            {
                runesList[i].RemoveFromGrid();
                runesList.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < runesList.Count; i++)
        {
            runesList[i].currentCell.UpdateRunes();
        }
    }
    public void SummonRandomRune(Vector2 coords)
    {
        Transform nearestTile = hexGrid.FromWorldPosToTile(coords.x, coords.y);

        if (nearestTile.GetComponent<HexCell>().CanAcceptRune)
        {
            Rune summonedRune = Instantiate(rune, nearestTile.transform.position, Quaternion.identity).GetComponent<Rune>();

            summonedRune.currentCell = nearestTile.GetComponent<HexCell>();
            summonedRune.currentCell.CanAcceptRune = false;
            summonedRune.currentCell.rune = summonedRune;
            summonedRune.runeID = (int)Random.Range(0, runeSprites.Length) + 1;
            Debug.Log(summonedRune.runeID - 1);
            summonedRune.GetComponent<SpriteRenderer>().sprite = runeSprites[summonedRune.runeID - 1];
            summonedRune.runeGrid = this;
            summonedRune.currentCell.runeID = summonedRune.runeID;
            summonedRune.nextCell = summonedRune.currentCell.FindRuneNextPos();
            runesList.Add(summonedRune);
        }
    }
}
