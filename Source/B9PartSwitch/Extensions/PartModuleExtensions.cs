/*
	This file is part of B9PartSwitch /L Unleashed
		© 2021-2022 LisiasT : http://lisias.net <support@lisias.net>
		© 2016-2021 blowfish
		© 2015 bac9

	B9PartSwitch /L Unofficial is licensed as follows:
		* LGPL 3.0 : https://www.gnu.org/licenses/lgpl-3.0.txt

	B9PartSwitch /L Unleashed is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU Lesser General Public License 3.0
	along with B9PartSwitch /L Unofficial. If not, see <https://www.gnu.org/licenses/>.

*/
namespace B9PartSwitch
{
    public static class PartModuleExtensions
    {
        // partInfo is assigned after modules are created and loaded
        public static bool ParsedPrefab(this PartModule module) => module.part.partInfo != null;

        public static void SetUiGroups(this PartModule module, string uiGroupName, string uiGroupDisplayName)
        {
            module.ThrowIfNullArgument(nameof(module));

            foreach (BaseField field in module.Fields)
            {
                if (!field.group.name.IsNullOrEmpty()) continue;

                field.group.name = uiGroupName;
                field.group.displayName = uiGroupDisplayName;
            }

            foreach (BaseEvent baseEvent in module.Events)
            {
                if (!baseEvent.group.name.IsNullOrEmpty()) continue;

                baseEvent.group.name = uiGroupName;
                baseEvent.group.displayName = uiGroupDisplayName;
            }
        }

        #region Logging

        public static void LogInfo(this PartModule module, object message) => module.part.LogInfo($"{module.LogTagString()} {message}");
        public static void LogWarning(this PartModule module, object message) => module.part.LogWarning($"{module.LogTagString()} {message}");
        public static void LogError(this PartModule module, object message) => module.part.LogError($"{module.LogTagString()} {message}");

        public static string LogTagString(this PartModule module)
        {
            string info = module.GetType().Name;

            if (module is CustomPartModule utilModule && !utilModule.moduleID.IsNullOrEmpty())
                info += $" '{utilModule.moduleID}'";

            return $"[{info}]";
        }

        #endregion
    }
}
