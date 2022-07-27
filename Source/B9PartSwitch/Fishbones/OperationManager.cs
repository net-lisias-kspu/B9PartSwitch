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
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.Fishbones.NodeDataMappers;

namespace B9PartSwitch.Fishbones
{
    public interface IOperaitonManager
    {
        INodeDataMapper MapperFor(Operation operation);
    }

    public class OperationManager : IOperaitonManager
    {
        public readonly INodeDataMapper parseMapper;
        public readonly INodeDataMapper loadSaveMapper;
        public readonly INodeDataMapper serializeMapper;

        public OperationManager(INodeDataMapper parseMapper, INodeDataMapper loadSaveMapper, INodeDataMapper serializeMapper)
        {
            this.parseMapper = parseMapper;
            this.loadSaveMapper = loadSaveMapper;
            this.serializeMapper = serializeMapper;
        }

        public INodeDataMapper MapperFor(Operation op)
        {
            if (op == Operation.LoadPrefab)
                return parseMapper;
            else if (op == Operation.LoadInstance || op == Operation.Save)
                return loadSaveMapper;
            else if (op == Operation.Deserialize || op == Operation.Serialize)
                return serializeMapper;
            else
                return null;
        }
    }
}
