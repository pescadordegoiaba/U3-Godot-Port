using UnityEngine;
using UnityEngine.Events;

namespace TMPro;

public class TMP_Text : Behaviour
{
    public string text { get; set; } = string.Empty;

    public Color color { get; set; } = Color.white;

    public float fontSize { get; set; } = 14f;

    public TMP_FontAsset? font { get; set; }
}

public class TextMeshProUGUI : TMP_Text
{
}

public class TextMeshPro : TMP_Text
{
}

public class TMP_FontAsset : ScriptableObject
{
}

public class TMP_Dropdown : Behaviour
{
    private int _value;

    public int value
    {
        get => _value;
        set
        {
            _value = value;
            onValueChanged.Invoke(value);
        }
    }

    public UnityEvent<int> onValueChanged { get; } = new();
}

public class TMP_InputField : Behaviour
{
    private string _text = string.Empty;

    public string text
    {
        get => _text;
        set
        {
            _text = value;
            onValueChanged.Invoke(value);
        }
    }

    public UnityEvent<string> onValueChanged { get; } = new();
}
