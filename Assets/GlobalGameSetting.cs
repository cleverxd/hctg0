using UnityEngine;

public class GlobalGameSetting : MonoBehaviour
{
    public int framerateLock = 60; // Target frame rate

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = framerateLock;
    }
}
