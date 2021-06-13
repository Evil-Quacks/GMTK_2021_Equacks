
using System;
using UnityEngine;

[Serializable]
public class Narrative
{
    public int orderInStory;
    public string messageToShow;
    public NarrativeType moralType;
}

public enum NarrativeType
{
    DEFAULT,
    GOOD,
    BAD
}
