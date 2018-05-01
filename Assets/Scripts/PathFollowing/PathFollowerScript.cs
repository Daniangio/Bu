using UnityEngine;
using System.Collections;

public class PathFollowerScript : MonoBehaviour {
	
	Node [] PathNode;
	public GameObject [] Target;
	public float MoveSpeed;
	float DistanceTolerance = 0.5f;
	int CurrentNode = 0;
	static Vector3 CurrentPositionHolder;


	void Start () {
		PathNode = GetComponentsInChildren<Node> ();
		CheckNode ();
	
	}

	/// <summary>
	/// we will make a function to check current Node and move to it. by save the node position to CurrenPositionHolder
	/// </summary>
	/// 
	void CheckNode(){
		if (CurrentNode < PathNode.Length) {
			CurrentPositionHolder = PathNode [CurrentNode].transform.position;
		} else {
			CurrentNode = 0;
			CurrentPositionHolder = PathNode [CurrentNode].transform.position;
		}
	}

	void DrawLine(){
		for (int i = 0; i < PathNode.Length; i++) {
		//we will paint from PathNode[0] to 1 , 1 to 2 and like this to end of Pathnode
			if (i < PathNode.Length - 1) {
				Debug.DrawLine (PathNode [i].transform.position, PathNode [i + 1].transform.position, Color.green);
			} else {
				Debug.DrawLine (PathNode [i].transform.position, PathNode [0].transform.position, Color.green);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		DrawLine ();
		//Debug.Log (CurrentNode);
		//this will make the path moving
		foreach (GameObject g in Target) {
			if ((g.transform.position - CurrentPositionHolder).magnitude > DistanceTolerance) {
				//if player position not equal Node position we will move the player to node
				g.transform.position = Vector3.Lerp (g.transform.position, CurrentPositionHolder, Mathf.SmoothStep (0.01f, 2f, (Time.deltaTime * MoveSpeed)));

			} else {
				if (CurrentNode < PathNode.Length) {
					//if it equal lthe node we will go next node
					CurrentNode++;
					//here 
					CheckNode ();
				}
			}

		}
	}
}
