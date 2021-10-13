﻿using MscModApi.Parts;
using MscModApi.Tools;
using System;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi.Shopping
{
	internal class PartPanel
	{
		internal static GameObject prefab;
		internal static GameObject panel;
		private static GameObject list;
		internal Shop.ShopLocation shopLocation;
		private string name;
		private ShopInterface shopInterface;
		private Part part;
		private GameObject partPanel;

		public PartPanel(ShopInterface shopInterface, string name, float prize, Part part, string iconName, Shop.ShopLocation shopLocation, Shop.SpawnLocation spawnLocation)
		{
			this.shopLocation = shopLocation;
			this.name = name;

			part.SetDefaultPosition(Shop.GetSpawnLocation(shopLocation, spawnLocation));
			if (part.partSave.bought == PartSave.BoughtState.NotConfigured)
			{
				part.partSave.bought = PartSave.BoughtState.No;
			}
			this.shopInterface = shopInterface;
			this.part = part;
			partPanel = GameObject.Instantiate(prefab);
			var partImage = partPanel.FindChild("part_image").GetComponent<Image>();
			partImage.sprite = part.partBaseInfo.assetBundle.LoadAsset<Sprite>(iconName) ?? partImage.sprite;
			var btnBuyPart = partPanel.FindChild("part_buy").GetComponent<Button>();
			btnBuyPart.onClick.AddListener(OnPartBought);

			var partName = partPanel.FindChild("part_name").GetComponent<Text>();
			partName.text = name;
			var partPrize = partPanel.FindChild("part_prize_panel/part_prize").GetComponent<Text>();
			partPrize.text = prize.ToString();

			partPanel.transform.SetParent(list.transform);
			partPanel.transform.localScale = new Vector3(1, 1, 1);
		}

		internal static void Init(GameObject shopInterfaceObject)
		{
			panel = shopInterfaceObject.FindChild("panel/shop/parts_panel");
			list = panel.FindChild("parts_list/list/grid");
		}

		internal void OnPartBought()
		{

		}

		internal string GetName()
		{
			return name;
		}

		internal void SetVisible(bool visible)
		{
			partPanel.SetActive(visible);
		}

		internal bool GetVisible()
		{
			return partPanel.activeSelf;
		}

		internal Part GetPart()
		{
			return part;
		}
	}
}