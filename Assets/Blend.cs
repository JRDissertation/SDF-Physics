using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blend : MonoBehaviour
{
    public enum BlendType {
        None,
        Add,
        Subtract
    }
    public BlendType blendType;
    public Shape shape1;
    public Shape shape2;

}
