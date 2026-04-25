using UnityEngine;

public class CameraPresetController : MonoBehaviour
{
    public ModelOrbitController orbitController;

    public void SetFrontView()   => ApplyPreset(yaw: 0f,   pitch: 0f);
    public void SetSideView()    => ApplyPreset(yaw: 90f,  pitch: 0f);
    public void SetTopView()     => ApplyPreset(yaw: 0f,   pitch: 89f);
    public void SetPerspView()   => ApplyPreset(yaw: 30f,  pitch: 25f);

    private void ApplyPreset(float yaw, float pitch)
    {
        if (orbitController == null) return;
        orbitController.SetOrbitAngles(yaw, pitch);
    }
}
