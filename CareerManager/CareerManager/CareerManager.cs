using CareerManagerUI;
//using ImportantUtilities;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgradeables;

namespace CareerManagerNS
{

//    [KSPScenario(ScenarioCreationOptions.AddToExistingCareerGames | ScenarioCreationOptions.AddToNewCareerGames, GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.SPACECENTER, GameScenes.TRACKSTATION)]
    [KSPScenario(ScenarioCreationOptions.AddToExistingCareerGames | ScenarioCreationOptions.AddToNewCareerGames, GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.TRACKSTATION)]
    public class CareerManager : ScenarioModule
    {
        public static CareerManager Instance;
        public static double MONEY_LOCK = 99999999999.0;

        public static float SCIENCE_LOCK = 9999999f;

        public static float REPUTATION_LOCK = 1000;

        CareerManagerGUIClass CareerManagerGUI = new CareerManagerGUIClass();

        internal TechnologyUnlock unlockTechnology = TechnologyUnlock.OFF;

        private double revertFunds = CareerManager.MONEY_LOCK;

        private float revertScience = CareerManager.SCIENCE_LOCK;

        private float revertReputation = CareerManager.REPUTATION_LOCK;

        private Dictionary<string, float> facilities = new Dictionary<string, float>();

        internal bool RnDOpen = false;

        public CareerManager()
        {
            this.facilities.Add("LaunchPad", 0f);
            this.facilities.Add("Runway", 0f);
            this.facilities.Add("VehicleAssemblyBuilding", 0f);
            this.facilities.Add("TrackingStation", 0f);
            this.facilities.Add("AstronautComplex", 0f);
            this.facilities.Add("MissionControl", 0f);
            this.facilities.Add("ResearchAndDevelopment", 0f);
            this.facilities.Add("Administration", 0f);
            this.facilities.Add("SpaceplaneHangar", 0f);
            this.SetupToggles();
            Instance = this;
        }

        private void SetupToggles()
        {
            Rect rect = new Rect(0f, 0f, 195f, 20f);
            this.CareerManagerGUI.CreateToggle(CareerOptions.LOCKFUNDS, rect, false, "Lock funds", new Action<bool>(this.FundsLocked), new GameScenes[]
            {
                GameScenes.SPACECENTER,
                GameScenes.FLIGHT,
                GameScenes.TRACKSTATION
            });
            this.CareerManagerGUI.CreateToggle(CareerOptions.LOCKSCIENCE, rect, false, "Lock science points", new Action<bool>(this.ScienceLocked), new GameScenes[]
            {
                GameScenes.SPACECENTER,
                GameScenes.FLIGHT,
                GameScenes.TRACKSTATION
            });
            this.CareerManagerGUI.CreateToggle(CareerOptions.LOCKREPUTATION, rect, false, "Lock reputation", new Action<bool>(this.ReputationLocked), new GameScenes[]
            {
                GameScenes.SPACECENTER,
                GameScenes.FLIGHT,
                GameScenes.TRACKSTATION
            });
            this.CareerManagerGUI.CreateToggle(CareerOptions.UNLOCKBUILDINGS, rect, false, "Unlock buildings", new Action<bool>(this.BuildingsUnlocked));
            this.CareerManagerGUI.CreateToggle(CareerOptions.UNLOCKTECH, rect, false, "Unlock technologies", new Action<bool>(this.TechnologiesUnlocked));
            this.CareerManagerGUI.CreateToggle(CareerOptions.KICKSTART, rect, false, "Kickstart career", new Action<bool>(this.Kickstart));

        }

        public void FundsChanged(double amount, TransactionReasons reason)
        {
            //bool flag = !this.CareerManagerGUI.GetOption(CareerOptions.LOCKFUNDS);
            //if (!flag)
            if (this.CareerManagerGUI.GetOption(CareerOptions.LOCKFUNDS))
            {
                bool flag2 = reason != TransactionReasons.Cheating;
                if (flag2)
                {
                    this.LockMoney();
                }
            }
        }

