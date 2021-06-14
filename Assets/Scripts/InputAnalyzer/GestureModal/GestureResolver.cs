using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap;
using Leap.Unity.Attributes;
using UnityEngine;

public class GestureResolver : MonoBehaviour, IModal, ISignalReceiver
{

    [Tooltip("threshold")]
    [Units("seconds")]
    public float threshold = 4f;

    [Tooltip("interval in seconds to resolve the captured gesture")]
    [Units("seconds")]
    public float period = .3f;

    public bool DebugResult = false;
    public bool DebugMatchedSignal = false;
    public bool DebugAllSignal = false;
    public bool DebugMatchingProcess = false;
    public string SignalSourceId = "LEAP";

    public int MaximumGuesses = 3;
    public string DebugTag = "GestureResolver";

    public ISignalSender<Gesture> GestureWatcher;

    private Queue<Gesture> States = new Queue<Gesture>();

    private event Action<List<UnifiedStructure>> OnSignal;

    private List<UnifiedStructure> LatestSignal;

    IEnumerator ResolverJob;
    public SignalDatabase Database;
    private Dictionary<Gesture, string> GestureDict;
    bool statement;
    private Controller Controller;
    private Device Device;
    private GestureEnvironment environment;
    private GestureEnvironment[] environments = new GestureEnvironment[3]{
        new GestureEnvironment("normal", 1f, (s, l) => !s && !l),
        new GestureEnvironment("moderate", 0.6f, (s, l) => s || l),
        new GestureEnvironment("extreme", 0.3f, (s, l) => s && l)
    };

    /*ExtendedFingerState thumbExt = new ExtendedFingerState(
        new PointingState[]{
            PointingState.Extended,
            PointingState.NotExtended,
            PointingState.NotExtended,
            PointingState.NotExtended,
            PointingState.NotExtended,
        });

     ExtendedFingerState indexExt = new ExtendedFingerState(
        new PointingState[]{
            PointingState.NotExtended,
            PointingState.Extended,
            PointingState.NotExtended,
            PointingState.NotExtended,
            PointingState.NotExtended,
        });*/

    private void Awake()
    {
        //GestureDict.Clear();
        //StartCoroutine(Initialize(0.5f));
        GestureDict = Database.GetGestureDictionary();
        ResolveSignalSenders();
        ResolverJob = GestureResolverJob();
        Controller = new Controller();
    }

    private void OnEnable()
    {
        GestureWatcher.RegisterObserver(Gesture_onSignal);
        StartCoroutine(ResolverJob);
    }
    private void OnDisable()
    {
        GestureWatcher.UnregisterObserver(Gesture_onSignal);
        StopCoroutine(ResolverJob);
    }

    private List<UnifiedStructure> Resolve(Gesture gesture)
    {
        if (Controller.Devices.Count > 0 && Device == null) {
            Device = Controller.Devices[0];
        }

        if (gesture == null)
        {
            LoggerUtil.LogError(DebugTag, "input signal is null");
            return null;
        }

        if (DebugAllSignal)
        {
            LoggerUtil.Log(DebugTag, "new signal:\n" + gesture.ToString());
        }

        if (GestureDict == null || GestureDict.Count == 0)
        {
            LoggerUtil.LogError(DebugTag, "GestureList count is null");
            return null;
        }

        foreach (GestureEnvironment environment in environments)
        {
            if (environment.passes(Device.IsSmudged, Device.IsLightingBad)) {
                this.environment = environment;
                break;
            }
        }

        float currentTime = Time.time;

        float time = Time.time;
        List<UnifiedStructure> results = new List<UnifiedStructure>();

        //TODO: better search algorithm?

        foreach (KeyValuePair<Gesture, string> entry in GestureDict)
        {
            bool extendedFingerPresent = (gesture.ExtendedFingerState.Equals(entry.Key.ExtendedFingerState)) ? true : false;
            bool fingerDirectionPresent = gesture.FingerDirection != null
                ? gesture.FingerDirection.Equals(entry.Key.FingerDirection)
                : false;
            if (extendedFingerPresent && fingerDirectionPresent)
            {
                float confidence = ResolveConfidence(gesture, entry.Key) * this.environment.weight;
                results.Add(new UnifiedStructure(SignalSourceId, time, entry.Value, confidence, time + threshold));
                if (DebugMatchingProcess)
                {
                    LoggerUtil.Log(DebugTag,
                    "possible match: " + entry.Value +
                    "\nconfidence: " + confidence
                    );
                }

            }
        }

        if (results.Count == 0)
        {
            if (DebugAllSignal || DebugMatchingProcess) LoggerUtil.Log(DebugTag, "no entry found");
            if (DebugAllSignal) LoggerUtil.Log(DebugTag, gesture.ToString());
            return null;
        }
        else
        {
            var selectedGuesses = GetNHighestConfidence(results, MaximumGuesses);
            if (DebugAllSignal || DebugMatchedSignal || DebugMatchingProcess)
            {
                string guessMsg = "  entry(s) found\n\ngesture:\n" + gesture.ToString() + "\n\nhypothesis:\n";
                foreach (var item in selectedGuesses)
                {
                    guessMsg += item.ToString() + "\n\n";
                }
                LoggerUtil.Log(DebugTag, guessMsg);
                if(selectedGuesses.Count > 1)
                {
                    guessComparison(selectedGuesses, gesture, GestureDict);
                }
            }
            return selectedGuesses;
        }
    }

