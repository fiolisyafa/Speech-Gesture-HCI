using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Windows.Speech;
using System.Linq;

public class SpeechInput : MonoBehaviour
{
    //todo make private, but exposed to the editor, tricky wayaroundusing_ 
	public AudioSource audioSource;
	public AudioMixerGroup mixerGroup;
    public SpeechEnvironment currentEnvironment;
    //public SpeechWatcher sw;
    //bool endWatch;
    float counter; // for timer
    float sum = 0;
    float rmsValue; //value for RMS
    float decibelValue; //decibel value
    private const int Qsamples = 1024; //array size for average sound level
    private const float defaultRms = 0.0001f; //RMS for 0dB, the smaller the float the higher the
    float[] samples; //audio samples
    public string microphoneName;
    //private List<float> dblist = new List<float>();
    private List<string> deviceOptions = new List<string>();
    private SpeechEnvironment[] environments = new SpeechEnvironment[3]{
        new SpeechEnvironment("normal", 1f, (n) => n <= 50),
        new SpeechEnvironment("moderate", 0.6f, (n) => n > 50 && n < 65),
        new SpeechEnvironment("extreme", 0.3f, (n) => n >= 65)
    };

    //DictationRecognizer dictationRecognizer;

    /*IEnumerator timer()
    {
        counter = 0;
        while(counter < dictationRecognizer.InitialSilenceTimeoutSeconds || counter < dictationRecognizer.AutoSilenceTimeoutSeconds){
            counter += Time.deltaTime;
            yield return null;
        }
        //when the timer reaches either timeout it will stop reading the volume
        CancelInvoke("GetVolume");
    }*/

    // Use this for initialization
    void Start()
    {
        // Initialize environment to prevent null environment
        currentEnvironment = environments[0];
        
        //dictationRecognizer = new DictationRecognizer();
        //dictationRecognizer.InitialSilenceTimeoutSeconds = 30f;
        if (audioSource == null) {
			audioSource = GetComponent<AudioSource>();
		}

        LoadMicrophone();
        samples = new float[Qsamples];
        //fSamples = AudioSettings.outputSampleRate;

        if (deviceOptions.Count > 0)
        {
			//todo perhaps passing null for unity to get the default microphone
            microphoneName = deviceOptions[deviceOptions.Count - 1];
			Debug.Log("selected microphone:"+ microphoneName);
            //StartCoroutine(timer());
            UpdateMicrophone();
        }
        else
        {
            Debug.Log("no microphone available");
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void UpdateMicrophone(){
        audioSource.Stop();
        audioSource.clip = Microphone.Start(microphoneName, true, 10, AudioSettings.outputSampleRate);

		if (Microphone.IsRecording(microphoneName)){
			while(!(Microphone.GetPosition(microphoneName)> 0)) {
				//wait until the recording has started
			}
			Debug.Log("recording start with: " + microphoneName );
            Debug.Log("now playing");
			audioSource.loop = true;
			audioSource.outputAudioMixerGroup = mixerGroup;
            audioSource.Play();
            InvokeRepeating("GetVolume", 0f, 4f);
            //audioSource.GetOutputData(samples, 0);
        } else {
            Debug.Log("microphone does not work");
        }
	}

    private void LoadMicrophone()
    {
        //detect no mic available
        foreach (string device in Microphone.devices)
        {
            deviceOptions.Add(device);
        }
    }

    /*when the mic is recording, audio samples will be kept in the array samples by GetOutputData.
    the values of the array will then be squared and added to find the RMS which is the square root
    of the average of the square of the values and then find the decibel*/
    private void GetVolume(){
        audioSource.GetOutputData(samples, 0);
        rmsValue = Mathf.Sqrt(samples.Select(x => Mathf.Pow(x, 2)).Average());
        decibelValue = 20 * Mathf.Log10(rmsValue / defaultRms);
        if(decibelValue < -160) decibelValue = -160; //to prevent results with infinity as value
        if(decibelValue != -160){
            foreach (var environment in environments) {
                if (environment.passes(decibelValue)) {
                    currentEnvironment = environment;
                    break;
                }
            }
            string message = string.Format("env. volume: {0}dB\nenvironment: {1}",
                decibelValue.ToString("0.00"),
                currentEnvironment.name
            );
            Debug.Log(message);
        }
    }
}
