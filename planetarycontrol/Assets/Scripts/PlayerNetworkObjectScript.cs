using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkObjectScript : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		if (isLocalPlayer == false) {
			return;
		}

		CmdSpawnMyUnit ();
	}

	public GameObject playerUnitPrefab;
	
	// Update is called once per frame
	void Update () {
		
	}

	[Command]
	void CmdSpawnMyUnit() {
		Debug.Log ("PlayerNetworkObject:: Spawning A Player Unit");
		GameObject go = Instantiate (playerUnitPrefab);
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
	}
}
