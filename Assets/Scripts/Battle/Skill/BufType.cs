using UnityEngine;

public class BufType: MonoBehaviour
{
    public enum Type
    {
        _UNDEFINED, attackBuf,attackDebuf, burning, focus, protect, speeddown, speedup, stun
    }

    public Type type=Type._UNDEFINED;
}