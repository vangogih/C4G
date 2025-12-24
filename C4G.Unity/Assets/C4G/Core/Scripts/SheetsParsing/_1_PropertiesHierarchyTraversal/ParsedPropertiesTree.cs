using System.Collections.Generic;

namespace C4G.Core.SheetsParsing._1_PropertiesHierarchyTraversal
{
    public readonly struct ParsedPropertiesTree
    {
        public enum ENodeType : byte
        {
            Property = 0,
            Class = 1
        }

        public readonly struct Node
        {
            public readonly string Name;
            public readonly ENodeType NodeType;
            public readonly string ValueType;
            public readonly IReadOnlyList<Node> Children;

            public Node(string name, ENodeType nodeType, string valueType, IReadOnlyList<Node> children)
            {
                Name = name;
                NodeType = nodeType;
                ValueType = valueType;
                Children = children;
            }
        }

        public readonly Node Root;

        public ParsedPropertiesTree(Node root)
        {
            Root = root;
        }
    }
}