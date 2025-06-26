using UnityEngine;

public class ShowOnMobile : MonoBehaviour
{
    private void Awake()
    {
        this.gameObject.SetActive(Application.isMobilePlatform);
    }
}

