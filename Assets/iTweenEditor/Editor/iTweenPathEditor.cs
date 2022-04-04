//by Bob Berkebile : Pixelplacement : http://www.pixelplacement.com

using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;

[CustomEditor(typeof(iTweenPath))]
public class iTweenPathEditor : Editor
{
	iTweenPath _target;
	GUIStyle style = new GUIStyle();
	public static int count = 0;
	
	void OnEnable()
	{
		//i like bold handle labels since I'm getting old:
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		_target = (iTweenPath)target;
		
		//lock in a default path name:
		if(!_target.initialized){
			_target.initialized = true;
			_target.pathName = "New Path " + ++count;
			_target.initialName = _target.pathName;
		}

	}
	
	// fixed to UIElement version by jito.
	public override VisualElement CreateInspectorGUI()
	{

		var root = new VisualElement();

		var lPathName = new TextField("Path Name");
		lPathName.value = _target.pathName;

		lPathName.RegisterValueChangedCallback(x => _target.pathName = x.ToString());

		root.Add(lPathName);

		var lPathColor = new ColorField("Path Color");
		lPathColor.value = _target.pathColor;
		lPathColor.RegisterValueChangedCallback(x => _target.pathColor = x.newValue);

		root.Add(lPathColor);

		var lNodeSlider = new SliderInt("Node Count", 0, 20);
		lNodeSlider.value = _target.nodes.Count;
		//lNodeSlider.style. = 80%;
		//lNodeSlider.highValue = 20;



		var lNodeVolume = new IntegerField();
		lNodeVolume.value = _target.nodes.Count;
		lNodeVolume.style.width = 30;
		lNodeVolume.style.alignSelf = Align.FlexEnd;
		lNodeVolume.style.unityTextAlign = TextAnchor.UpperRight;
		lNodeSlider.Add(lNodeVolume);


		root.Add(lNodeSlider);

		var lNodes = new VisualElement();
		// ƒCƒ“ƒfƒ“ƒg
		lNodes.style.marginLeft = 20;
		root.Add(lNodes);

		//Debug.Log(mTestObject.mIntLists.Count);

		AddNodes(0, _target.nodes.Count, lNodes);


		lNodeVolume.RegisterValueChangedCallback(evt => lNodeSlider.value = evt.newValue);

		lNodeSlider.ReleaseMouse();

		lNodeSlider.RegisterCallback<MouseCaptureOutEvent>(evt =>
		{
			lNodeVolume.value = lNodeSlider.value;
			Debug.Log("mouse release.");
		});

		lNodeSlider.RegisterValueChangedCallback(evt =>
		{
			lNodeVolume.value = evt.newValue;
		});

		lNodeSlider.RegisterCallback<MouseCaptureOutEvent>(evt =>
		{
			int lListLength;
			if (lNodeSlider.value < 2)
			{
				lListLength = 2;
			}
			else if (20 < lNodeSlider.value)
			{
				lListLength = 20;
			}
			else
			{
				lListLength = lNodeSlider.value;
			}
			lNodeVolume.value = lListLength;
			//lLabel.text = evt.newValue.ToString();

			//add node?
			if (lListLength > _target.nodes.Count)
			{

				var lPreviousCount = _target.nodes.Count;
				AddNodes(_target.nodes.Count, lListLength, lNodes);
				for (int i = lPreviousCount; i < lListLength; i++)
				{
					_target.nodes.Add(Vector3.zero);
				}

			}


			//remove node?
			if (lListLength < _target.nodes.Count)
			{

				if (EditorUtility.DisplayDialog("Remove node?", "Shortening the node list will permantently destory parts of your path. This operation cannot be undone.", "OK", "Cancel"))
				{
					int lRemoveCount = _target.nodes.Count - lListLength;
					for (int i = _target.nodes.Count - 1; i > lListLength - 1; i--)
					{
						lNodes.RemoveAt(i);

					}

					_target.nodes.RemoveRange(_target.nodes.Count - lRemoveCount, lRemoveCount);
				}
				else
				{
					lNodeVolume.value = lListLength;
				}
			}


		});


		return root;
	}
	
	private void AddNodes(int pStart, int pEnd, VisualElement pRoot)
	{
		for (int i = pStart; i < pEnd; i++)
		{
			var node = new Vector3Field("Node " + (i + 1));
			if(i < _target.nodes.Count)
            {
				node.value = _target.nodes[i];
			}
			int count = i;
			node.RegisterValueChangedCallback(x => {
				_target.nodes[count] = x.newValue;
			});
			node.SetEnabled(true);
			//_target.nodes[i] = node.value;
			pRoot.Add(node);
		}

	}

	void OnSceneGUI()
	{
		if (_target.enabled)
		{ // dkoontz
			if (_target.nodes.Count > 0)
			{
				//allow path adjustment undo:
				Undo.RecordObject(_target, "Adjust iTween Path");

				// start arclee
				//parent.
				Handles.Label(_target.transform.position, "'" + _target.pathName + "' Parent", style);

				//path begin and end labels:
				Vector3 labb = _target.transform.TransformPoint(_target.nodes[0]);
				Vector3 labe = _target.transform.TransformPoint(_target.nodes[_target.nodes.Count - 1]);
				Handles.Label(labb, "'" + _target.pathName + "' Begin", style);
				Handles.Label(labe, "'" + _target.pathName + "' End", style);

				//node handle display:
				for (int i = 0; i < _target.nodes.Count; i++)
				{
					Vector3 fp = _target.transform.TransformPoint(_target.nodes[i]);
					//_target.nodes[i] = Handles.PositionHandle(fp, _target.transform.rotation);

					Vector3 ivp = Handles.PositionHandle(fp, _target.transform.rotation);
					_target.nodes[i] = _target.transform.InverseTransformPoint(ivp);
				}
				// end arclee
			}
		} // dkoontz
	}
}