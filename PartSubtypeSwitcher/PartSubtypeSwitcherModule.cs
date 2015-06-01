using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using System.Reflection;

namespace PartSubtypeSwitcher
{
    public class PartSubtypeSwitcherModule : PartModule, IPartCostModifier
    {
        public const string configNodeSubtype = "SUBTYPE";
        public const string configNodePresetNode = "PRESET_NODE";
        public const string configNodePresetObject = "PRESET_OBJECT";
        public const string configNodePresetResource = "PRESET_RESOURCE";
        public const string configNodePresetResourceContainer = "PRESET_RESOURCE_CONTAINER";

        [Serializable]
        public class Container
        {
            public Dictionary<int, PresetNode> presetsNode;
            public Dictionary<int, PresetObject> presetsObject;
            public Dictionary<int, PresetResource> presetsResource;
            public List<Subtype> subtypes = new List<Subtype> ();
        }

        public static Dictionary<int, Container> cachedConfigurations = new Dictionary<int, Container> ();

        [Serializable]
        public class PresetNode
        {
            public int key;
            public string name;
            public AttachNode reference;
        }

        [Serializable]
        public class PresetObject
        {
            public int key;
            public string name;
            public Transform target;
        }

        [Serializable]
        public class PresetResource
        {
            public int key;
            public string configNodeID;
            public string name;
            public List<ResourceContainer> resources = new List<ResourceContainer> ();
        }

        [Serializable]
        public class Subtype
        {
            public string name;
            public float massAdded;
            public float costAdded;
            public List<int> nodeKeys = new List<int> ();
            public List<int> objectKeys = new List<int> ();
            public int resourceKey = 0;
        }

        [Serializable]
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

        // public List<int> serializedKeysNode = new List<int> ();
        // public List<int> serializedKeysObject = new List<int> ();
        // public List<int> serializedKeysResource = new List<int> ();

        // public List<PresetNode> serializedValuesNode = new List<PresetNode> ();
        // public List<PresetObject> serializedValuesObject = new List<PresetObject> ();
        // public List<PresetResource> serializedValuesResource = new List<PresetResource> ();

        public List<Subtype> subtypes = new List<Subtype> ();
        public Subtype subtypeSelected;

        [KSPField (isPersistant = true)]
        public int subtypeSelectedIndex = -1;

        [KSPField]
        public int subtypeGroup = 0;

        [KSPField]
        public float massBase = 0.25f;

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


        public override void OnLoad (ConfigNode node)
        {
            Debug.Log ("PSSM | OnLoad | Invoked");
            base.OnLoad (node);
            if (!configLoaded)
            {
                Debug.Log ("PSSM | OnLoad | Config was not loaded yet");

                presetsNode = new Dictionary<int, PresetNode> ();
                presetsObject = new Dictionary<int, PresetObject> ();
                presetsResource = new Dictionary<int, PresetResource> ();

                // serializedKeysNode = new List<int> ();
                // serializedKeysObject = new List<int> ();
                // serializedKeysResource = new List<int> ();

                // serializedValuesNode = new List<PresetNode> ();
                // serializedValuesObject = new List<PresetObject> ();
                // serializedValuesResource = new List<PresetResource> ();

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
                        Debug.Log ("PSSM | OnLoad | Node preset found | Key: " + preset.key + " | Name: " + preset.name);

                        // serializedKeysNode.Add (preset.key);
                        // serializedValuesNode.Add (preset);
                        presetsNode.Add (preset.key, preset);
                    }
                    else if (string.Equals (nodeTop.name, configNodePresetObject))
                    {
                        PresetObject preset = new PresetObject ();
                        preset.key = int.Parse (nodeTop.GetValue ("key"));
                        preset.name = nodeTop.GetValue ("name");
                        // preset.target = PartSubtypeTools.FindTransform (part.transform, preset.name);
                        Debug.Log ("PSSM | OnLoad | Object preset found | Key: " + preset.key + " | Name: " + preset.name + " | Object was " + (preset.target == null ? "not found" : "found"));

                        // serializedKeysObject.Add (preset.key);
                        // serializedValuesObject.Add (preset);
                        presetsObject.Add (preset.key, preset);
                    }
                    else if (string.Equals (nodeTop.name, configNodePresetResource))
                    {
                        PresetResource preset = new PresetResource ();
                        preset.key = int.Parse (nodeTop.GetValue ("key"));
                        preset.name = nodeTop.GetValue ("name");
                        preset.configNodeID = nodeTop.id;
                        preset.resources = new List<ResourceContainer> ();
                        Debug.Log ("PSSM | OnLoad | Resource preset found | Key: " + preset.key + " | Name: " + preset.name);

                        // serializedKeysResource.Add (preset.key);
                        // serializedValuesResource.Add (preset);
                        presetsResource.Add (preset.key, preset);

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

                int key = part.name.GetHashCode ();
                if (cachedConfigurations.ContainsKey (key))
                {
                    Debug.Log ("PSSM | OnLoad | Cached configurations dictionary already contains key " + key);
                }
                else
                {
                    Debug.Log ("PSSM | OnLoad | Adding new cached configuration for part type key " + key);
                    Container container = new Container ();
                    container.presetsNode = presetsNode;
                    container.presetsObject = presetsObject;
                    container.presetsResource = presetsResource;
                    container.subtypes = subtypes;
                    cachedConfigurations.Add (key, container);
                }

                configLoaded = true;
            }
            else
            {
                Debug.Log ("PSSM | OnLoad | Config was already loaded");
            }

            if (subtypes == null) Debug.Log ("PSSM | OnLoad | Subtype list is null");
            else Debug.Log ("PSSM | OnLoad | Subtype list contains " + subtypes.Count + " entries");
        }

