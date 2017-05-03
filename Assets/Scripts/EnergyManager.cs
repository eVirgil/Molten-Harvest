using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{

    public GameObject combatEnergyMeter;
    public GameObject postCombatEnergyMeter;
    public Slider postCombatEnergySlider;
    public GameObject researchMeter;

    public float energy;
    public int MAX_ENERGY = 4000;

    public float research;
    public int MAX_RESEARCH = 10000;

    private bool postCombatDisplayActive;
    private bool combatDisplayActive;

    public float fillRate = 2.0f;

    public float primaryLaserCost;

    void Start()
    {

    }

    void Update()
    {
        updateEnergyStatus();
        updateResearchStatus();
    }

    private void updateEnergyStatus()
    {
        float energyPercent = energy / MAX_ENERGY;
        float currentPercent = combatEnergyMeter.GetComponent<Image>().fillAmount;
        combatEnergyMeter.GetComponent<Image>().fillAmount = Mathf.Lerp(currentPercent, energyPercent, Time.deltaTime * fillRate);
        postCombatEnergyMeter.GetComponent<Image>().fillAmount = Mathf.Lerp(currentPercent, energyPercent, Time.deltaTime * fillRate);
    }

    private void updateResearchStatus()
    {
        float researchpercent = research / MAX_RESEARCH;
        float currentPercent = researchMeter.GetComponent<Image>().fillAmount;
        researchMeter.GetComponent<Image>().fillAmount = Mathf.Lerp(currentPercent, researchpercent, Time.deltaTime * fillRate);
    }

    public void increaseEnergy(float f)
    {
        energy += f;
        if (energy > MAX_ENERGY)
        {
            energy = MAX_ENERGY;
        }
    }

    public void decreaseEnergy(float f)
    {
        energy -= f;
        if (energy < 0)
        {
            energy = 0;
        }
    }

    public bool hasEnergyRemaining()
    {
        return energy > 0;
    }

    public void fillEnergyMeter()
    {
        energy = MAX_ENERGY;
    }

    public void emptyResearchMeter()
    {
        research = 0;
    }

    public bool researchMeterIsFull()
    {
        return research >= MAX_RESEARCH && researchMeter.GetComponent<Image>().fillAmount >= 0.99f;
    }

    public void setEnergy(float energy)
    {
        this.energy = energy;
    }

    public float getEnergy()
    {
        return energy;
    }

    public void transferEnergy()
    {
        float sliderValue = (postCombatEnergySlider.value <= energy) ? postCombatEnergySlider.value : energy;
        float transferValue = energy - sliderValue;
        energy -= transferValue;
        research += transferValue;
    }

}
