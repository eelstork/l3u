using System.Collections.Generic;
using UnityEngine;

namespace L3{
public abstract class Branch : Node{

    [HideInInspector] public bool expanded;
    public abstract Node[] children { get; }

    public abstract void AddChild(Node arg);

}}
