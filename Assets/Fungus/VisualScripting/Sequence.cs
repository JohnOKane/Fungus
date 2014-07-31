#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class Sequence : MonoBehaviour 
	{
		public string titleText;

		[HideInInspector]
		public Rect nodeRect = new Rect(10, 10, 100, 100);
	
		[System.NonSerialized]
		public FungusScript fungusScript;

		[System.NonSerialized]
		public FungusCommand activeCommand;

		public virtual void Start()
		{
			fungusScript = GetFungusScript();
		}

		public FungusScript GetFungusScript()
		{
			FungusScript sc = null;
			Transform parent = transform.parent;		
			while (parent != null)
			{
				sc = parent.gameObject.GetComponent<FungusScript>();
				if (sc != null)
				{
					break;
				}
			}
			return sc;
		}

		public bool HasError()
		{
			FungusCommand[] commands = GetComponents<FungusCommand>();
			foreach (FungusCommand command in commands)
			{
				if (command.errorMessage.Length > 0)
				{
					return true;
				}
			}

			return false;
		}

		public bool IsRunning()
		{
			if (fungusScript == null ||
			    fungusScript.activeSequence == null)
			{
				return false;
			}

			return (fungusScript.activeSequence == this);
		}

		public void ExecuteNextCommand(FungusCommand currentCommand = null)
		{
			activeCommand = null;
			FungusCommand nextCommand = null;

			bool executeNext = (currentCommand == null);
			FungusCommand[] commands = GetComponents<FungusCommand>();
			foreach (FungusCommand command in commands)
			{
				if (command == currentCommand)
				{
					executeNext = true;
				}
				else if (executeNext)
				{
					nextCommand = command;
					break;
				}
			}

			if (nextCommand == null)
			{
				Finish();
			}
			else
			{
				if (fungusScript.stepTime == 0f)
				{
					activeCommand = nextCommand;
					nextCommand.Execute();
				}
				else
				{
					StartCoroutine(ExecuteAfterDelay(nextCommand, fungusScript.stepTime));
				}
			}

		}

		IEnumerator ExecuteAfterDelay(FungusCommand command, float delay)
		{
			activeCommand = command;
			yield return new WaitForSeconds(delay);
			command.Execute();
		}

		public void Finish()
		{
			activeCommand = null;
			fungusScript.activeSequence = null;

			// No more commands to run in current sequence
			#if UNITY_EDITOR
			Selection.activeGameObject = fungusScript.gameObject;
			#endif
		}
	}
}
