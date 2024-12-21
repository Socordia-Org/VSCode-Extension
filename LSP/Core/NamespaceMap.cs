using System.Collections;
using System.Reflection;
using Backlang.Codeanalysis.Parsing.AST;
using Backlang.Driver;
using Furesoft.Core.CodeDom.Compiler.Core.Names;

namespace Socordia.LSP.Core;

internal class NamespaceMap : IEnumerable<UnqualifiedName>
{
    private readonly Dictionary<UnqualifiedName, NamespaceMap> namespaces = new();

    public IEnumerator<UnqualifiedName> GetEnumerator()
    {
        return namespaces.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return namespaces.Keys.GetEnumerator();
    }

    public static NamespaceMap From(IEnumerable<string> namespaceNames)
    {
        var map = new NamespaceMap();

        foreach (var ns in namespaceNames) map.Add(Utils.QualifyNamespace(ns));

        return map;
    }

    //ToDo: Implement full namespacemap concat
    public static NamespaceMap Concat(params NamespaceMap[] maps)
    {
        var map = new NamespaceMap();

        foreach (var m in maps)
        foreach (var ns in m.namespaces)
            map.namespaces.Add(ns.Key, ns.Value);

        return map;
    }

    public static NamespaceMap From(params Assembly[] assemblies)
    {
        var namespaces = assemblies.SelectMany(_ => _.GetTypes())
            .Where(_ => _.IsPublic && !string.IsNullOrEmpty(_.Namespace))
            .Select(_ => _.Namespace);

        return From(namespaces);
    }

    public static NamespaceMap From(params CompilationUnit[] trees)
    {
        var moduleNames = trees
            .Select(_ =>
                ConversionUtils.GetModuleName(_).ToString()
            ).Where(_ => !string.IsNullOrEmpty(_));

        return From(moduleNames);
    }

    public void Add(QualifiedName name)
    {
        if (!name.IsEmpty && namespaces.ContainsKey(name.Qualifier))
        {
            var innerName = name.Slice(1);

            if (!innerName.IsEmpty) namespaces[name.Qualifier].Add(innerName);
        }
        else
        {
            if (!name.IsEmpty) namespaces.Add(name.Qualifier, new NamespaceMap());

            var map = namespaces;
            var innerName = name.Name;
            foreach (var ns in name.Path.Skip(1))
            {
                map[name.Qualifier].namespaces.Add(innerName.Qualifier, new NamespaceMap());

                map = map[name.Qualifier].namespaces;
                name = name.Name;
                innerName = name.Name;
            }
        }
    }

    public IEnumerable<string> Resolve(QualifiedName requestedName)
    {
        var tmp = requestedName;
        var ns = namespaces;

        // little hack to make the logic works
        tmp = tmp.Qualify(tmp.Path[0]);

        while (true)
        {
            if (tmp.Name.ToString() == "#error") return ns.Select(_ => _.Key.ToString());
            if (tmp.IsEmpty) break;

            tmp = tmp.Name;

            if (tmp.IsEmpty) break;
            ns = ns[tmp.Qualifier].namespaces;
        }

        return ns.Select(_ => _.Key.ToString());
    }
}