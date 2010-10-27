// Copyright (c) 2009 David Koontz
// Please direct any bugs/comments/suggestions to david@koontzfamily.org
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Linq;

[CustomEditor(typeof(iTweenEvent))]
public class iTweenEventDataEditor : Editor {
	Dictionary<string, object> values;
	Dictionary<string, bool> propertiesEnabled = new Dictionary<string, bool>();
	iTweenEvent.TweenType previousType;
	
	[MenuItem("Component/iTween/iTweenEvent")]
    static void AddiTweenEvent () {
		if(Selection.activeGameObject != null) {
			Selection.activeGameObject.AddComponent(typeof(iTweenEvent));
		}
    }
	
	public void OnEnable() {
		var evt = (iTweenEvent)target;
		foreach(var key in EventParamMappings.mappings[evt.type].Keys) {
			propertiesEnabled[key] = false;
		}
		previousType = evt.type;
	}
	
	public override void OnInspectorGUI() {
		var evt = (iTweenEvent)target;
		values = evt.Values;
		var keys = values.Keys.ToArray();
		
		foreach(var key in keys) {
			propertiesEnabled[key] = true;
			if(typeof(Vector3OrTransform) == EventParamMappings.mappings[evt.type][key]) {
				var val = new Vector3OrTransform();
				
				if(null == values[key] || typeof(Transform) == values[key].GetType()) {
					if(null == values[key]) {
						val.transform = null;
					}
					else {
						val.transform = (Transform)values[key];
					}
					val.selected = Vector3OrTransform.transformSelected;
				}
				else if(typeof(Vector3) == values[key].GetType()) {
					val.vector = (Vector3)values[key];
					val.selected = Vector3OrTransform.vector3Selected;
				}
				
				values[key] = val;
			}
		}
		
		GUILayout.Label("iTween Event Editor v0.1");
		EditorGUILayout.Separator();
 		
		GUILayout.BeginHorizontal();
			evt.playAutomatically = GUILayout.Toggle(evt.playAutomatically, " Play Automatically");
		GUILayout.EndHorizontal();
		
		if(evt.playAutomatically) {
			GUILayout.BeginHorizontal();
			GUILayout.Label("Delay");
			evt.delay = float.Parse(GUILayout.TextField(evt.delay.ToString()));
			GUILayout.EndHorizontal();
		}
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
			GUILayout.Label("Event Type");
			evt.type = (iTweenEvent.TweenType)EditorGUILayout.EnumPopup(evt.type);
		GUILayout.EndHorizontal();
		
		if(evt.type != previousType) {
			foreach(var key in EventParamMappings.mappings[evt.type].Keys) {
				propertiesEnabled[key] = false;
			}
			evt.Values = new Dictionary<string, object>();
			previousType = evt.type;
			return;
		}
		
		var properties = EventParamMappings.mappings[evt.type];
		foreach(var pair in properties) {
			var key = pair.Key;
			
			GUILayout.BeginHorizontal();
			
			if(EditorGUILayout.BeginToggleGroup(key, propertiesEnabled[key])) {
				propertiesEnabled[key] = true;
				
				GUILayout.BeginVertical();
			
				if(typeof(string) == pair.Value) {
					values[key] = EditorGUILayout.TextField(values.ContainsKey(key) ? (string)values[key] : "");
				}
				else if(typeof(float) == pair.Value) {
					values[key] = EditorGUILayout.FloatField(values.ContainsKey(key) ? (float)values[key] : 0);
				}
				else if(typeof(int) == pair.Value) {
					values[key] = EditorGUILayout.IntField(values.ContainsKey(key) ? (int)values[key] : 0);
				}
				else if(typeof(bool) == pair.Value) {
					values[key] = propertiesEnabled[key];
				}
				else if(typeof(Vector3) == pair.Value) {
					values[key] = EditorGUILayout.Vector3Field("", values.ContainsKey(key) ? (Vector3)values[key] : Vector3.zero);
				}
				else if(typeof(Vector3OrTransform) == pair.Value) {
					if(!values.ContainsKey(key)) {
						values[key] = new Vector3OrTransform();
					}
					var val = (Vector3OrTransform)values[key];
					
					val.selected = GUILayout.SelectionGrid(val.selected, Vector3OrTransform.choices, 2);
	
					if(Vector3OrTransform.vector3Selected == val.selected) {
						val.vector = EditorGUILayout.Vector3Field("", val.vector);
					}
					else {
						val.transform = (Transform)EditorGUILayout.ObjectField(val.transform, typeof(Transform));
					}
					values[key] = val;
				}
				else if(typeof(iTween.LoopType) == pair.Value) {
					values[key] = EditorGUILayout.EnumPopup(values.ContainsKey(key) ? (iTween.LoopType)values[key] : iTween.LoopType.none);
				}
				else if(typeof(iTween.EaseType) == pair.Value) {
					values[key] = EditorGUILayout.EnumPopup(values.ContainsKey(key) ? (iTween.EaseType)values[key] : iTween.EaseType.linear);
				}
				else if(typeof(AudioSource) == pair.Value) {
					values[key] = (AudioSource)EditorGUILayout.ObjectField(values.ContainsKey(key) ? (AudioSource)values[key] : null, typeof(AudioSource));
				}
				else if(typeof(AudioClip) == pair.Value) {
					values[key] = (AudioClip)EditorGUILayout.ObjectField(values.ContainsKey(key) ? (AudioClip)values[key] : null, typeof(AudioClip));
				}
				else if(typeof(Color) == pair.Value) {
					values[key] = EditorGUILayout.ColorField(values.ContainsKey(key) ? (Color)values[key] : Color.white);
				}
				else if(typeof(Space) == pair.Value) {
					values[key] = EditorGUILayout.EnumPopup(values.ContainsKey(key) ? (Space)values[key] : Space.Self);
				}
				
				GUILayout.EndVertical();
			}
			else {
				propertiesEnabled[key] = false;
				values.Remove(key);
			}
			
			
			EditorGUILayout.EndToggleGroup();
			GUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}
		
		keys = values.Keys.ToArray();
		
		foreach(var key in keys) {
			if(values[key] != null && values[key].GetType() == typeof(Vector3OrTransform)) {
				var val = (Vector3OrTransform)values[key];
				if(Vector3OrTransform.vector3Selected == val.selected) {
					values[key] = val.vector;
				}
				else {
					values[key] = val.transform;
				}
			}
		}
		
		evt.Values = values;
		previousType = evt.type;
	}
}