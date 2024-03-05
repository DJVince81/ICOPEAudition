using System.Collections.Generic;
using UnityEngine;

public class ShowTextFunction : BaseFunction
{
    internal override void Execute(List<string> args)
    {
        if (args != null)
        {
            for (int i = 0; i < args.Count; i++)
            {
                Debug.Log(args[i]);
            }
        }
    }

    internal override void Execute(List<object> args)
    {
        if (args != null)
        {
            for (int i = 0; i < args.Count; i++)
            {
                if (args[i].GetType() == typeof(Clickable.Args))
                {
                    Clickable.Args clickableArgs = (Clickable.Args)args[i];
                    Debug.Log(clickableArgs.type.ToString() + " : " + clickableArgs.GetValue<string>());
                }
                else
                {
                    Debug.Log(args[i]);
                }
            }
        }
    }
}
