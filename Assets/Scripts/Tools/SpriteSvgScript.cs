using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VectorGraphics;
using UnityEngine;

[ExecuteInEditMode]
public class BaseSvgScript : MonoBehaviour
{
    [Multiline()] public string svg = "";
    public int pixelsPerUnit;
    public bool flipYAxis;

    protected List<VectorUtils.Geometry> GetGeometries()
    {
        using var textReader = new StringReader(svg);
        var sceneInfo = SVGParser.ImportSVG(textReader);

        return VectorUtils.TessellateScene(sceneInfo.Scene, new VectorUtils.TessellationOptions
        {
            StepDistance = 10,
            SamplingStepSize = 100,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f
        });
    }
}


[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSvgScript : BaseSvgScript
{
    void Update()
    {
        List<VectorUtils.Geometry> geometries = GetGeometries();

        var sprite = VectorUtils.BuildSprite(geometries, pixelsPerUnit, VectorUtils.Alignment.Center, Vector2.zero, 128, flipYAxis);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
