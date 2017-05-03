using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameManager : MonoBehaviour {
    bool isEventCalled = false;
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        
	    if(Network.peerType == NetworkPeerType.Client) {
            if (!isEventCalled) {
                //OnPlayerConnected(Network.player);
                isEventCalled = true;
            }
        }

        if (Network.isClient) {
            Debug.Log("client");
        }
        if (Network.isServer) {
            Debug.Log("server");
        }
    }

    void OnClientConnected(NetworkConnection player) {
        Debug.Log("Player connected");
    }

    void OnConnectedToServer() {
        Debug.Log("Connected to server");
    }

 /*   [Command]
    void CmdCall() {
        if (Network.isClient) {
            Debug.Log("client");
        }
        if(Network.isServer) {
            Debug.Log("server");
        }
    }*/
}
