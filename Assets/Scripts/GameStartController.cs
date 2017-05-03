using UnityEngine;
using System.Collections;

public class GameStartController : MonoBehaviour
{
    public GameObject spawnManager;
    public GameObject shipEntity;
    public GameObject postLevelUI;
    public GameObject planet;
    public Sprite[] planetPhase;
    public Sprite planetDestroyed;
    public GameObject combatHud;
    public GameObject energyManager;
    public GameObject gameOverUI;
    public GameObject victoryUI;

    public float levelDuration;
    public float levelStartTime;
    public float levelElapsedTime;
    public bool levelIsActive;
    public bool playerAlive;
    public float energyReward;
    public float spawnDelayDecrement;

    private float phaseDuration;
    private float[] startOfPhaseTime;
    public int currentPhase;
    private int numberOfPhases;

    public Color planetColor;
    public int colorChoice;
    private int lastColorChoice = -1;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        numberOfPhases = planetPhase.Length;
        levelIsActive = false;
        combatHud.SetActive(false);
        postLevelUI.SetActive(false);
        initializePhaseDurations();
        //beginLevel(); // Temp code to test duration before start linking has been implemented. should be called externally
    }
    
    void Update()
    {
        updateLevelState();
        checkEndOfLevel();
    }

    public void beginLevel()
    {
        currentPhase = 0;
        levelIsActive = true;
        combatHud.SetActive(true);
        spawnManager.SetActive(true);
        levelStartTime = Time.time;
        postLevelUI.SetActive(false);
        shipEntity.SetActive(true);
        setRandomPlanetColor();
        decreaseUnitSpawnDelay(spawnDelayDecrement);
    }

    public void beginFirstLevel()
    {
        currentPhase = 0;
        playerAlive = true;
        energyManager.GetComponent<EnergyManager>().fillEnergyMeter();
        energyManager.GetComponent<EnergyManager>().emptyResearchMeter();
        levelIsActive = true;
        combatHud.SetActive(true);
        spawnManager.SetActive(true);
        levelStartTime = Time.time;
        postLevelUI.SetActive(false);
        gameOverUI.SetActive(false);
        shipEntity.SetActive(true);
        setRandomPlanetColor();
    }

    private void setRandomPlanetColor()
    {
        while (colorChoice == lastColorChoice)
        {
            colorChoice = Random.Range(0, 5);
            Debug.Log("Color Choice selected: " + colorChoice);
        }
        if (colorChoice != lastColorChoice)
        {
            lastColorChoice = colorChoice;
        }
        Debug.Log("Random value generated: " + colorChoice);
        switch (colorChoice)
        {
            case 0:
                planetColor = Color.magenta;
                break;
            case 1:
                planetColor = new Color(0.5f, 0.5f, 1f, 1f);
                break;
            case 2:
                planetColor = new Color(0.5f, 1f, 1f, 1f);
                break;
            case 3:
                planetColor = Color.white;
                break;
            case 4:
                planetColor = new Color(0.5f, 1f, 0.5f, 1f);
                break;
            case 5:
                planetColor = new Color(1f, 0.5f, 0.5f, 1f);
                break;
            default:
                planetColor = new Color(1f, 1f, 1f, 1f);
                break;
        }
        planet.GetComponent<SpriteRenderer>().material.SetColor("_Color", planetColor);
    }

    private void checkEndOfLevel()
    {
        levelElapsedTime = Time.time - levelStartTime;

        playerAlive = energyManager.GetComponent<EnergyManager>().hasEnergyRemaining() || !levelIsActive;

        if (playerAlive)
        {
            if (levelIsActive && (levelElapsedTime >= levelDuration))
            {
                levelIsActive = false;
                combatHud.SetActive(false);
                disableSpawnManager();
                shipEntity.SetActive(false);
                postLevelUI.SetActive(true);
                planet.GetComponent<SpriteRenderer>().sprite = planetDestroyed;
                energyManager.GetComponent<EnergyManager>().increaseEnergy(energyReward);
            }
            else if (levelIsActive && (levelElapsedTime < levelDuration) && postLevelUI.activeSelf)
            {
                postLevelUI.SetActive(false);
            }
            else if (!levelIsActive && energyManager.GetComponent<EnergyManager>().researchMeterIsFull())
            {
                postLevelUI.SetActive(false);
                victoryUI.SetActive(true);
            }
        }
        else
        {
            disableSpawnManager();
            combatHud.SetActive(false);
            shipEntity.SetActive(false);
            gameOverUI.SetActive(true);
        }
        
    }

    /*
     * Disables the spawn manager as well as destroying all of its spawned enemies
     */
    private void disableSpawnManager()
    {
        spawnManager.SetActive(false);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            Destroy(enemies[i]);
        }
    }

    private void initializePhaseDurations()
    {
        phaseDuration = levelDuration / numberOfPhases;
        startOfPhaseTime = new float[numberOfPhases];
        currentPhase = 0;
        for (int i = 0; i < numberOfPhases; i++)
        {
            startOfPhaseTime[i] = phaseDuration * i;
        }
    }

    private void updateLevelState()
    {
        for (int i = 0; i < numberOfPhases; i++)
        {
            if (levelElapsedTime >= startOfPhaseTime[i])
            {
                currentPhase = i;
            }
        }

        if (levelIsActive)
        {
            updatePlanetIndicator();
        }
    }

    private void updatePlanetIndicator()
    {
        planet.GetComponent<SpriteRenderer>().sprite = planetPhase[currentPhase];
    }

    private void decreaseUnitSpawnDelay(float f)
    {
        spawnManager.GetComponent<UnitSpawnManager>().decreaseSpawnDelay(f);
    }
    
}
