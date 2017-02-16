using CareerManager;
using ImportantUtilities;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CareerManagerUI
{
    public class CareerManagerGUIClass : MonoBehaviour
    {


        private bool guiActive = false;

        public bool useAppLauncher = true;

        private IButton toolbarButton = null;

        private ApplicationLauncherButton appLauncherButton = null;

        private int windowID;

        private Rect optionsWindowRect;

        private Dictionary<CareerOptions, MenuToggle> options;

        public bool GuiActive
        {
            get
            {
                return this.guiActive;
            }
            set
            {
                this.guiActive = value;
            }
        }

        public Dictionary<CareerOptions, MenuToggle> Options
        {
            get
            {
                return this.options;
            }
        }

        public void ToggleGui(bool show)
        {
            this.GuiActive = show;
        }

        public CareerManagerGUIClass()
        {
            this.options = new Dictionary<CareerOptions, MenuToggle>();
            bool toolbarAvailable = ToolbarManager.ToolbarAvailable;
            if (toolbarAvailable)
            {
                this.useAppLauncher = false;
                this.toolbarButton = ToolbarManager.Instance.add("CareerManager", "careermanagerbutton");
                this.toolbarButton.TexturePath = "CareerManager/icons/careermanager_stock";
                this.toolbarButton.ToolTip = "CareerManager options";
                this.toolbarButton.OnClick += delegate (ClickEvent e)
                {
                    this.ToggleGui(!this.guiActive);
                };
            }
            else
            {
                bool flag = this.appLauncherButton == null;
                if (flag)
                {
                    this.appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                     () => {
                        ToggleGui(true);
                    },
                  () => {
                        ToggleGui(false);
                    },
                    null, null, null, null,
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.TRACKSTATION, GameDatabase.Instance.GetTexture("CareerManager/icons/careermanager_stock", false));
                }
            }
            this.windowID = Guid.NewGuid().GetHashCode();
            this.optionsWindowRect = new Rect(200f, 175f, 200f, 25f);
        }

        public void CreateToggle(CareerOptions opt, Rect rect, bool defaultstate, string description, Action<bool> cback)
        {
            this.options.Add(opt, new MenuToggle(rect, defaultstate, description, cback));
        }

        public void CreateToggle(CareerOptions opt, Rect rect, bool defaultstate, string description, Action<bool> cback, GameScenes[] scenes)
        {
            this.options.Add(opt, new MenuToggle(rect, defaultstate, description, cback, scenes));
        }

        public bool GetOption(CareerOptions opt)
        {
            bool flag = this.options.ContainsKey(opt);
            return flag && this.options[opt].GetState();
        }

        public void SetOption(CareerOptions opt, bool value)
        {
            bool flag = this.options.ContainsKey(opt);
            if (flag)
            {
                this.options[opt].state = value;
            }
        }

        public void LoadSettings(ConfigNode node)
        {
            node.GetConfigValue(out this.options[CareerOptions.LOCKFUNDS]._state, "LockFunds");
            node.GetConfigValue(out this.options[CareerOptions.LOCKSCIENCE]._state, "LockScience");
            node.GetConfigValue(out this.options[CareerOptions.UNLOCKBUILDINGS]._state, "UnlockBuildings");
            node.GetConfigValue(out this.options[CareerOptions.UNLOCKTECH]._state, "UnlockTech");
        }

        public void SaveSettings(ConfigNode node)
        {
            node.AddValue("LockFunds", this.GetOption(CareerOptions.LOCKFUNDS).ToString());
            node.AddValue("LockScience", this.GetOption(CareerOptions.LOCKSCIENCE).ToString());
            node.AddValue("UnlockBuildings", this.GetOption(CareerOptions.UNLOCKBUILDINGS).ToString());
            node.AddValue("UnlockTech", this.GetOption(CareerOptions.UNLOCKTECH).ToString());
        }

        public void DrawGUI()
        {
            bool flag = this.GuiActive;
            if (flag)
            {
                GUI.skin = HighLogic.Skin;
                this.optionsWindowRect = GUILayout.Window(this.windowID, this.optionsWindowRect, new GUI.WindowFunction(this.Draw), "CareerManager Options");
            }
        }

        public void Draw(int windowID)
        {
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            foreach (KeyValuePair<CareerOptions, MenuToggle> current in this.options)
            {
                current.Value.draw(HighLogic.LoadedScene);
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        public void OnDisable()
        {
            bool toolbarAvailable = ToolbarManager.ToolbarAvailable;
            if (toolbarAvailable)
            {
                this.toolbarButton.Destroy();
            }
            else
            {
                ApplicationLauncher.Instance.RemoveModApplication(this.appLauncherButton);
            }
        }
    }
}
