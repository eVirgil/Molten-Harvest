using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class NetworkPlayerController : NetworkBehaviour {

    int playerCount = 0;

    // Use this for initialization
    void Start () {
        if( isLocalPlayer) {
            Debug.Log("Start: " + Common._PlayerName);

            // telling server to add player by the name
            CmdAddPlayerToList(Common._PlayerName);
        }
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

    }

    // Update is called once per frame
    void Update () {
        
        if(isServer) {
            if (playerCount != GameNetworkManager.singleton.playerNameList.Count) {
                playerCount = GameNetworkManager.singleton.playerNameList.Count;

                //connectionToServer.

                Debug.Log("player list changes");
                RpcSetPlayerNameList(GameNetworkManager.singleton.playerNameList.ToArray());
            }
        }
        
        /*if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("s pressed");
            if( isClient) {
                CmdCall();
            }
        }*/
    }
    

    [Command]
    void CmdAddPlayerToList(string pPlayerName) {
        GameNetworkManager.singleton.AddPlayerName(pPlayerName);
        //MenuManager.singleton.menuNetworkManager.Rpcmeh();
        //MenuNetworkManager.singleton
    }

    [ClientRpc]
    void RpcSetPlayerNameList(string[] pPlayerList) {
        Debug.Log("received list player list from host");
        MenuManager.singleton.SetPlayerList(pPlayerList, isLocalPlayer);
    }


    public void Position1Clicked() {
        if (!isLocalPlayer)
            return;
        
        CmdPlayerSelectedPosition(Common._PlayerName, 0);

    }

    public void Position2Clicked() {
        if (!isLocalPlayer)
            return;

        CmdPlayerSelectedPosition(Common._PlayerName, 1);
        
    }

    public void Position3Clicked() {
        if (!isLocalPlayer)
            return;

        CmdPlayerSelectedPosition(Common._PlayerName, 2);
        
    }

    public void Position4Clicked() {
        if (!isLocalPlayer)
            return;

        CmdPlayerSelectedPosition(Common._PlayerName, 3);
        
    }

    public void PlayerReady() {
        if (!isLocalPlayer)
            return;

        CmdPlayerReady(Common._PlayerName);
    }
    


    [Command]
    void CmdPlayerSelectedPosition(string pPlayerName, int pPosition) {
        // update player status only if player is not ready
        
        if (isServer) {
            if (GameNetworkManager.singleton.positionsOccupiedByPlayers == null) {
                GameNetworkManager.singleton.positionsOccupiedByPlayers = new string[4];

                for (int i = 0; i < GameNetworkManager.singleton.positionsOccupiedByPlayers.Length; i++) {
                    GameNetworkManager.singleton.positionsOccupiedByPlayers[i] = "";
                }
            }

            if (GameNetworkManager.singleton.isPlayersReady == null) {
                GameNetworkManager.singleton.isPlayersReady = new bool[4];

                for (int i = 0; i < GameNetworkManager.singleton.isPlayersReady.Length; i++) {
                    GameNetworkManager.singleton.isPlayersReady[i] = false;
                } 
            }

            // if player clicked ready and still change his position then update his ready
            
            for (int i = 0; i < GameNetworkManager.singleton.positionsOccupiedByPlayers.Length; i++) {
                if (GameNetworkManager.singleton.positionsOccupiedByPlayers[i] == pPlayerName) {
                    GameNetworkManager.singleton.positionsOccupiedByPlayers[i] = "";
                    GameNetworkManager.singleton.isPlayersReady[i] = false;
                }
            }
            
            GameNetworkManager.singleton.positionsOccupiedByPlayers[pPosition] = pPlayerName;
            
            
            RpcPlayerStatusUpdated(GameNetworkManager.singleton.positionsOccupiedByPlayers, GameNetworkManager.singleton.isPlayersReady);
            
        }
    }

    [ClientRpc]
    void RpcPlayerStatusUpdated(string[] pPlayerPositions, bool[] pPlayerReady) {
        MenuManager.singleton.PlayerStatusUpdate(pPlayerPositions, pPlayerReady);
    }

    [Command]
    public void CmdStartGame() {
        if (isServer) {
            RpcShowPlayerSelectionScene();
        }

    }

    [ClientRpc]
    void RpcShowPlayerSelectionScene() {
        //int a = connectionToClient.connectionId;
        MenuManager.singleton.ShowMenu(IEnums.MenuState.PositionSelection);
    }
    
    [Command]
    void CmdPlayerReady(string pPlayerName) {
        
        for( int i=0; i < GameNetworkManager.singleton.positionsOccupiedByPlayers.Length; i++) {
            if(GameNetworkManager.singleton.positionsOccupiedByPlayers[i] == pPlayerName) {
                GameNetworkManager.singleton.isPlayersReady[i] = true;
                break;
            }
            
        }

        RpcPlayerStatusUpdated(GameNetworkManager.singleton.positionsOccupiedByPlayers, GameNetworkManager.singleton.isPlayersReady);
        int playerCount = GameNetworkManager.singleton.playerNameList.Count;
        int numPlayersReady = 0;
        for( int  i=0 ; i < GameNetworkManager.singleton.isPlayersReady.Length; i++) {
            if (GameNetworkManager.singleton.isPlayersReady[i]) numPlayersReady++;
        }

        Debug.Log("if " + numPlayersReady + ">=" + playerCount+" then show in game");

        IEnums.PlayerPosition playerSelectedPosition = IEnums.PlayerPosition.CoaxialCannon;
        for (int i = 0; i < GameNetworkManager.singleton.positionsOccupiedByPlayers.Length; i++) {
            //Debug.Log(GameNetworkManager.singleton.positionsOccupiedByPlayers[i] + " == " + pPlayerName);
            if (GameNetworkManager.singleton.positionsOccupiedByPlayers[i] == pPlayerName) {
                switch (i) {
                    case 0:
                        playerSelectedPosition = IEnums.PlayerPosition.CoaxialCannon;
                        break;
                    case 1:
                        playerSelectedPosition = IEnums.PlayerPosition.Drones;
                        break;
                    case 2:
                        playerSelectedPosition = IEnums.PlayerPosition.Missiles;
                        break;
                    case 3:
                        playerSelectedPosition = IEnums.PlayerPosition.Shield;
                        break;
                }

                RpcSetPlayerPosition(pPlayerName, playerSelectedPosition);
                break;
            }
        }

        if (numPlayersReady > 0 && numPlayersReady >= playerCount) {
            RpcShowIngameUI();
            GameNetworkManager.singleton.EnableEnemySpawner();
            //GameNetworkManager.singleton.gameStartController.beginLevel();
            GameNetworkManager.singleton.EnemySpawner.spawningEnabled = true;
        } // end of if (numPlayersReady)
    }

    [ClientRpc]
    void RpcSetPlayerPosition( string pPlayerName, IEnums.PlayerPosition pPlayerPosition ) {
        if( pPlayerName == Common._PlayerName) {
            Debug.Log(Common._PlayerName + ": " + pPlayerName + " selected " + pPlayerPosition);
            Common._PlayerPosition = pPlayerPosition;
        }
        GameNetworkManager.singleton.gameStartController.beginLevel();
        Debug.Log("RpcSetPlayerPosition called");
    }

    [ClientRpc]
    void RpcShowIngameUI() {
        MenuManager.singleton.ShowMenu(IEnums.MenuState.InGameUI);
    }


    public void FireLaserGun() {
        if (!isLocalPlayer)
            return;

        // ask server to fire laser gun
        CmdFireLaserGun();
    }

    [Command]
    void CmdFireLaserGun() {
        // in server check if ship can fire
        if (GameNetworkManager.singleton.shipMovementController.laserReady) {
            GameNetworkManager.singleton.shipMovementController.laserReady = false;
            GameNetworkManager.singleton.shipMovementController.laserFired = true;

            RpcFireLaserGun();
        }
    }

    
    public void SetPlayerInput(Vector3[] pInputTouches) {
        CmdSetPlayerInput( Common._PlayerPosition, pInputTouches );
    }

    [Command]
    void CmdSetPlayerInput(IEnums.PlayerPosition pPosition, Vector3[] pInputTouches) {
        // server
        GameNetworkManager.singleton.shipMovementController.SyncInputFromClients(pPosition, pInputTouches);
    }

    [ClientRpc]
    void RpcFireLaserGun() {
        GameNetworkManager.singleton.shipMovementController.FireLaserGunApproved();
    }
}
