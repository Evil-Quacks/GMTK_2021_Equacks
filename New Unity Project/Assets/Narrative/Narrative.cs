
using System;
using UnityEngine;

[Serializable]
public class Narrative
{
    int orderInStory;
    string messageToShow;
    
}

public enum NarrativeType
{
    DEFAULT,
    GOOD,
    BAD
}
