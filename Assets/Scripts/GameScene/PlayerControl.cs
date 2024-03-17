using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer playerRenderer;
    private LevelGrid levelGrid;
    private TilemapManager tilemapManager;

    public float moveSpeed = 2f;

    internal bool IsOnBackground = false;
    internal bool CanDraw = false;
    internal bool IsDrawing = false;
    internal bool DrawingBan = true;

    internal State state = State.Waiting;
    State stateResume;
    internal enum State
    {
        Playing,
        Waiting,
        Dead,
        Win,
        Paused,
    }

    public void Setup(LevelGrid levelGrid, TilemapManager tilemapManager)
    {
        this.levelGrid = levelGrid;
        this.tilemapManager = tilemapManager;
    }

    void Update()
    {
        switch (state)
        {
            case State.Playing:
                HandleMovement();
                HandleLogic();
                break;
            case State.Waiting:
                HandleFirstMove();
                break;
            case State.Dead:
                break;
            case State.Win:
                HandleMovement();
                break;
            case State.Paused:
                break;
        }
    }

    private void HandleMovement()
    {
        float dX = Input.GetAxis("Horizontal");
        float dY = Input.GetAxis("Vertical");
        Vector2 movement = new(dX, dY);
        rb.velocity = movement * moveSpeed;

        levelGrid.PlayerMoved(GetGridPosition());
    }

    private void HandleLogic()
    {
        CheckIsPlayerOnBackground();
        PlayerColor();
    }

    private void HandleFirstMove()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
        {
            state = State.Playing;
            PlayerUnfreeze();
            tilemapManager.Enemy.StartMovement();
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
        if (state == State.Playing)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                PlayerDied();
            }
        }

        //if (collision.gameObject.CompareTag("GreenTile"))
        //{
        //    tilemapSpawner.DestroyGreenTile(transform.position, collision.GetContact(0).point);
        //}
    }

    internal void CheckIsPlayerOnBackground()
    {
        Vector3 playerCenter = transform.position;
        Vector3Int cellPosition = tilemapManager.TilemapBackground.WorldToCell(playerCenter);
        TileBase tile = tilemapManager.TilemapBackground.GetTile(cellPosition);
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

    internal void NewLife()
    {
        state = State.Waiting;
        PlayerColor();
        DrawingBan = true;
    }

    internal void PlayerDied()
    {
        state = State.Dead;
        PlayerColor();
        PlayerFreeze();
        GameHandler.PlayerDied(tilemapManager);
    }

    internal void PlayerWin()
    {
        state = State.Win;
        PlayerColor();
    }

    internal void PlayerPause()
    {
        stateResume = state;
        state = State.Paused;
        PlayerFreeze();
        tilemapManager.Enemy.EnemyStopped();
    }

    internal void PlayerResume()
    {
        state = stateResume;
        PlayerUnfreeze();
        tilemapManager.Enemy.EnemyMoving();
    }

    void PlayerFreeze()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void PlayerUnfreeze()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void PlayerColor()
    {
        if (state==State.Playing)
        {
            if (DrawingBan == true)
            {
                playerRenderer.color = Color.red;
            }
            else
            {
                playerRenderer.color = Color.yellow;
            }
        }
        else if (state==State.Win || state==State.Waiting)
        {
            playerRenderer.color = new Color(0.05f, 0.4f, 0.56f);
        }
        else if (state==State.Dead)
        {
            playerRenderer.color = Color.gray;
        }
    }
}
