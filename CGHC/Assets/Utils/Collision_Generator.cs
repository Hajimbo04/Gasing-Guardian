using UnityEngine;
using UnityEditor;

public class CollisionGeneratorWindow : EditorWindow
{
    bool usePolygon;

    [MenuItem("Tools/Collision Generator")]
    public static void ShowWindow()
    {
        GetWindow<CollisionGeneratorWindow>("Collision Generator");
    }

    void OnGUI()
    {
        usePolygon = EditorGUILayout.Toggle("Use Polygon", usePolygon);

        if (GUILayout.Button("Generate"))
        {
            if (Selection.activeGameObject == null) return;

            GameObject target = Selection.activeGameObject;
            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null) return;

            Sprite sprite = sr.sprite;

            GameObject go = new GameObject("GeneratedCollision");
            go.transform.SetParent(target.transform, false);

            if (usePolygon)
            {
                PolygonCollider2D poly = go.AddComponent<PolygonCollider2D>();
                poly.SetPath(0, GetSpritePath(sprite));
            }
            else
            {
                BoxCollider2D box = go.AddComponent<BoxCollider2D>();
                Bounds b = sprite.bounds;
                box.offset = b.center;
                box.size = b.size;
            }
        }
    }

    Vector2[] GetSpritePath(Sprite s)
    {
        Vector2 min = s.bounds.min;
        Vector2 max = s.bounds.max;
        return new Vector2[]
        {
            new Vector2(min.x, min.y),
            new Vector2(min.x, max.y),
            new Vector2(max.x, max.y),
            new Vector2(max.x, min.y)
        };
    }
}
