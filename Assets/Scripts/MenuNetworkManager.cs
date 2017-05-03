using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MenuNetworkManager : NetworkBehaviour {

    string[] positionsOccupiedByPlayers;
    bool[] isPlayersReady;
    /*
    private static MenuNetworkManager instance;

    public static MenuNetworkManager singleton {
        get {
            return instance;
        }

        private set {
            instance = value;
        }
    }
    */
    // Use this for initialization
    void Start () {
        //instance = this;
	    
	}
    
    // Update is called once per frame
    void Update () {
	
	}



    public void Position1Clicked() {
        //MenuManager.singleton.PlayerSelectedPosition(Common._PlayerName, 0);
        if (!GetComponent<NetworkIdentity>().isLocalPlayer)
            return;
        Debug.Log("Position1Clicked");
        CmdPlayerSelectedPosition(netId, Common._PlayerName, 0);
        
    }

    public void Position2Clicked() {
        if (isClient) {
            CmdPlayerSelectedPosition(netId, Common._PlayerName, 1);
        }
    }

    public void Position3Clicked() {
        if (isClient) {
            CmdPlayerSelectedPosition(netId, Common._PlayerName, 2);
        }
    }

    public void Position4Clicked() {
        if (isClient) {
            CmdPlayerSelectedPosition(netId, Common._PlayerName, 3);
        }
    }


    [Command]
    void CmdPlayerSelectedPosition(NetworkInstanceId id, string pPlayerName, int pPosition) {
        // update player status only if player is not ready
        if( isServer) {
            if(positionsOccupiedByPlayers == null) {
                positionsOccupiedByPlayers = new string[4];

                for (int i = 0; i < positionsOccupiedByPlayers.Length; i++) {
                    positionsOccupiedByPlayers[i] = "";
                }
            }

            if(isPlayersReady == null) {
                isPlayersReady = new bool[4];

                for (int i = 0; i < isPlayersReady.Length; i++) {
                    isPlayersReady[i] = false;
                }
            }
            for(int i = 0; i < positionsOccupiedByPlayers.Length; i++) {
                if(positionsOccupiedByPlayers[i] == pPlayerName) {
                    positionsOccupiedByPlayers[i] = "";
                }
            }
            if (!isPlayersReady[pPosition]) {
                positionsOccupiedByPlayers[pPosition] = pPlayerName;
            }

            if (!GetComponent<NetworkIdentity>().isLocalPlayer)
                return;
            NetworkServer.FindLocalObject(id).GetComponent<MenuNetworkManager>().RpcPlayerStatusUpdated(positionsOccupiedByPlayers, isPlayersReady);

            Rpcmeh();
        }
    }

    [ClientRpc]
    void RpcPlayerStatusUpdated(string[] pPlayerPositions, bool[] pPlayerReady) {
        MenuManager.singleton.PlayerStatusUpdate(pPlayerPositions, pPlayerReady);
    }

    [Command]
    public void CmdStartGame() {
        if( isServer) {
            Debug.Log("CmdStartGame");
            Rpcmeh();
            RpcShowPlayerSelectionScene(12);
        }
        
    }

    [ClientRpc]
    void RpcShowPlayerSelectionScene(int ah) {
        //int a = connectionToClient.connectionId;
        Debug.Log("RpcShowPlayerSelectionScene, connection id:"+ah);
        MenuManager.singleton.ShowMenu(IEnums.MenuState.PositionSelection);
    }

    [ClientRpc]
    public void Rpcmeh() {
        Debug.Log("inside meh");
    }
}
