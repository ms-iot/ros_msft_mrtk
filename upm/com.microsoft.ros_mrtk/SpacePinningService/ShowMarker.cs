// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

//#define BARN_DISABLE_AT_END

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Simple animation to highlight where a QR code has been scanned.
/// </summary>
public class ShowMarker : MonoBehaviour
{
    [Tooltip("Number of seconds to ramp up at activation.")]
    public float rampUp = 1.0f;

    [Tooltip("Number of seconds to ramp down after finishing ramp up.")]
    public float rampDown = 5.0f;

    /// <summary>
    /// Maximum scale factor to apply, i.e. scale at end of ramp up and beginning of ramp down.
    /// </summary>
    public float maxSize = 4.0f;

    /// <summary>
    /// Number of seconds animation has been going.
    /// </summary>
    private float age = 0.0f;

    /// <summary>
    /// Cache for material to animate and relevant properties.
    /// </summary>
    private struct AnimMat
    {
        public Material material;

        public Color diffuse;

        public Color emissive;

    }

    /// <summary>
    /// List of materials to animate.
    /// </summary>
    private readonly SortedSet<AnimMat> materials = new SortedSet<AnimMat>();

    /// <summary>
    /// Initial non-animated scale this object.
    /// </summary>
    private Vector3 initScale;

    /// <summary>
    /// Shader tag for the diffuse color.
    /// </summary>
    private readonly string diffuseName = "_Color";

    /// <summary>
    /// Shader tag for the emissive color.
    /// </summary>
    private readonly string emissiveName = "_EmissiveColor";

    /// <summary>
    /// Cache the materials to animate and anything else worth saving.
    /// </summary>
    private void Start()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            var rend = renderers[i];
            AnimMat animMat = new AnimMat()
            {
                material = rend.material,
                diffuse = rend.material.GetColor(diffuseName),
                emissive = rend.material.GetColor(emissiveName),
            };
            rend.material.SetColor(diffuseName, Color.black);
            SetIntensity(0.0f, animMat);
            materials.Add(animMat);
        }
        initScale = transform.localScale;
        age = 0.0f;
    }

    /// <summary>
    /// Apply the time based animation.
    /// </summary>
    void Update()
    {
        if (age < rampUp)
        {
            RampUp(age);
        }
        else if (age < rampUp + rampDown)
        {
            RampDown(age);
        }
        else
        {
            SetStatic();
        }
        age += Time.deltaTime;
    }

    /// <summary>
    /// Turn off the animation at the end.
    /// </summary>
    private void SetStatic()
    {
#if BARN_DISABLE_AT_END
    gameObject.SetActive(false);
#else
        for (var iter = materials.GetEnumerator(); iter.MoveNext();)
        {
            AnimMat animMat = iter.Current;
            Color emissive = animMat.diffuse;
            animMat.material.SetColor(emissiveName, emissive);
        }
#endif
    }

    /// <summary>
    /// Standard hermite interpolation. 
    /// </summary>
    /// <param name="zero">Time at which to have value of zero.</param>
    /// <param name="one">Time at which to have value of one.</param>
    /// <param name="t">Time value.</param>
    /// <returns>Value in range [0..1]</returns>
    private float SmoothStep(float zero, float one, float t)
    {
        t = t - zero;
        t = t / (one - zero);
        if (t <= 0)
            return 0;
        if (t >= 1)
            return 1;
        return (3 - 2 * t) * t * t;
    }

    /// <summary>
    /// Ramp up the animation at beginning.
    /// </summary>
    /// <param name="age"></param>
    private void RampUp(float age)
    {
        float intensity = SmoothStep(0.0f, 1.0f, age / rampUp);
        for (var iter = materials.GetEnumerator(); iter.MoveNext();)
        {
            AnimMat animMat = iter.Current;
            SetIntensity(intensity, animMat);
        }
        SetSizeFromAge(age);
    }

    /// <summary>
    /// Ramp down the animation at the end.
    /// </summary>
    /// <param name="age"></param>
    private void RampDown(float age)
    {
        float intensity = SmoothStep(0.0f, 1.0f, 1.0f - (age - rampUp) / rampDown);
        for (var iter = materials.GetEnumerator(); iter.MoveNext();)
        {
            AnimMat animMat = iter.Current;

            BlendIntensity(intensity, animMat);
        }
        SetSizeFromAge(age);
    }

    /// <summary>
    /// Set the scale for the object (subtree root) based on its age.
    /// </summary>
    /// <param name="age"></param>
    private void SetSizeFromAge(float age)
    {
        float t = age / (rampUp + rampDown);
        t *= 2.0f;
        t -= 1.0f;
        t = Mathf.Abs(t);
        t = 1.0f - t;
        t = 1.0f + Mathf.Pow(t - 1.0f, 3.0f);
        float minSize = 1.0f;
        float size = minSize + t * (maxSize - minSize);

        SetSize(size);
    }

    /// <summary>
    /// Apply scale based on original scale.
    /// </summary>
    /// <param name="scale">Relative scale.</param>
    private void SetSize(float scale)
    {
        transform.localScale = initScale * scale;
    }

    /// <summary>
    /// Set the emissive color to intensity.
    /// </summary>
    /// <param name="intensity">Intensity to set emissive.</param>
    /// <param name="animMat">Which material to set.</param>
    private void SetIntensity(float intensity, AnimMat animMat)
    {
        Color emissive = animMat.emissive * intensity;
        animMat.material.SetColor(emissiveName, emissive);
    }

    /// <summary>
    /// Interpolate between original emissive and diffuse colors based on intensity.
    /// </summary>
    /// <param name="intensity">Interpolant, as intensity goes to one, color goes to diffuse.</param>
    /// <param name="animMat">Which material to set.</param>
    private void BlendIntensity(float intensity, AnimMat animMat)
    {
        Color emissive = animMat.emissive * intensity + animMat.diffuse * (1.0f - intensity);
        animMat.material.SetColor(emissiveName, emissive);
    }

}