        public override void OnSave (ConfigNode node)
        {
            base.OnSave (node);

            if (subtypes == null) Debug.Log ("PSSM | OnSave | Subtype list is null");
            else Debug.Log ("PSSM | OnSave | Subtype list contains " + subtypes.Count + " entries");

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

        public override void OnAwake ()
        {
            base.OnAwake ();
            if (subtypes == null) Debug.Log ("PSSM | OnAwake | Subtype list is null");
            else Debug.Log ("PSSM | OnAwake | Subtype list contains " + subtypes.Count + " entries");
        }

        public override void OnStart (PartModule.StartState state)
        {
            base.OnStart (state);
            if (subtypes == null) Debug.Log ("PSSM | OnStart | Subtype list is null");
            else Debug.Log ("PSSM | OnStart | Subtype list contains " + subtypes.Count + " entries");
            CreatePresetDictionaries ();
            SetupUI ();
            StartSubtypeSwitch (subtypeSelectedIndex, false);
        }

        private void CreatePresetDictionaries ()
        {
            int key = part.name.GetHashCode ();
            if (cachedConfigurations.ContainsKey (key))
            {
                Container container = cachedConfigurations[key];
                presetsNode = container.presetsNode;
                presetsObject = container.presetsObject;
                presetsResource = container.presetsResource;
                subtypes = container.subtypes;
                Debug.Log ("PSSM | CreatePresetDictionaries | Dictionary entry found | Node presets: " + presetsNode.Count + " | Object presets: " + presetsObject.Count + " | Resource presets: " + presetsResource.Count + " | Subtypes: " + subtypes.Count);

                for (int i = 0; i < presetsNode.Count; ++i)
                {
                    for (int a = 0; a < part.attachNodes.Count; ++a)
                    {
                        if (string.Equals (part.attachNodes[a].id, presetsNode[i].name))
                        {
                            Debug.Log ("PSSM | CreatePresetDictionaries | Found the node " + presetsNode[i].name);
                            presetsNode[i].reference = part.attachNodes[a];
                        }
                    }
                }

                Transform partTransform = part.gameObject.GetComponent<Transform> ();
                for (int i = 0; i < presetsObject.Count; ++i)
                {
                    presetsObject[i].target = PartSubtypeTools.FindTransform (partTransform, presetsObject[i].name);
                }
            }
            else
            {
                Debug.Log ("PSSM | CreatePresetDictionaries | There is no dictionary entry for key " + key);
            }
            /*
            if (serializedKeysNode != null && serializedValuesNode != null)
            {
                Debug.Log ("PSSM | CreatePresetDictionaries | Serialized lists found for the node preset dictionary");
                presetsNode = new Dictionary<int, PresetNode> ();
                for (int i = 0; i < serializedKeysNode.Count; ++i)
                {
                    if (i > serializedValuesNode.Count - 1)
                    {
                        Debug.Log ("PSSM | CreatePresetDictionaries | Node preset value list length is too short for index " + i);
                        break;
                    }
                    if (serializedValuesNode[i] == null)
                    {
                        Debug.Log ("PSSM | CreatePresetDictionaries | Node preset value list returns null at index " + i);
                        break;
                    }
                    presetsNode.Add (serializedKeysNode[i], serializedValuesNode[i]);
                }
            }
            else
            {
                Debug.Log ("PSSM | CreatePresetDictionaries | Serialized lists for the node preset dictionary are missing");
            }

            if (serializedKeysObject != null && serializedValuesObject != null)
            {
                Debug.Log ("PSSM | CreatePresetDictionaries | Serialized lists found for the object preset dictionary");
                presetsObject = new Dictionary<int, PresetObject> ();
                for (int i = 0; i < serializedKeysObject.Count; ++i) presetsObject.Add (serializedKeysObject[i], serializedValuesObject[i]);
            }
            else
            {
                Debug.Log ("PSSM | CreatePresetDictionaries | Serialized lists for the object preset dictionary are missing");
            }

            if (serializedKeysResource != null && serializedValuesResource != null)
            {
                Debug.Log ("PSSM | CreatePresetDictionaries | Serialized lists found for the resource preset dictionary");
                presetsResource = new Dictionary<int, PresetResource> ();
                for (int i = 0; i < serializedKeysResource.Count; ++i) presetsResource.Add (serializedKeysResource[i], serializedValuesResource[i]);
            }
            else
            {
                Debug.Log ("PSSM | CreatePresetDictionaries | Serialized lists for the resource preset dictionary are missing");
            }
            */
        }

        private void SetupUI ()
        {
            if (initialized) return;
            initialized = true;

            Events["EventSwitchToNext"].guiActive = availableInFlight;
            Events["EventSwitchToNext"].guiActiveEditor = availableInEditor;
            Events["EventSwitchToNext"].guiName = uiCaptionNext;

            Events["EventSwitchToPrevious"].guiActive = availableInFlight;
            Events["EventSwitchToPrevious"].guiActiveEditor = availableInEditor;
            Events["EventSwitchToPrevious"].guiName = uiCaptionPrev;

            if (!uiUseSecondButton) Events["EventSwitchToPrevious"].guiActiveEditor = false;
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
            Debug.Log ("PSSM | SetSubtype | Starting the switch to subtype " + subtypeSelectedIndexNext);
            if (subtypeSelectedIndexNext == -1) subtypeSelectedIndexNext = 0;
            if (subtypeSelectedIndexNext == subtypeSelectedIndex)
            {
                Debug.Log ("PSSM | SetSubtype | Requested subtype index (" + subtypeSelectedIndexNext + ") is already in use");
                return;
            }
            Debug.Log ("PSSM | SetSubtype | Passed check 1");
            if (!configLoaded)
            {
                Debug.Log ("PSSM | SetSubtype | Config was not yet loaded, aborting");
                return;
            }
            Debug.Log ("PSSM | SetSubtype | Passed check 2");
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)
            {
                Debug.Log ("PSSM | SetSubtype | Scene is not valid, aborting");
                return;
            }
            Debug.Log ("PSSM | SetSubtype | Passed check 3");
            if (subtypes == null)
            {
                Debug.Log ("PSSM | SetSubtype | Subtype list is null, what the fuck");
                return;
            }
            Debug.Log ("PSSM | SetSubtype | Passed check 4");
            if (subtypes[subtypeSelectedIndexNext] == null)
            {
                Debug.Log ("PSSM | SetSubtype | Requested subtype at index (" + subtypeSelectedIndexNext + ") is null");
                return;
            }

            SetupUI ();
            Debug.Log ("PSSM | SetSubtype | Passed initial checks to switch to subtype " + subtypeSelectedIndexNext + " | Registered subtypes: " + subtypes.Count);
            subtypeSelected = subtypes[subtypeSelectedIndexNext];
            subtypeSelectedIndex = subtypeSelectedIndexNext;
            subtypeSelectedNameUI = subtypeSelected.name + " (" + presetsResource[subtypeSelected.resourceKey].name + ")";

            Debug.Log ("PSSM | SetSubtype | Iterating through node presets");
            for (int i = 0; i < presetsNode.Count; ++i)
            {
                bool used = subtypeSelected.nodeKeys.Contains (presetsNode[i].key);
                SetNodeState (presetsNode[i], used);
            }

            Debug.Log ("PSSM | SetSubtype | Iterating through object presets");
            for (int i = 0; i < presetsObject.Count; ++i)
            {
                bool used = subtypeSelected.objectKeys.Contains (presetsObject[i].key);
                SetObjectState (presetsObject[i], used);
            }

            Debug.Log ("PSSM | SetSubtype | Assigning the resource preset " + subtypeSelected.resourceKey + " (" + presetsResource[subtypeSelected.resourceKey].name + ")");
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
            if (subtypeSelected != null) part.mass = massBase + subtypeSelected.massAdded;
            else part.mass = massBase;
        }

        public void UpdateCost ()
        {
            if (subtypeSelected != null) uiCaptionAddedCost = subtypeSelected.costAdded;
            else uiCaptionAddedCost = 0f;
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
