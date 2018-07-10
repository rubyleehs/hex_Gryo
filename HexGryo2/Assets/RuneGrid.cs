using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneGrid : MonoBehaviour
{
    public HexGrid hexGrid;
    public List<Rune> runesList;
    private List<Rune> inactiveRunes = new List<Rune>();
    public Sprite[] sprites;
    public Color32[] spriteColor;

    public GameObject rune;
    public Transform runeParent;
    public float runeMovePeriod;

    private float timeElapsed=0;
    private bool IsMoving = false;
    private bool ToCheckChain = false;
    private Vector3 mousePos;
    private bool ResortRunes = true;
    private bool AllowInput = false;
    private float stationaryTime;
    // Use this for initialization
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (ResortRunes)
        {
            ResortRunes = false;
            runesList.Sort(SortByHeight);
        }
        //IsMoving = true;
        timeElapsed += Time.deltaTime;
        for (int i = 0; i < runesList.Count; i++)
        {
            runesList[i].transform.position = Vector3.Lerp(runesList[i].currentCell.transform.position, runesList[i].nextCell.transform.position, timeElapsed / runeMovePeriod);
            if (runesList[i].IsRotating != 0 && runesList[i].runeID != 1)
            {
                AllowInput = false;
                IsMoving = true;
                SpriteRenderer spriteRenderer = runesList[i].transform.GetComponent<SpriteRenderer>();
                spriteRenderer.color = Color.Lerp(runesList[i].currentColor, spriteColor[runesList[i].runeID - 1], timeElapsed / runeMovePeriod);

                runesList[i].transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(new Vector3(0, 0, -60 * runesList[i].IsRotating)), timeElapsed / runeMovePeriod);

                if (timeElapsed > runeMovePeriod)
                {
                    runesList[i].transform.rotation = Quaternion.identity;
                    spriteRenderer.flipY = !spriteRenderer.flipY;
                    runesList[i].currentColor = spriteColor[runesList[i].runeID - 1];
                    runesList[i].IsRotating = 0;
                }
            }

        }
        if (timeElapsed > runeMovePeriod)
        {
            RespawnRunes();
            runesList.Sort(SortByHeight);
            timeElapsed -= runeMovePeriod;

            IsMoving = false;
            for (int i = 0; i < runesList.Count; i++)
            {
                runesList[i].FinishMoveSetup();
                runesList[i].nextCell = runesList[i].currentCell.FindRuneNextCell();
                if (runesList[i].nextCell != runesList[i].currentCell)
                {
                    stationaryTime = 0;
                    IsMoving = true;
                    AllowInput = false;
                    runesList[i].StartMoveSetup();
                }

            }
        }

        if (!IsMoving)
        {
            stationaryTime += Time.deltaTime;
            for (int i = 0; i < runesList.Count; i++)
            {
                CheckAndDestroyChains();
            }
        }



        mousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(0);
        Transform cellTransform = hexGrid.FromWorldPosToTile(mousePos.x, mousePos.y);
        if (cellTransform == null) return;

        HexCell selectedCell = cellTransform.GetComponent<HexCell>();
        hexGrid.ShowSelectionPoints(selectedCell);

        if (Input.GetButtonDown("Fire1") && selectedCell != null && !IsMoving && AllowInput && stationaryTime >= runeMovePeriod)
        {
            stationaryTime = 0;
            timeElapsed = 0;
            if (selectedCell.rune.runeID != 1)
            {
                selectedCell.rune.IsRotating = 1;
                if (selectedCell.rune.runeID % 2 == 0)
                {
                    selectedCell.rune.runeID++;
                }
                else selectedCell.rune.runeID--;
            }
            for (int i = 0; i < 6; i++)
            {
                selectedCell.cellNeighbours[i].rune.nextCell = selectedCell.cellNeighbours[(i + 1) % 6].rune.currentCell;
                if (selectedCell.cellNeighbours[i].rune.runeID != 1)
                {
                    selectedCell.cellNeighbours[i].rune.IsRotating = 1;
                    if (selectedCell.cellNeighbours[i].rune.runeID % 2 == 0)
                    {
                        selectedCell.cellNeighbours[i].rune.runeID++;
                    }
                    else selectedCell.cellNeighbours[i].rune.runeID--;
                }
                ResortRunes = true;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            for (int y = 0; y < hexGrid.height; y++)
            {
                for (int x = 0; x < hexGrid.width; x++)
                {
                    SummonRandomRune(hexGrid.hexCells[x, y].transform.position);
                }
            }
            ResortRunes = true;
        }
      
    }

    public void SummonRandomRune(Vector2 coords)
    {
        Transform nearestTile = hexGrid.FromWorldPosToTile(coords.x, coords.y);
        if (nearestTile.GetComponent<HexCell>().rune == null)
        {
            Rune summonedRune;
            if (inactiveRunes.Count == 0)
            {
                summonedRune = Instantiate(rune, nearestTile.transform.position, Quaternion.identity, runeParent).GetComponent<Rune>();
            }
            else
            {
                inactiveRunes[0].gameObject.SetActive(true);
                summonedRune = inactiveRunes[0];
                inactiveRunes.RemoveAt(0);
            }
            int spriteInt = Random.Range(0, sprites.Length);
            SpriteRenderer spriteRenderer = summonedRune.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprites[spriteInt];
            spriteRenderer.color = spriteColor[spriteInt];
            summonedRune.currentColor = spriteColor[spriteInt];

            if (spriteInt%2 == 1)
            {
                spriteRenderer.flipY = true;
            }


            summonedRune.runeID = spriteInt + 1;
            summonedRune.currentCell = nearestTile.GetComponent<HexCell>();
            summonedRune.currentCell.rune = summonedRune;
            summonedRune.nextCell = nearestTile.GetComponent<HexCell>();
            summonedRune.StartMoveSetup();
            runesList.Add(summonedRune);
        }
    }

    void CheckAndDestroyChains()
    {
        AllowInput = true;
        ToCheckChain = false;
        for (int i = 0; i < runesList.Count; i++)
        {
            if (runesList[i].gameObject.activeInHierarchy)
            {
                runesList[i].currentCell.CheckAllAdjRuneChain();
            }
            else
            {
                AllowInput = false;
  
                runesList.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < runesList.Count; i++)
        {
            if (runesList[i].InChain)
            {
                AllowInput = false;
                inactiveRunes.Add(runesList[i]);
                runesList[i].RemoveFromGrid();
                runesList.RemoveAt(i);
                i--;
            }
        }
        
    }

    static int SortByHeight(Rune _a, Rune _b)
    {
        return _a.transform.position.y.CompareTo(_b.transform.position.y);
    }

    public void RespawnRunes()
    {
        for (int x = 0; x < hexGrid.width; x++)
        {
            SummonRandomRune(hexGrid.hexCells[x, hexGrid.height-1].transform.position);
        }
    }

}
