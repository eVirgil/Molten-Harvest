using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class MenuManager : MonoBehaviour {
    
    public MenuNetworkManager menuNetworkManager;
    public GameObject inGameHUD;
    public GameStartController gameStartController;

    // reference to all the objects in main menu canvas
    [SerializeField]
    Canvas StartMenu;

    [SerializeField]
    Canvas CreditsMenu;

    // reference to all the objects in main menu canvas
    [SerializeField]
    Canvas MainMenu;

    [SerializeField]
    Canvas HostGame;

    [SerializeField]
    InputField HostName;


    // reference to all the objects in hosted game canvas
    [SerializeField]
    Canvas HostedGameDetails;

    [SerializeField]
    Text ipAddress;

    [SerializeField]
    Text[] host_playerNameList;

    // reference to all the objects in join game canvas
    [SerializeField]
    Canvas JoinGame;

    [SerializeField]
    InputField ClientName;

    [SerializeField]
    InputField JoinIPAddress;


    // reference to all objects in waiting for host to start game canvas
    [SerializeField]
    Canvas WaitingForHost;

    [SerializeField]
    Text[] client_playerNameList;


    // position selection scene
    [SerializeField]
    Canvas PositionSelecitonScene;

    [SerializeField]
    Text[] position_playerNameList;

    [SerializeField]
    Button[] position_positionsList;

    // in game ui
    [SerializeField]
    Canvas InGameUI;

    // local variables
    Canvas prevShowingMenu;
    Canvas currentShowingMenu;

    private static MenuManager instance;

    public static MenuManager singleton {
        get {
            return instance;
        }

        private set {
            instance = value;
        }
    }
	// Use this for initialization
	void Start () {
        instance = this;
        //menuNetworkManager.gameObject.SetActive(true);

    }

    public void SetHostIP(string pHostIPAddress) {
        ipAddress.text = pHostIPAddress;
    }
    
    public string GetHostName() {
        return HostName.text;
    }

    public string GetClientName() {
        return ClientName.text;
    }

    public string GetJoinIP() {
        return JoinIPAddress.text;
    }

    public void PlayerSelectedPosition(string pPlayerName, int pPosition) {
        position_positionsList[pPosition].interactable = false;
        position_positionsList[pPosition].transform.GetChild(0).GetComponent<Text>().text = pPlayerName;

        for(int i = 0; i < position_playerNameList.Length; i++) {
            for( int j = 0; j < GameNetworkManager.singleton.playerNameList.Count; j++) {
                if(position_playerNameList[i].text == GameNetworkManager.singleton.playerNameList[j]) {
                    position_playerNameList[i].color = new Color(1.0f, 1.0f, 0.0f);

                    break;
                }
            }
        }   
    }
    
    public void PlayerStatusUpdate(string[] pPlayerPosition, bool[] pIsPlayerReady) {
        for (int j = 0; j < pPlayerPosition.Length; j++) {
            for (int i = 0; i < position_playerNameList.Length; i++) {
                if (position_playerNameList[i].text == pPlayerPosition[j]) {
                    position_playerNameList[i].color = pIsPlayerReady[j] ? new Color(0.0f, 1.0f, 0.0f) : new Color(1.0f, 1.0f, 0.0f);
                    
                    break;
                }

            }

            position_positionsList[j].interactable = pPlayerPosition[j] == "";
            position_positionsList[j].transform.GetChild(0).GetComponent<Text>().text = pPlayerPosition[j] == "" ? GetPositionString(j) : pPlayerPosition[j]+" ("+ GetPositionString(j)+")";
            ColorBlock cb = position_positionsList[j].colors;
            cb.disabledColor = pIsPlayerReady[j] ? new Color(0.0f, 200.0f / 255.0f, 0.0f) : new Color(200.0f/255.0f, 200.0f / 255.0f, 0.0f);
            position_positionsList[j].colors = cb;
        }
     }

    string GetPositionString(int pIndex) {
        string str = "";
        
        if (IEnums.PlayerPosition.IsDefined(typeof(IEnums.PlayerPosition), pIndex))
            str = ((IEnums.PlayerPosition)pIndex).ToString();
        else
            str = "Invalid Position";

        return str;
    }

    // only for showing in screen
    public void SetPlayerList(string[] pPlayerList, bool pIsLocalPlayer) {
        Text[] playerList;
        if (pIsLocalPlayer) {
            playerList = host_playerNameList;
        }
        else{
            playerList = client_playerNameList;
        }

        for( int i = 0; i < playerList.Length; i++) {
            if( i < pPlayerList.Length) {
                playerList[i].text = pPlayerList[i];
                position_playerNameList[i].text = pPlayerList[i];
            }
            else {
                playerList[i].text = "";
                position_playerNameList[i].text = "";
            }
        }
    }


    public void ShowMenu(IEnums.MenuState pMenuState) {
        if (prevShowingMenu) {
            prevShowingMenu.gameObject.SetActive(false);
        }

        switch (pMenuState) {
            case IEnums.MenuState.NoMenu:
                currentShowingMenu = null;
                break;

            case IEnums.MenuState.StartMenu:
                currentShowingMenu = StartMenu;
                break;

            case IEnums.MenuState.CreditsMenu:
                currentShowingMenu = CreditsMenu;
                break;

            case IEnums.MenuState.MainMenu:
                currentShowingMenu = MainMenu;
                break;

            case IEnums.MenuState.HostGame:
                currentShowingMenu = HostGame;
                break;

            case IEnums.MenuState.HostedGameDetails:
                currentShowingMenu = HostedGameDetails;
                break;

            case IEnums.MenuState.JoinGame:
                currentShowingMenu = JoinGame;
                break;

            case IEnums.MenuState.WaitingForHost:
                currentShowingMenu = WaitingForHost;
                break;

            case IEnums.MenuState.PositionSelection:
                currentShowingMenu = PositionSelecitonScene;
                break;

            case IEnums.MenuState.InGameUI:
                currentShowingMenu = InGameUI;
                inGameHUD.SetActive(true);
                //gameStartController.GetComponent<GameStartController>().beginLevel();
                break;

            default:
                currentShowingMenu = null;
                break;
        }

        if (currentShowingMenu) {
            currentShowingMenu.gameObject.SetActive(true);
        }

        prevShowingMenu = currentShowingMenu;
    }
}
