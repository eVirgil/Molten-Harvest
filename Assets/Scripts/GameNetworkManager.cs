using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Net;

public class GameNetworkManager : NetworkManager {
    /*
     game network manager is a singleton class
     */

    private static GameNetworkManager instance;

    public static GameNetworkManager singleton {
        get {
            return instance;
        }

        private set {
            instance = value;
        }
    }


    public string[] positionsOccupiedByPlayers;
    public bool[] isPlayersReady;
    public ShipMovementController shipMovementController;


    public UnitSpawnManager EnemySpawner;
    public GameStartController gameStartController;

    public EnergyManager energyManager;

    private bool isLocalPlayer = false;

    List<NetworkClient> clientList;
    List<string> PlayerNamesList;

    public List<string> playerNameList {
        get {
            return this.PlayerNamesList;
        }
        private set {
            this.PlayerNamesList = value;
        }
    }
    // Use this for initialization
    void Start() {
        singleton = this;

        PlayerNamesList = new List<string>();
        
        positionsOccupiedByPlayers = new string[4];
        isPlayersReady = new bool[4];

        EnemySpawner.spawningEnabled = false;
        MenuManager.singleton.ShowMenu(IEnums.MenuState.StartMenu);
    }


    public void EnableEnemySpawner() {
        //EnemySpawner.SetActive(true);
        NetworkServer.SpawnObjects();
    }

    // Update is called once per frame
    void Update() {

    }


    public void ShowHostGameMenu() {
        MenuManager.singleton.ShowMenu(IEnums.MenuState.HostGame);
    }

    public void HostGame() {
        if (clientList == null) {
            clientList = new List<NetworkClient>();
        }
        clientList.Clear();

        isLocalPlayer = true;
        networkAddress = LocalIPAddress();
        NetworkClient localClient = StartHost();
        if (localClient == null) Debug.Log("Couldn't start host.");
        clientList.Add(localClient);


        //localClient.connection.connectionId
    }

    public override void OnStartHost() {
        base.OnStartHost();

        Debug.Log("Host Started");

        //NetworkServer.SpawnObjects();

        MenuManager.singleton.SetHostIP("Host IP: " + Network.player.ipAddress);//LocalIPAddress());
        MenuManager.singleton.ShowMenu(IEnums.MenuState.HostedGameDetails);
    }

    public void ShowJoinGameMenu() {
        MenuManager.singleton.ShowMenu(IEnums.MenuState.JoinGame);
    }

    public void JoinGame() {
        NetworkManager.singleton.networkAddress = MenuManager.singleton.GetJoinIP();
        GameNetworkManager.singleton.networkAddress = MenuManager.singleton.GetJoinIP();
        Debug.Log("Attempting to connect to : " + MenuManager.singleton.GetJoinIP());

        StartClient();
    }

    public override void OnStartClient(NetworkClient client) {
        base.OnStartClient(client);

        if (!isLocalPlayer) {
            MenuManager.singleton.ShowMenu(IEnums.MenuState.WaitingForHost);

            Common._PlayerName = MenuManager.singleton.GetClientName();
        }
        else {
            Common._PlayerName = MenuManager.singleton.GetHostName();
        }

    }

    public override void OnClientError(NetworkConnection conn, int errorCode) {
        base.OnClientError(conn, errorCode);

        Debug.Log("Error code: " + errorCode);
    }

    /*
        utility  functions
    */
    public string LocalIPAddress() {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList) {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }


    public void AddPlayerName(string pPlayerName) {
        Debug.Log("AddPlayerName: " + pPlayerName);
        PlayerNamesList.Add(pPlayerName);
    }

    public void ShowSelectPositionScene() {
        GameObject[] objArr = GameObject.FindGameObjectsWithTag("NetworkPlayer");
        for (int i = 0; i < objArr.Length; i++) {
            NetworkPlayerController controller = objArr[i].GetComponent<NetworkPlayerController>();

            if (controller) {
                controller.CmdStartGame();
            }
        }
    }

