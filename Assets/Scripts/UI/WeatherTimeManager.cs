using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeatherTimeManager : MonoBehaviour
{
    public static WeatherTimeManager Instance { get; private set; }
    public WeatherEnum weather;
    public TimeEnum time;
    public bool impactDronePhysics = false;
    public bool weatherResistance = false;

    
    // change drone physics
    public UnityEvent<WeatherEnum> _OnWeatherChanged;
    public UnityEvent<TimeEnum> _OnTimeChanged;

    float originalMass = 1.0f;

    //public UnityAction onWeatherChanged
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }


    public string[] GetAvailableWeatherPresets(WeatherEnum weatherEnum)
    {
        return (string[])System.Enum.GetNames(typeof(WeatherEnum));
    }

    public WeatherEnum SetWeatherEnum(int weatherEnum)
    {
        weather = (WeatherEnum)weatherEnum;
         _OnWeatherChanged.Invoke(weather);
        return weather;
    }

    public TimeEnum SetTimeEnum(int timeEnum)
    {
        time = (TimeEnum)timeEnum;
        Debug.Log("in time enum helloo");
         _OnTimeChanged.Invoke(time);
         Debug.Log("shouldve been invoked helloo??");
        return time;
    }

    public void OnWeatherChanged(WeatherEnum weathers)
    {
        int weatherSelection = (int)weathers;
        EnviroSkyMgr.instance.ChangeWeather(weatherSelection);

        if(weatherSelection == 7 || weatherSelection == 9)
        {
            impactDronePhysics = true;
            //weatherResistance = true;
        }
        else
        {
            weatherResistance = false;
            impactDronePhysics = false;
        }

    }

    public void OnTimeChanged(TimeEnum times)
    {
        float time = 0.1f;
        switch ((int)times)
        {
            case 0:
                time = 0.25f;
                this.time = TimeEnum.Sunrise;
                break;
            case 1:
                time = 0.38f;
                this.time = TimeEnum.Morning;
                break;
            case 2:
                time = 0.5f;
                this.time = TimeEnum.Afternoon;
                break;
            case 3:
                time = 0.74f;
                this.time = TimeEnum.Sunset;
                break;
            case 4:
                time = 0.785f;
                this.time = TimeEnum.Evening;
                break;
            case 5:
                time = 0.01f;
                this.time = TimeEnum.Night;
                break;
        }
        EnviroSkyMgr.instance.SetTimeOfDay(time * 24f);
        Debug.Log("plz");
    }

    public TimeEnum GetCurrentTimeEnum(double hour)
    {
        //double hour = double.Parse(time);
        hour = hour / 24f;

        if(hour <= 0.15f)
            return TimeEnum.Sunrise;
        else if(hour > 0.15f && hour <= 0.25f)
            return TimeEnum.Sunrise;
        else if(hour > 0.25f && hour <= 0.38f)
            return TimeEnum.Morning;
        else if(hour > 0.38f && hour <= 0.5f)
            return TimeEnum.Afternoon;
        else if(hour > 0.5f && hour <= 0.74f)
            return TimeEnum.Sunset;
        else if(hour > 0.74f && hour <= 0.785f)
            return TimeEnum.Evening;
        else if(hour > 0.785f && hour <= 0.99f)
            return TimeEnum.Night;
        else
            return TimeEnum.Afternoon;
    }
    public EnviroWeatherPreset GetCurrentWeatherPreset()
    {
        return EnviroSkyMgr.instance.GetCurrentWeatherPreset();
    }

    public bool CheckEnviroInstance()
    {
        return EnviroSkyMgr.instance == null || !EnviroSkyMgr.instance.IsAvailable();
    }

    public string GetTimeString()
    {
        return EnviroSkyMgr.instance.GetTimeString ();
    }

    public IEnumerator ShakeDrone(GameObject drone, float amount)
    {
        //drone.transform.position.y += (Mathf.Sin(Time.time * speed) * amount);
        //drone.transform.position += new Vector3(0, (Mathf.Sin(Time.deltaTime * speed) * amount), 0);
        if(impactDronePhysics)
        {
        drone.transform.position += new Vector3(0, amount, 0);
        yield return new WaitForSeconds(0.005f);
        drone.transform.position -= new Vector3(0, amount, 0); //sub more then you add so it can keep going down if you don't move
        yield return new WaitForSeconds(0.005f);
        }
    }

    public void ApplyWeatherResistance(Rigidbody rb)
    {
        
        // increase mass
        rb.mass = 3.5f;
        // add drag?
        //make it shake from the top side edges
    }

    public void RemoveWeatherResistance(Rigidbody rb)
    {
        rb.mass = originalMass;
    }
}
