using CareerManagerNS;
using ImportantUtilities;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using UnityEngine;

using ClickThroughFix;
using ToolbarControl_NS;

namespace CareerManagerUI
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(CareerManagerGUIClass.MODID, CareerManagerGUIClass.MODNAME);
        }
    }
    //[KSPAddon(KSPAddon.Startup.EditorAny | KSPAddon.Startup.SpaceCentre | KSPAddon.Startup.Flight | KSPAddon.Startup.TrackingStation, false)]
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class CareerManagerGUIClass : MonoBehaviour
    {


        private bool guiActive = false;

        internal static bool kickstartEntry = false;

        internal ToolbarControl toolbarControl = null;

        private int windowID;

        private Rect optionsWindowRect;
        private Rect kickstartWindowRect;

        private static Dictionary<CareerOptions, MenuToggle> options = new Dictionary<CareerOptions, MenuToggle>();

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
                return options;
            }
        }

        public void ToggleGui()
        {
            this.GuiActive = !this.GuiActive;
        }

        public void Start()
        {
            InitToolbar();
            //DontDestroyOnLoad(this);
        }
        internal const string MODID = "CareerManager_NS";
        internal const string MODNAME = "Career Manager";
        public void InitToolbar()
        {

            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(ToggleGui, ToggleGui,
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.TRACKSTATION,
                    MODID,
                    "careerManagerButton",
                    "CareerManager/icons/careermanager38",
                    "CareerManager/icons/careermanager24",
                    MODNAME
                );
            }
          
            this.windowID = Guid.NewGuid().GetHashCode();
            this.optionsWindowRect = new Rect(200f, 175f, 200f, 25f);
            kickstartWindowRect = new Rect(200f, 175f, 200f, 25f);
        }

        public CareerManagerGUIClass()
        {
            //options = new Dictionary<CareerOptions, MenuToggle>();
            // OnEnable();
        }

        public void CreateToggle(CareerOptions opt, Rect rect, bool defaultstate, string description, Action<bool> cback)
        {
            try
            {
                options.Add(opt, new MenuToggle(rect, defaultstate, description, cback));
            } catch
            {
                Debug.Log("Toggle (1) already exists: " + description);
            }
        }

        public void CreateToggle(CareerOptions opt, Rect rect, bool defaultstate, string description, Action<bool> cback, GameScenes[] scenes)
        {
            try
            {
                options.Add(opt, new MenuToggle(rect, defaultstate, description, cback, scenes));
            }
            catch
            {
                Debug.Log("Toggle (2) already exists: " + description);
            }
        }

        public bool GetOption(CareerOptions opt)
        {
            bool flag = options.ContainsKey(opt);
            return flag && options[opt].GetState();
        }

        public void SetOption(CareerOptions opt, bool value)
        {
            bool flag = options.ContainsKey(opt);
            if (flag)
            {
                options[opt].state = value;
            }
        }

        public void LoadSettings(ConfigNode node)
        {
            node.GetConfigValue(out options[CareerOptions.LOCKFUNDS]._state, "LockFunds");
            node.GetConfigValue(out options[CareerOptions.LOCKSCIENCE]._state, "LockScience");
            node.GetConfigValue(out options[CareerOptions.LOCKREPUTATION]._state, "LockReputation");
            node.GetConfigValue(out options[CareerOptions.UNLOCKBUILDINGS]._state, "UnlockBuildings");
            node.GetConfigValue(out options[CareerOptions.UNLOCKTECH]._state, "UnlockTech");
        }

        public void SaveSettings(ConfigNode node)
        {
            node.AddValue("LockFunds", this.GetOption(CareerOptions.LOCKFUNDS).ToString());
            node.AddValue("LockScience", this.GetOption(CareerOptions.LOCKSCIENCE).ToString());
            node.AddValue("LockReputation", this.GetOption(CareerOptions.LOCKREPUTATION).ToString());
            node.AddValue("UnlockBuildings", this.GetOption(CareerOptions.UNLOCKBUILDINGS).ToString());
            node.AddValue("UnlockTech", this.GetOption(CareerOptions.UNLOCKTECH).ToString());
        }
        private void OnGUI()
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<CareerManager_Settings>().enabled)
                DrawGUI();
        }

        public void DrawGUI()
        {
            if (this.GuiActive)
            {
                GUI.skin = HighLogic.Skin;
                this.optionsWindowRect = ClickThruBlocker.GUILayoutWindow(this.windowID, this.optionsWindowRect, this.Draw, "CareerManager Options");
            }
            if (kickstartEntry)
            {
                guiActive = false;
                kickstartWindowRect = ClickThruBlocker.GUILayoutWindow(this.windowID + 1, kickstartWindowRect, DrawKickstartWindow, "Kickstart Career");
            }
        }

        string kickstartLevel = "";
        void DrawKickstartWindow(int windowID)
        {
           
            SetOption(CareerOptions.UNLOCKTECH, true);
            SetOption(CareerOptions.KICKSTART, false);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enter kickstart level: ");
            kickstartLevel = GUILayout.TextField(kickstartLevel, 2);
            if (kickstartLevel.Length > 2)
                kickstartLevel = kickstartLevel.Substring(0, 2);
            if (kickstartLevel.Length > 0)
            {
                if (!char.IsDigit(kickstartLevel[kickstartLevel.Length - 1]))
                {
                    kickstartLevel = kickstartLevel.Substring(0, kickstartLevel.Length - 1);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply"))
            {
                if (kickstartLevel != "")
                    CareerManager.Instance.TechnologiesUnlocked(true, true, int.Parse(kickstartLevel));
                kickstartEntry = false;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel"))
            {
                kickstartEntry = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        public void Draw(int windowID)
        {
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            foreach (KeyValuePair<CareerOptions, MenuToggle> current in options)
            {
                current.Value.draw(HighLogic.LoadedScene);
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
        void OnDestroy()
        {
            OnDisable();
        }

        public void OnDisable()
        {

            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
            }

        }
    }
}
