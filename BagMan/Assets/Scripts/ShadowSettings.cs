using UnityEngine;

[CreateAssetMenu(fileName = "ShadowSettings", menuName = "Scriptable Objects/ShadowSettings")]
public class ShadowSettings : ScriptableObject
{
    [SerializeField] private Sprite _shadowSprite;
    [SerializeField] private Material _material;

    public Vector3 Offset = new Vector3(0f, -0.3f);
    public float Width = 0.5f;
    public float Height = 0.8f;

    public float ShadowIntensity = 0.7f;

    public Material Material => this._material;
    public Sprite ShadowSprite => this._shadowSprite;



}
