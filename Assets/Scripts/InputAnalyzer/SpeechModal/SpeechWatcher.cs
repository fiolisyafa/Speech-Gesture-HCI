using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
//using System.Speech.Recognition;
//TODO: change it into System.SpeechReognition for floating point confidence score
public class SpeechWatcher : MonoBehaviour, ISignalSender<Speech>
{
    public event Action<Speech> OnSignal;
    public AudioSource audioSource;

    public bool DebugResult = false;
    public bool DebugHypothesis = false;
    public string DebugTag = "SpeechWatcher";
    public DictationRecognizer dictationRecognizer;

    // Use this for initialization
    private void OnEnable()
    {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.InitialSilenceTimeoutSeconds = 30f;
        dictationRecognizer.DictationResult += DictationResult;
        dictationRecognizer.DictationHypothesis += DictationHypotesis;
        dictationRecognizer.DictationComplete += DictationComplete;
        dictationRecognizer.DictationError += DictationError;
        dictationRecognizer.Start();
        //SoundAnalyzer();
    }

    //TODO: confidence percentage
    private void DictationResult(string text, ConfidenceLevel confidence)
    {
        var confidenceValue = ResolveConfidenceLevel(confidence);
        if (DebugResult)
        {
            LoggerUtil.Log(DebugTag, "result -> text: " + text + " ; confidence: " + confidenceValue.ToString() + " (" + confidence.ToString() + ")");
        }
        if (OnSignal != null)
        {
            OnSignal(new Speech(text, ResolveConfidenceLevel(confidence)));
        }
    }

    private float ResolveConfidenceLevel(ConfidenceLevel confidence)
    {
        switch (confidence)
        {
            case ConfidenceLevel.High:
                return 0.9f;
            case ConfidenceLevel.Medium:
                return 0.6f;
            case ConfidenceLevel.Low:
                return 0.3f;
            default:
                return 0f;
        }
    }

    private void DictationHypotesis(string text)
    {
        if (DebugHypothesis)
        {
            // LoggerUtil.Log(DebugTag, "hypothesis -> text: " + text);
        }
    }

    private void DictationComplete(DictationCompletionCause cause)
    {
        // LoggerUtil.Log(DebugTag, "complete -> " + cause.ToString());
        
    }

    private void DictationError(string error, int hResult)
    {
        // LoggerUtil.LogError(DebugTag, "error ->" + error + " hresult: " + hResult);
    }

    private void OnDisable()
    {
        dictationRecognizer.DictationResult -= DictationResult;
        dictationRecognizer.DictationHypothesis -= DictationHypotesis;
        dictationRecognizer.DictationComplete -= DictationComplete;
        dictationRecognizer.DictationError -= DictationError;
        dictationRecognizer.Dispose();
    }

    void ISignalSender<Speech>.RegisterObserver(Action<Speech> onSignalCallBack)
    {
        OnSignal += onSignalCallBack;
    }

    void ISignalSender<Speech>.UnregisterObserver(Action<Speech> onSignalCallBack)
    {
        OnSignal -= onSignalCallBack;
    }
}
