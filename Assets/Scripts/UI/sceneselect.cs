using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneselect : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OpenScene(string name)
    {
            SceneManager.LoadScene(name);
    }


}
