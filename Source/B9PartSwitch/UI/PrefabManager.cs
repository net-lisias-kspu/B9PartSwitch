/*
	This file is part of B9PartSwitch /L Unleashed
		© 2021 Lisias T : http://lisias.net <support@lisias.net>
		© 216-2021 blowfish
		© 2015 bac9

	B9PartSwitch /L Unofficial is licensed as follows:

		* LGPL 3.0 : https://www.gnu.org/licenses/lgpl-3.0.txt

	B9PartSwitch /L Unleashed is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU Lesser General Public License 3.0
	along with B9PartSwitch /L Unofficial. If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace B9PartSwitch.UI
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class PrefabManagerInstant : MonoBehaviour
    {
        [SuppressMessage("Code Quality", "IDE0051", Justification = "Called by Unity")]
        private void Awake()
        {
            try
            {
                TooltipHelper.EnsurePrefabs();
            }
            catch (Exception ex)
            {
                FatalErrorHandler.HandleFatalError(ex);
                Log.error(ex, ex.Message);
            }

            Destroy(gameObject);
        }

    }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class PrefabManagerEditor : MonoBehaviour
    {
        [SuppressMessage("Code Quality", "IDE0051", Justification = "Called by Unity")]
        private void Start()
        {
            if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
            {
                try
                {
                    UIPartActionSubtypeSelector.EnsurePrefab();
                }
                catch (Exception ex)
                {
                    FatalErrorHandler.HandleFatalError(ex);
                    Log.error(ex, ex.Message);
                }
            }

            Destroy(gameObject);
        }
    }
}
