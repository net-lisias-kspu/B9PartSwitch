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
            public int id;
            public double amount = 0f;
            public double maxAmount = 0f;

            public ResourceContainer (string _name)
            {
                name = _name;
                id = _name.GetHashCode ();
            }
        }

        public List<int> serializedNodeKeys;
        public List<string> serializedNodeNames;

        public List<int> serializedObjectKeys;
        public List<string> serializedObjectNames;

        public List<int> serializedResourceKeys;
        public List<string> serializedResourceNames;
        public List<string> serializedResourceTypes;
        public List<string> serializedResourceSizes;

        public List<string> serializedSubtypeNames;
        public List<float> serializedSubtypeMassAdded;
        public List<float> serializedSubtypeCostAdded;
        public List<string> serializedSubtypeNodeKeys;
        public List<string> serializedSubtypeObjectKeys;
        public List<int> serializedSubtypeResourceKey;

        public Dictionary<int, PresetNode> presetsNode;
        public Dictionary<int, PresetObject> presetsObject;
        public Dictionary<int, PresetResource> presetsResource;

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
            int subtypeSelectedIndexNext = subtypeSelectedIndex + 1;
            if (subtypeSelectedIndexNext >= subtypes.Count) subtypeSelectedIndexNext = 0;
            StartSubtypeSwitch (subtypeSelectedIndexNext, true);
        }

        [KSPEvent (guiActive = false, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "Previous")]
        public void EventSwitchToPrevious ()
        {
            int subtypeSelectedIndexNext = subtypeSelectedIndex - 1;
            if (subtypeSelectedIndexNext < 0) subtypeSelectedIndexNext = subtypes.Count - 1;
            StartSubtypeSwitch (subtypeSelectedIndexNext, true);
        }

        #endregion




        #region Overrides


        public override void OnLoad (ConfigNode node)
        {
            Debug.Log ("PSSM | OnLoad | Invoked");
            base.OnLoad (node);
            if (!configLoaded) // !cachedConfigurations.ContainsKey (part.name.GetHashCode ())
            {
                Debug.Log ("PSSM | OnLoad | There is no container for the part type " + part.name + " in the cached configurations dictionary yet");

                serializedNodeKeys = new List<int> ();
                serializedNodeNames = new List<string> ();

                serializedObjectKeys = new List<int> ();
                serializedObjectNames = new List<string> ();

                serializedResourceKeys = new List<int> ();
                serializedResourceNames = new List<string> ();
                serializedResourceTypes = new List<string> ();
                serializedResourceSizes = new List<string> ();

                serializedSubtypeNames = new List<string> ();
                serializedSubtypeCostAdded = new List<float> ();
                serializedSubtypeMassAdded = new List<float> ();
                serializedSubtypeNodeKeys = new List<string> ();
                serializedSubtypeObjectKeys = new List<string> ();
                serializedSubtypeResourceKey = new List<int> ();

                ConfigNode[] nodesTop = node.GetNodes ();
                Debug.Log ("PSSM | OnLoad | Found " + nodesTop.Length + " nodes within the module node");

                for (int i = 0; i < nodesTop.Length; ++i)
                {
                    ConfigNode nodeTop = nodesTop[i];
                    if (string.Equals (nodeTop.name, configNodePresetNode))
                    {
                        int key = int.Parse (nodeTop.GetValue ("key"));
                        string name = nodeTop.GetValue ("name");

                        serializedNodeKeys.Add (key);
                        serializedNodeNames.Add (name);
                        Debug.Log ("PSSM | OnLoad | Node preset found | Key: " + key + " | Name: " + name);
                    }
                    else if (string.Equals (nodeTop.name, configNodePresetObject))
                    {
                        int key = int.Parse (nodeTop.GetValue ("key"));
                        string name = nodeTop.GetValue ("name");

                        serializedObjectKeys.Add (key);
                        serializedObjectNames.Add (name);
                        Debug.Log ("PSSM | OnLoad | Object preset found | Key: " + key + " | Name: " + name);
                    }
                    else if (string.Equals (nodeTop.name, configNodePresetResource))
                    {
                        int key = int.Parse (nodeTop.GetValue ("key"));
                        string name = nodeTop.GetValue ("name");
                        string type = nodeTop.HasValue ("type") ? nodeTop.GetValue ("type") : "none";
                        string size = nodeTop.HasValue ("size") ? nodeTop.GetValue ("size") : "none";

                        serializedResourceKeys.Add (key);
                        serializedResourceNames.Add (name);
                        serializedResourceTypes.Add (type);
                        serializedResourceSizes.Add (size);
                        Debug.Log ("PSSM | OnLoad | Resource preset found | Key: " + key + " | Name: " + name + " | Type: " + type + " | Size: " + size);
                    }
                    if (string.Equals (nodeTop.name, configNodeSubtype))
                    {
                        string name = nodeTop.GetValue ("name");
                        string nodeKeys = nodeTop.GetValue ("nodeKeys");
                        string objectKeys = nodeTop.GetValue ("objectKeys");
                        int resourceKey = int.Parse (nodeTop.GetValue ("resourceKey"));
                        float costAdded = float.Parse (nodeTop.GetValue ("costAdded"));
                        float massAdded = float.Parse (nodeTop.GetValue ("massAdded"));

                        serializedSubtypeNames.Add (name);
                        serializedSubtypeNodeKeys.Add (nodeKeys);
                        serializedSubtypeObjectKeys.Add (objectKeys);
                        serializedSubtypeResourceKey.Add (resourceKey);
                        serializedSubtypeCostAdded.Add (costAdded);
                        serializedSubtypeMassAdded.Add (massAdded);
                        Debug.Log ("PSSM | OnLoad | Subtype found | Name: " + name + " | Nodes: " + nodeKeys + " | Objects: " + objectKeys + " | Resource preset: " + resourceKey);
                    }
                }

                Debug.Log ("PSSM | OnLoad | Subtype loading complete, total number: " + serializedSubtypeNames.Count);
                configLoaded = true;
            }
            else
            {
                Debug.Log ("PSSM | OnLoad | Config was already loaded");
            }
        }

        public override void OnSave (ConfigNode node)
        {
            base.OnSave (node);
        }

        public override void OnStart (PartModule.StartState state)
        {
            base.OnStart (state);
            if (subtypes == null) Debug.Log ("PSSM | OnStart | Subtype list is null");
            else Debug.Log ("PSSM | OnStart | Subtype list contains " + subtypes.Count + " entries");
            ParseSerializedLists ();
            SetupUI ();
            StartSubtypeSwitch (subtypeSelectedIndex, false);
        }

        private void ParseSerializedLists ()
        {
            presetsNode = new Dictionary<int, PresetNode> ();
            presetsObject = new Dictionary<int, PresetObject> ();
            presetsResource = new Dictionary<int, PresetResource> ();
            subtypes = new List<Subtype> ();

            Debug.Log ("PSSM | ParseSerializedLists | Node presets found: " + serializedNodeKeys.Count);
            for (int i = 0; i < serializedNodeKeys.Count; ++i)
            {
                PresetNode preset = new PresetNode ();
                preset.key = serializedNodeKeys[i];
                preset.name = serializedNodeNames[i];
                preset.reference = PartSubtypeTools.FindAttachNode (part, preset.name);
                presetsNode.Add (preset.key, preset);
            }

            Debug.Log ("PSSM | ParseSerializedLists | Object presets found: " + serializedObjectKeys.Count);
            for (int i = 0; i < serializedObjectKeys.Count; ++i)
            {
                PresetObject preset = new PresetObject ();
                preset.key = serializedObjectKeys[i];
                preset.name = serializedObjectNames[i];
                preset.target = PartSubtypeTools.FindTransform (part.transform, preset.name, true);
                presetsObject.Add (preset.key, preset);
            }

            Debug.Log ("PSSM | ParseSerializedLists | Resource presets found: " + serializedResourceKeys.Count);
            for (int i = 0; i < serializedResourceKeys.Count; ++i)
            {
                Debug.Log ("PSSM | ParseSerializedLists | Resource loop " + i);
                PresetResource preset = new PresetResource ();
                preset.key = serializedResourceKeys[i];
                preset.name = serializedResourceNames[i];
                presetsResource.Add (preset.key, preset);

                if (!string.Equals ("none", serializedResourceTypes[i]))
                {
                    string[] types = serializedResourceTypes[i].Split (',');
                    string[] sizes = serializedResourceSizes[i].Split (',');
                    for (int a = 0; a < types.Length; ++a)
                    {
                        Debug.Log ("PSSM | ParseSerializedLists | Found resource type: " + types[a] + " with max amount of " + sizes[a]);
                        ResourceContainer container = new ResourceContainer (types[a]);
                        container.maxAmount = double.Parse (sizes[a]);
                        container.amount = container.maxAmount;
                        preset.resources.Add (container);
                    }
                }
            }

            Debug.Log ("PSSM | ParseSerializedLists | Subtypes found: " + serializedSubtypeNames.Count);
            for (int i = 0; i < serializedSubtypeNames.Count; ++i)
            {
                Subtype subtype = new Subtype ();
                subtype.name = serializedSubtypeNames[i];
                subtype.costAdded = serializedSubtypeCostAdded[i];
                subtype.massAdded = serializedSubtypeMassAdded[i];
                subtype.nodeKeys = PartSubtypeTools.GetIntListFromField (serializedSubtypeNodeKeys[i]);
                subtype.objectKeys = PartSubtypeTools.GetIntListFromField (serializedSubtypeObjectKeys[i]);
                subtype.resourceKey = serializedSubtypeResourceKey[i];
                subtypes.Add (subtype);
            }
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
            for (int i = 0; i < partResources.Length; i++)
            {
                DestroyImmediate (partResources[i]);
            }
            for (int i = 0; i < preset.resources.Count; i++)
            {
                ConfigNode resourceNode = new ConfigNode ("RESOURCE");
                resourceNode.AddValue ("name", preset.resources[i].name);
                resourceNode.AddValue ("maxAmount", preset.resources[i].maxAmount);

                if (calledByPlayer && !HighLogic.LoadedSceneIsEditor) resourceNode.AddValue ("amount", 0.0f);
                else resourceNode.AddValue ("amount", preset.resources[i].amount);
                part.AddResource (resourceNode);
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

        public static Transform FindTransform (Transform parent, string name, bool reportSuccess)
        {
            if (parent.name.Equals (name)) return parent;
            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform child = parent.GetChild (i);
                Transform result = FindTransform (child, name, false);
                if (result != null)
                {
                    if (reportSuccess) Debug.Log ("PSSM | FindTransform | Found the transform " + name);
                    return result;
                }
            }
            return null;
        }

        public static AttachNode FindAttachNode (Part part, string name)
        {
            for (int i = 0; i < part.attachNodes.Count; ++i)
            {
                if (string.Equals (part.attachNodes[i].id, name))
                {
                    Debug.Log ("PSSM | FindAttachNode | Found the node " + name);
                    return part.attachNodes[i];
                }
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