    /*- search through and compare the current gesture's finger state and palm direction 
        to the entries stored in the database.
      - then, it will find the guess with the correct semantic.
      - if the guess have incorrect semantic, it will be discarded from the guess list */
    //not really the best way to do it tbh
    private void guessComparison(List<UnifiedStructure> list, Gesture g, Dictionary<Gesture, string> dict)
    {
        string msg = "multiple guesses found\nclosest guess: ";
        //List<KeyValuePair<Gesture,string>> temp = dict.ToList<KeyValuePair<Gesture,string>>();
        foreach(KeyValuePair<Gesture,string> item in dict)
        {
            var efs = item.Key.ExtendedFingerState;
            var pd = item.Key.PalmDirection;
            var fd = item.Key.FingerDirection;
            if(g.PalmDirection.Equals(pd) && g.ExtendedFingerState.Equals(efs) && g.FingerDirection.Equals(fd))
            {
               var largerConfidence = list.Aggregate((x, y) => x.SemanticMeaning.Equals(item.Value) ? x : y);
               msg += largerConfidence.SemanticMeaning + " | confidence: " + largerConfidence.ConfidenceLevel.ToString();
               list.RemoveAll(z => !z.SemanticMeaning.Equals(item.Value));
               LoggerUtil.Log(DebugTag, msg);
            }

        }
    }

    //TODO: not scalable, not SRP
    private float ResolveConfidence(Gesture input, Gesture databaseEntry)
    {
        string message = "found db entry, resolving confidence of:\n" + input.ToString()
             + "\n\ndb entry:\n" + databaseEntry.ToString() + "\n";

        float confidence = 0f;

        if (input.ExtendedFingerState.Equals(databaseEntry.ExtendedFingerState))
        {
            confidence += databaseEntry.ExtendedFingerStateWeight;
            message += "from efs: " + confidence;
        }

        if (input.PalmDirection.Equals(databaseEntry.PalmDirection))
        {
            confidence += databaseEntry.PalmDirectionWeight;
            message += "from pds: " + confidence;
        }

        Debug.Log(input);
        Debug.Log(databaseEntry);

        if (input.FingerDirection.Equals(databaseEntry.FingerDirection))
        {
            confidence += databaseEntry.FingerDirectionWeight;
            message += "from fd: " + confidence;
        }

        if (DebugMatchingProcess)
        {
            LoggerUtil.Log(DebugTag, message);
        }

        return confidence;
    }

    /*private bool isGestureComplete(Gesture g, Gesture db)
    {
        //bool statement = false; //default value
        if(!g.Equals(db))
        {
            statement = false;
        }
        else
        {
            statement = true;
        }
        return statement;
    }*/
    private UnifiedStructure FindHighestConfidence(List<UnifiedStructure> list)
    {
        if (list == null || list.Count <= 0) return null;
        List<UnifiedStructure> sorted = list.OrderByDescending(i => i.ConfidenceLevel).ToList();
        return sorted.First();
    }

    private List<UnifiedStructure> GetNHighestConfidence(List<UnifiedStructure> list, int n)
    {
        if (list == null || list.Count <= 0) return null;
        List<UnifiedStructure> sorted = list
            .OrderByDescending(i => i.ConfidenceLevel)
            .Take(n)
            .ToList();
        return sorted;
    }

    private void Gesture_onSignal(Gesture newState)
    {
        States.Enqueue(newState);

    }

    //TODO: passing IEnumerator instance in the queue to have more generic states
    IEnumerator GestureResolverJob()
    {
        while (true)
        {
            if (States.Count > 0)
            {
                Gesture g = States.Dequeue();

                //LatestSignal.Clear(); TODO: garbage collection? performance?

                LatestSignal = Resolve(g);
            }

            yield return new WaitForSeconds(period);
        }

    }

    private void OnDestroy()
    {
        //TODO: do we need to clear them manually?
        States.Clear();
    }

    private void ResolveSignalSenders()
    {
        GestureWatcher = GetComponents<ISignalSender<Gesture>>()[0] as ISignalSender<Gesture>;
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
