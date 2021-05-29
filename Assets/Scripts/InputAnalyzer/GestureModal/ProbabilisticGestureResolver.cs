/**
if (gesture == null)
        {
            LoggerUtil.LogError(DebugTag, "input signal is null");
            return null;
        }
        float currentTime = Time.time;

        float time = Time.time;
        List<UnifiedStructure> results = new List<UnifiedStructure>();

        List<KeyValuePair<Gesture, String>> matches = Database.FindLenientEqualsGesture(gesture);

        //TODO: better search algorithm?
        foreach (KeyValuePair<Gesture, string> entry in GestureDict)
        {
            if (gesture.LenientEquals(entry.Key))
            {
                if (DebugSignal) LoggerUtil.Log(DebugTag, "possible match: " + entry.Value);
                float confidence = ResolveConfidence(gesture, entry.Key);
                results.Add(new UnifiedStructure(time, entry.Value, confidence, time + threshold));
            }
        }
        string numberOfMatchMsg = "match found: " + matches.Count + "\n\n";
        string signalMsg = "";
        foreach (KeyValuePair<Gesture, String> item in matches)
        {
            float confidence = ResolveConfidence(gesture, item.Key);
            UnifiedStructure newItem = new UnifiedStructure(time, item.Value, confidence, time + threshold);
            results.Add(newItem);
            signalMsg += newItem.ToString() + "\n\n";
        }

        if (results.Count == 0)
        {
            if (DebugResult) LoggerUtil.Log(DebugTag, "no entry found");
            return null;
        }
        else
        {
            if (DebugResult) LoggerUtil.Log(DebugTag, numberOfMatchMsg);
            if (DebugSignal) LoggerUtil.Log(DebugTag, signalMsg);
            return FindNHighestConfidence(results, MaxGuesses);
        }
        
 */