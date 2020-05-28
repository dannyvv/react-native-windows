using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.ReactNative.Managed.CodeGen.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ReactNative.Managed.CodeGen
{
  public class App
  {
    private CancellationToken m_cancellationToken;

    public App(CancellationToken cancellationToken)
    {
      m_cancellationToken = cancellationToken;
    }

    public async Task<int> Run(Options options)
    {
      var codeAnalyzer = new CodeAnalyzer(m_cancellationToken);

      using (new ConsoleMeasurement("Parsing source files"))
      {
        await codeAnalyzer.ParseSourceFilesAsync(
          GetDistinctExistingFiles(options.SourceFiles),
          options.Defines.Select(def => def.Trim()));
      }

      using (new ConsoleMeasurement("Loading references"))
      {
        await codeAnalyzer.LoadMetadataReferencesAsync(
          GetDistinctExistingFiles(options.References));
      }

      using (new ConsoleMeasurement("Compiling"))
      {
        if (!codeAnalyzer.TryCompileAndCheckForErrors())
        {
          return 1;
        }
      }

      ReactAssembly assembly;
      using (new ConsoleMeasurement("Finding types"))
      {
        assembly = codeAnalyzer.AnalyzeAndFindReactNativeInformation();
      }

      // $TODO: Errors

      //if (codeAnalyzer.Errors.Any())
      //{
      //  var oldColor = Console.ForegroundColor;
      //  Console.ForegroundColor = ConsoleColor.Red;
      //  foreach (var error in codeAnalyzer.Errors)
      //  {
      //    Console.WriteLine(error.ToString());
      //  }

      //  Console.ForegroundColor = oldColor;
      //  return 1;
      //}

      var codeGenerator = new CodeGenerator(codeAnalyzer.ReactTypes, options.Namespace);
      CSharpSyntaxNode node;
      using (new ConsoleMeasurement("Generating code"))
      {
        node = codeGenerator.Generate(assembly);
      }

      var code = node.NormalizeWhitespace(indentation: "  ", elasticTrivia: false).ToFullString();

      // $TODO: Check contents and not write if the same since msbuild is timestamp based, not hash based.
      File.WriteAllText(options.OutputFile, code);

      return 0;
    }

    private static IEnumerable<string> GetDistinctExistingFiles(IEnumerable<string> files)
    {
      if (files == null)
      {
        return Enumerable.Empty<string>();
      }

      return files
        .Select(file => Path.GetFullPath(file.Trim()))
        .Where(filePath => File.Exists(filePath))
        .Distinct(StringComparer.OrdinalIgnoreCase);
    }
  }
}