    /*
        Command functions can only be called through the local player
        and the local player is the player that is spawned by the network manager 

        So find the object with network player and call function
    */
    public void Position1Clicked() {
        //MenuManager.singleton.PlayerSelectedPosition(Common._PlayerName, 0);
        GameObject[] objArr = GameObject.FindGameObjectsWithTag("NetworkPlayer");
        for (int i = 0; i < objArr.Length; i++) {
            NetworkPlayerController controller = objArr[i].GetComponent<NetworkPlayerController>();

            if (controller) {
                controller.Position1Clicked();
            }
        }
    }

    public void Position2Clicked() {
        //MenuManager.singleton.PlayerSelectedPosition(Common._PlayerName, 0);
        GameObject[] objArr = GameObject.FindGameObjectsWithTag("NetworkPlayer");
        for (int i = 0; i < objArr.Length; i++) {
            NetworkPlayerController controller = objArr[i].GetComponent<NetworkPlayerController>();

            if (controller) {
                controller.Position2Clicked();
            }
        }
    }

    public void Position3Clicked() {
        //MenuManager.singleton.PlayerSelectedPosition(Common._PlayerName, 0);
        GameObject[] objArr = GameObject.FindGameObjectsWithTag("NetworkPlayer");
        for (int i = 0; i < objArr.Length; i++) {
            NetworkPlayerController controller = objArr[i].GetComponent<NetworkPlayerController>();

            if (controller) {
                controller.Position3Clicked();
            }
        }
    }

    public void Position4Clicked() {
        //MenuManager.singleton.PlayerSelectedPosition(Common._PlayerName, 0);
        GameObject[] objArr = GameObject.FindGameObjectsWithTag("NetworkPlayer");
        for (int i = 0; i < objArr.Length; i++) {
            NetworkPlayerController controller = objArr[i].GetComponent<NetworkPlayerController>();

            if (controller) {
                controller.Position4Clicked();
            }
        }
    }

    public void PlayerReady() {
        GameObject[] objArr = GameObject.FindGameObjectsWithTag("NetworkPlayer");
        for (int i = 0; i < objArr.Length; i++) {
            NetworkPlayerController controller = objArr[i].GetComponent<NetworkPlayerController>();

            if (controller) {
                controller.PlayerReady();
            }
        }
    }


    public GameObject SpawnNetworkAwareObject(GameObject pPrefab, Vector3 pPosition, Quaternion pRotation) {
        GameObject ship = (GameObject)Instantiate(pPrefab, pPosition, pRotation);
        NetworkServer.Spawn(ship);

        return ship;
    }

    public void FireLaserGun() {

        GameObject[] objArr = GameObject.FindGameObjectsWithTag("NetworkPlayer");
        for (int i = 0; i < objArr.Length; i++) {
            NetworkPlayerController controller = objArr[i].GetComponent<NetworkPlayerController>();

            if (controller) {
                controller.FireLaserGun();

                energyManager.GetComponent<EnergyManager>()
                    .decreaseEnergy(energyManager.GetComponent<EnergyManager>()
                    .primaryLaserCost);
            }
        }
    }

    public void SendInput( Vector3[] pInputTouches ) {
        GameObject[] objArr = GameObject.FindGameObjectsWithTag("NetworkPlayer");
        for (int i = 0; i < objArr.Length; i++) {
            NetworkPlayerController controller = objArr[i].GetComponent<NetworkPlayerController>();

            if (controller) {
                controller.SetPlayerInput(pInputTouches);
            }
        }
    }

    public void ShowStartMenu() {
        MenuManager.singleton.ShowMenu(IEnums.MenuState.StartMenu);
    }

    public void ShowMainMenu() {
        MenuManager.singleton.ShowMenu(IEnums.MenuState.MainMenu);
    }

    public void ShowCredits() {
        MenuManager.singleton.ShowMenu(IEnums.MenuState.CreditsMenu);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