        public void ScienceChanged(float amount, TransactionReasons reason)
        {
            //bool flag = !this.CareerManagerGUI.GetOption(CareerOptions.LOCKSCIENCE);
            //if (!flag)
            if (this.CareerManagerGUI.GetOption(CareerOptions.LOCKSCIENCE))
            {
                bool flag2 = reason != TransactionReasons.Cheating;
                if (flag2)
                {
                    this.LockScience();
                }
            }
        }
        public void ReputationChanged(float amount, TransactionReasons reason)
        {
            //bool flag = !this.CareerManagerGUI.GetOption(CareerOptions.LOCKSCIENCE);
            //if (!flag)
            if (this.CareerManagerGUI.GetOption(CareerOptions.LOCKREPUTATION))
            {
                bool flag2 = reason != TransactionReasons.Cheating;
                if (flag2)
                {
                    this.LockReputation();
                }
            }
        }

        public void FundsLocked(bool state)
        {
            if (state)
            {
                this.revertFunds = Funding.Instance.Funds;
                this.LockMoney();
                ScreenMessages.PostScreenMessage("CareerManager: Funds locked.");
            }
            else
            {
                Funding.Instance.AddFunds(-Funding.Instance.Funds, TransactionReasons.Cheating);
                Funding.Instance.AddFunds(this.revertFunds, TransactionReasons.Cheating);
                ScreenMessages.PostScreenMessage("CareerManager: Funds reverted.");
            }
        }

        public void ScienceLocked(bool state)
        {
            if (state)
            {
                this.revertScience = ResearchAndDevelopment.Instance.Science;
                this.LockScience();
                ScreenMessages.PostScreenMessage("CareerManager: Science locked.");
            }
            else
            {
                ResearchAndDevelopment.Instance.AddScience(-ResearchAndDevelopment.Instance.Science, TransactionReasons.Cheating);
                ResearchAndDevelopment.Instance.AddScience(this.revertScience, TransactionReasons.Cheating);
                ScreenMessages.PostScreenMessage("CareerManager: Science reverted.");
            }
        }

        public void ReputationLocked(bool state)
        {
            if (state)
            {
                this.revertReputation = Reputation.Instance.reputation;
                this.LockReputation();
                ScreenMessages.PostScreenMessage("CareerManager: Reputation locked.");
            }
            else
            {
                Reputation.Instance.AddReputation(-Reputation.Instance.reputation, TransactionReasons.Cheating);
                Reputation.Instance.AddReputation(this.revertReputation, TransactionReasons.Cheating);
                ScreenMessages.PostScreenMessage("CareerManager: Reputation reverted.");
            }
        }


        public void BuildingsUnlocked(bool state)
        {
            if (state)
            {
                this.UpgradeAllBuildings();
                ScreenMessages.PostScreenMessage("CareerManager: Buildings upgraded!");
            }
            else
            {
                this.DowngradeAllBuildings();
                ScreenMessages.PostScreenMessage("CareerManager: Buildings downgraded.");
            }
        }

        public void TechnologiesUnlocked(bool state)
        {
            TechnologiesUnlocked(state, true, 999);
        }

        public void Kickstart(bool state)
        {
            if (!CareerManager.Instance.RnDOpen)
            {
                CareerManager.Instance.unlockTechnology = TechnologyUnlock.OFF;
                CareerManagerGUI.SetOption(CareerOptions.KICKSTART, false);
                ScreenMessages.PostScreenMessage("CareerManager: Please visit the R&D building to use this feature.");
                return;
            }
            CareerManagerGUIClass.kickstartEntry = true;
        }


        public void TechnologiesUnlocked(bool state, bool all, int maxDepth)
        {
            if (state)
            {
                this.unlockTechnology = TechnologyUnlock.UNLOCK;
                bool rnDOpen = this.RnDOpen;
                Debug.Log("TechnologiesUnlocked, RnDOpen: " + this.RnDOpen);
               // rnDOpen = true;
                if (rnDOpen)
                {
                    this.UnlockTechnologies(all, maxDepth);
                    ScreenMessages.PostScreenMessage("CareerManager: Technologies unlocked. Close and reopen the R&D screen to see changes.");
                }
                else
                {
                    this.unlockTechnology = TechnologyUnlock.OFF;
                    this.CareerManagerGUI.SetOption(CareerOptions.UNLOCKTECH, false);
                    ScreenMessages.PostScreenMessage("CareerManager: Please visit the R&D building to use this feature.");
                }
            }
            else
            {
                this.unlockTechnology = TechnologyUnlock.REVERT;
                bool rnDOpen2 = this.RnDOpen;
                Debug.Log("TechnologiesUnlocked, RnDOpen: " + this.RnDOpen);
               // rnDOpen2 = true;
                if (rnDOpen2)
                {
                    this.LockTechnologies();
                    ScreenMessages.PostScreenMessage("CareerManager: Technologies locked. Close and reopen the R&D screen to see changes.");
                }
                else
                {
                    this.unlockTechnology = TechnologyUnlock.OFF;
                    this.CareerManagerGUI.SetOption(CareerOptions.UNLOCKTECH, true);
                    ScreenMessages.PostScreenMessage("CareerManager: Please visit the R&D building to use this feature.");
                }
            }
        }

