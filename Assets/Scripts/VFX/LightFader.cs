using UnityEngine;

public sealed class LightFader : MonoBehaviour
{
    public float Duration = 0.18f;
    public AnimationCurve Curve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    Light _light;
    float _startIntensity;
    float _t;

    void Awake()
    {
        _light = GetComponent<Light>();
        if (_light != null) _startIntensity = _light.intensity;
    }

    void Update()
    {
        if (_light == null) return;

        _t += Time.deltaTime;
        var t01 = Duration <= 0f ? 1f : Mathf.Clamp01(_t / Duration);
        _light.intensity = _startIntensity * Mathf.Clamp01(Curve.Evaluate(t01));
        if (t01 >= 1f) Destroy(this);
    }
}

