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
using System.Collections.Generic;
using UnityEngine;

namespace B9PartSwitch
{
    public static class TransformExtensions
    {
        public static void Enable(this Transform trans) => trans.gameObject.SetActive(true);
        public static void Disable(this Transform trans) => trans.gameObject.SetActive(false);

        public static IEnumerable<Transform> TraverseHierarchy(this Transform transform)
        {
            transform.ThrowIfNullArgument(nameof(transform));

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                yield return child;

                foreach (Transform childChild in child.TraverseHierarchy())
                {
                    yield return childChild;
                }
            }
        }
    }
}
