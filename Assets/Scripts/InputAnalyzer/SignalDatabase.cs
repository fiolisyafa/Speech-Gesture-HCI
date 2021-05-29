using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SignalDatabase : MonoBehaviour
{
    [Serializable]
    public class GestureEntry
    {
        public string Meaning;
        public GestureData[] GestureData;
    }

    [Serializable]
    public class SpeechEntry
    {
        public string Meaning;
        public SpeechData[] SpeechData;
    }

    public GestureEntry[] GestureEntryList;
    public SpeechEntry[] SpeechEntryList;

    private List<String> SpeechMeaning = new List<String>();
    private List<List<Speech>> SpeechList = new List<List<Speech>>();

    private List<String> GestureMeaning = new List<String>();
    private List<List<Gesture>> GestureList = new List<List<Gesture>>();

    private Dictionary<Gesture, string> GestureDict = new Dictionary<Gesture, string>();
    private Dictionary<Speech, string> SpeechDict = new Dictionary<Speech, string>();

    private void Awake()
    {

        PopulateGestureDict();
        PopulateSpeechDict();
        // PopulateSpeechList();
        // PopulateGestureList();
    }

    private void PopulateSpeechDict()
    {
        for (int i = 0; i < SpeechEntryList.Length; i++)
        {
            SpeechEntry currentEntry = SpeechEntryList[i];

            Speech firstData = new Speech(currentEntry.SpeechData[0]);
            if(!SpeechDict.ContainsKey(firstData)){
                SpeechDict.Add(firstData, currentEntry.Meaning);
            }
        }
    }

    private void PopulateGestureDict()
    {
        for (int i = 0; i < GestureEntryList.Length; i++)
        {
            GestureEntry currentEntry = GestureEntryList[i];

            Gesture firstData = new Gesture(currentEntry.GestureData[0]);
            if(!GestureDict.ContainsKey(firstData)){
                GestureDict.Add(firstData, currentEntry.Meaning);
            }
        }
    }

    private void PopulateSpeechList()
    {
        for (int i = 0; i < SpeechEntryList.Length; i++)
        {
            SpeechEntry currentEntry = SpeechEntryList[i];

            List<Speech> speechList = new List<Speech>(currentEntry.SpeechData.Length);
            for (int j = 0; j < currentEntry.SpeechData.Length; j++)
            {
                speechList[j] = new Speech(currentEntry.SpeechData[j]);
            }

            SpeechMeaning.Add(currentEntry.Meaning);
            SpeechList.Add(speechList);
        }
    }

    private void PopulateGestureList()
    {
        
        for (int i = 0; i < GestureEntryList.Length; i++)
        {
            GestureEntry currentEntry = GestureEntryList[i];

            List<Gesture> gestureList = new List<Gesture>(currentEntry.GestureData.Length);
            for (int j = 0; j < currentEntry.GestureData.Length; j++)
            {
                gestureList[j] = new Gesture(currentEntry.GestureData[j]);
            }
            GestureMeaning.Add(currentEntry.Meaning);
            GestureList.Add(gestureList);
        }
    }

    public Dictionary<Gesture, string> GetGestureDictionary()
    {
        return GestureDict;
    }

    public Dictionary<Speech, string> GetSpeechDictionary()
    {
        return SpeechDict;
    }

    public List<KeyValuePair<Speech, String>> FindEqualsSpeech(Speech speechSignal)
    {
        List<KeyValuePair<Speech, String>> matching = new List<KeyValuePair<Speech, String>>();
        for (int i = 0; i < SpeechList.Count; i++)
        {
            List<Speech> subList = SpeechList[i];
            for (int j = 0; j < subList.Count; j++)
            {
                if (speechSignal.Equals(subList[j]))
                {
                    matching.Add(new KeyValuePair<Speech, String>(
                        subList[j],
                        SpeechMeaning[i]
                    ));
                }
            }
        }

        return matching;
    }

    public List<KeyValuePair<Gesture, String>> FindLenientEqualsGesture(Gesture gestureSignal)
    {
        List<KeyValuePair<Gesture, String>> matching = new List<KeyValuePair<Gesture, String>>();
        for (int i = 0; i < GestureList.Count; i++)
        {
            List<Gesture> subList = GestureList[i];
            for (int j = 0; j < subList.Count; j++)
            {
                if (gestureSignal.LenientEquals(subList[j]))
                {
                    matching.Add(new KeyValuePair<Gesture, String>(
                        subList[j],
                        GestureMeaning[i]
                    ));
                }
            }
        }

        return matching;
    }
}
