using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ShadyPixel.CameraSystem;

public class CustomMenuItems {

    [MenuItem("GameObject/ShadyPixel/Prefab Spawner", false, -50)]
    static void CreatePrefabSpawner(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("NewPrefabSpawner");
        // Adds a PrefabSpawner component.
        go.AddComponent<PrefabSpawner>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/ShadyPixel/Camera Zone", false, -50)]
    static void CreateCameraZone(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("NewCameraZone");
        // Adds a PrefabSpawner component.
        go.AddComponent<Zone>();
        BoxCollider2D col = go.GetComponent<BoxCollider2D>();
        col.size = new Vector2(16f,10f);
        col.isTrigger = true;

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/ShadyPixel/Destructable", false, -1)]
    static void CreateDestructable(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("NewDestructable");
        // Adds a PrefabSpawner component.
        go.AddComponent<BoxCollider2D>();
        go.AddComponent<Destructable>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
    [MenuItem("GameObject/ShadyPixel/Entity", false, -1)]
    static void CreateEntity(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("NewEntity");
        // Adds a PrefabSpawner component.
        go.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        go.AddComponent<Entity>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }


}
