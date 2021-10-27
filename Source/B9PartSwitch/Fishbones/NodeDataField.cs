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
using B9PartSwitch.Fishbones.FieldWrappers;
using B9PartSwitch.Fishbones.NodeDataMappers;
using B9PartSwitch.Fishbones.Context;

namespace B9PartSwitch.Fishbones
{
    public interface INodeDataField
    {
        string Name { get; }

        void Load(ConfigNode node, OperationContext context);
        void Save(ConfigNode node, OperationContext context);
    }

    public class NodeDataField : INodeDataField
    {
        public readonly IFieldWrapper field;
        public readonly IOperaitonManager operationManager;

        public string Name => field.Name;

        public NodeDataField(IFieldWrapper field, IOperaitonManager operationManager)
        {
            field.ThrowIfNullArgument(nameof(field));
            operationManager.ThrowIfNullArgument(nameof(operationManager));

            this.field = field;
            this.operationManager = operationManager;
        }

        public void Load(ConfigNode node, OperationContext context)
        {
            node.ThrowIfNullArgument(nameof(node));
            context.ThrowIfNullArgument(nameof(context));

            INodeDataMapper mapper = operationManager.MapperFor(context.Operation);
            if (mapper.IsNull()) return;

            object value = field.GetValue(context.Subject);

            if (mapper.Load(ref value, node, context))
                field.SetValue(context.Subject, value);
        }

        public void Save(ConfigNode node, OperationContext context)
        {
            node.ThrowIfNullArgument(nameof(node));
            context.ThrowIfNullArgument(nameof(context));

            INodeDataMapper mapper = operationManager.MapperFor(context.Operation);
            if (mapper.IsNull()) return;

            object value = field.GetValue(context.Subject);
            mapper.Save(value, node, context);
        }
    }
}
