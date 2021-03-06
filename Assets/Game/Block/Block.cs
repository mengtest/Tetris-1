﻿using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
    public enum rotation { DOWN, RIGHT, UP, LEFT };
    public rotation actitveRotation = rotation.DOWN;
    public bool speedUp = false;
    public bool lockRotation = false;

    private BlockTile[] tile;
    private Arena managerArena;
    private blocksManager managerBlocks;
    private Detector detector;

    private float fallTimer = 0f;
    private float moveDownMultiplier = 0f;
    private float timer = 0;

    void Start() {
        tile = new BlockTile[transform.childCount - 1];

        for (int i = 0; i < transform.childCount - 1; ++i) tile[i] = transform.GetChild(i).GetComponent<BlockTile>();
        managerArena = GameObject.FindGameObjectWithTag("Game_arena").GetComponent<Arena>();
        managerBlocks = GameObject.FindGameObjectWithTag("Game_blocks").GetComponent<blocksManager>();
        detector = transform.GetComponentInChildren<Detector>();
    }

    void Update() {
        if(!GamePause.isPaused()) {
            if (detector.canMoveVertical(actitveRotation)) {
                if (timer <= 0) {
                    moveTilesVertical();
                    if (!speedUp) timer = fallTimer;
                    else timer = fallTimer / moveDownMultiplier;
                }
                else timer -= Time.deltaTime;
            }
            else {
                managerBlocks.pushBlock();
                destroy();
            }
        }
    }

    public void rotate() {
        if (detector.canRotate() && !lockRotation) {
            transform.Rotate(0, 0, 90f);
            int rot = (int)transform.eulerAngles.z / 90;
            actitveRotation = (rotation)rot;
        }
    }

    public void turnLeft() { if (detector.canMoveHorizontal(actitveRotation, -1)) moveTilesHorizontal(-1); }
    public void turnRight() { if (detector.canMoveHorizontal(actitveRotation, 1)) moveTilesHorizontal(1); }

    public void randColor() {
        BlockTile.blockColor col = (BlockTile.blockColor)Random.Range(0, 7);

        foreach(Transform tile in transform) {
            if (tile.GetComponent<BlockTile>() == null) return;
            else tile.GetComponent<BlockTile>().setColor(col);
        }
    }

    public void setSpeed(float fall, float multiplier) {
        fallTimer = fall;
        moveDownMultiplier = multiplier;
    }

    private void moveTilesHorizontal(int direction) { transform.position = managerArena.tile[tile[0].arenaTile.posX + direction, tile[0].arenaTile.posY].transform.position; }
    private void moveTilesVertical(int direction = 1) { transform.position = managerArena.tile[tile[0].arenaTile.posX, tile[0].arenaTile.posY + direction].transform.position; }

    private void destroy() {
        foreach (BlockTile tl in tile) tl.blockControllerRemoved = true;
        Destroy(detector.gameObject);
        Destroy(GetComponent<Block>());
    }
}
