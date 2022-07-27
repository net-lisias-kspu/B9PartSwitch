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
using B9PartSwitch.Fishbones.Parsers;
using B9PartSwitch.Utils;

namespace B9PartSwitch
{
    public class ModuleMatcher
    {
        public sealed class CannotParseFieldException : Exception
        {
            private CannotParseFieldException(string message) : base(message) { }
            private CannotParseFieldException(string message, Exception innerException) : base(message, innerException) { }

            public static CannotParseFieldException CannotFindParser(string fieldName, Type fieldType)
            {
                fieldName.ThrowIfNullArgument(nameof(fieldName));
                fieldType.ThrowIfNullArgument(nameof(fieldType));

                string message = $"Could not find a suitable way to parse type {fieldType.Name} for field {fieldName}";
                return new CannotParseFieldException(message);
            }

            public static CannotParseFieldException ExceptionWhileParsing(string fieldName, Type fieldType, Exception innerException)
            {
                fieldName.ThrowIfNullArgument(nameof(fieldName));
                fieldType.ThrowIfNullArgument(nameof(fieldType));
                innerException.ThrowIfNullArgument(nameof(innerException));

                string message = $"Exception while parsing type {fieldType.Name} for field {fieldName}";
                return new CannotParseFieldException(message, innerException);
            }
        }

        private readonly ConfigNode identifierNode;
        private readonly IStringMatcher moduleName;

        public ModuleMatcher(ConfigNode identifierNode)
        {
            this.identifierNode = identifierNode ?? throw new ArgumentNullException(nameof(identifierNode));

            string moduleNameStr = identifierNode.GetValue("name");
            if (moduleNameStr == null) throw new ArgumentException("node has no name", nameof(identifierNode));
            if (moduleNameStr == "") throw new ArgumentException("node has empty name", nameof(identifierNode));
            moduleName = StringMatcher.Parse(moduleNameStr);
        }

        public PartModule FindModule(Part part)
        {
            PartModule matchedModule = null;

            foreach (PartModule module in part.Modules)
            {
                if (!IsMatch(module)) continue;

                if (matchedModule.IsNotNull()) throw new Exception("Found more than one matching module");

                matchedModule = module;
            }

            if (matchedModule.IsNull()) throw new Exception("Could not find matching module");

            return matchedModule;
        }

        public ConfigNode FindPrefabNode(PartModule module)
        {
            if (!(module.part.partInfo is AvailablePart partInfo)) throw new InvalidOperationException($"partInfo is null on part {module.part.name}");
            if (!(partInfo.partConfig is ConfigNode partConfig)) throw new InvalidOperationException($"partInfo.partConfig is null on part {partInfo.name}");

            ConfigNode matchedNode = null;

            foreach (ConfigNode subNode in partConfig.nodes)
            {
                if (subNode.name != "MODULE") continue;

                if (!NodeMatchesModule(module, subNode)) continue;

                if (matchedNode.IsNotNull()) throw new Exception("Found more than one matching module node");

                matchedNode = subNode;
            }

            if (matchedNode.IsNull()) throw new Exception("Could not find matching module node");

            return matchedNode;
        }

        private bool IsMatch(PartModule module)
        {
            if (!moduleName.Match(module.GetType().Name)) return false;

            foreach (ConfigNode.Value value in identifierNode.values)
            {
                if (value.name == "name") continue;

                if (module.Fields[value.name] is BaseField baseField)
                {
                    IValueParser parser;
                    object parsedValue;

                    try
                    {
                        parser = DefaultValueParseMap.Instance.GetParser(baseField.FieldInfo.FieldType);
                    }
                    catch (ParseTypeNotRegisteredException)
                    {
                        throw CannotParseFieldException.CannotFindParser(baseField.name, baseField.FieldInfo.FieldType);
                    }

                    try
                    {
                        parsedValue = parser.Parse(value.value);
                    }
                    catch (Exception ex)
                    {
                        throw CannotParseFieldException.ExceptionWhileParsing(baseField.name, baseField.FieldInfo.FieldType, ex);
                    }

                    if (!Equals(parsedValue, baseField.GetValue(module))) return false;
                }
                else if (module is CustomPartModule cpm && value.name == "moduleID")
                {
                    if (cpm.moduleID != value.value) return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private bool NodeMatchesModule(PartModule module, ConfigNode node)
        {
            string nameValue = node.GetValue("name");
            if (nameValue.IsNullOrEmpty()) throw new ArgumentException("Cannot match a module node without a name!");

            if (!moduleName.Match(nameValue)) return false;

            foreach (ConfigNode.Value value in identifierNode.values)
            {
                if (value.name == "name") continue;

                if (!(node.GetValue(value.name) is string testValue)) return false;

                if (module.Fields[value.name] is BaseField baseField)
                {
                    object parsedValue, nodeValue;

                    try
                    {
                        IValueParser parser = DefaultValueParseMap.Instance.GetParser(baseField.FieldInfo.FieldType);
                        parsedValue = parser.Parse(value.value);
                        nodeValue = parser.Parse(testValue);
                    }
                    catch (ParseTypeNotRegisteredException)
                    {
                        throw CannotParseFieldException.CannotFindParser(baseField.name, baseField.FieldInfo.FieldType);
                    }
                    catch (Exception ex)
                    {
                        throw CannotParseFieldException.ExceptionWhileParsing(baseField.name, baseField.FieldInfo.FieldType, ex);
                    }

                    if (!Equals(parsedValue, nodeValue)) return false;
                }
                else if (module is CustomPartModule && value.name == nameof(CustomPartModule.moduleID))
                {
                    if (node.GetValue(nameof(CustomPartModule.moduleID)) != value.value) return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
