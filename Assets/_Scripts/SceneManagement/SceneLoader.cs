using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private static Scenes targetScene;

    public static void LoadScene(Scenes scene) {
        targetScene = scene;

        SceneManager.LoadScene(Scenes.LoadingScene.ToString());
    }

    public static void LoadSceneAdditively(Scenes scene)
    {
        SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
