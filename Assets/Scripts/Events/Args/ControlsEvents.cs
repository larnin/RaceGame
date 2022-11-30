using System.Collections;
using UnityEngine;

public class StartBoostEvent { }
public class EndBoostEvent { }

public class GetInputsEvent
{
    public float forward;
    public float steering;
    public bool boost;
}