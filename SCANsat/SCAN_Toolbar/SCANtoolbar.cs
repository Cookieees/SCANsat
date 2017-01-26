﻿#region license
/*
 *  [Scientific Committee on Advanced Navigation]
 * 			S.C.A.N. Satellite
 *
 * SCANtoolbar -	optional integration with Blizzy's toolvar
 *
 * Copyright (c)2014 David Grandy <david.grandy@gmail.com>;
 * Copyright (c)2014 technogeeky <technogeeky@gmail.com>;
 * Copyright (c)2014 (Your Name Here) <your email here>; see LICENSE.txt for licensing details.
 *
 * Created by David to allow the SCANsat plugin to function through the toolbar interface
 */
#endregion

using System.IO;
using UnityEngine;
using SCANsat.SCAN_Platform;

namespace SCANsat.SCAN_Toolbar
{
	[SCAN_KSPAddonImproved(SCAN_KSPAddonImproved.Startup.TimeElapses, false)]
	class SCANtoolbar : MonoBehaviour
	{
		private IButton SCANButton;
		private IButton MapButton;
		private IButton SmallButton;
		private IButton OverlayButton;
		private IButton KSCButton;
		private IButton ZoomButton;

		internal SCANtoolbar()
		{
			if (!ToolbarManager.ToolbarAvailable) return; // bail if we don't have a toolbar

			if (HighLogic.LoadedSceneIsFlight)
			{
				SCANButton = ToolbarManager.Instance.add("SCANsat", "UIMenu");
				MapButton = ToolbarManager.Instance.add("SCANsat", "BigMap");
				SmallButton = ToolbarManager.Instance.add("SCANsat", "SmallMap");
				OverlayButton = ToolbarManager.Instance.add("SCANsat", "Overlay");
				ZoomButton = ToolbarManager.Instance.add("SCANsat", "ZoomMap");

				//Fall back to some default toolbar icons if someone deletes the SCANsat icons or puts them in the wrong folder
				if (File.Exists(Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/SCANsat/Icons/SCANsat_Icon.png").Replace("\\", "/")))
					SCANButton.TexturePath = "SCANsat/Icons/SCANsat_Icon"; // S.C.A.N
				else
					SCANButton.TexturePath = "000_Toolbar/toolbar-dropdown";
				if (File.Exists(Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/SCANsat/Icons/SCANsat_Map_Icon.png").Replace("\\", "/")))
					MapButton.TexturePath = "SCANsat/Icons/SCANsat_Map_Icon"; // from in-game biome map of Kerbin
				else
					MapButton.TexturePath = "000_Toolbar/move-cursor";
				if (File.Exists(Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/SCANsat/Icons/SCANsat_SmallMap_Icon.png").Replace("\\", "/")))
					SmallButton.TexturePath = "SCANsat/Icons/SCANsat_SmallMap_Icon"; // from unity, edited by DG
				else
					SmallButton.TexturePath = "000_Toolbar/resize-cursor";
				if (File.Exists(Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/SCANsat/Icons/SCANsat_OverlayToolbar_Icon.png").Replace("\\", "/")))
					OverlayButton.TexturePath = "SCANsat/Icons/SCANsat_OverlayToolbar_Icon";
				else
					OverlayButton.TexturePath = "000_Toolbar/resize-cursor";
				if (File.Exists(Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/SCANsat/Icons/SCANsat_ZoomToolbar_Icon.png").Replace("\\", "/")))
					ZoomButton.TexturePath = "SCANsat/Icons/SCANsat_ZoomToolbar_Icon";
				else
					ZoomButton.TexturePath = "000_Toolbar/resize-cursor";

				SCANButton.ToolTip = "SCANsat";
				MapButton.ToolTip = "SCANsat Big Map";
				SmallButton.ToolTip = "SCANsat Small Map";
				OverlayButton.ToolTip = "SCANsat Overlay Controller";
				ZoomButton.ToolTip = "SCANsat Zoom Map";

				SCANButton.OnClick += (e) =>
					{
						if (SCANcontroller.controller != null)
						{
							toggleMenu(SCANButton);
						}
					};
				MapButton.OnClick += (e) =>
					{
						if (SCANcontroller.controller != null)
						{
							if (SCANcontroller.controller._bigMap.IsVisible)
								SCANcontroller.controller._bigMap.Close();
							else
								SCANcontroller.controller._bigMap.Open();
						}
					};
				SmallButton.OnClick += (e) =>
					{
						if (SCANcontroller.controller != null)
						{
							if (SCANcontroller.controller._mainMap.IsVisible)
								SCANcontroller.controller._mainMap.Close();
							else
								SCANcontroller.controller._mainMap.Open();
						}
					};
				OverlayButton.OnClick += (e) =>
					{
						if (SCANcontroller.controller != null)
						{
							if (SCANcontroller.controller._overlay.IsVisible)
								SCANcontroller.controller._overlay.Close();
							else
								SCANcontroller.controller._overlay.Open();
						}
					};
				ZoomButton.OnClick += (e) =>
					{
						if (SCANcontroller.controller != null)
						{
							SCANcontroller.controller.zoomMap.Visible = !SCANcontroller.controller.zoomMap.Visible;
							if (SCANcontroller.controller.zoomMap.Visible && !SCANcontroller.controller.zoomMap.Initialized)
								SCANcontroller.controller.zoomMap.initializeMap();
						}
					};
			}
			else if (HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.TRACKSTATION)
			{
				KSCButton = ToolbarManager.Instance.add("SCANsat", "KSCMap");

				if (File.Exists(Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/SCANsat/Icons/SCANsat_Map_Icon.png").Replace("\\", "/")))
					KSCButton.TexturePath = "SCANsat/Icons/SCANsat_Map_Icon"; // from in-game biome map of Kerbin
				else
					KSCButton.TexturePath = "000_Toolbar/move-cursor";

				KSCButton.ToolTip = "SCANsat KSC Map";

				KSCButton.OnClick += (e) =>
					{
						if (SCANcontroller.controller != null)
						{
							
						}
					};
			}
		}

		private void toggleMenu(IButton menu)
		{
			if (menu.Drawable == null)
				createMenu(menu);
			else
				destroyMenu(menu);
		}

		private void createMenu(IButton menu)
		{
			if (!ToolbarManager.ToolbarAvailable) return; // bail if we don't have a toolbar

			PopupMenuDrawable list = new PopupMenuDrawable();

			IButton smallMap = list.AddOption("Small Map");
			IButton instrument = list.AddOption("Instruments");
			IButton bigMap = list.AddOption("Big Map");
			IButton zoomMap = list.AddOption("Zoom Map");
			IButton settings = list.AddOption("Settings");
			IButton resource = list.AddOption("Planetary Overlay");

			smallMap.OnClick += (e2) =>
				{
					if (SCANcontroller.controller._mainMap.IsVisible)
						SCANcontroller.controller._mainMap.Close();
					else
						SCANcontroller.controller._mainMap.Open();
				};
			instrument.OnClick += (e2) =>
				{
					if (SCANcontroller.controller._instruments.IsVisible)
						SCANcontroller.controller._instruments.Close();
					else
						SCANcontroller.controller._instruments.Open();
				};
			bigMap.OnClick += (e2) =>
				{
					if (SCANcontroller.controller._bigMap.IsVisible)
						SCANcontroller.controller._bigMap.Close();
					else
						SCANcontroller.controller._bigMap.Open();
				};
			zoomMap.OnClick += (e2) =>
				{
					SCANcontroller.controller.zoomMap.Visible = !SCANcontroller.controller.zoomMap.Visible;
					if (SCANcontroller.controller.zoomMap.Visible && !SCANcontroller.controller.zoomMap.Initialized)
						SCANcontroller.controller.zoomMap.initializeMap();
				};
			settings.OnClick += (e2) =>
				{
					if (SCANcontroller.controller._settings.IsVisible)
						SCANcontroller.controller._settings.Close();
					else
						SCANcontroller.controller._settings.Open();
				};
			resource.OnClick += (e2) =>
				{
					if (SCANcontroller.controller._overlay.IsVisible)
						SCANcontroller.controller._overlay.Close();
					else
						SCANcontroller.controller._overlay.Open();
				};
			list.OnAnyOptionClicked += () => destroyMenu(menu);
			menu.Drawable = list;
		}

		private void destroyMenu(IButton menu)
		{
			((PopupMenuDrawable)menu.Drawable).Destroy();
			menu.Drawable = null;
		}

		internal void OnDestroy()
		{
			if (SCANButton != null)
				SCANButton.Destroy();
			if (MapButton != null)
				MapButton.Destroy();
			if (SmallButton != null)
				SmallButton.Destroy();
			if (KSCButton != null)
				KSCButton.Destroy();
			if (OverlayButton != null)
				OverlayButton.Destroy();
			if (ZoomButton != null)
				ZoomButton.Destroy();
		}

	}
}
