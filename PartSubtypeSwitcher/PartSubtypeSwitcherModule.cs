using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using System.Reflection;

namespace PartSubtypeSwitcher
{
    /// <summary>
    /// The class is based primarily on the Firespitter mod FSFuelSwitch and FSMeshSwitch PartModules
    /// All credit for the implementation of fuel and mesh switching, as well as parsing methods, goes to Andreas Gogstad (Snjo), original developer of the Firespitter mod
    /// </summary>

    public class PartSubtypeSwitcherModule : PartModule, IPartCostModifier
    {
        public const string configNodeSubtype = "SUBTYPE";
        public const string configNodePresetNode = "PRESET_NODE";
        public const string configNodePresetObject = "PRESET_OBJECT";
        public const string configNodePresetResource = "PRESET_RESOURCE";
        public const string configNodePresetResourceContainer = "PRESET_RESOURCE_CONTAINER";

        public class PresetNode
        {
            public int key;
            public string name;
            public AttachNode reference;
        }

        public class PresetObject
        {
            public int key;
            public string name;
            public Transform target;
        }

        public class PresetResource
        {
            public int key;
            public string configNodeID;
            public string name;
            public List<ResourceContainer> resources = new List<ResourceContainer> ();
        }

        public class Subtype
        {
            public string name;
            public float massAdded;
            public float costAdded;
            public List<int> nodeKeys = new List<int> ();
            public List<int> objectKeys = new List<int> ();
            public int resourceKey = 0;
        }

        public class ResourceContainer
        {
            public string name;
            public string configNodeID;
            public int id;
            public double amount = 0f;
            public double maxAmount = 0f;

            public ResourceContainer (string _name)
            {
                name = _name;
                id = _name.GetHashCode ();
            }
        }

        public Dictionary<int, PresetNode> presetsNode;
        public Dictionary<int, PresetObject> presetsObject;
        public Dictionary<int, PresetResource> presetsResource;

        public List<Subtype> subtypes;
        public Subtype subtypeSelected;

        [KSPField]
        public int subtypeGroup = 0;

        [KSPField]
        public float massBase = 0.25f;

        [KSPField (isPersistant = true)]
        public int subtypeSelectedIndex = 0;

        [KSPField]
        public string uiCaptionNext = "Next";

        [KSPField]
        public string uiCaptionPrev = "Back";

        [KSPField (guiActive = false, guiActiveEditor = false, guiName = "Added cost")]
        public float uiCaptionAddedCost = 0f;

        [KSPField (guiActive = false, guiActiveEditor = true, guiName = "Dry mass")]
        public float uiCaptionDryMass = 0f;

        [KSPField]
        public bool uiUseSecondButton = true;

        [KSPField] 
        public bool availableInFlight = false;

        [KSPField] 
        public bool availableInEditor = true;

        [KSPField (isPersistant = true)]
        public bool configLoaded = false;

        private bool initialized = false;
        UIPartActionWindow tweakableUI;



        #region Events

        [KSPField (guiActiveEditor = true, guiName = "Subtype")]
        public string subtypeSelectedNameUI = string.Empty;
        
        [KSPEvent (guiActive = false, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "Next")]
        public void EventSwitchToNext ()
        {
            subtypeSelectedIndex++;
            if (subtypeSelectedIndex >= subtypes.Count) subtypeSelectedIndex = 0;
            StartSubtypeSwitch (subtypeSelectedIndex, true);
        }

        [KSPEvent (guiActive = false, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "Previous")]
        public void EventSwitchToPrevious ()
        {
            subtypeSelectedIndex--;
            if (subtypeSelectedIndex < 0) subtypeSelectedIndex = subtypes.Count - 1;
            StartSubtypeSwitch (subtypeSelectedIndex, true);
        }

        #endregion




        #region Overrides

        public override void OnStart (PartModule.StartState state)
        {
            StartSubtypeSwitch (subtypeSelectedIndex, false);
            Events["EventSwitchToNext"].guiName = uiCaptionNext;
            Events["EventSwitchToPrevious"].guiName = uiCaptionPrev;
            if (!uiUseSecondButton) Events["EventSwitchToPrevious"].guiActiveEditor = false;
        }

        public override void OnAwake ()
        {
            StartSubtypeSwitch (subtypeSelectedIndex, false);
        }

