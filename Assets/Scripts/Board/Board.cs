using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    private int boardSizeX;
    private int boardSizeY;
    private Cell[,,] m_cells;
    private GameObject[,,] goCells;
    public List<Item> trayItems;
    public List<(GameObject obj, bool isActive)> goTrayCells;
    private Transform m_root;
    private int m_matchMin;

    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        m_matchMin = gameSettings.MatchesMin;

        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;

        m_cells = new Cell[Constants.LAYER_NUMBER, boardSizeX + Constants.LAYER_NUMBER, boardSizeY + Constants.LAYER_NUMBER];
        goCells = new GameObject[Constants.LAYER_NUMBER, boardSizeX + Constants.LAYER_NUMBER, boardSizeY + Constants.LAYER_NUMBER];
        trayItems = new List<Item>();
        goTrayCells = new List<(GameObject, bool)>();

        CreateBoard();
        CreateTray();
    }

    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);

        for (int layer = 0; layer < Constants.LAYER_NUMBER; layer += 1)
        {
            int layerSizeX = boardSizeX + layer;
            int layerSizeY = boardSizeY + layer;

            for (int x = 0; x < layerSizeX; x++)
            {
                for (int y = 0; y < layerSizeY; y++)
                {
                    GameObject go = GameObject.Instantiate(prefabBG);
                    go.transform.position = origin + new Vector3(x - (layer / 2f), y - (layer / 2f), 0f);
                    go.transform.SetParent(m_root);
                    go.GetComponent<SpriteRenderer>().sortingOrder = -(layer * 2 + 1);

                    Cell cell = go.GetComponent<Cell>();
                    cell.Setup(x, y, layer);

                    m_cells[layer, x, y] = cell;
                    goCells[layer, x, y] = go;

                    if (layer == 0)
                    {
                        go.tag = "Boerd0";
                    }
                    if (layer == 1)
                    {
                        go.GetComponent<BoxCollider2D>().enabled = false;
                        go.tag = "Boerd1";
                    }
                    if (layer == 2)
                    {
                        go.GetComponent<BoxCollider2D>().enabled = false;
                        go.tag = "Boerd2";
                    }
                    if (layer == 3)
                    {
                        go.GetComponent<BoxCollider2D>().enabled = false;
                        go.tag = "Boerd3";
                    }
                }
            }
        }
    }
    private void CreateTray()
    {
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < Constants.TRAY_SIZE_X; x++)
        {
            GameObject go = GameObject.Instantiate(prefabBG);
            go.tag = "Tray";
            go.transform.position = new Vector3(x - 2, -4.5f, 0);
            go.transform.SetParent(m_root);
            go.GetComponent<SpriteRenderer>().sortingOrder = -10;

            Cell cell = go.GetComponent<Cell>();
            cell.Setup(x, -3, 0);

            goTrayCells.Add((go, false));
        }
    }

    internal void Fill()
    {
        for (int layer = 0; layer < Constants.LAYER_NUMBER; layer += 1)
        {
            int layerSizeX = boardSizeX + layer;
            int layerSizeY = boardSizeY + layer;

            for (int x = 0; x < layerSizeX; x++)
            {
                for (int y = 0; y < layerSizeY; y++)
                {
                    Cell cell = m_cells[layer, x, y];
                    NormalItem item = new NormalItem();

                    List<NormalItem.eNormalType> types = new List<NormalItem.eNormalType>();

                    item.SetType(Utils.GetRandomNormalTypeExcept(types.ToArray()));
                    item.SetView();
                    item.SetViewRoot(m_root);
                    item.View.gameObject.tag = "Boerd";

                    cell.Assign(item);
                    cell.ApplyItemPosition(false, -(layer * 2));

                    if (layer != 0)
                    {
                        cell.Item.View.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                    }


                }
            }
        }
    }
}
