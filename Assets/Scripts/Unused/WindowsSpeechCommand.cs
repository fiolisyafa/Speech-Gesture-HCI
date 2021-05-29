using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class WindowsSpeechCommand : MonoBehaviour
{

    private KeywordRecognizer keywordRecognizer;

    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    // Use this for initialization
    void Start()
    {
        actions.Add("hello", Hello);
        actions.Add("world", World);
        actions.Add("good", Good);
        actions.Add("morning", Morning);
        actions.Add("a", A);
        actions.Add("test", Test);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;

        Debug.Log("keyword recognizer starts");
        keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Hello()
    {
        Debug.Log("hello");
    }

    private void World()
    {
        Debug.Log("world");
    }

    private void Good()
    {
        Debug.Log("good");
    }

    private void Morning()
    {
        Debug.Log("morning");
    }
    private void A()
    {
        Debug.Log("a");
    }

    private void Test()
    {
        Debug.Log("test");
    }
}
