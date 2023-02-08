using UnityEngine;
using System.Collections;
public class DayNightController : MonoBehaviour
{
    public float dayCycleLength;
    public float currentCycleTime;
    public DayPhase currentPhase;
    public float hoursPerDay;
    public float dawnTimeOffset;
    public int worldTimeHour;
    public Color fullLight;
    public Color fullDark;
    public Material daySkybox;
    public Color dayFog;
    public Material nightSkybox;
    public Color nightFog; 
    private float dayTime;
    private float nightTime;
    private float quarterDay;
    private float lightIntensity;
    void Initialize()
    {
        //remainingTransition = skyTransitionTime; //Would indicate that the game should start with an active transition, if UpdateSkybox were used.
        quarterDay = dayCycleLength /2;
        nightTime = 0.0f;
        dayTime = quarterDay;
        if (GetComponent<Light>() != null)
        { lightIntensity = GetComponent<Light>().intensity; }
    }
    void Reset()
    {
        dayCycleLength = 120.0f;
        //skyTransitionTime = 3.0f; //would be set if UpdateSkybox were used.
        hoursPerDay = 24.0f;
        dawnTimeOffset = 3.0f;
        fullDark = new Color(32.0f / 255.0f, 28.0f / 255.0f, 46.0f / 255.0f);
        fullLight = new Color(253.0f / 255.0f, 248.0f / 255.0f, 223.0f / 255.0f);
        dayFog = new Color(180.0f / 255.0f, 208.0f / 255.0f, 209.0f / 255.0f);
        nightFog = new Color(12.0f / 255.0f, 15.0f / 255.0f, 91.0f / 255.0f);
        /*Skybox[] skyboxes = AssetBundle.FindObjectsOfTypeIncludingAssets(typeof(Skybox)) as Skybox[];
        foreach (Skybox box in skyboxes)
        {
            if (box.name == "StarryNight Skybox")
            { nightSkybox = box.material; }
            else if (box.name == "Sunny2 Skybox")
            { daySkybox = box.material; }
        }*/
    }
    void Start()
    {
        Initialize();
    }
    void Update()
    {
        // Rudementary phase-check algorithm:
        if (currentCycleTime > nightTime && currentCycleTime < dayTime && currentPhase == DayPhase.Day)
        {
            SetNight();
        }
        else if (currentCycleTime > dayTime && currentPhase == DayPhase.Night)
        {
            SetDay();
        }
 
        // Perform standard updates:
        UpdateWorldTime();
        UpdateDaylight();
        UpdateFog();
        currentCycleTime += Time.deltaTime;
        currentCycleTime = currentCycleTime % dayCycleLength;
    }
    public void SetDay()
    {
        RenderSettings.skybox = daySkybox; //would be commented out or removed if UpdateSkybox were used.
        //remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
        RenderSettings.ambientLight = fullLight;
        if (GetComponent<Light>() != null)
        { GetComponent<Light>().intensity = lightIntensity; }
        currentPhase = DayPhase.Day;
    }
    public void SetNight()
    {
        RenderSettings.skybox = nightSkybox; //would be commented out or removed if UpdateSkybox were used.
        //remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
        RenderSettings.ambientLight = fullDark;
        if (GetComponent<Light>() != null)
        { GetComponent<Light>().enabled = false; }
        currentPhase = DayPhase.Night;
    }
    private void UpdateDaylight()
    {
        if (currentPhase == DayPhase.Day)
        {
            
            RenderSettings.ambientLight = Color.Lerp(fullDark, fullLight, dayTime/4);
            if (GetComponent<Light>() != null)
            {
                //GetComponent<Light>().intensity = lightIntensity * quarterDay;
            }
        }
        else if (currentPhase == DayPhase.Night)
        {
            RenderSettings.ambientLight = Color.Lerp(fullLight, fullDark, nightTime/4);
            if (GetComponent<Light>() != null)
            {
                //GetComponent<Light>().intensity = lightIntensity * quarterDay;
            }
        }
 
        transform.Rotate(new Vector3((((Time.deltaTime / dayCycleLength) * 360.0f)),0,0), Space.Self);
   }
    private void UpdateFog()
    {

        if (currentPhase == DayPhase.Day)
        {
            float relativeTime = currentCycleTime - dayTime;
            RenderSettings.fogColor = Color.Lerp(dayFog, nightFog, relativeTime / quarterDay);
        }
        else if (currentPhase == DayPhase.Night)
        {
            float relativeTime = currentCycleTime - nightTime;
            RenderSettings.fogColor = Color.Lerp(nightFog, dayFog, relativeTime / quarterDay);
        }
    }
    private void UpdateWorldTime()
    { 
        worldTimeHour = (int)((Mathf.Ceil((currentCycleTime / dayCycleLength) * hoursPerDay) + dawnTimeOffset) % hoursPerDay) + 1;
    }
    public enum DayPhase
    {
        Night = 0,
        Day = 1,
    }
}
