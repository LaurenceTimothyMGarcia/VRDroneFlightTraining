using System.Collections;
using System;
using UnityEngine;

public class WeatherTimeUI2 : MonoBehaviour
{
    private bool started = false;
    public UnityEngine.UI.Text weatherText;
    public UnityEngine.UI.Text timeText;
    public GameObject drone;
    public Rigidbody rb;

    void Start()
    {

        SetUpDropDowns();
        if(EnviroSkyMgr.instance == null || !EnviroSkyMgr.instance.IsAvailable())
            {
                this.enabled = false;
                return;
            }

        if (WeatherTimeMgr.Instance.CheckEnviroInstance())
        {
            this.enabled = false;
            return;
        }
        EnviroSkyMgr.instance.OnWeatherChanged += (EnviroWeatherPreset type) =>
        {
           //UpdateWeather();
        };

       rb = drone.GetComponent<Rigidbody>();

      // weatherText.text = EnviroSkyMgr.instance.Weather.currentActiveWeatherPreset.Name;
      WeatherTimeMgr.Instance.time = WeatherTimeMgr.Instance.GetCurrentTimeEnum(EnviroSkyMgr.instance.GetCurrentTimeInHours());
      timeText.text = WeatherTimeMgr.Instance.time.ToString();

    }

    void Update()
    {
        if (!EnviroSkyMgr.instance.IsStarted())
            return;
        else
        {
            if (!started)
            {
                //StartCoroutine(setupDropdown(weatherDropdown, WeatherTimeMgr.Instance.weather));
                //UpdateWeather();
                //StartCoroutine(setupDropdown(timeDropdown, WeatherTimeMgr.Instance.time));
                //UpdateTime();
            }
        }

        if (WeatherTimeMgr.Instance.GetCurrentWeatherPreset() != null)
            weatherText.text = WeatherTimeMgr.Instance.GetCurrentWeatherPreset().Name;

        timeText.text = WeatherTimeMgr.Instance.time.ToString();

        if(WeatherTimeMgr.Instance.impactDronePhysics)
        {
            StartCoroutine(WeatherTimeMgr.Instance.ShakeDrone(drone, 0.01f));
            ApplyWeatherEffectsToDrone();
        }
        else
            RemoveWeatherEffectsOnDrone();


    }

    IEnumerator setupDropdown(UnityEngine.UI.Dropdown dropdown, Enum enumType)
    {
        Debug.Log("and set up the dumb dropdowns??");
        started = true;
        yield return new WaitForSeconds(0.1f);

        Type type = enumType.GetType();
        for (int i = 0; i < Enum.GetNames(type).Length; i++)
        {
            UnityEngine.UI.Dropdown.OptionData optionData = new UnityEngine.UI.Dropdown.OptionData();
            optionData.text = Enum.GetName(type, i);
            dropdown.options.Add(optionData);
            Debug.Log("iteration " + i);
        }

        dropdown.value = Array.IndexOf(Enum.GetValues(enumType.GetType()), enumType);
        yield return new WaitForSeconds(0.1f);

    }

    public void ConvertToEnum(int selected, Enum enumType)
    {
        Debug.Log("does this read");
        Type type = enumType.GetType();

        if (type == typeof(WeatherEnum))
            WeatherTimeMgr.Instance.SetWeatherEnum(selected);
        else
            WeatherTimeMgr.Instance.SetTimeEnum(selected);
    }
    public void ChangeWeather(WeatherEnum weather)
    {
        WeatherTimeMgr.Instance.OnWeatherChanged(weather);
    }

    public void ChangeTime(TimeEnum time)
    {
        WeatherTimeMgr.Instance.OnTimeChanged(time);
    }


    void SetUpDropDowns()
    {
        //StartCoroutine(setupDropdown(weatherDropdown, WeatherTimeMgr.Instance.weather));
        //StartCoroutine(setupDropdown(timeDropdown, WeatherTimeMgr.Instance.time));

        // Add Listeners for weather buttons
        for (int i = 0; i < WeatherTimeMgr.Instance.weatherButtons.Count; i++)
        {
        int closureIndex = i ; // Prevents the closure problem
        WeatherTimeMgr.Instance.weatherButtons[closureIndex].onClick.AddListener( () => ConvertToEnum(closureIndex, WeatherTimeMgr.Instance.weather) );
        }

        // Add Listeners for time buttons
        for (int i = 0; i < WeatherTimeMgr.Instance.timeButtons.Count; i++)
        {
        int closureIndex = i ; // Prevents the closure problem
        WeatherTimeMgr.Instance.timeButtons[closureIndex].onClick.AddListener( () => ConvertToEnum(closureIndex, WeatherTimeMgr.Instance.time) );
        }
    }

    void ApplyWeatherEffectsToDrone()
    {
        if(!WeatherTimeMgr.Instance.weatherResistance)
        {
        WeatherTimeMgr.Instance.ApplyWeatherResistance(rb);
        WeatherTimeMgr.Instance.weatherResistance = true;
        }
    }

    void RemoveWeatherEffectsOnDrone()
    {
        WeatherTimeMgr.Instance.RemoveWeatherResistance(rb);
        WeatherTimeMgr.Instance.impactDronePhysics = false;
        WeatherTimeMgr.Instance.weatherResistance = false;
    }
}
