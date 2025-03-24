using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace C4G.Editor
{
    public class CodeWriter : IDisposable
    {
        private readonly string _outputPath;
        private readonly StringBuilder _builder;
        private readonly HashSet<string> _usings;
        private readonly string _indentString;

        private int _indentLevel;

        public CodeWriter(string outputPath, string indentString = "    ")
        {
            _outputPath = outputPath;
            _indentString = indentString;
            _builder = new StringBuilder();
            _usings = new HashSet<string>();
        }

        public CodeWriter(string indentString = "    ") 
            : this(string.Empty, indentString) { }

        public void AddUsing(string directive)
        {
            if (!string.IsNullOrWhiteSpace(directive))
                _usings.Add(directive);
        }

        public void WriteLine(string line = "")
        {
            _builder.Append(new string(' ', _indentLevel * _indentString.Length));
            _builder.AppendLine(line);
        }

        public void WriteEmptyLine()
        {
            _builder.AppendLine();
        }

        public void WriteComment(string comment)
        {
            WriteLine($"// {comment}");
        }

        public void WriteBlock(string header, Action blockContent)
        {
            WriteLine(header);
            WriteLine("{");
            _indentLevel++;
            blockContent?.Invoke();
            _indentLevel--;
            WriteLine("}");
        }

        public void WriteNamespace(string namespaceName, Action content)
        {
            WriteBlock($"namespace {namespaceName}", content);
        }

        public void WriteClass(string className, Action content)
        {
            WriteClass(className, isPartial: false, baseClass: null, content: content);
        }

        public void WritePartialClass(string className, Action content)
        {
            WriteClass(className, isPartial: true, baseClass: null, content: content);
        }

        public void WriteClass(string className, bool isPartial, string baseClass, Action content)
        {
            string partialText = isPartial ? "partial " : "";
            string baseText = string.IsNullOrWhiteSpace(baseClass) ? "" : $" : {baseClass}";
            WriteBlock($"public {partialText}class {className}{baseText}", content);
        }

        public void WriteInterface(string interfaceName, Action content)
        {
            WriteBlock($"public interface {interfaceName}", content);
        }

        public void WriteEnum(string enumName, IEnumerable<string> values)
        {
            WriteLine($"public enum {enumName}");
            WriteLine("{");
            _indentLevel++;

            var first = true;
            foreach (var value in values)
            {
                if (!first)
                    WriteLine(",");

                WriteLine(value);
                first = false;
            }

            _indentLevel--;
            WriteLine("}");
        }

        public void WriteMethod(string signature, Action content)
        {
            WriteBlock(signature, content);
        }

        public void WriteConstructor(string className, string args = "", Action content = null)
        {
            WriteBlock($"public {className}({args})", content);
        }

        public void WriteIf(string condition, Action content)
        {
            WriteBlock($"if ({condition})", content);
        }

        public void WriteSwitch(string expression, Action content)
        {
            WriteBlock($"switch ({expression})", content);
        }

        public void WriteFor(string iterator, string start, string condition, string iteratorExpression, Action content)
        {
            WriteBlock($"for (int {iterator} = {start}; {condition}; {iteratorExpression})", content);
        }

        public void WriteForeach(string iterator, string collection, Action content)
        {
            WriteBlock($"foreach (var {iterator} in {collection})", content);
        }

        public void WriteWhile(string condition, Action content)
        {
            WriteBlock($"while ({condition})", content);
        }

        public void WriteDoWhile(string condition, Action content)
        {
            WriteLine("do");
            WriteLine("{");
            _indentLevel++;
            content?.Invoke();
            _indentLevel--;
            WriteLine("}");
            WriteLine($"while ({condition});");
        }

        public void WritePublicProperty(string name, string type, bool hasSetter = true, string result = null)
        {
            var resultPart = !string.IsNullOrEmpty(result) ? " = " + result + ";" : "";
            var setter = hasSetter ? "set;" : "private set;";
            WriteLine($"public {type} {name} {{ get; {setter} }}{resultPart}");
        }

        public void WritePrivateField(string name, string type, bool isReadonly = false, string result = null)
        {
            var resultPart = !string.IsNullOrEmpty(result) ? " = " + result : "";
            var readonlyPart = isReadonly ? "readonly " : "";
            WriteLine($"private {readonlyPart}{type} {name}{readonlyPart};");
        }

        public void WritePublicField(string name, string type, bool isReadonly = false, string result = null)
        {
            var resultPart = !string.IsNullOrEmpty(result) ? " = " + result : "";
            var readonlyPart = isReadonly ? "readonly " : "";
            WriteLine($"public {readonlyPart}{type} {name}{resultPart};");
        }

        public void WritePublicFieldConstant(string name, string type, string result = null)
        {
            var resultPart = !string.IsNullOrEmpty(result) ? " = " + result : "";
            WriteLine($"public const {type} {name}{resultPart};");
        }

        public void WriteTryCatch(Action tryContent, string exceptionType, string exceptionVar, string catchMessage)
        {
            WriteLine("try");
            WriteLine("{");
            _indentLevel++;
            tryContent?.Invoke();
            _indentLevel--;
            WriteLine("}");
            WriteLine($"catch ({exceptionType} {exceptionVar})");
            WriteLine("{");
            _indentLevel++;

            if (!string.IsNullOrWhiteSpace(catchMessage))
                WriteLine($"throw new {exceptionType}(\"{catchMessage}\", {exceptionVar});");

            _indentLevel--;
            WriteLine("}");
            WriteEmptyLine();
        }

        public void WriteTryFinally(Action tryContent, Action finallyContent)
        {
            WriteLine("try");
            WriteLine("{");
            _indentLevel++;
            tryContent?.Invoke();
            _indentLevel--;
            WriteLine("}");
            WriteLine("finally");
            WriteLine("{");
            _indentLevel++;
            finallyContent?.Invoke();
            _indentLevel--;
            WriteLine("}");
            WriteEmptyLine();
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.AppendLine("// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            result.AppendLine("//     THIS CODE IS GENERATED   ");
            result.AppendLine("//          DO NOT EDIT         ");
            result.AppendLine("// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            result.AppendLine();

            foreach (var directive in _usings)
                result.AppendLine($"using {directive};");

            result.AppendLine();
            result.Append(_builder);
            return result.ToString();
        }

        public void WriteAllToFile(string outputPath)
        {
            var directory = Path.GetDirectoryName(outputPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(outputPath, ToString());
        }

        public void Dispose()
        {
            if (string.IsNullOrEmpty(_outputPath))
                throw new InvalidOperationException("The file cannot be written because the outputPath is missing");

            WriteAllToFile(_outputPath);
        }
    }
}