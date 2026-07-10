using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.UI;

public class Selectable : Behaviour
{
    public bool interactable { get; set; } = true;
}

public class Graphic : Behaviour
{
    public Color color { get; set; } = Color.white;

    public bool raycastTarget { get; set; } = true;
}

public class MaskableGraphic : Graphic
{
    public bool maskable { get; set; } = true;
}

public class Text : MaskableGraphic
{
    public string text { get; set; } = string.Empty;

    public int fontSize { get; set; } = 14;
}

public class Image : MaskableGraphic
{
    public Sprite? sprite { get; set; }

    public Texture? mainTexture { get; set; }
}

public class RawImage : MaskableGraphic
{
    public Texture? texture { get; set; }
}

public class Button : Selectable
{
    public ButtonClickedEvent onClick { get; } = new();

    public sealed class ButtonClickedEvent : UnityEvent
    {
    }
}

public class Toggle : Selectable
{
    private bool _isOn;

    public ToggleEvent onValueChanged { get; } = new();

    public bool isOn
    {
        get => _isOn;
        set
        {
            _isOn = value;
            onValueChanged.Invoke(value);
        }
    }

    public sealed class ToggleEvent : UnityEvent<bool>
    {
    }
}

public class Slider : Selectable
{
    private float _value;

    public float minValue { get; set; }

    public float maxValue { get; set; } = 1f;

    public SliderEvent onValueChanged { get; } = new();

    public float value
    {
        get => _value;
        set
        {
            _value = value;
            onValueChanged.Invoke(value);
        }
    }

    public sealed class SliderEvent : UnityEvent<float>
    {
    }
}

public class ScrollRect : Component
{
    public RectTransform? content { get; set; }

    public bool horizontal { get; set; } = true;

    public bool vertical { get; set; } = true;
}

public class Canvas : Behaviour
{
}

public class CanvasGroup : Behaviour
{
    public float alpha { get; set; } = 1f;

    public bool interactable { get; set; } = true;

    public bool blocksRaycasts { get; set; } = true;
}

public class LayoutElement : Behaviour
{
    public float preferredWidth { get; set; } = -1f;

    public float preferredHeight { get; set; } = -1f;

    public bool ignoreLayout { get; set; }
}

public class ContentSizeFitter : Behaviour
{
}

public class HorizontalLayoutGroup : Behaviour
{
}

public class VerticalLayoutGroup : Behaviour
{
}

public class GridLayoutGroup : Behaviour
{
}

public class InputField : Selectable
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