        public override void OnLoad (ConfigNode node)
        {
            base.OnLoad (node);
            if (!configLoaded)
            {
                presetsNode = new Dictionary<int, PresetNode> ();
                presetsObject = new Dictionary<int, PresetObject> ();
                presetsResource = new Dictionary<int, PresetResource> ();
                subtypes = new List<Subtype> ();

                ConfigNode[] nodesTop = node.GetNodes ();
                Debug.Log ("PSSM | OnLoad | Found " + nodesTop.Length + " nodes within the module node");

                for (int i = 0; i < nodesTop.Length; ++i)
                {
                    ConfigNode nodeTop = nodesTop[i];
                    if (string.Equals (nodeTop.name, configNodePresetNode))
                    {
                        PresetNode preset = new PresetNode ();
                        preset.key = int.Parse (nodeTop.GetValue ("key"));
                        preset.name = nodeTop.GetValue ("name");
                        presetsNode.Add (preset.key, preset);
                        Debug.Log ("PSSM | OnLoad | Node preset found | Key: " + preset.key + " | Name: " + preset.name);
                    }
                    else if (string.Equals (nodeTop.name, configNodePresetObject))
                    {
                        PresetObject preset = new PresetObject ();
                        preset.key = int.Parse (nodeTop.GetValue ("key"));
                        preset.name = nodeTop.GetValue ("name");
                        preset.target = PartSubtypeTools.FindTransform (part.transform, preset.name);
                        presetsObject.Add (preset.key, preset);
                        Debug.Log ("PSSM | OnLoad | Object preset found | Key: " + preset.key + " | Name: " + preset.name + " | Object was " + (preset.target == null ? "not found" : "found"));
                    }
                    else if (string.Equals (nodeTop.name, configNodePresetResource))
                    {
                        PresetResource preset = new PresetResource ();
                        preset.key = int.Parse (nodeTop.GetValue ("key"));
                        preset.name = nodeTop.GetValue ("name");
                        preset.configNodeID = nodeTop.id;
                        preset.resources = new List<ResourceContainer> ();
                        presetsResource.Add (preset.key, preset);
                        Debug.Log ("PSSM | OnLoad | Resource preset found | Key: " + preset.key + " | Name: " + preset.name);

                        ConfigNode[] nodesInPreset = nodeTop.GetNodes ();
                        for (int a = 0; a < nodesInPreset.Length; ++a)
                        {
                            ConfigNode nodeInPreset = nodesInPreset[a];
                            if (string.Equals (nodeInPreset.name, configNodePresetResourceContainer))
                            {
                                ResourceContainer container = new ResourceContainer (nodeInPreset.GetValue ("name"));
                                container.maxAmount = double.Parse (nodeInPreset.GetValue ("amountMax"));
                                if (nodeInPreset.HasValue ("amount")) container.amount = double.Parse (nodeInPreset.GetValue ("amount"));
                                else container.amount = container.maxAmount;
                                preset.resources.Add (container);
                                Debug.Log ("PSSM | OnLoad | Resource preset container found | Name: " + container.name + " | Amount: " + container.amount);
                            }
                        }
                    }
                    if (string.Equals (nodeTop.name, configNodeSubtype))
                    {
                        Subtype subtype = new Subtype ();
                        subtype.name = nodeTop.GetValue ("name");

                        string nodeKeysRaw = nodeTop.GetValue ("nodeKeys");
                        string objectKeysRaw = nodeTop.GetValue ("objectKeys");

                        subtype.nodeKeys = PartSubtypeTools.GetIntListFromField (nodeKeysRaw);
                        subtype.objectKeys = PartSubtypeTools.GetIntListFromField (objectKeysRaw);
                        subtype.resourceKey = int.Parse (nodeTop.GetValue ("resourceKey"));

                        Debug.Log ("PSSM | OnLoad | Subtype found | Name: " + subtype.name + " | Nodes: " + nodeKeysRaw + " | Objects: " + objectKeysRaw + " | Resources: " + subtype.resourceKey);

                        subtype.costAdded = float.Parse (nodeTop.GetValue ("costAdded"));
                        subtype.massAdded = float.Parse (nodeTop.GetValue ("massAdded"));

                        subtypes.Add (subtype);
                    }
                }

                Debug.Log ("PSSM | OnLoad | Subtype loading complete, total number: " + subtypes.Count);
                configLoaded = true;
            }
        }