        public void LockMoney()
        {
            bool flag = Funding.Instance.Funds < CareerManager.MONEY_LOCK;
            if (flag)
            {
                Funding.Instance.AddFunds(CareerManager.MONEY_LOCK - Funding.Instance.Funds, TransactionReasons.Cheating);
            }
            else
            {
                bool flag2 = Funding.Instance.Funds > CareerManager.MONEY_LOCK;
                if (flag2)
                {
                    Funding.Instance.AddFunds(-Funding.Instance.Funds, TransactionReasons.Cheating);
                    Funding.Instance.AddFunds(CareerManager.MONEY_LOCK, TransactionReasons.Cheating);
                }
            }
        }

        public void LockScience()
        {
            //bool flag = ResearchAndDevelopment.Instance.Science < CareerManager.SCIENCE_LOCK;
            //if (flag)
            if (ResearchAndDevelopment.Instance.Science < CareerManager.SCIENCE_LOCK)
            {
                ResearchAndDevelopment.Instance.AddScience(CareerManager.SCIENCE_LOCK - ResearchAndDevelopment.Instance.Science, TransactionReasons.Cheating);
            }
            else
            {
                //bool flag2 = Funding.Instance.Funds > CareerManager.MONEY_LOCK;
                //if (flag2)
                if (Funding.Instance.Funds > CareerManager.MONEY_LOCK)
                {
                    ResearchAndDevelopment.Instance.AddScience(-ResearchAndDevelopment.Instance.Science, TransactionReasons.Cheating);
                    ResearchAndDevelopment.Instance.AddScience(CareerManager.SCIENCE_LOCK, TransactionReasons.Cheating);
                }
            }
        }

        public void LockReputation()
        {
            //bool flag = ResearchAndDevelopment.Instance.Science < CareerManager.SCIENCE_LOCK;
            //if (flag)
            if (Reputation.Instance.reputation < CareerManager.REPUTATION_LOCK)
            {
                Reputation.Instance.AddReputation(CareerManager.REPUTATION_LOCK - Reputation.Instance.reputation, TransactionReasons.Cheating);
            }
            else
            {
                //bool flag2 = Funding.Instance.Funds > CareerManager.MONEY_LOCK;
                //if (flag2)
                if (Funding.Instance.Funds > CareerManager.MONEY_LOCK)
                {
                    Reputation.Instance.AddReputation(-Reputation.Instance.reputation, TransactionReasons.Cheating);
                    Reputation.Instance.AddReputation(CareerManager.REPUTATION_LOCK, TransactionReasons.Cheating);
                }
            }
        }

        public void RnDOpened(RDController controller)
        {
            //bool flag = this.unlockTechnology == TechnologyUnlock.UNLOCK;
            //if (flag)
            if (this.unlockTechnology == TechnologyUnlock.UNLOCK)
            {
                this.UnlockTechnologies();
            }
            else
            {
                //bool flag2 = this.unlockTechnology == TechnologyUnlock.REVERT;
                //if (flag2)
                if (this.unlockTechnology == TechnologyUnlock.REVERT)
                {
                    this.LockTechnologies();
                }
            }
        }

        public void RnDGUIClosed()
        {
            Debug.Log("RnDGUIClosed");
            this.RnDOpen = false;
        }

        public void RnDGUIOpened()
        {
            Debug.Log("RnDGUIOpened");
            this.RnDOpen = true;
        }

