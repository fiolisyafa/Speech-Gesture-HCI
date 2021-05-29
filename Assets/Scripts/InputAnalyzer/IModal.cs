using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModal : ISignalSender<List<UnifiedStructure>>
{
    List<UnifiedStructure> GetLatestSignal();
}