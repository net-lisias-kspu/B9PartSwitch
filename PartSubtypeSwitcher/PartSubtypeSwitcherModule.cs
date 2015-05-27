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
        #region FieldsMeshSwitching

        [KSPField] public int moduleID = 0;
        [KSPField] public string buttonName = "Next part variant";
        [KSPField] public string previousButtonName = "Prev part variant";

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

        [KSPField] public float basePartMass = 0.25f;
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

        private List<ModularTank> tankList;
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
            InitializeData ();
            SwitchToObject (selectedObject, false);
            Events["EventSwitchToNext"].guiName = buttonName;
            Events["EventSwitchToPrevious"].guiName = previousButtonName;
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
                InitializeData ();
            configLoaded = true;
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
                currentPart.mass = (float) (basePartMass + weightList[newTankSetup]);
        }

        public void Update ()
        {
            if (HighLogic.LoadedSceneIsEditor)
                dryMassInfo = part.mass;
        }

        private void SetupTankList (bool calledByPlayer)
        {
            tankList = new List<ModularTank> ();
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
                ModularTank newTank = new ModularTank ();
                tankList.Add (newTank);
                string[] resourceNameArray = tankArray[tankCount].Split (',');
                for (int nameCount = 0; nameCount < resourceNameArray.Length; nameCount++)
                {
                    PartResourceContainer newResource = new PartResourceContainer (resourceNameArray[nameCount].Trim (' '));
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
        public List<AttachNode> partAttachNodesCached;
        private List<string[]> nodeMappingList = new List<string[]> ();
        Material nodeMaterial;
        public static Dictionary<string, List<AttachNode>> nodeReference = new Dictionary<string, List<AttachNode>> ();

        private static AttachNode NodeCreateFromBlueprint (Part part, AttachNode blueprint)
        {
            AttachNode node = new AttachNode ();
            node.attachedPart = null;
            node.attachedPartId = blueprint.attachedPartId;
            node.attachMethod = blueprint.attachMethod;
            node.breakingForce = blueprint.breakingForce;
            node.breakingTorque = blueprint.breakingTorque;
            node.contactArea = blueprint.contactArea;
            node.icon = blueprint.icon;
            node.id = blueprint.id;
            node.nodeTransform = blueprint.nodeTransform;
            node.nodeType = blueprint.nodeType;
            node.offset = blueprint.offset;
            node.orientation = blueprint.orientation;
            node.originalOrientation = blueprint.originalOrientation;
            node.originalPosition = blueprint.originalPosition;
            node.originalSecondaryAxis = blueprint.originalSecondaryAxis;
            node.owner = part;
            node.position = blueprint.position;
            node.radius = blueprint.radius;
            node.requestGate = blueprint.requestGate;
            node.ResourceXFeed = blueprint.ResourceXFeed;
            node.secondaryAxis = blueprint.secondaryAxis;
            node.size = blueprint.size;
            return node;
        }

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

                if (partAttachNodesCached == null || partAttachNodesCached.Count == 0)
                {
                    Debug.Log ("PSSM | NodeOnStart | Node reference is empty or null present, therefore the part is brand new, filling the reference list");
                    for (int i = 0; i < part.attachNodes.Count; ++i) partAttachNodesCached.Add (part.attachNodes[i]);
                }
                else
                {
                    Debug.Log ("PSSM | NodeOnStart | Node reference is present, therefore the part is a duplicate, clearing active nodes");
                    part.attachNodes = new List<AttachNode> ();
                    for (int i = 0; i < partAttachNodesCached.Count; ++i)
                    {
                        Debug.Log ("Creating node " + partAttachNodesCached[i].id + " for part type " + part.name);
                        AttachNode node = NodeCreateFromBlueprint (part, partAttachNodesCached[i]);
                        part.attachNodes.Add (node);
                        partAttachNodesCached[i] = node;
                    }
                }
                try
                {
                    EditorVesselOverlays vesselOverlays = (EditorVesselOverlays) GameObject.FindObjectOfType (typeof (EditorVesselOverlays));
                    nodeMaterial = vesselOverlays.CoMmarker.gameObject.renderer.material;
                }
                catch (Exception ex)
                {
                    Debug.Log ("PSSM | NodeOnStart | Exception while acquiring nodeMaterial: " + ex.ToString ());
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
            for (int i = 0; i < nodeMappingList[configuration].Length; ++i)
                report += nodeMappingList[configuration][i] + "; ";
            Debug.Log ("PSSM | NodeRemapForConfiguration | Nodes: " + partAttachNodesCached.Count + " | Requested node set (" + configuration + "):" + report);

            for (int i = 0; i < partAttachNodesCached.Count; ++i)
            {
                if (nodeMappingList[configuration].Contains<string> (partAttachNodesCached[i].id))
                {
                    Debug.Log ("PSSM | NodeRemapForConfiguration | List contains requested node " + partAttachNodesCached[i].id);
                    NodeSetState (partAttachNodesCached[i].GetHashCode (), true);
                }
                else
                {
                    Debug.Log ("PSSM | NodeRemapForConfiguration | List contains unused node " + partAttachNodesCached[i].id);
                    NodeSetState (partAttachNodesCached[i].GetHashCode (), false);
                }
            }
        }

        public void NodeSetState (int caller, bool state)
        {
            int hashcode = caller.GetHashCode ();
            AttachNode node = partAttachNodesCached.Find (a => a.GetHashCode () == caller.GetHashCode ());
            if (!state)
            {
                if (part.attachNodes.Contains (node))
                {
                    Debug.Log ("PSSM | NodeSetState | Node exists, removing " + node.id);
                    part.attachNodes.Remove (node);
                }
                else
                {
                    Debug.Log ("PSSM | NodeSetState | Node " + node.id + " is already absent from the active list, no action required");
                }
            }
            else
            {
                if (!part.attachNodes.Contains (node))
                {
                    Debug.Log ("PSSM | NodeSetState | Node absent, adding " + node.id);
                    part.attachNodes.Add (node);
                }
                else
                {
                    Debug.Log ("PSSM | NodeSetState | Node " + node.id + " is already present in the active list, no action required");
                }
            }
        }

        private void NodeCreateVisible (AttachNode node)
        {
            if (nodeMaterial != null)
            {
                if (node.icon == null)
                {
                    node.icon = GameObject.CreatePrimitive (PrimitiveType.Sphere);
                    node.icon.renderer.material = nodeMaterial;
                }
                node.icon.SetActive (true);
                node.icon.transform.localScale = ((Vector3.one * node.radius) * (node.size != 0 ? (float) node.size : (float) node.size + 0.5f));
                node.icon.renderer.material.color = XKCDColors.RadioactiveGreen;
                node.icon.transform.position = (this.part.transform.TransformPoint (node.position));
                node.icon.renderer.enabled = true;
            }
        }

        #endregion
    }




    public static class PartSubtypeTools
    {
        #region Parsing

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




    public class ModularTank
    {
        public List<PartResourceContainer> resources = new List<PartResourceContainer> ();
    }




    public class PartResourceContainer
    {
        public string name;
        public int ID;
        public float ratio;
        public double currentSupply = 0f;
        public double amount = 0f;
        public double maxAmount = 0f;

        public PartResourceContainer (string _name, float _ratio)
        {
            name = _name;
            ID = _name.GetHashCode ();
            ratio = _ratio;
        }

        public PartResourceContainer (string _name)
        {
            name = _name;
            ID = _name.GetHashCode ();
            ratio = 1f;
        }
    }
}
