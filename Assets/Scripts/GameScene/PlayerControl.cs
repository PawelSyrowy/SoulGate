using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer playerRenderer;
    private Tilemap tilemapBackground;
    private LevelGrid levelGrid;
    private WinningManager winningManager;

    public float moveSpeed = 2f;

    internal bool IsOnBackground = false;
    internal bool CanDraw = false;
    internal bool IsDrawing = false;
    internal bool DrawingBan = true;

    internal enum State
    {
        Alive,
        Dead
    }

    internal State state = State.Alive;

    public void Setup(Tilemap tilemapBackground, LevelGrid levelGrid, WinningManager winningManager)
    {
        this.tilemapBackground = tilemapBackground;
        this.levelGrid = levelGrid;
        this.winningManager = winningManager;
    }

    void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleMovement();
                HandleLogic();
                break;
            case State.Dead:
                break;
        }
    }

    private void HandleMovement()
    {
        float dX = Input.GetAxis("Horizontal");
        float dY = Input.GetAxis("Vertical");
        Vector2 movement = new(dX, dY);
        rb.velocity = movement * moveSpeed;

        levelGrid.SnakeMoved(GetGridPosition());
    }

    private void HandleLogic()
    {
        CheckIsPlayerOnBackground();

        if (DrawingBan == true)
        {
            playerRenderer.color = Color.red;
        }
        else
        {
            playerRenderer.color = Color.yellow;
        }
    }

    public Vector2Int GetGridPosition()
    {
        Vector3Int roundedPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x),
                                                    Mathf.RoundToInt(transform.position.y),
                                                    Mathf.RoundToInt(transform.position.z));
        return new Vector2Int(roundedPosition.x, roundedPosition.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            state = State.Dead;
            playerRenderer.color = Color.gray;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            winningManager.GetPlayerState(false);
        }

        //if (collision.gameObject.CompareTag("GreenTile"))
        //{
        //    tilemapSpawner.DestroyGreenTile(transform.position, collision.GetContact(0).point);
        //}
    }

    internal void CheckIsPlayerOnBackground()
    {
        Vector3 playerCenter = transform.position;
        Vector3Int cellPosition = tilemapBackground.WorldToCell(playerCenter);
        TileBase tile = tilemapBackground.GetTile(cellPosition);
        if (tile != null)
        {
            IsOnBackground = true;
        }
        else
        {
            IsOnBackground = false;
            DrawingBan = false;
        }
    }

    internal void CheckCanPlayerDraw(bool isOnGhost, bool isOnSafe)
    {
        if (DrawingBan)
        {
            CanDraw = false;
        }
        else if (IsOnBackground == true)
        {
            if (!isOnGhost && !isOnSafe)
            {
                CanDraw = true;
            }
            else
            {
                CanDraw = false;
            }
        }
        else
        {
            CanDraw = false;
        }
    }

    internal bool CheckPlayerCanFinishDrawing(bool isOnSafe)
    {
        if ((IsOnBackground == false || isOnSafe == true) && CanDraw == false && IsDrawing == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal void CheckIsPlayerDrawing(bool ghostExists)
    {
        if (ghostExists)
        {
            IsDrawing = true;
        }
        else
        {
            IsDrawing = false;
        }
    }
}
