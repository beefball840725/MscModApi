﻿using System;
using HutongGames.PlayMaker;
using MSCLoader;
using System.Linq;
using MSCLoader.Helper;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts.ReplacePart
{
	public class OldPart
	{
		private PlayMakerFSM fsm;
		private GameObject gameObject;
		private GameObject trigger;
		private FsmBool installed;
		private FsmBool bolted;
		private PlayMakerFSM assembleFsm;
		private PlayMakerFSM removalFsm;
		private GameObject oldFsmGameObject;
		private bool allowSettingFakedStatus;
		internal bool justUninstalled = false;

		public OldPart(GameObject oldFsmGameObject, bool allowSettingFakedStatus = true)
		{
			this.oldFsmGameObject = oldFsmGameObject;
			this.allowSettingFakedStatus = allowSettingFakedStatus;
			fsm = oldFsmGameObject.FindFsm("Data");
			gameObject = fsm.FsmVariables.FindFsmGameObject("ThisPart").Value;
			trigger = fsm.FsmVariables.FindFsmGameObject("Trigger").Value;
			installed = fsm.FsmVariables.FindFsmBool("Installed");
			bolted = fsm.FsmVariables.FindFsmBool("Bolted");

			assembleFsm = GetFsmByName(trigger, "Assembly");
			removalFsm = GetFsmByName(gameObject, "Removal");

			if (!assembleFsm.Fsm.Initialized)
			{
				assembleFsm.InitializeFSM();
			}

			if (!removalFsm.Fsm.Initialized)
			{
				removalFsm.InitializeFSM();
			}
		}

		private static PlayMakerFSM GetFsmByName(GameObject gameObject, string fsmName)
		{
			return gameObject.GetComponents<PlayMakerFSM>().FirstOrDefault(comp => comp.FsmName == fsmName);
		}

		public bool IsInstallBlocked()
		{
			return !assembleFsm.enabled;
		}

		public void BlockInstall(bool blocked)
		{
			assembleFsm.enabled = !blocked;
		}

		public bool IsInstalled()
		{
			if (justUninstalled)
			{
				justUninstalled = false;
				return false;
			}

			return installed.Value;
		}

		public bool IsFixed() => IsInstalled() && bolted.Value;

		public void Uninstall() => removalFsm.SendEvent("REMOVE");

		internal void SetFakedInstallStatus(bool status)
		{
			if (!allowSettingFakedStatus) return;
			installed.Value = status;
			bolted.Value = status;
		}

		internal void Setup(ReplacePart.ReplacementPart replacementPart)
		{
			FsmHook.FsmInject(oldFsmGameObject, "Save game", replacementPart.OnOldSave);
		}

		internal void SetInstallAction(Action installAction)
		{
			FsmHook.FsmInject(trigger, "Assemble", installAction);
		}

		internal void SetUninstallAction(Action uninstallAction)
		{
			FsmHook.FsmInject(gameObject, "Remove part", delegate
			{
				justUninstalled = true;
				uninstallAction.Invoke();
			});
		}
	}
}