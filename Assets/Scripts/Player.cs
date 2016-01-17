﻿using UnityEngine;
using System.Collections;

public class Player : BoardObject
{

	private Animator animator;
	public bool isBot; 
	public float jumpDelay = 0.2f;
	private bool isJumping;
	private float jumpTime;
	private int prevDirection = 4;
	private bool jumpAgainstWall = false;
	private Vector2 jumpStartPosition;

	// Use this for initialization
	void Start ()
	{
		boardType = BoardType.PLAYER;
		animator = GetComponent<Animator> ();
	}
		

	// Update is called once per frame
	void Update ()
	{
		base.Update ();

		if (isJumping) {
			jumpTime += Time.deltaTime * speed;
			if (jumpAgainstWall) {
				transform.position = new Vector2 (jumpStartPosition.x, jumpStartPosition.y + GetJumpHeight(jumpTime));
			} else {
				transform.position = new Vector2 (transform.position.x, transform.position.y + GetJumpHeight(jumpTime));
			}

			if (jumpTime >= 1) {
				isJumping = false;
				jumpAgainstWall = false;
				jumpStartPosition = Vector2.zero;
			}
		}
	
		if (!moving) {
			int direction = 0; // L R U D
			int destR = row;
			int destC = col;
			bool jump_key; 
			bool pull_key;

			if (!isBot) {
				if (Input.GetKeyDown (KeyCode.Space) && !isBot && !isJumping) {
					Jump ();
					return;
				}

				if (Input.GetAxis ("Horizontal") < 0) {
					destC = Mathf.Clamp (col - 1, 0, mgr.boardSize + 1);
					direction = 1;
					prevDirection = direction;
				} else if (Input.GetAxis ("Horizontal") > 0) {
					destC = Mathf.Clamp (col + 1, 0, mgr.boardSize + 1);
					direction = 2;
					prevDirection = direction;
				} else if (Input.GetAxis ("Vertical") < 0) {
					destR = Mathf.Clamp (row + 1, 0, mgr.boardSize + 1);
					direction = 3;
					prevDirection = direction;
				} else if (Input.GetAxis ("Vertical") > 0) {
					destR = Mathf.Clamp (row - 1, 0, mgr.boardSize + 1);
					direction = 4;
					prevDirection = direction;
				} else {
					animator.SetBool ("Pushing", false);
				}

				animator.SetInteger ("Direction", direction);
				jump_key = Input.GetKey (KeyCode.RightShift) || Input.GetKey (KeyCode.LeftShift);
				pull_key = Input.GetKey (KeyCode.Tab);
			} else {
				direction = Random.Range (0, 11);
				jump_key = Random.Range (0, 1)==1; //since c# can't make int to bool	
				pull_key = Random.Range (0, 1) ==1 ;

				if (direction == 1) {
					destC = Mathf.Clamp (col - 1, 0, mgr.boardSize + 1);
				} else if (direction == 2) {
					destC = Mathf.Clamp (col + 1, 0, mgr.boardSize + 1);
				} else if (direction == 3) {
					destR = Mathf.Clamp (row + 1, 0, mgr.boardSize + 1);
				} else if (direction == 4) {
					destR = Mathf.Clamp (row - 1, 0, mgr.boardSize + 1);
				}
			}


			if (direction >= 1 && direction <=4) {

				if ((mgr.GetPlayerInPosition (destR, destC) != null) && 
					(mgr.CheckForBlock(destR, destC) == false)) 
					return; //another player in your destination square, and no block underneath them

				if (jump_key) {
					//jump on top of existing block.  Just move player. 
					Move (destR, destC);
					mgr.SetPlayerPosition (destR, destC, this); 
					mgr.VacatePlayerPosition (row, col); 

				} else if (pull_key) {
					//pulling -- when pulling, player moves first
					animator.SetBool ("Pulling", true);
					Move (destR, destC);
					mgr.SetPlayerPosition (destR, destC, this); 
					mgr.VacatePlayerPosition (row, col); 

					//blocks move next to rol, col (where player was)
					mgr.PullBlock (direction, row, col); 

				} else if (mgr.CheckForBlock (destR, destC)) {
					//push if block is in the way
					animator.SetBool ("Pushing", true);
					if (mgr.PushBlock (direction, destR, destC)) {  //recursive call allows you to move a row of blocks 
						//when pushing, blocks move first
						//player moves next
						Move (destR, destC);
						mgr.SetPlayerPosition (destR, destC, this); 
						mgr.VacatePlayerPosition (row, col);
					}
				} else {
					//pure movement 
					Move (destR, destC);
					mgr.SetPlayerPosition (destR, destC, this); 
					mgr.VacatePlayerPosition (row, col);
					return; 
				}
			}
		} 
	}

	private void Jump()
	{
		jumpTime = 0;
		isJumping = true;	

		//pure movement 
		int destR = row;
		int destC = col;
		if (prevDirection == 1) {
			destC = Mathf.Clamp (col - 1, 0, mgr.boardSize + 1);
		} else if (prevDirection == 2) {
			destC = Mathf.Clamp (col + 1, 0, mgr.boardSize + 1);
		} else if (prevDirection == 3) {
			destR = Mathf.Clamp (row + 1, 0, mgr.boardSize + 1);
		} else if (prevDirection == 4) {
			destR = Mathf.Clamp (row - 1, 0, mgr.boardSize + 1);
		}
		if (destR == row && destC == col) {
			jumpAgainstWall = true;
			jumpStartPosition = GetBoardPosition (row, col);
		}
		Move (destR, destC);
		mgr.SetPlayerPosition (destR, destC, this); 
		mgr.VacatePlayerPosition (row, col);
	}

	private float GetJumpHeight(float time)
	{
		float a = 1f;
		float diff = time - 0.5f;
		float height = (-a * diff * diff) + 0.25f;
		return Mathf.Clamp (height, 0, 100);
	}
}
