// http://baba-s.hatenablog.com/entry/2018/12/27/160000

using SyntaxTree.VisualStudio.Unity.Bridge;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEditor;

[InitializeOnLoad]
public static class ProjectFileHook
{
    static ProjectFileHook()
    {
        ProjectFilesGenerator.ProjectFileGeneration += OnGeneration;
    }

    private static string OnGeneration(string name, string content)
    {
        var document = XDocument.Parse(content);
        var ns = document.Root.Name.Namespace;
        var list = document.Root
            .Descendants()
            .Where(x => x.Name.LocalName == "PropertyGroup")
            .Descendants()
            .Where(x => x.Name == ns + "NoWarn")
        ;

        foreach (var xe in list)
        {
            xe.Value = xe.Value + ";0649";
        }

        var stream = new Utf8StringWriter();
        document.Save(stream);

        return stream.ToString();
    }

    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