        int NodeDepth(RDNode current)
        {
            int depth = 1;
            while (current.parents.Count() > 0)
            {
                current = current.parents[0].parent.node;
                depth++;
            }

            return depth;
        }
        public void UnlockTechnologies(bool all = true, int maxDepth = 999)
        {
            //bool flag = !this.RnDOpen;
            //if (!flag)
            if (this.RnDOpen)
            {
                this.unlockTechnology = TechnologyUnlock.OFF;
                float scienceCostLimit = GameVariables.Instance.GetScienceCostLimit(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.ResearchAndDevelopment));
                foreach (RDNode current in RDController.Instance.nodes)
                {
                    if (NodeDepth(current) <= maxDepth)
                    {
                        double currentFunds = 0;
                        if (!all)
                        {
                            currentFunds = Funding.Instance.Funds;
                            Funding.Instance.AddFunds(CareerManager.MONEY_LOCK - Funding.Instance.Funds, TransactionReasons.Cheating);

                        }
                        bool flag2 = current.tech != null && (float)current.tech.scienceCost < scienceCostLimit;
                        if (flag2)
                        {
                            current.tech.UnlockTech(true);
                        }
                        if (!all)
                        {
                            Funding.Instance.AddFunds(-Funding.Instance.Funds, TransactionReasons.Cheating);
                            Funding.Instance.AddFunds(currentFunds, TransactionReasons.Cheating);
                        }
                    }
                }
            }
        }

        private void LockTechnologies()
        {
            //bool flag = !this.RnDOpen;
            //if (!flag)
            if (this.RnDOpen)
            {
                this.unlockTechnology = TechnologyUnlock.OFF;
                foreach (RDNode current in RDController.Instance.nodes)
                {
                    //bool flag2 = current.tech.scienceCost > 0;
                    //if (flag2)
                    if (current != null && current.tech.scienceCost > 0)
                    {
                        ProtoTechNode techState = ResearchAndDevelopment.Instance.GetTechState(current.tech.techID);
                        techState.state = RDTech.State.Unavailable;
                        techState.partsPurchased.Clear();
                        ResearchAndDevelopment.Instance.SetTechState(current.tech.techID, techState);
                    }
                }
            }
        }

        public void UpgradeAllBuildings()
        {
            ScenarioUpgradeableFacilities.protoUpgradeables.ToList<KeyValuePair<string, ScenarioUpgradeableFacilities.ProtoUpgradeable>>().ForEach(delegate (KeyValuePair<string, ScenarioUpgradeableFacilities.ProtoUpgradeable> pu)
            {
                foreach (UpgradeableFacility current in pu.Value.facilityRefs)
                {
                    //bool flag = !this.facilities.ContainsKey(current.name);
                    //if (flag)
                    if (!this.facilities.ContainsKey(current.name))
                    {
                        this.facilities.Add(current.name, ScenarioUpgradeableFacilities.GetFacilityLevel(current.name));
                    }
                    else
                    {
                        this.facilities[current.name] = ScenarioUpgradeableFacilities.GetFacilityLevel(current.name);
                    }
                    current.SetLevel(current.MaxLevel);
                }
            });
        }

        public void DowngradeAllBuildings()
        {
            ScenarioUpgradeableFacilities.protoUpgradeables.ToList<KeyValuePair<string, ScenarioUpgradeableFacilities.ProtoUpgradeable>>().ForEach(delegate (KeyValuePair<string, ScenarioUpgradeableFacilities.ProtoUpgradeable> pu)
            {
                foreach (UpgradeableFacility current in pu.Value.facilityRefs)
                {
                    //bool flag = facilities.ContainsKey(current.name);
                    //if (flag)
                    if (facilities.ContainsKey(current.name))
                    {
                        float num = facilities[current.name];
                        current.SetLevel((int)(num * (float)pu.Value.GetLevelCount()));
                    }
                    else
                    {
                        Utilities.Log("CareerManager", base.GetInstanceID(), "Facility " + current.name + " was missing in our registry!");
                    }
                }
            });
        }

        bool settingsEnabled = false;
        bool blizzyEnabled = false;
        void restart()
        {

            Utilities.Log("restart, enabled: " + HighLogic.CurrentGame.Parameters.CustomParams<CareerManager_Settings>().enabled.ToString());
            Utilities.Log("settingsEnabled: " + settingsEnabled.ToString() + "    blizzyEnabled: " + blizzyEnabled.ToString());

            if (HighLogic.CurrentGame.Parameters.CustomParams<CareerManager_Settings>().enabled)
            {
                if (!settingsEnabled)
                {
                    GameEvents.OnFundsChanged.Add(new EventData<double, TransactionReasons>.OnEvent(FundsChanged));
                    GameEvents.OnScienceChanged.Add(new EventData<float, TransactionReasons>.OnEvent(ScienceChanged));
                    RDController.OnRDTreeSpawn.Add(new EventData<RDController>.OnEvent(RnDOpened));
                    GameEvents.onGUIRnDComplexSpawn.Add(new EventVoid.OnEvent(RnDGUIOpened));
                    GameEvents.onGUIRnDComplexDespawn.Add(new EventVoid.OnEvent(RnDGUIClosed));
                    //CareerManagerGUI.OnEnable();
                    settingsEnabled = true;

                }
            }


            else
            {
                if (settingsEnabled)
                {
                    GameEvents.OnFundsChanged.Remove(new EventData<double, TransactionReasons>.OnEvent(this.FundsChanged));
                    GameEvents.OnScienceChanged.Remove(new EventData<float, TransactionReasons>.OnEvent(this.ScienceChanged));
                    RDController.OnRDTreeSpawn.Remove(new EventData<RDController>.OnEvent(this.RnDOpened));
                    GameEvents.onGUIRnDComplexSpawn.Remove(new EventVoid.OnEvent(this.RnDGUIOpened));
                    GameEvents.onGUIRnDComplexDespawn.Remove(new EventVoid.OnEvent(this.RnDGUIClosed));
                   // CareerManagerGUI.OnDisable();
                    settingsEnabled = false;
                    blizzyEnabled = false;
                }
            }
        }

