using Furesoft.Core.CodeDom.Compiler.Core.Names;
using System.Collections;
using System.Reflection;

namespace LSP_Server.Core
{
    internal class NamespaceMap : IEnumerable<UnqualifiedName>
    {
        private Dictionary<UnqualifiedName, NamespaceMap> namespaces = new();

        public static NamespaceMap From(IEnumerable<string> namespaceNames)
        {
            NamespaceMap map = new NamespaceMap();

            foreach (var ns in namespaceNames)
            {
                map.Add(Utils.QualifyNamespace(ns));
            }

            return map;
        }

        public static NamespaceMap From(params Assembly[] assemblies)
        {
            var namespaces = assemblies.SelectMany(_ => _.GetTypes())
                                       .Where(_ => _.IsPublic && !string.IsNullOrEmpty(_.Namespace))
                                       .Select(_ => _.Namespace);

            return From(namespaces);
        }

        public void Add(QualifiedName name)
        {
            if (!name.IsEmpty && namespaces.ContainsKey(name.Qualifier))
            {
                var innerName = name.Slice(1);

                if (!innerName.IsEmpty)
                {
                    namespaces[name.Qualifier].Add(innerName);
                }
            }
            else
            {
                if (!name.IsEmpty)
                {
                    namespaces.Add(name.Qualifier, new());
                }

                var map = namespaces;
                var innerName = name.Name;
                foreach (var ns in name.Path.Skip(1))
                {
                    map[name.Qualifier].namespaces.Add(innerName.Qualifier, new());

                    map = map[name.Qualifier].namespaces;
                    name = name.Name;
                    innerName = name.Name;
                }
            }
        }

        public IEnumerator<UnqualifiedName> GetEnumerator()
        {
            return namespaces.Keys.GetEnumerator();
        }

        public IEnumerable<string> Resolve(QualifiedName requestedName)
        {
            QualifiedName tmp = requestedName;
            var ns = namespaces;

            // little hack to make the logic works
            tmp = tmp.Qualify(tmp.Path[0]);

            while (true)
            {
                if (tmp.Name.ToString() == "#error")
                {
                    return ns.Select(_ => _.Key.ToString());
                }
                if (tmp.IsEmpty) break;

                tmp = tmp.Name;

                if (tmp.IsEmpty) break;
                ns = ns[tmp.Qualifier].namespaces;
            }

            return ns.Select(_ => _.Key.ToString());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return namespaces.Keys.GetEnumerator();
        }
    }
}