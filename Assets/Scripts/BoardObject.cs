﻿using UnityEngine;
using System.Collections;

public class BoardObject : MonoBehaviour
{

	public enum BoardType
	{
		BLOCK,
		PLAYER}
	;

	public BoardType boardType;
	public float speed = 5;
	public int row;
	public int col;
	public int destRow;
	public int destCol;

	protected bool moving;
	protected float moveTime;
	protected GameManager mgr;

	void Awake ()
	{
		mgr = GameObject.Find ("GameManager").GetComponent<GameManager> ();

		SetBoardPosition ();
		transform.position = GetBoardPosition (row, col);
	}

	protected void Update ()
	{
		if (moving) {
			moveTime += Time.deltaTime * speed;
			transform.position = Vector2.Lerp (GetBoardPosition (row, col), GetBoardPosition (destRow, destCol), moveTime);
			if (moveTime >= 1) {
				moving = false;
				SetBoardPosition ();
			}
		}
	}

	public void Move(int row, int col)
	{
		destRow = row;
		destCol = col;
		moving = true;
		moveTime = 0;
	}

	protected Vector2 GetBoardPosition (int row, int col)
	{
		float xPos = -4.5f + col * 1.5f;
		float yPos = 4.5f - row * 1.5f;
		return new Vector2 (xPos, yPos);
	}

	protected void SetBoardPosition ()
	{
		row = destRow;
		col = destCol;
	}

}