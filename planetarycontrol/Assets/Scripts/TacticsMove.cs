using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour {

	List<Tile> selectableTiles = new List<Tile> ();
	GameObject[] tiles;

	public bool turn = false;

	Stack<Tile> path = new Stack<Tile> ();
	Tile currentTile;

	public int moveRange = 5;
	public float climbHeight = 2;
	public float moveSpeed = 2;
	public float climbVelocity = 4.5f;
	public bool moving = false;

	Vector3 velocity = new Vector3();
	Vector3 heading = new Vector3 ();

	float halfHeight = 0;

	bool climbingDown = false;
	bool climbingUp = false;
	bool movingEdge = false;

	Vector3 climbTarget;

	//Initailize game board
	protected void Init()
	{
		tiles = GameObject.FindGameObjectsWithTag ("Tile");

		halfHeight = GetComponent<Collider> ().bounds.extents.y;

		TurnManager.AddUnit (this);
	}

	public void GetCurrentTile()
	{
		currentTile = GetTargetTile(gameObject);
		currentTile.current = true;
	}

	public Tile GetTargetTile(GameObject target)
	{
		RaycastHit hit;
		Tile tile = null;

		if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
		{
			tile = hit.collider.GetComponent<Tile>();
		}

		return tile;
	}

	public void ComputeAdjacencyLists(float climbHeight)
	{
		foreach (GameObject tile in tiles) {
			Tile t = tile.GetComponent<Tile> ();
			t.FindNeighbors (climbHeight);
		}
	}

	public void FindSelectableTiles()
	{
		ComputeAdjacencyLists(climbHeight);
		GetCurrentTile();

		Queue<Tile> process = new Queue<Tile>();

		process.Enqueue(currentTile);
		currentTile.visited = true;
		//currentTile.parent = ??  leave as null 

		while (process.Count > 0)
		{
			Tile t = process.Dequeue();

			selectableTiles.Add(t);
			t.selectable = true;

			if (t.distance < moveRange)
			{
				foreach (Tile tile in t.adjacencyList)
				{
					if (!tile.visited)
					{
						tile.parent = t;
						tile.visited = true;
						tile.distance = 1 + t.distance;
						process.Enqueue(tile);
					}
				}
			}
		}
	}

	public void MoveToTile(Tile tile)
	{
		path.Clear ();
		tile.target = true;
		moving = true;

		Tile next = tile;
		while (next != null) 
		{
			path.Push (next);
			next = next.parent;
		}
			
	}

	public void Move()
	{
		if (path.Count > 0) {
			Tile t = path.Peek ();
			Vector3 target = t.transform.position;

			//calculate the unit's position on top of the target tile;
			target.y += halfHeight + t.GetComponent<Collider> ().bounds.extents.y;

			if (Vector3.Distance (transform.position, target) >= 0.05f) {

				//if there is a height difference 
				bool climb = transform.position.y != target.y;

				if (climb) {
					Climb (target);
				} else {
					CalculateHeading (target);	
					SetHorizontalVelocity ();
				}
				transform.forward = heading;
				transform.position += velocity * Time.deltaTime;
			} 
			else 
			{
				transform.position = target;
				path.Pop ();
			}

		} else {
			RemoveSelectableTiles ();
			moving = false;
			TurnManager.EndTurn ();
		}
	}

	void CalculateHeading(Vector3 target)
	{
		heading = target - transform.position;
		heading.Normalize ();
	}

	void SetHorizontalVelocity()
	{
		velocity = heading * moveSpeed;
	}

	protected void RemoveSelectableTiles()
	{
		if (currentTile != null) 
		{
			currentTile.current = false;
			currentTile = null;
		}

		foreach (Tile tile in selectableTiles) 
		{
			tile.Reset ();
		}

		selectableTiles.Clear ();

	}

	void Climb(Vector3 target)
	{
		if (climbingDown) 
		{
			ClimbingDown (target);
		} else if (climbingUp)
		{
			ClimbingUp (target);
		} 
		else if (movingEdge) 
		{
			MoveToEdge ();
		} 
		else
		{
			PrepareClimb (target);
		}
	}

	void PrepareClimb(Vector3 target)
	{
		float targetY = target.y;

		target.y = transform.position.y;

		CalculateHeading (target);

		if (transform.position.y > targetY) 
		{
			climbingDown = false;
			movingEdge = true;
			climbingUp = false;

			climbTarget = transform.position + (target - transform.position) / 2.0f;
		}
		else
		{
			climbingDown = false;
			movingEdge = false;
			climbingUp = true;

			velocity = heading * moveSpeed / 5.0f;

			float difference = targetY - transform.position.y;

			velocity.y = climbVelocity * (0.5f + difference / 2.0f);
		}

	}

	void ClimbingDown(Vector3 target)
	{
		velocity += Physics.gravity * Time.deltaTime;

		if (transform.position.y <= target.y) 
		{
			climbingDown = false;
			climbingUp = false;
			movingEdge = false;

			Vector3 p = transform.position;
			p.y = target.y;
			transform.position = p;

			velocity = new Vector3 ();
		}
	}

	void ClimbingUp(Vector3 target)
	{
		velocity += Physics.gravity * Time.deltaTime;

		if (transform.position.y > target.y) 
		{
			climbingUp = false;
			climbingDown = true;
		}
	}

	void MoveToEdge()
	{
		if (Vector3.Distance(transform.position, climbTarget) >= 0.05f) {
			SetHorizontalVelocity ();
		} 
		else 
		{
			movingEdge = false;
			climbingDown = true;

			velocity /= 5.0f;
			velocity.y = 1.5f;
		}
	}

	public void BeginTurn()
	{
		turn = true;
	}

	public void EndTurn()
	{
		turn = false;
	}
}
