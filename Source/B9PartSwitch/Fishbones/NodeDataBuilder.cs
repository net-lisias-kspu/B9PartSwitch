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
using System.Linq;
using B9PartSwitch.Fishbones.Parsers;
using B9PartSwitch.Fishbones.NodeDataMappers;
using B9PartSwitch.Fishbones.FieldWrappers;

namespace B9PartSwitch.Fishbones
{
    public interface INodeDataBuilder
    {
        INodeDataField CreateNodeDataField();
    }

    public class NodeDataBuilder : INodeDataBuilder
    {
        public readonly NodeData nodeData;
        public readonly IValueParseMap valueParseMap;
        public readonly IFieldWrapper fieldWrapper;

        public NodeDataBuilder(NodeData nodeData, IFieldWrapper fieldWrapper, IValueParseMap defaultValueParseMap)
        {
            nodeData.ThrowIfNullArgument(nameof(nodeData));
            fieldWrapper.ThrowIfNullArgument(nameof(fieldWrapper));
            defaultValueParseMap.ThrowIfNullArgument(nameof(defaultValueParseMap));

            this.nodeData = nodeData;
            this.fieldWrapper = fieldWrapper;

            object[] attributes = fieldWrapper.MemberInfo.GetCustomAttributes(true);

            if (attributes.OfType<IUseParser>().Any())
            {
                IValueParser[] overrides = attributes.OfType<IUseParser>().Select(x => x.CreateParser()).ToArray();
                valueParseMap = new OverrideValueParseMap(defaultValueParseMap, overrides);
            }
            else
            {
                valueParseMap = defaultValueParseMap;
            }
        }

        public INodeDataField CreateNodeDataField()
        {
            return new NodeDataField(fieldWrapper, CreateOperationManager());
        }

        public virtual IOperaitonManager CreateOperationManager()
        {
            return new OperationManager(CreateParseMapper(), CreateLoadSaveMapper(), CreateSerializeMapper());
        }

        public virtual INodeDataMapper CreateParseMapper()
        {
            return CreateMapperWithParsePriority();
        }

        public virtual INodeDataMapper CreateLoadSaveMapper()
        {
            if (!nodeData.persistent) return null;
            return CreateMapperWithParsePriority();
        }

        public virtual INodeDataMapper CreateSerializeMapper()
        {
            if (!nodeData.alwaysSerialize && fieldWrapper.MemberInfo.ReflectedType.Implements<UnityEngine.Object>()) return null;
            return CreateMapperWithSerializePriority();
        }

        public virtual INodeDataMapper CreateMapperWithParsePriority()
        {
            return BuildFromPrioritizedList(CreateValueScalarMapperBuilder(), CreateNodeScalarMapperBuilder(), CreateValueListMapperBuilder(), CreateNodeListMapperBuilder());
        }

        public virtual INodeDataMapper CreateMapperWithSerializePriority()
        {
            return BuildFromPrioritizedList(CreateNodeListMapperBuilder(), CreateNodeScalarMapperBuilder(), CreateValueListMapperBuilder(), CreateValueScalarMapperBuilder());
        }

        public virtual INodeDataMapper BuildFromPrioritizedList(params INodeDataMapperBuilder[] list)
        {
            INodeDataMapperBuilder builder = list.FirstOrDefault(x => x.CanBuild);

            if (builder.IsNotNull())
                return builder.BuildMapper();
            else
                throw new NotImplementedException($"Cannot find a suitable way to load node data into field {fieldWrapper.MemberInfo.Name}");
        }

        #region Mapper Builders

        public virtual INodeDataMapperBuilder CreateValueScalarMapperBuilder() => new ValueScalarMapperBuilder(NodeDataName, fieldWrapper.FieldType, valueParseMap);
        public virtual INodeDataMapperBuilder CreateValueListMapperBuilder() => new ValueListMapperBuilder(NodeDataName, fieldWrapper.FieldType, valueParseMap);
        public virtual INodeDataMapperBuilder CreateNodeScalarMapperBuilder() => new NodeScalarMapperBuilder(NodeDataName, fieldWrapper.FieldType);
        public virtual INodeDataMapperBuilder CreateNodeListMapperBuilder() => new NodeListMapperBuilder(NodeDataName, fieldWrapper.FieldType);

        #endregion

        #region Utility Methods

        public virtual string NodeDataName => nodeData.name.IsNullOrEmpty() ? fieldWrapper.MemberInfo.Name : nodeData.name;

        #endregion
    }
}
