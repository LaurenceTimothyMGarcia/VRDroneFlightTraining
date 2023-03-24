using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTest : MonoBehaviour
{
  private void OnMouseDown()
    {
        Debug.Log("Click!");
        EnviroSkyMgr.instance.ChangeWeather(0);
    }
}
