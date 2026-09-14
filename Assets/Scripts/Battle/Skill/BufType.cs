using UnityEngine;

public class BufType: MonoBehaviour
{
    public enum Type
    {
        _UNDEFINED = 0, attackBuf = 1, attackDebuf = 2, burning = 3, focus = 4, protect = 5, stun = 8
    }

    public Type type = Type._UNDEFINED;
}
