using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity.Attributes;
using UnityEngine;


//TODO: create abstract class/interface for all resolver types
public class SpeechResolver : MonoBehaviour, IModal, ISignalReceiver
{

    [Tooltip("threshold")]
    [Units("seconds")]
    public float threshold = 1f;

    [Tooltip("interval in seconds to resolve the captured gesture")]
    [Units("seconds")]
    public float period = .1f;
    public ISignalSender<Speech> SpeechWatcher;

    public bool DebugResult = false;
    public string DebugTag = "speechResolver";
    public string SignalSourceId = "VOICE";

    private Queue<Speech> SpeechStates = new Queue<Speech>();
    private List<UnifiedStructure> LatestSignal;
    private event Action<List<UnifiedStructure>> OnSignal;
    private SpeechInput speechInput;

    IEnumerator ResolverJob;
    public SignalDatabase Database;
    //TODO: Load from external database source
    Dictionary<Speech, string> SpeechDict = new Dictionary<Speech, string>();

    private void Awake()
    {
        this.speechInput = GameObject.FindObjectOfType<SpeechInput>();

        SpeechDict = Database.GetSpeechDictionary();
        SpeechWatcher = GetComponents<ISignalSender<Speech>>()[0] as ISignalSender<Speech>;

        // Speech approvalSpeech = new Speech("yes");
        // Database.Add(approvalSpeech, "approve");

        ResolverJob = SpeechResolverJob();
    }

    IEnumerator SpeechResolverJob()
    {
        while (true)
        {
            //if(SpeechStates.Count == 0) Debug.Log("receiving signal...");
            if (SpeechStates.Count > 0)
            {
                Speech currentState = SpeechStates.Dequeue();
                LatestSignal = Resolve(currentState);
                    if (LatestSignal != null)
                    {
                        if (DebugResult) LoggerUtil.Log(DebugTag, "latest signal: \n" + LatestSignal[0].ToString());
                    }
                
            }

            yield return new WaitForSeconds(period);
        }

    }

    private List<UnifiedStructure> Resolve(Speech speech)
    {

        if (speech == null)
            return null;
            
        if (SpeechDict.ContainsKey(speech))
        {
            string semantic = SpeechDict[speech];
            float timeStamp = Time.time;
            double environmentWeight = speechInput.currentEnvironment.weight;
            var item = new UnifiedStructure(SignalSourceId, timeStamp, semantic, speech.Confidence, timeStamp + threshold, environmentWeight);
            return new List<UnifiedStructure> { item }; //TODO: definetely bad performance
        }
        else
        {
            return null;
        }

    }
    private void SpeechWatcher_onSignal(Speech newSpeech)
    {
        SpeechStates.Enqueue(newSpeech);
    }

    private void OnEnable()
    {
        SpeechWatcher.RegisterObserver(SpeechWatcher_onSignal);
        StartCoroutine(ResolverJob);
    }

    private void OnDisable()
    {
        SpeechWatcher.UnregisterObserver(SpeechWatcher_onSignal);
        StopCoroutine(ResolverJob);
        SpeechStates.Clear();
    }

    void ISignalSender<List<UnifiedStructure>>.RegisterObserver(Action<List<UnifiedStructure>> onSignalCallBack)
    {
        OnSignal += onSignalCallBack;
    }

    void ISignalSender<List<UnifiedStructure>>.UnregisterObserver(Action<List<UnifiedStructure>> onSignalCallBack)
    {
        OnSignal -= onSignalCallBack;
    }

    public List<UnifiedStructure> GetLatestSignal()
    {
        if (LatestSignal == null)
        {
            return null;
        }
        else
        {
            //TODO: definetely bad performance
            UnifiedStructure[] latest = new UnifiedStructure[LatestSignal.Count];
            LatestSignal.CopyTo(latest, 0);
            LatestSignal = null;

            return latest.ToList();
        }
    }
}