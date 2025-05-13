using UnityEngine;

[CreateAssetMenu(fileName = "UI_Tags", menuName = "Scriptable Objects/UI_Tags")]
public class UI_Tags : ScriptableObject
{

    //----------- Tags ------------------------------------
    [Header("UI HUD Container tags")]
    [Tooltip("Tag")]
    public readonly string HUD_Property = "UI_HUD_Property";
    [Tooltip("Tag")]
    public readonly string HUD_Container = "UI_HUD_Container";
    [Tooltip("Tag")]
    public readonly string HUD = "UI_HUD";
}
