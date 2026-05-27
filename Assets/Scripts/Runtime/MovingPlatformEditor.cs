using UnityEditor;
using UnityEngine;

[ CustomEditor ( typeof ( MovingPlatform ))]
public class MovingPlatformEditor : Editor
{
    private SerializedProperty start;
    private SerializedProperty end;
    private MovingPlatform platform;

    private void OnEnable () {
        this.platform = this.target as MovingPlatform;
   //     this.start = this.serializedObject.FindProperty( "start" );
     //   this.end = this.serializedObject.FindProperty( "end" );
    }
    
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        this.platform.platformSpeed = EditorGUILayout.Slider("Speed", this.platform.platformSpeed, 0.0f, 10.0f);

        GUILayout.Label("Debug");
        this.platform.editorPlatformPercent = EditorGUILayout.Slider(this.platform.editorPlatformPercent, 0.0f, 1.0f);
    }
    
    private void OnSceneGUI ()
    {
        Handles.color = Color.green;
        Handles.DrawLine( this.platform.start, this.platform.end);
        
        this.platform.start = Handles.PositionHandle( this.platform.start, Quaternion.identity);
        this.platform.end = Handles.PositionHandle( this.platform.end, Quaternion.identity);
    }


}