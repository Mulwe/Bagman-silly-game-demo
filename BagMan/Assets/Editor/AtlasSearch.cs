using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

public class AtlasSearch : MonoBehaviour
{
    [MenuItem("Tools/Find Sprite Atlases")]
    public static void FindAtlases()
    {
        var guids = AssetDatabase.FindAssets("t:SpriteAtlas");
        if (guids.Length == 0)
        {
            Debug.Log("❌ Sprite Atlases not found in Assets folder.");
        }
        else
        {
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"✅ Found Sprite Atlas: {path}");
            }
        }
    }
}
#endif
