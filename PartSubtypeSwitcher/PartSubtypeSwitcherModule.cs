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
            public bool saved;
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
        public List<string> serializedResourceAmounts;

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
            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Invoked");
            base.OnLoad (node);

            if (!configLoaded)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Config was not loaded yet, creating serialized lists with all the values");

                serializedNodeKeys = new List<int> ();
                serializedNodeNames = new List<string> ();

                serializedObjectKeys = new List<int> ();
                serializedObjectNames = new List<string> ();

                serializedResourceKeys = new List<int> ();
                serializedResourceNames = new List<string> ();
                serializedResourceTypes = new List<string> ();
                serializedResourceSizes = new List<string> ();
                serializedResourceAmounts = new List<string> ();

                serializedSubtypeNames = new List<string> ();
                serializedSubtypeCostAdded = new List<float> ();
                serializedSubtypeMassAdded = new List<float> ();
                serializedSubtypeNodeKeys = new List<string> ();
                serializedSubtypeObjectKeys = new List<string> ();
                serializedSubtypeResourceKey = new List<int> ();

                ConfigNode[] nodesTop = node.GetNodes ();
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Found " + nodesTop.Length + " nodes within the module node");

                for (int i = 0; i < nodesTop.Length; ++i)
                {
                    ConfigNode nodeTop = nodesTop[i];
                    if (string.Equals (nodeTop.name, configNodePresetNode))
                    {
                        int key = int.Parse (nodeTop.GetValue ("key"));
                        string name = nodeTop.GetValue ("name");

                        serializedNodeKeys.Add (key);
                        serializedNodeNames.Add (name);
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Node preset found | Key: " + key + " | Name: " + name);
                    }
                    else if (string.Equals (nodeTop.name, configNodePresetObject))
                    {
                        int key = int.Parse (nodeTop.GetValue ("key"));
                        string name = nodeTop.GetValue ("name");

                        serializedObjectKeys.Add (key);
                        serializedObjectNames.Add (name);
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Object preset found | Key: " + key + " | Name: " + name);
                    }
                    else if (string.Equals (nodeTop.name, configNodePresetResource))
                    {
                        int key = int.Parse (nodeTop.GetValue ("key"));
                        string name = nodeTop.GetValue ("name");
                        string type = nodeTop.HasValue ("type") ? nodeTop.GetValue ("type") : "none";
                        string size = nodeTop.HasValue ("size") ? nodeTop.GetValue ("size") : "none";
                        string amount = nodeTop.HasValue ("amount") ? nodeTop.GetValue ("amount") : "none";

                        serializedResourceKeys.Add (key);
                        serializedResourceNames.Add (name);
                        serializedResourceTypes.Add (type);
                        serializedResourceSizes.Add (size);
                        serializedResourceAmounts.Add (amount);
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Resource preset found | Key: " + key + " | Name: " + name + " | Type: " + type + " | Size: " + size + " | Amount: " + amount);
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
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Subtype found | Name: " + name + " | Nodes: " + nodeKeys + " | Objects: " + objectKeys + " | Resource preset: " + resourceKey);
                    }
                }

                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Config loading complete");
                configLoaded = true;
            }
            else
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Config was already loaded, updating only resource amounts");

                serializedResourceAmounts = new List<string> ();

                ConfigNode[] nodesTop = node.GetNodes ();
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Found " + nodesTop.Length + " nodes within the module node");

                for (int i = 0; i < nodesTop.Length; ++i)
                {
                    ConfigNode nodeTop = nodesTop[i];
                    if (string.Equals (nodeTop.name, configNodePresetResource))
                    {
                        int key = int.Parse (nodeTop.GetValue ("key"));
                        string amount = nodeTop.HasValue ("amount") ? nodeTop.GetValue ("amount") : "none";

                        serializedResourceAmounts.Add (amount);
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnLoad | Resource preset found | Key: " + key + " | Name: " + name + " | Amount: " + amount);
                    }
                }

                ParseSerializedLists ();
                SetupUI ();
                StartSubtypeSwitch (subtypeSelectedIndex, false);
            }
        }

        public override void OnSave (ConfigNode node)
        {
            base.OnSave (node);
            ReportOnModelState ("OnSave");
            SaveResourceAmountToPreset ();

            foreach (KeyValuePair<int, PresetResource> entry in presetsResource)
            {
                entry.Value.saved = false;
            }

            ConfigNode[] nodesTop = node.GetNodes ();
            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnSave | Found " + nodesTop.Length + " nodes within the module node");

            for (int i = 0; i < nodesTop.Length; ++i)
            {
                ConfigNode nodeTop = nodesTop[i];
                if (string.Equals (nodeTop.name, configNodePresetResource))
                {
                    int key = int.Parse (nodeTop.GetValue ("key"));
                    string name = nodeTop.GetValue ("name");
                    // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnSave | Resource preset node found | Key: " + key + " | Name: " + name);

                    PresetResource preset = presetsResource[key];
                    preset.saved = true;

                    string amount = string.Empty;
                    List<ResourceContainer> containers = preset.resources;
                    for (int a = 0; a < containers.Count; ++a)
                    {
                        if (amount.Length > 0) amount += ",";
                        amount += containers[a].amount;
                    }

                    if (amount.Length > 0)
                    {
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnSave | Node " + i + " (key " + key + ", name " + name + ") receives the following amount string: " + amount);
                        nodeTop.SetValue ("amount", amount, true);
                    }
                    else
                    {
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnSave | Node " + i + " (key " + key + ", name " + name + ") has no resources");
                    }
                }
            }

            foreach (KeyValuePair<int, PresetResource> entry in presetsResource)
            {
                PresetResource preset = entry.Value;
                if (!preset.saved)
                {
                    // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnSave | Preset " + preset.key + " (" + preset.name + ") is not marked as saved, indicating absence of a config node");
                    ConfigNode nodePreset = new ConfigNode (configNodePresetResource);

                    string amount = string.Empty;
                    List<ResourceContainer> containers = preset.resources;
                    for (int a = 0; a < containers.Count; ++a)
                    {
                        if (amount.Length > 0) amount += ",";
                        amount += containers[a].amount;
                    }

                    nodePreset.AddValue ("key", preset.key.ToString ());
                    nodePreset.AddValue ("name", preset.name);

                    if (amount.Length > 0)
                    {
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnSave | New config node receives the following amount string: " + amount);
                        nodePreset.SetValue ("amount", amount, true);
                    }
                    else
                    {
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnSave | New config node has no resources");
                    }

                    node.AddNode (nodePreset);
                    preset.saved = true;
                }
                else
                {
                    // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnSave | Preset " + preset.key + " (" + preset.name + ") is marked as saved");
                }
            }
        }

        private void SaveResourceAmountToPreset ()
        {
            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SaveResourceAmountToPreset | Invoked");
            
            if (subtypeSelected == null || subtypeSelectedIndex == -1)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SaveResourceAmountToPreset | There is no selected subtype, aborting...");
                return;
            }

            List<ResourceContainer> resourcesPreset = presetsResource[subtypeSelected.resourceKey].resources;
            List<PartResource> resourcesPart = part.Resources.list;

            if (resourcesPreset.Count != resourcesPart.Count)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SaveResourceAmountToPreset | Preset has different number of resources (" + resourcesPreset.Count + ") from the number of resources in the part (" + resourcesPart.Count + "), which should not happen with correctly executed setup.");
                return;
            }

            if (resourcesPreset.Count == 0 && resourcesPart.Count == 0)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SaveResourceAmountToPreset | No resources used in the current preset or exist on the part");
                return;
            }

            for (int i = 0; i < resourcesPreset.Count; ++i)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SaveResourceAmountToPreset | Resource " + i + " (" + resourcesPreset[i].name + ") | Amount before/after: " + resourcesPreset[i].amount + " > " + resourcesPart[i].amount);
                resourcesPreset[i].amount = resourcesPart[i].amount;
            }
        }

        public override void OnStart (PartModule.StartState state)
        {
            base.OnStart (state);

            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | OnStart | Invoked");
            ParseSerializedLists ();
            SetupUI ();
            StartSubtypeSwitch (subtypeSelectedIndex, false);
        }

        private void ReportOnModelState (string caller)
        {
            string report = "PSSM | " + debugCounter.ToString ("0000") + " | " + caller + " | Model state: \n";
            report += "Subtypes: " + (subtypes == null ? "null" : subtypes.Count.ToString ()) + "\n";
            report += "Node presets: " + (presetsNode == null ? "null" : presetsNode.Count.ToString ()) + "\n";
            report += "Object presets: " + (presetsObject == null ? "null" : presetsObject.Count.ToString ()) + "\n";
            report += "Resource presets: " + (presetsResource == null ? "null" : presetsResource.Count.ToString ()) + "\n";
            // Debug.Log (report);
        }

        private void ParseSerializedLists ()
        {
            ReportOnModelState ("ParseSerializedLists");
            if (presetsNode != null && presetsObject != null && presetsResource != null && subtypes != null)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | ParseSerializedLists | Looks like all presets and subtypes were already loaded, aborting the method");
                return;
            }

            presetsNode = new Dictionary<int, PresetNode> ();
            presetsObject = new Dictionary<int, PresetObject> ();
            presetsResource = new Dictionary<int, PresetResource> ();
            subtypes = new List<Subtype> ();

            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | ParseSerializedLists | Node presets found: " + serializedNodeKeys.Count);
            for (int i = 0; i < serializedNodeKeys.Count; ++i)
            {
                PresetNode preset = new PresetNode ();
                preset.key = serializedNodeKeys[i];
                preset.name = serializedNodeNames[i];
                preset.reference = PartSubtypeTools.FindAttachNode (part, preset.name);
                presetsNode.Add (preset.key, preset);
            }

            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | ParseSerializedLists | Object presets found: " + serializedObjectKeys.Count);
            for (int i = 0; i < serializedObjectKeys.Count; ++i)
            {
                PresetObject preset = new PresetObject ();
                preset.key = serializedObjectKeys[i];
                preset.name = serializedObjectNames[i];
                preset.target = PartSubtypeTools.FindTransform (part.transform, preset.name, true);
                presetsObject.Add (preset.key, preset);
            }

            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | ParseSerializedLists | Resource presets found: " + serializedResourceKeys.Count);
            for (int i = 0; i < serializedResourceKeys.Count; ++i)
            {
                PresetResource preset = new PresetResource ();
                preset.key = serializedResourceKeys[i];
                preset.name = serializedResourceNames[i];
                presetsResource.Add (preset.key, preset);
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | ParseSerializedLists | Writing resource preset | Key: " + preset.key + " | Name: " + preset.name + " | Config node ID: " + preset.configNodeID);

                if (!string.Equals ("none", serializedResourceTypes[i]))
                {
                    string[] types = serializedResourceTypes[i].Split (',');
                    string[] sizes = serializedResourceSizes[i].Split (',');
                    string[] amounts = serializedResourceAmounts[i].Split (',');

                    bool useAmounts = false;
                    if (!string.Equals (serializedResourceAmounts[i], "none"))
                    {
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | ParseSerializedLists | Resource preset " + preset.name + " has amount listed: " + serializedResourceAmounts[i]);
                        useAmounts = true;
                    }

                    for (int a = 0; a < types.Length; ++a)
                    {
                        // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | ParseSerializedLists | Found resource type: " + types[a] + " with max amount of " + sizes[a]);
                        ResourceContainer container = new ResourceContainer (types[a]);
                        container.maxAmount = double.Parse (sizes[a]);
                        container.amount = useAmounts ? double.Parse (amounts[a]) : container.maxAmount;
                        preset.resources.Add (container);
                    }
                }
            }

            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | ParseSerializedLists | Subtypes found: " + serializedSubtypeNames.Count);
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
            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Starting the switch to subtype " + subtypeSelectedIndexNext);
            if (subtypeSelectedIndexNext == -1) subtypeSelectedIndexNext = 0;
            if (!configLoaded)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Config was not yet loaded, aborting");
                return;
            }
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Scene is not valid, aborting");
                return;
            }
            if (subtypes == null)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Subtype list is null, what the fuck");
                return;
            }
            if (subtypes[subtypeSelectedIndexNext] == null)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Requested subtype at index (" + subtypeSelectedIndexNext + ") is null");
                return;
            }

            SetupUI ();
            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Passed initial checks to switch to subtype " + subtypeSelectedIndexNext + " | Registered subtypes: " + subtypes.Count);

            SaveResourceAmountToPreset ();
            subtypeSelected = subtypes[subtypeSelectedIndexNext];
            subtypeSelectedIndex = subtypeSelectedIndexNext;
            subtypeSelectedNameUI = subtypeSelectedIndex + "/" + subtypes.Count + ": " + subtypeSelected.name + " (" + presetsResource[subtypeSelected.resourceKey].name + ")";

            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Iterating through node presets");
            for (int i = 0; i < presetsNode.Count; ++i)
            {
                bool used = subtypeSelected.nodeKeys.Contains (presetsNode[i].key);
                SetNodeState (presetsNode[i], used);
            }

            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Iterating through object presets");
            for (int i = 0; i < presetsObject.Count; ++i)
            {
                bool used = subtypeSelected.objectKeys.Contains (presetsObject[i].key);
                SetObjectState (presetsObject[i], used);
            }

            // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Assigning the resource preset " + subtypeSelected.resourceKey + " (" + presetsResource[subtypeSelected.resourceKey].name + ")");
            AssignResourcePreset (presetsResource[subtypeSelected.resourceKey], calledByPlayer);
            UpdateWeight ();
            UpdateCost ();
        }

        private void SetNodeState (PresetNode preset, bool used)
        {
            if (preset.reference == null)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetNodeState | Node reference in node preset " + preset.key + " is null, aborting the switch");
                return;
            }
            if (!used && preset.reference.nodeType == AttachNode.NodeType.Stack)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetNodeState | Disabling node " + preset.reference.id);
                preset.reference.nodeType = AttachNode.NodeType.Dock;
                preset.reference.radius = 0.001f;
            }
            else if (preset.reference.nodeType == AttachNode.NodeType.Dock)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetNodeState | Enabling node " + preset.reference.id);
                preset.reference.nodeType = AttachNode.NodeType.Stack;
                preset.reference.radius = 0.4f;
            }
            else
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetSubtype | Node " + preset.reference.id + " is not affected by this switch");
            }
        }

        private void SetObjectState (PresetObject preset, bool used)
        {
            if (preset.target == null)
            {
                // Debug.Log ("PSSM | " + debugCounter.ToString ("0000") + " | SetNodeState | Transform reference in object preset " + preset.key + " is null, aborting the switch");
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

        private int debugCounter = 0;

        public void Update ()
        {
            debugCounter += 1;
            if (debugCounter > 1000) debugCounter = 0;
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
                    if (reportSuccess) // Debug.Log ("PSSM | FindTransform | Found the transform " + name);
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
                    // Debug.Log ("PSSM | FindAttachNode | Found the node " + name);
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
                // Debug.LogWarning ("*PartUtils* Unable to find UIPartActionWindow list");
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