        public override void OnSave (ConfigNode node)
        {
            base.OnSave (node);

            for (int i = 0; i < presetsResource.Count; ++i)
            {
                ConfigNode nodeResourcePreset = node.GetNodeID (presetsResource[i].configNodeID);
                if (nodeResourcePreset != null)
                {
                    for (int a = 0; a < presetsResource[i].resources.Count; ++a)
                    {
                        ConfigNode nodeResourceContainer = nodeResourcePreset.GetNodeID (presetsResource[i].resources[a].configNodeID);
                        if (nodeResourceContainer != null)
                        {
                            nodeResourceContainer.SetValue ("amount", presetsResource[i].resources[a].amount.ToString (), true);
                        }
                        else
                        {
                            Debug.Log ("PSSM | OnSave | Resource container node " + a + " not found in resource preset node " + i);
                        }
                    }
                }
            }
        }

        #endregion




        #region Switching

        /// <summary>
        /// First part of the switching process, started just once on original part instance and performing invocation of the switch method on all symmetry counterparts
        /// </summary>
        /// <param name="subtypeSelectedIndexNext"></param>
        /// <param name="calledByPlayer"></param>

        private void StartSubtypeSwitch (int subtypeSelectedIndexNext, bool calledByPlayer)
        {
            if (!configLoaded)
            {
                Debug.Log ("PSSM | SetSubtype | Config was not yet loaded, aborting");
                return;
            }

            SetSubtype (subtypeSelectedIndexNext, calledByPlayer);
            for (int i = 0; i < part.symmetryCounterparts.Count; i++)
            {
                PartSubtypeSwitcherModule[] modules = part.symmetryCounterparts[i].GetComponents<PartSubtypeSwitcherModule> ();
                for (int a = 0; a < modules.Length; a++)
                {
                    PartSubtypeSwitcherModule module = modules[a];
                    if (module.subtypeGroup == subtypeGroup && module.subtypeSelectedIndex != subtypeSelectedIndex)
                        modules[a].SetSubtype (subtypeSelectedIndexNext, calledByPlayer);
                }
            }

            if (tweakableUI == null) tweakableUI = PartSubtypeTools.FindActionWindow (part);
            if (tweakableUI != null) tweakableUI.displayDirty = true;
        }

        /// <summary>
        /// Second part of the switching process, invoked from StartSubtypeSwitch method
        /// </summary>
        /// <param name="subtypeSelectedIndexNext"></param>
        /// <param name="calledByPlayer"></param>

        private void SetSubtype (int subtypeSelectedIndexNext, bool calledByPlayer)
        {
            if (subtypeSelectedIndexNext == subtypeSelectedIndex)
            {
                Debug.Log ("PSSM | SetSubtype | Requested subtype index is already in use");
                return;
            }

            subtypeSelected = subtypes[subtypeSelectedIndexNext];
            subtypeSelectedIndex = subtypeSelectedIndexNext;
            subtypeSelectedNameUI = subtypeSelected.name + " (" + presetsResource[subtypeSelected.resourceKey].name + ")";

            for (int i = 0; i < presetsNode.Count; ++i)
            {
                bool used = subtypeSelected.nodeKeys.Contains (presetsNode[i].key);
                SetNodeState (presetsNode[i], used);
            }

            for (int i = 0; i < presetsObject.Count; ++i)
            {
                bool used = subtypeSelected.objectKeys.Contains (presetsObject[i].key);
                SetObjectState (presetsObject[i], used);
            }

            AssignResourcePreset (presetsResource[subtypeSelected.resourceKey], calledByPlayer);
            UpdateWeight ();
            UpdateCost ();
        }

        private void SetNodeState (PresetNode preset, bool used)
        {
            if (preset.reference == null)
            {
                Debug.Log ("PSSM | SetNodeState | Node reference in node preset " + preset.key + " is null, aborting the switch");
                return;
            }
            if (!used && preset.reference.nodeType == AttachNode.NodeType.Stack)
            {
                Debug.Log ("PSSM | SetNodeState | Disabling node " + preset.reference.id);
                preset.reference.nodeType = AttachNode.NodeType.Dock;
                preset.reference.radius = 0.001f;
            }
            else if (preset.reference.nodeType == AttachNode.NodeType.Dock)
            {
                Debug.Log ("PSSM | SetNodeState | Enabling node " + preset.reference.id);
                preset.reference.nodeType = AttachNode.NodeType.Stack;
                preset.reference.radius = 0.4f;
            }
            else
            {
                Debug.Log ("PSSM | SetSubtype | Node " + preset.reference.id + " is not affected by this switch");
            }
        }

