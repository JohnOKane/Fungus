﻿using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

[CustomEditor (typeof(Sequence))]
public class SequenceEditor : Editor 
{

	static public Sequence SequenceField(GUIContent label, FungusScript fungusScript, Sequence sequence)
	{
		if (fungusScript == null)
		{
			return null;
		}
		
		Sequence result = sequence;
		
		// Build dictionary of child sequences
		List<GUIContent> sequenceNames = new List<GUIContent>();
		
		int selectedIndex = 0;
		sequenceNames.Add(new GUIContent("<None>"));
		Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>();
		for (int i = 0; i < sequences.Length; ++i)
		{
			sequenceNames.Add(new GUIContent(sequences[i].name));
			
			if (sequence == sequences[i])
			{
				selectedIndex = i + 1;
			}
		}
		
		selectedIndex = EditorGUILayout.Popup(label, selectedIndex, sequenceNames.ToArray());
		if (selectedIndex == 0)
		{
			result = null; // Option 'None'
		}
		else
		{
			result = sequences[selectedIndex - 1];
		}
		
		return result;
	}
}