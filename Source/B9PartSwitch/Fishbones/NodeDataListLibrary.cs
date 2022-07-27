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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace B9PartSwitch.Fishbones
{
    public static class NodeDataListLibrary
    {
        private static readonly Dictionary<Type, NodeDataList> dict = new Dictionary<Type, NodeDataList>();

        public static NodeDataList Get<T>() => Get(typeof(T));

        public static NodeDataList Get(Type type)
        {
            type.ThrowIfNullArgument(nameof(type));

            if (dict.TryGetValue(type, out NodeDataList list))
                return list;

            try
            {
                Log.info("Generating field configuration for type {0}", type);
                NodeDataListBuilder builder = new NodeDataListBuilder(type);
                list = builder.CreateList();
            }
            catch (Exception e)
            {
                Exception e2 = new Exception($"Fatal exception while generating field configuration for type {type}", e);
                FatalErrorHandler.HandleFatalError(e2);
                throw e2;
            }

            dict[type] = list;
            return list;
        }
    }
}
