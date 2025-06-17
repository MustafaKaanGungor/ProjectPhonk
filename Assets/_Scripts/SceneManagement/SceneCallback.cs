using UnityEngine;

public class SceneCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;
    void Update()
    {
        if(isFirstUpdate) {
            isFirstUpdate = false;

            SceneLoader.LoaderCallback();
        }
    }
}
