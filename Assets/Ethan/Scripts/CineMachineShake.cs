using UnityEngine;
using Unity.Cinemachine;
public class CineMachineShake : MonoBehaviour
{
    public static CineMachineShake Instance; // This is the instance of the CineMachineShake
    [SerializeField] private CinemachineBasicMultiChannelPerlin noise; // This is the noise for the shake
    [SerializeField] private float shakeTimer; // This is the timer for the shake
    [SerializeField] private float defaultShakeTime = 0.2f; // Default shake time

    // Awake is called before the first frame update
    private void Awake(){ // using awake to set the instance to this so it can be accessed from other scripts
        Instance = this; // Set the instance to this
    }
    // Update is called once per frame
    private void Update(){
        if(shakeTimer > 0f){ // If the shake timer is greater than 0, then shake the camera
        shakeTimer -= Time.deltaTime; // Subtract the time from the shake timer
        }
        if(shakeTimer <= 0f && noise != null){ // If the shake timer is less than or equal to 0 and the noise is not null, then stop the shake
            noise.AmplitudeGain = 0f; // Set the amplitude gain to 0 which is the intensity of the shake
            noise.FrequencyGain = 0f;// Set the frequency gain to 0 which is the frequency of the shake
            // gain is the intensity of the shake
        }
    }
    public void ShakeCamera(float intensity){ // This is a function that is called to shake the camera
        if(noise == null) return; // If the noise is null, then return
        noise.AmplitudeGain = intensity; // Set the amplitude gain to the intensity
        noise.FrequencyGain = 2f; // Set the frequency gain to 2
        shakeTimer = defaultShakeTime; // Set the shake timer to the default shake time
    } // end of ShakeCamera function
}
