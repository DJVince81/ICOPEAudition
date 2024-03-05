using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFunction : MonoBehaviour
{
    internal abstract void Execute(List<string> args);
    internal abstract void Execute(List<object> args);
}
