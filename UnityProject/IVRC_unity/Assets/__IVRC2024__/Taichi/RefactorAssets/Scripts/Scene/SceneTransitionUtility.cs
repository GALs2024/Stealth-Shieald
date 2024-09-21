using UnityEngine.SceneManagement;

public static class SceneTransitionUtility
{
    // 指定したシーンを同期的に読み込む
    public static void LoadScene(string sceneName)
    {
        if (SceneExists(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            UnityEngine.Debug.LogError($"シーン '{sceneName}' は存在しません");
        }
    }

    // 現在のシーンをリロードする
    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // シーンが存在するか確認する
    private static bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}
