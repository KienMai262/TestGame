using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };
    public bool IsBusy { get; private set; }
    private Board m_board;
    private GameManager m_gameManager;
    private bool m_isDragging;
    private Camera m_cam;
    private Collider2D m_hitCollider;
    private GameSettings m_gameSettings;
    private bool m_gameOver;
    public List<GameObject> trayItems = new List<GameObject>();

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);

        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
        }
    }

    public Vector3 trayPosition = Vector3.zero;
    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;
        if(CheckWin()){
            m_gameManager.m_uiMenu.GameWin.SetActive(true);
            Time.timeScale = 1f;
        }
        if (Input.GetMouseButtonDown(0) && !m_isDragging)
        {
            if (m_board.trayItems.Count() == Constants.MAX_TRAY_LENGTH)
            {
                m_gameManager.m_uiMenu.GameOver.SetActive(true);
                Time.timeScale = 1f;
            }
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Boerd0" || hit.collider.gameObject.tag == "Boerd1" || hit.collider.gameObject.tag == "Boerd2" || hit.collider.gameObject.tag == "Boerd3")
                {
                    m_isDragging = true;
                    m_hitCollider = hit.collider;
                    if (m_hitCollider.GetComponent<Cell>().Item.IsSelected == true)
                    {
                        for (int i = 0; i < m_board.goTrayCells.Count; i++)
                        {
                            if (m_board.goTrayCells[i].isActive == false)
                            {
                                trayPosition = m_board.goTrayCells[i].Item1.transform.position;
                                m_board.goTrayCells[i] = (m_board.goTrayCells[i].obj, true);
                                m_board.trayItems.Add(m_hitCollider.GetComponent<Cell>().Item);
                                trayItems.Add(m_hitCollider.GetComponent<Cell>().Item.View.gameObject);

                                break;
                            }
                        }
                        MoveItemToTray(m_hitCollider.GetComponent<Cell>().Item, trayPosition);
                        m_hitCollider.GetComponent<SpriteRenderer>().enabled = false;
                        m_hitCollider.GetComponent<BoxCollider2D>().enabled = false;
                        m_hitCollider.gameObject.SetActive(false);
                        // Destroy(m_hitCollider.gameObject);

                        Invoke("RemoveB", 0.2f);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            ResetRayCast();
        }
    }

    public bool CheckWin()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    public void RemoveB()
    {
        if (m_board.trayItems.Count >= 3)
        {
            int lastIndex = m_board.trayItems.Count - 1;
            if (m_board.trayItems[lastIndex].View.gameObject.name == m_board.trayItems[lastIndex - 1].View.gameObject.name &&
                m_board.trayItems[lastIndex].View.gameObject.name == m_board.trayItems[lastIndex - 2].View.gameObject.name)
            {
                m_board.trayItems.RemoveRange(lastIndex - 2, 3);
                m_board.goTrayCells[lastIndex] = (m_board.goTrayCells[lastIndex].obj, false);
                m_board.goTrayCells[lastIndex - 1] = (m_board.goTrayCells[lastIndex - 1].obj, false);
                m_board.goTrayCells[lastIndex - 2] = (m_board.goTrayCells[lastIndex - 2].obj, false);
                trayItems[lastIndex].SetActive(false);
                trayItems[lastIndex - 1].SetActive(false);
                trayItems[lastIndex - 2].SetActive(false);
                trayItems.RemoveRange(lastIndex - 2, 3);
            }
        }
    }

    private void ResetRayCast()
    {
        m_isDragging = false;
        m_hitCollider = null;
    }

    private void MoveItemToTray(Item item, Vector3 trayPosition)
    {
        item.View.DOMove(trayPosition, 0.1f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                SetActiveBoard();
            });
    }

    private void SetActiveBoard()
    {

        List<Transform> allObjects = new List<Transform>();
        foreach (Transform child in transform)
        {
            allObjects.Add(child);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.CompareTag("Boerd0") || child.CompareTag("Boerd1") || child.CompareTag("Boerd2") || child.CompareTag("Boerd3"))
            {
                Cell cell = child.GetComponent<Cell>();
                if (cell != null && cell.Item != null)
                {
                    if (!cell.Item.IsOverlapping(child.gameObject, allObjects))
                    {
                        BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
                        if (collider != null) collider.enabled = true;

                        SpriteRenderer sprite = cell.Item.View.GetComponent<SpriteRenderer>();
                        if (sprite != null) sprite.color = new Color(1, 1, 1, 1f);
                    }
                }
            }
        }

    }

}
