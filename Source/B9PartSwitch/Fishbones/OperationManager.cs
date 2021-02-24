﻿using B9PartSwitch.Fishbones.Context;
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