        private void SetObjectState (PresetObject preset, bool used)
        {
            if (preset.target == null)
            {
                Debug.Log ("PSSM | SetNodeState | Transform reference in object preset " + preset.key + " is null, aborting the switch");
                return;
            }
            preset.target.gameObject.SetActive (used);
        }

        private void AssignResourcePreset (PresetResource preset, bool calledByPlayer)
        {
            part.Resources.list.Clear ();
            PartResource[] partResources = part.GetComponents<PartResource> ();
            for (int i = 0; i < partResources.Length; i++) DestroyImmediate (partResources[i]);

            for (int i = 0; i < preset.resources.Count; i++)
            {
                if (preset.resources[i].name != "Structural")
                {
                    ConfigNode resourceNode = new ConfigNode ("RESOURCE");
                    resourceNode.AddValue ("name", preset.resources[i].name);
                    resourceNode.AddValue ("maxAmount", preset.resources[i].maxAmount);

                    if (calledByPlayer && !HighLogic.LoadedSceneIsEditor) resourceNode.AddValue ("amount", 0.0f);
                    else resourceNode.AddValue ("amount", preset.resources[i].amount);
                    part.AddResource (resourceNode);
                }
            }

            part.Resources.UpdateList ();
        }

        private void UpdateWeight ()
        {
            part.mass = massBase + subtypeSelected.massAdded;
        }

        public void UpdateCost ()
        {
            uiCaptionAddedCost = subtypeSelected.costAdded;
        }

        public void Update ()
        {
            if (HighLogic.LoadedSceneIsEditor)
                uiCaptionDryMass = part.mass;
        }

        public float GetModuleCost ()
        {
            UpdateCost ();
            return uiCaptionAddedCost;
        }
        public float GetModuleCost (float modifier)
        {
            UpdateCost ();
            return uiCaptionAddedCost * modifier;
        }

        #endregion
    }




    public static class PartSubtypeTools
    {
        #region Parsing

        public static List<int> GetIntListFromField (string field)
        {
            List<int> integers = new List<int> ();
            string[] strings = field.Split (',');
            for (int i = 0; i < strings.Length; ++i)
            {
                int integer = int.Parse (strings[i].Trim (' '));
                integers.Add (integer);
            }
            return integers;
        }

        public static Transform FindTransform (Transform parent, string name)
        {
            if (parent.name.Equals (name)) return parent;
            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform child = parent.GetChild (i);
                Transform result = FindTransform (child, name);
                if (result != null) return result;
            }
            return null;
        }

        #endregion




        #region Window

        // Code from https://github.com/Swamp-Ig/KSPAPIExtensions/blob/master/Source/Utils/KSPUtils.cs#L62

        private static FieldInfo windowListField;

        /// <summary>
        /// Find the UIPartActionWindow for a part. Usually this is useful just to mark it as dirty.
        /// </summary>
        public static UIPartActionWindow FindActionWindow (this Part part)
        {
            if (part == null)
                return null;

            // We need to do quite a bit of piss-farting about with reflection to dig the thing out. 
            // We could just use Object.Find, but that requires hitting a heap more objects

            UIPartActionController controller = UIPartActionController.Instance;
            if (controller == null)
                return null;

            if (windowListField == null)
            {
                Type cntrType = typeof (UIPartActionController);
                foreach (FieldInfo info in cntrType.GetFields (BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (info.FieldType == typeof (List<UIPartActionWindow>))
                    {
                        windowListField = info;
                        goto foundField;
                    }
                }
                Debug.LogWarning ("*PartUtils* Unable to find UIPartActionWindow list");
                return null;
            }

            foundField:
            List<UIPartActionWindow> uiPartActionWindows = (List<UIPartActionWindow>) windowListField.GetValue (controller);
            if (uiPartActionWindows == null)
                return null;

            return uiPartActionWindows.FirstOrDefault (window => window != null && window.part == part);
        }

        #endregion
    }
}
