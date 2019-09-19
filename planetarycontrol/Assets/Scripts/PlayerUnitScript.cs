using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUnitScript : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (hasAuthority == false) {
			return;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			this.transform.Translate (0, 1, 0);
		}
	}
}
