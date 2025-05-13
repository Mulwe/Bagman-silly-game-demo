using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    //корутина
    private IEnumerator Start()
    {
        /*
         * Init Loading Screen Service
         * Init Cloud Service
         * Init Storage service
         */
        //Load imitation
        var loadingDuration = 5f;

        while (loadingDuration > 0f)
        {
            loadingDuration -= Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("Gameplay"); //Menu
    }
}
