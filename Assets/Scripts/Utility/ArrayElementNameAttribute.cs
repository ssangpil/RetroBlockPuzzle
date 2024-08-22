using System;
using UnityEngine;

public class ArrayElementNameAttribute : PropertyAttribute
{
    public Array names;
    public int maxIndex;
    public int[] arrDisable = null;
    public ArrayElementNameAttribute(Type enum_type, int max_index, params int[] disable_index)
    {
        if( enum_type.IsEnum )
        {
            this.names = Enum.GetValues(enum_type);
            this.maxIndex = Mathf.Min( max_index, names.GetLength( 0 ) );
            this.arrDisable = disable_index;
        }
    }

    public bool IsDisableIndex( int index )
    {
        if( arrDisable != null )
        {
            for( int i = 0, _max = arrDisable.Length; i < _max; ++i )
            {
                if( arrDisable[i] == index ){
                    return true;
                }
            }
        }

        return false;
    }
}
#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer( typeof( ArrayElementNameAttribute ) )]
public class ElementTitleDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI( Rect position, UnityEditor.SerializedProperty property, GUIContent label )
    {
        int _pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
        var _config = attribute as ArrayElementNameAttribute;
               
        if( _pos < _config.maxIndex )
        {
            if( _config.IsDisableIndex( _pos ) == false )
            {
                UnityEditor.EditorGUI.PropertyField( position, property, new GUIContent( _config.names.GetValue( _pos ).ToString(), label.tooltip ), true );
            }
            else
            {
                UnityEditor.EditorGUI.BeginDisabledGroup( true );
                property.objectReferenceValue = null;
                UnityEditor.EditorGUI.PropertyField( position, property, new GUIContent( _config.names.GetValue( _pos ).ToString(), label.tooltip ), true );
                UnityEditor.EditorGUI.EndDisabledGroup();
            }
        }
        else
        {
            UnityEditor.EditorGUI.BeginDisabledGroup( true );
            property.objectReferenceValue = null;
            UnityEditor.EditorGUI.PropertyField( position, property, label );
            UnityEditor.EditorGUI.EndDisabledGroup();
        }
    }
}
#endif