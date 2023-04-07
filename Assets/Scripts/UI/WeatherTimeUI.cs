using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WeatherTimeUI : MonoBehaviour
{

    private bool started = false;
    public UnityEngine.UI.Dropdown weatherDropdown;
    public UnityEngine.UI.Dropdown timeDropdown;
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

        if (WeatherTimeManager.Instance.CheckEnviroInstance())
        {
            this.enabled = false;
            return;
        }
        EnviroSkyMgr.instance.OnWeatherChanged += (EnviroWeatherPreset type) =>
       {
        Debug.Log("are we in here");
           UpdateWeather();
       };

       rb = drone.GetComponent<Rigidbody>();

      // weatherText.text = EnviroSkyMgr.instance.Weather.currentActiveWeatherPreset.Name;
      WeatherTimeManager.Instance.time = WeatherTimeManager.Instance.GetCurrentTimeEnum(EnviroSkyMgr.instance.GetCurrentTimeInHours());
      timeText.text = WeatherTimeManager.Instance.time.ToString();

    }

    void Update()
    {
        if (!EnviroSkyMgr.instance.IsStarted())
            return;
        else
        {
            if (!started)
            {
                weatherDropdown.ClearOptions();
                timeDropdown.ClearOptions();
                StartCoroutine(setupDropdown(weatherDropdown, WeatherTimeManager.Instance.weather));
                UpdateWeather();
                StartCoroutine(setupDropdown(timeDropdown, WeatherTimeManager.Instance.time));
                UpdateTime();
            }
        }

        if (WeatherTimeManager.Instance.GetCurrentWeatherPreset() != null)
            weatherText.text = WeatherTimeManager.Instance.GetCurrentWeatherPreset().Name;

        timeText.text = WeatherTimeManager.Instance.time.ToString();

        if(WeatherTimeManager.Instance.impactDronePhysics)
        {
            StartCoroutine(WeatherTimeManager.Instance.ShakeDrone(drone, 0.01f));//ApplyWeatherEffectsToDrone();
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

    IEnumerator setupWeather()
    {
        started = true;
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < EnviroSkyMgr.instance.GetCurrentWeatherPresetList().Count; i++)
        {
            UnityEngine.UI.Dropdown.OptionData weather = new UnityEngine.UI.Dropdown.OptionData();
            weather.text = EnviroSkyMgr.instance.GetCurrentWeatherPresetList()[i].Name;
            weatherDropdown.options.Add(weather);
        }

        yield return new WaitForSeconds(0.1f);

        UpdateWeather();
    }

    public void ConvertToEnum(int id)
    {
        WeatherTimeManager.Instance.SetWeatherEnum(id);
    }

    public void ConvertToEnum(Dropdown change, Enum enumType)
    {
        Debug.Log("does this read");
        Type type = enumType.GetType();

        if (type == typeof(WeatherEnum))
            WeatherTimeManager.Instance.SetWeatherEnum(change.value);
        else
            WeatherTimeManager.Instance.SetTimeEnum(change.value);
    }
    public void ChangeWeather(WeatherEnum weather)
    {
        WeatherTimeManager.Instance.OnWeatherChanged(weather);
    }

    public void ChangeTime(TimeEnum time)
    {
        Debug.Log("Before?");
        WeatherTimeManager.Instance.OnTimeChanged(time);
        Debug.Log("tryna figure out order here");
    }

    void UpdateWeather()
    {
        if (WeatherTimeManager.Instance.GetCurrentWeatherPreset() != null)
        {
            for (int i = 0; i < weatherDropdown.options.Count; i++)
            {
                if (weatherDropdown.options[i].text == WeatherTimeManager.Instance.GetCurrentWeatherPreset().Name)
                {
                    weatherDropdown.value = i;
                }
            }
        }
    }

    void UpdateTime()
    {

            for (int i = 0; i < weatherDropdown.options.Count; i++)
            {
                if (timeDropdown.options[i].text == WeatherTimeManager.Instance.time.ToString())
                {
                    timeDropdown.value = i;
                }
            }

    }

    void SetUpDropDowns()
    {
        StartCoroutine(setupDropdown(weatherDropdown, WeatherTimeManager.Instance.weather));
        StartCoroutine(setupDropdown(timeDropdown, WeatherTimeManager.Instance.time));

        weatherDropdown.onValueChanged.AddListener(delegate
        {
            ConvertToEnum(weatherDropdown, WeatherTimeManager.Instance.weather);
           // _OnWeatherChanged.Invoke(WeatherTimeManager.Instance.weather);
        });

        timeDropdown.onValueChanged.AddListener(delegate
        {
            ConvertToEnum(timeDropdown, WeatherTimeManager.Instance.time);
          //  _OnTimeChanged.Invoke(WeatherTimeManager.Instance.time);
        });
    }

    void ApplyWeatherEffectsToDrone()
    {
        if(!WeatherTimeManager.Instance.weatherResistance)
        {
        WeatherTimeManager.Instance.ApplyWeatherResistance(rb);
        WeatherTimeManager.Instance.weatherResistance = true;
        }
    }

    void RemoveWeatherEffectsOnDrone()
    {
        WeatherTimeManager.Instance.RemoveWeatherResistance(rb);
        WeatherTimeManager.Instance.impactDronePhysics = false;
        WeatherTimeManager.Instance.weatherResistance = false;
    }


    /** Old timeSlider code **/

    /**
    public void ChangeTimeSlider () 
	{
        //WeatherTimeManager.Instance.UpdateSlider();
		if (timeSlider.value < 0f)
			timeSlider.value = 0f;
		EnviroSkyMgr.instance.SetTimeOfDay (timeSlider.value * 24f);
	}
    **/

    /**
    void LateUpdate ()
	{
		timeSlider.value = EnviroSkyMgr.instance.GetTimeOfDay() / 24f;
	}
    **/
}