        private void Start()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<CareerManager_Settings>().enabled)
                return;
            GameEvents.OnGameSettingsApplied.Add(restart);


            restart();
        }

        public void Update()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<CareerManager_Settings>().enabled)
                return;

            bool flag = this.RnDOpen && this.unlockTechnology == TechnologyUnlock.REVERT;
            if (flag)
            {
                this.LockTechnologies();
            }
            else
            {
                bool flag2 = this.RnDOpen && this.unlockTechnology == TechnologyUnlock.UNLOCK;
                if (flag2)
                {
                    this.UnlockTechnologies();
                }
            }
        }

        public override void OnSave(ConfigNode node)
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<CareerManager_Settings>().enabled)
                return;

            bool flag = this.unlockTechnology == TechnologyUnlock.UNLOCK;
            if (flag)
            {
                CareerManagerGUI.SetOption(CareerOptions.UNLOCKTECH, false);
                ScreenMessages.PostScreenMessage("CareerManager: Timed out technology unlocking.");
            }
            else
            {
                bool flag2 = this.unlockTechnology == TechnologyUnlock.REVERT;
                if (flag2)
                {
                    CareerManagerGUI.SetOption(CareerOptions.UNLOCKTECH, true);
                    ScreenMessages.PostScreenMessage("CareerManager: Timed out technology locking.");
                }
            }
            CareerManagerGUI.SaveSettings(node);
            ConfigNode configNode = new ConfigNode("BUILDING_LEVELS");
            foreach (string current in facilities.Keys)
            {
                configNode.AddValue(current, facilities[current].ToString());
            }
            node.AddValue("RevertFunds", revertFunds);
            node.AddValue("RevertScience", revertScience);
            node.AddNode(configNode);
        }

        public override void OnLoad(ConfigNode node)
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<CareerManager_Settings>().enabled)
                return;

            CareerManagerGUI.LoadSettings(node);
            bool flag = node.HasValue("RevertFunds");
            if (flag)
            {
                node.GetConfigValue(out this.revertFunds, "RevertFunds");
            }
            bool flag2 = node.HasValue("RevertScience");
            if (flag2)
            {
                node.GetConfigValue(out this.revertScience, "RevertScience");
            }
            bool flag3 = node.HasNode("BUILDING_LEVELS");
            if (flag3)
            {
                ConfigNode node2 = node.GetNode("BUILDING_LEVELS");
                List<string> list = new List<string>(facilities.Keys);
                foreach (string current in list)
                {
                    float value;
                    node2.GetConfigValue(out value, current);
                    facilities[current] = value;
                }
            }
        }

        private void OnDestroy()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<CareerManager_Settings>().enabled)
                return;

            GameEvents.OnFundsChanged.Remove(new EventData<double, TransactionReasons>.OnEvent(this.FundsChanged));
            GameEvents.OnScienceChanged.Remove(new EventData<float, TransactionReasons>.OnEvent(this.ScienceChanged));
            RDController.OnRDTreeSpawn.Remove(new EventData<RDController>.OnEvent(this.RnDOpened));
            GameEvents.onGUIRnDComplexSpawn.Remove(new EventVoid.OnEvent(this.RnDGUIOpened));
            GameEvents.onGUIRnDComplexDespawn.Remove(new EventVoid.OnEvent(this.RnDGUIClosed));
            GameEvents.OnGameSettingsApplied.Remove(restart);

        }
    }
}
