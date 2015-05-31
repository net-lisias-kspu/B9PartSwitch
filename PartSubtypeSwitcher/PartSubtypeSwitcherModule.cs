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

    public class PartSubtypeSwitcherModule: PartModule
    {
        public const string nodeNameSubtype = "SUBTYPE";
        public const string nodeNamePresetNode = "PRESET_NODE";
        public const string nodeNamePresetObject = "PRESET_OBJECT";
        public const string nodeNamePresetResource = "PRESET_RESOURCE";
        public const string nodeNamePresetResourceBlock = "PRESET_RESOURCE_CONTAINER";

        public class PresetNode
        {
            public int key;
            public string name;
        }

        public class PresetObject
        {
            public int key;
            public string name;
            public Transform t;
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
            public double massAdded;
            public double costAdded;
            public List<int> indexesNodes = new List<int> ();
            public List<int> indexesObjects = new List<int> ();
            public List<int> indexesResources = new List<int> ();
        }

        public class ResourceContainer
        {
            public string name;
            public string configNodeID;
            public int id;
            public double amount = 0f;
            public double amountMax = 0f;

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


        [KSPField]
        public float massBase = 0.25f;

        [KSPField (isPersistant = true)]
        public int subtypeSelected = 0;

        [KSPField]
        public string uiCaptionNext = "Next subtype";

        [KSPField]
        public string uiCaptionPrev = "Prev subtype";






        #region FieldsMeshSwitching

        [KSPField] public int moduleID = 0;


        [KSPField] public string objectDisplayNames = string.Empty;
        [KSPField] public bool showPreviousButton = true;
        [KSPField] public string fuelTankSetups = "0";

        [KSPField] public string objects = string.Empty;
        [KSPField] public bool updateSymmetry = true;
        [KSPField] public bool affectColliders = true;

        // [KSPField] public bool showInfo = true;
        // [KSPField] public bool debugMode = false;
        [KSPField(isPersistant = true)] public int selectedObject = 0;

        private List<List<Transform>> objectTransforms = new List<List<Transform>>();
        private List<int> fuelTankSetupList = new List<int>();
        private List<string> objectDisplayList = new List<string>();

        #endregion




        #region FieldsFuelSwitching

        [KSPField] public string resourceNames = "ElectricCharge;LiquidFuel,Oxidizer;MonoPropellant";
        [KSPField] public string resourceAmounts = "100;75,25;200";
        [KSPField] public string initialResourceAmounts = "";

        [KSPField] public string tankMass = "0;0;0;0";
        [KSPField] public string tankCost = "0; 0; 0; 0";

        // [KSPField] public bool displayCurrentTankCost = false;
        [KSPField] public bool availableInFlight = false;

        [KSPField] public bool availableInEditor = true;
        [KSPField (isPersistant = true)] public int selectedTankSetup = -1;
        [KSPField (isPersistant = true)] public bool hasLaunched = false;

        [KSPField (guiActive = false, guiActiveEditor = false, guiName = "Added cost")]
        public float addedCost = 0f;

        [KSPField (guiActive = false, guiActiveEditor = true, guiName = "Dry mass")]
        public float dryMassInfo = 0f;

        // private List<Tank> tankList;
        private List<double> weightList;
        private List<double> tankCostList;

        [KSPField (isPersistant = true)]
        public bool configLoaded = false;
        private bool initialized = false;
        UIPartActionWindow tweakableUI;

        #endregion



        #region Events

        [KSPField (guiActiveEditor = true, guiName = "Current variant")]
        public string currentObjectName = string.Empty;
        
        [KSPEvent (guiActive = false, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "Next variant")]
        public void EventSwitchToNext ()
        {
            selectedObject++;
            if (selectedObject >= objectTransforms.Count)
                selectedObject = 0;
            SwitchToObject(selectedObject, true);
            NodeRemapForConfiguration (selectedObject);
        }

        [KSPEvent (guiActive = false, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "Prev variant")]
        public void EventSwitchToPrevious ()
        {
            selectedObject--;
            if (selectedObject < 0)
                selectedObject = objectTransforms.Count - 1;
            SwitchToObject(selectedObject, true);
            NodeRemapForConfiguration (selectedObject);
        }

        #endregion




        #region Overrides

        public override void OnStart (PartModule.StartState state)
        {
            // LoadConfig ();
            SwitchToObject (selectedObject, false);

            Events["EventSwitchToNext"].guiName = uiCaptionNext;
            Events["EventSwitchToPrevious"].guiName = uiCaptionPrev;

            if (!showPreviousButton) Events["EventSwitchToPrevious"].guiActiveEditor = false;
            if (selectedTankSetup == -1)
            {
                selectedTankSetup = 0;
                AssignResourcesToPart (false);
            }

            NodeOnStart ();
        }

        public override void OnAwake ()
        {
            if (configLoaded)
                InitializeData ();
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
                    if (string.Equals (nodeTop.name, nodeNamePresetNode))
                    {
                        PresetNode preset = new PresetNode ();
                        preset.key = int.Parse (nodeTop.GetValue ("key"));
                        preset.name = nodeTop.GetValue ("name");
                        presetsNode.Add (preset.key, preset);
                        Debug.Log ("PSSM | OnLoad | Node preset found | Key: " + preset.key + " | Name: " + preset.name);
                    }
                    else if (string.Equals (nodeTop.name, nodeNamePresetObject))
                    {
                        PresetObject preset = new PresetObject ();
                        preset.key = int.Parse (nodeTop.GetValue ("key"));
                        preset.name = nodeTop.GetValue ("name");
                        preset.t = PartSubtypeTools.FindTransform (part.transform, preset.name);
                        presetsObject.Add (preset.key, preset);
                        Debug.Log ("PSSM | OnLoad | Object preset found | Key: " + preset.key + " | Name: " + preset.name + " | Object was " + (preset.t == null ? "not found" : "found"));
                    }
                    else if (string.Equals (nodeTop.name, nodeNamePresetResource))
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
                            if (string.Equals (nodeInPreset.name, nodeNamePresetResourceBlock))
                            {
                                ResourceContainer container = new ResourceContainer (nodeInPreset.GetValue ("name"));
                                container.amountMax = double.Parse (nodeInPreset.GetValue ("amountMax"));
                                if (nodeInPreset.HasValue ("amount")) container.amount = double.Parse (nodeInPreset.GetValue ("amount"));
                                else container.amount = container.amountMax;
                                preset.resources.Add (container);
                                Debug.Log ("PSSM | OnLoad | Resource preset container found | Name: " + container.name + " | Amount: " + container.amount);
                            }
                        }
                    }
                    else if (string.Equals (nodeTop.name, nodeNameSubtype))
                    {
                        Subtype subtype = new Subtype ();
                        subtype.name = nodeTop.GetValue ("name");

                        string indexesNodesRaw = nodeTop.GetValue ("indexesNodes");
                        string indexesObjectsRaw = nodeTop.GetValue ("indexesObjects");
                        string indexesResourcesRaw = nodeTop.GetValue ("indexesResources");

                        Debug.Log ("PSSM | OnLoad | Subtype found | Name: " + subtype.name + " | Nodes: " + indexesNodesRaw + " | Objects: " + indexesObjectsRaw + " | Resources: " + indexesResourcesRaw);

                        subtype.indexesNodes = PartSubtypeTools.GetIntListFromField (indexesNodesRaw);
                        subtype.indexesObjects = PartSubtypeTools.GetIntListFromField (indexesObjectsRaw);
                        subtype.indexesResources = PartSubtypeTools.GetIntListFromField (indexesResourcesRaw);

                        // Add calculation of added mass and cost here
                        // based on extracted resource data

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


        public void InitializeData ()
        {
            if (!initialized)
            {
                updateSymmetry = true;
                ParseObjectNames ();
                fuelTankSetupList = PartSubtypeTools.ParseIntegers (fuelTankSetups);
                objectDisplayList = PartSubtypeTools.ParseNames (objectDisplayNames);
                SetupTankList (false);
                weightList = PartSubtypeTools.ParseDoubles (tankMass);
                tankCostList = PartSubtypeTools.ParseDoubles (tankCost);

                if (HighLogic.LoadedSceneIsFlight) hasLaunched = true;
                // if (HighLogic.CurrentGame == null || HighLogic.CurrentGame.Mode == Game.Modes.CAREER) Fields["addedCost"].guiActiveEditor = displayCurrentTankCost;

                initialized = true;
            }
        }

        #endregion




        #region MeshSwitching

        private void SwitchToObject (int objectNumber, bool calledByPlayer)
        {
            SetObject(objectNumber, calledByPlayer);
            if (updateSymmetry)
            {
                for (int i = 0; i < part.symmetryCounterparts.Count; i++)
                {
                    PartSubtypeSwitcherModule[] modules = part.symmetryCounterparts[i].GetComponents<PartSubtypeSwitcherModule> ();
                    for (int j = 0; j < modules.Length; j++)
                    {
                        if (modules[j].moduleID == moduleID)
                        {
                            modules[j].selectedObject = selectedObject;
                            modules[j].SetObject(objectNumber, calledByPlayer);
                        }
                    }
                }
            }
        }

        public Dictionary<int, Collider> cachedColliders = new Dictionary<int,Collider> ();

        private void SetObject (int objectNumber, bool calledByPlayer)
        {
            InitializeData ();
            for (int i = 0; i < objectTransforms.Count; i++)
            {
                for (int j = 0; j < objectTransforms[i].Count; j++)
                {
                    // Debug.Log ("PSSM | SetObject | Setting object enabled");
                    objectTransforms[i][j].gameObject.SetActive(false);
                    if (affectColliders)
                    {
                        SetCollider (objectTransforms[i][j], false);
                        //Debug.Log ("PSSM | SetObject | Setting collider states");
                        //if (!cachedColliders.ContainsKey (objectTransforms[i][j].GetInstanceID ()))
                        //{
                        //    if (objectTransforms[i][j].gameObject.collider != null)
                        //        objectTransforms[i][j].gameObject.collider.enabled = false;
                        //}
                    }                    
                }
            }
            
            // Enable the selected one last because there might be several entries with the same object
            // We don't want to disable it after it's been enabled

            for (int i = 0; i < objectTransforms[objectNumber].Count; i++)
            {
                objectTransforms[objectNumber][i].gameObject.SetActive(true);
                if (affectColliders)
                {
                    SetCollider (objectTransforms[objectNumber][i], true);
                    //if (objectTransforms[objectNumber][i].gameObject.collider != null)
                    //{
                    //    Debug.Log ("PSSM | SetObject | Setting collider true on new active object");
                    //    objectTransforms[objectNumber][i].gameObject.collider.enabled = true;
                    //}
                }                
            }            

            if (objectNumber < fuelTankSetupList.Count) SelectTankSetup (fuelTankSetupList[objectNumber], calledByPlayer);
            else Debug.Log ("PSSM | SetObject | No such fuel tank setup");

            SetCurrentObjectName ();
        }

        private void SetCollider (Transform t, bool state)
        {
            if (t == null) return;
            int instanceID = t.GetInstanceID ();
            if (cachedColliders.ContainsKey (instanceID))
            {
                if (cachedColliders[instanceID] != null)
                {
                    cachedColliders[instanceID].enabled = state;
                }
            }
            else
            {
                Collider collider = t.gameObject.GetComponent<Collider> ();
                if (collider != null)
                {
                    cachedColliders.Add (instanceID, collider);
                    collider.enabled = state;
                }
                else
                {
                    cachedColliders.Add (instanceID, null);
                }
            }
        }

        private void SetCurrentObjectName ()
        {
            if (selectedObject > objectDisplayList.Count - 1)
                currentObjectName = "Unnamed";
            else
                currentObjectName = objectDisplayList[selectedObject];
        }

        private void ParseObjectNames ()
        {
            string[] objectBatchNames = objects.Split (';');
            if (objectBatchNames.Length < 1)
                Debug.Log ("PSSM | ParseObjectNames | Found no object names in the object list");
            else
            {
                objectTransforms.Clear ();
                for (int batchCount = 0; batchCount < objectBatchNames.Length; batchCount++)
                {
                    List <Transform> newObjects = new List<Transform> ();
                    string[] objectNames = objectBatchNames[batchCount].Split (',');
                    for (int objectCount = 0; objectCount < objectNames.Length; objectCount++)
                    {
                        Transform newTransform = part.FindModelTransform (objectNames[objectCount].Trim (' '));
                        if (newTransform != null)
                        {
                            newObjects.Add (newTransform);
                            Debug.Log ("PSSM | ParseObjectNames | Added object to list: " + objectNames[objectCount]);
                        }
                        else
                        {
                            Debug.Log ("PSSM | ParseObjectNames | Could not find object " + objectNames[objectCount]);
                        }
                    }
                    if (newObjects.Count > 0) objectTransforms.Add (newObjects);
                }
            }
        }

        #endregion




        #region FuelSwitching

        public void SelectTankSetup (int i, bool calledByPlayer)
        {
            InitializeData ();
            if (selectedTankSetup != i)
            {
                selectedTankSetup = i;
                AssignResourcesToPart (calledByPlayer);
            }
        }

        private void AssignResourcesToPart (bool calledByPlayer)
        {
            SetupTankInPart (part, calledByPlayer);
            if (HighLogic.LoadedSceneIsEditor)
            {
                for (int s = 0; s < part.symmetryCounterparts.Count; s++)
                {
                    SetupTankInPart (part.symmetryCounterparts[s], calledByPlayer);
                    PartSubtypeSwitcherModule module = part.symmetryCounterparts[s].GetComponent<PartSubtypeSwitcherModule> ();
                    if (module != null)
                        module.selectedTankSetup = selectedTankSetup;
                }
            }

            if (tweakableUI == null)
                tweakableUI = PartSubtypeTools.FindActionWindow (part);

            if (tweakableUI != null)
                tweakableUI.displayDirty = true;
        }

        private void SetupTankInPart (Part currentPart, bool calledByPlayer)
        {
            /*
            currentPart.Resources.list.Clear ();
            PartResource[] partResources = currentPart.GetComponents<PartResource> ();
            for (int i = 0; i < partResources.Length; i++)
            {
                DestroyImmediate (partResources[i]);
            }

            for (int tankCount = 0; tankCount < tankList.Count; tankCount++)
            {
                if (selectedTankSetup == tankCount)
                {
                    for (int resourceCount = 0; resourceCount < tankList[tankCount].resources.Count; resourceCount++)
                    {
                        if (tankList[tankCount].resources[resourceCount].name != "Structural")
                        {
                            ConfigNode newResourceNode = new ConfigNode ("RESOURCE");
                            newResourceNode.AddValue ("name", tankList[tankCount].resources[resourceCount].name);
                            newResourceNode.AddValue ("maxAmount", tankList[tankCount].resources[resourceCount].maxAmount);

                            if (calledByPlayer && !HighLogic.LoadedSceneIsEditor)
                                newResourceNode.AddValue ("amount", 0.0f);
                            else
                                newResourceNode.AddValue ("amount", tankList[tankCount].resources[resourceCount].amount);
                            currentPart.AddResource (newResourceNode);
                        }
                    }
                }
            }
            currentPart.Resources.UpdateList ();
            UpdateWeight (currentPart, selectedTankSetup);
            UpdateCost ();
            */
        }

        private float UpdateCost ()
        {
            if (selectedTankSetup >= 0 && selectedTankSetup < tankCostList.Count)
                addedCost = (float) tankCostList[selectedTankSetup];
            else
                addedCost = 0f;
            return addedCost;
        }

        private void UpdateWeight (Part currentPart, int newTankSetup)
        {
            if (newTankSetup < weightList.Count)
                currentPart.mass = (float) (massBase + weightList[newTankSetup]);
        }

        public void Update ()
        {
            if (HighLogic.LoadedSceneIsEditor)
                dryMassInfo = part.mass;
        }

        private void SetupTankList (bool calledByPlayer)
        {
            /*
            tankList = new List<Tank> ();
            weightList = new List<double> ();
            tankCostList = new List<double> ();

            // First find the amounts each tank type is filled with

            List<List<double>> resourceList = new List<List<double>> ();
            List<List<double>> initialResourceList = new List<List<double>> ();

            string[] resourceTankArray = resourceAmounts.Split (';');
            string[] initialResourceTankArray = initialResourceAmounts.Split (';');

            if (initialResourceAmounts.Equals ("") || initialResourceTankArray.Length != resourceTankArray.Length)
                initialResourceTankArray = resourceTankArray;

            for (int tankCount = 0; tankCount < resourceTankArray.Length; tankCount++)
            {
                resourceList.Add (new List<double> ());
                initialResourceList.Add (new List<double> ());

                string[] resourceAmountArray = resourceTankArray[tankCount].Trim ().Split (',');
                string[] initialResourceAmountArray = initialResourceTankArray[tankCount].Trim ().Split (',');

                if (initialResourceAmounts.Equals ("") || initialResourceAmountArray.Length != resourceAmountArray.Length)
                    initialResourceAmountArray = resourceAmountArray;

                for (int amountCount = 0; amountCount < resourceAmountArray.Length; amountCount++)
                {
                    try
                    {
                        resourceList[tankCount].Add (double.Parse (resourceAmountArray[amountCount].Trim ()));
                        initialResourceList[tankCount].Add (double.Parse (initialResourceAmountArray[amountCount].Trim ()));
                    }
                    catch
                    {
                        Debug.Log ("PSSM | SetupTankList | Error parsing resource amount " + tankCount + "/" + amountCount + ": '" + resourceTankArray[amountCount] + "': '" + resourceAmountArray[amountCount].Trim () + "'");
                    }
                }
            }

            // Then find the kinds of resources each tank holds, and fill them with the amounts found previously, or the amount hey held last 
            // Values are persisted in the craft

            string[] tankArray = resourceNames.Split (';');
            for (int tankCount = 0; tankCount < tankArray.Length; tankCount++)
            {
                Tank newTank = new Tank ();
                tankList.Add (newTank);
                string[] resourceNameArray = tankArray[tankCount].Split (',');
                for (int nameCount = 0; nameCount < resourceNameArray.Length; nameCount++)
                {
                    ResourceContainer newResource = new ResourceContainer (resourceNameArray[nameCount].Trim (' '));
                    if (resourceList[tankCount] != null)
                    {
                        if (nameCount < resourceList[tankCount].Count)
                        {
                            newResource.maxAmount = resourceList[tankCount][nameCount];
                            newResource.amount = initialResourceList[tankCount][nameCount];
                        }
                    }
                    newTank.resources.Add (newResource);
                }
            }
            */
        }

        public float GetModuleCost ()
        {
            return UpdateCost ();
        }
        public float GetModuleCost (float modifier)
        {
            return UpdateCost ();
        }

        #endregion




        #region Nodes

        [KSPField]
        public string nodeMapping = string.Empty;
        private List<string[]> nodeMappingList = new List<string[]> ();

        private void NodeOnStart ()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                string[] nodeConfigurations = nodeMapping.Split (';');
                for (int a = 0; a < nodeConfigurations.Length; a++)
                {
                    Debug.Log ("PSSM | NodeOnStart | Node mapping " + a + ": " + nodeConfigurations[a]);
                    string[] nodesInConfiguration = nodeConfigurations[a].Split (',');
                    nodeMappingList.Add (nodesInConfiguration);
                }
                NodeRemapForConfiguration (selectedObject);
            }
        }

        private void NodeRemapForConfiguration (int configuration)
        {
            if (configuration > nodeMappingList.Count - 1 || configuration < 0)
            {
                Debug.Log ("PSSM | NodeRemapForConfiguration | Requested index does not exist in the parsed list of node mapping configurations");
                return;
            }

            string report = string.Empty;
            for (int i = 0; i < nodeMappingList[configuration].Length; ++i) report += nodeMappingList[configuration][i] + "; ";
            Debug.Log ("PSSM | NodeRemapForConfiguration | Nodes: " + part.attachNodes.Count + " | Requested node set (" + configuration + "):" + report);

            for (int i = 0, count = part.attachNodes.Count; i < count; ++i)
            {
                if (nodeMappingList[configuration].Contains<string> (part.attachNodes[i].id) && part.attachNodes[i].nodeType == AttachNode.NodeType.Stack)
                {
                    Debug.Log ("PSSM | NodeRemapForConfiguration | Disabling node " + part.attachNodes[i].id);
                    part.attachNodes[i].nodeType = AttachNode.NodeType.Dock;
                    part.attachNodes[i].radius = 0.001f;
                }
                else if (part.attachNodes[i].nodeType == AttachNode.NodeType.Dock)
                {
                    Debug.Log ("PSSM | NodeRemapForConfiguration | Enabling node " + part.attachNodes[i].id);
                    part.attachNodes[i].nodeType = AttachNode.NodeType.Stack;
                    part.attachNodes[i].radius = 0.4f;
                }
                else
                {
                    Debug.Log ("PSSM | NodeRemapForConfiguration | Node " + part.attachNodes[i].id + " is not affected by this switch");
                }
            }
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

        public static List<string> ParseNames (string names)
        {
            return ParseNames (names, false, true, string.Empty);
        }

        public static List<string> ParseNames (string names, bool replaceBackslashErrors)
        {
            return ParseNames (names, replaceBackslashErrors, true, string.Empty);
        }

        public static List<string> ParseNames (string names, bool replaceBackslashErrors, bool trimWhiteSpace, string prefix)
        {
            List<string> source = names.Split (';').ToList<string> ();
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i] == string.Empty)
                    source.RemoveAt (i);
            }
            if (trimWhiteSpace)
            {
                for (int i = 0; i < source.Count; i++)
                    source[i] = source[i].Trim (' ');
            }
            if (prefix != string.Empty)
            {
                for (int i = 0; i < source.Count; i++)
                    source[i] = prefix + source[i];
            }
            if (replaceBackslashErrors)
            {
                for (int i = 0; i < source.Count; i++)
                    source[i] = source[i].Replace ('\\', '/');
            }
            return source.ToList<string> ();
        }

        public static List<int> ParseIntegers (string stringOfInts)
        {
            List<int> newIntList = new List<int> ();
            string[] valueArray = stringOfInts.Split (';');
            for (int i = 0; i < valueArray.Length; i++)
            {
                int newValue = 0;
                if (int.TryParse (valueArray[i], out newValue))
                    newIntList.Add (newValue);
                else
                    Debug.Log ("PSSM | ParseIntegers | Invalid integer: " + valueArray[i]);
            }
            return newIntList;
        }

        public static List<double> ParseDoubles (string stringOfDoubles)
        {
            System.Collections.Generic.List<double> list = new System.Collections.Generic.List<double> ();
            string[] array = stringOfDoubles.Trim ().Split (';');
            for (int i = 0; i < array.Length; i++)
            {
                double item = 0f;
                if (double.TryParse (array[i].Trim (), out item))
                {
                    Debug.Log ("PSSM | ParseDoubles | Valid double: " + item);
                    list.Add (item);
                }
                else
                {
                    Debug.Log ("PSSM | ParseDoubles | Invalid double: [len:" + array[i].Length + "] '" + array[i] + "']");
                }
            }
            return list;
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
