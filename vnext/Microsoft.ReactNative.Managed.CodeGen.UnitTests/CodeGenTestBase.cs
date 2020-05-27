// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ReactNative.Managed.CodeGen.UnitTests
{
  public abstract class CodeGenTestBase : AnalysysTestBase
  {
    /// <summary>
    /// This property when set to true will update all LKG files automatically.
    /// </summary>
    protected const bool AutoFixLkgs = false;

    protected void TestCodeGen<TSymbol>(string csSnippet, Func<CodeGenerator, TSymbol, SyntaxNode> generateCode, string lkgName = null)
    {
      TestMultipleCodeGen<TSymbol>(csSnippet, (codeGen, symbols) => generateCode(codeGen, symbols.FirstOrDefault()), lkgName);
    }

    protected void TestMultipleCodeGen<TSymbol>(string csSnippet, Func<CodeGenerator, IEnumerable<TSymbol>, SyntaxNode> generateCode, string lkgName = null)
    {
      var (analyzer, type) = AnalyzeModuleFile(csSnippet);
      IEnumerable<TSymbol> symbols = type.GetMembers().OfType<TSymbol>();
      Assert.IsTrue(symbols.Any());

      var codeGen = new CodeGenerator(analyzer.ReactTypes, "Test.TestNS");
      var syntax = generateCode(codeGen, symbols);

      var codeToCompare = syntax.NormalizeWhitespace().ToFullString();
      if (AutoFixLkgs)
      {
        var lkgSourceFile = GetLkgSourceFile(lkgName);
        File.WriteAllText(lkgSourceFile, codeToCompare);
      }
      else
      {
        var lkgFile = GetLkgPath(lkgName);
        if (!File.Exists(lkgFile))
        {
          TraceEncountered(codeToCompare, "Encountered");
          Assert.Fail(
            $"Could not find expected LKG file: '{lkgFile}'. To generate the file, set CodeGenTestBase.AutoFixLkgs temporarily to true.");
        }

        var lkgContents = File.ReadAllText(lkgFile);
        if (!String.Equals(lkgContents, codeToCompare, StringComparison.Ordinal))
        {
          TraceEncountered(lkgContents, "Expected");
          TraceEncountered(codeToCompare, "Encountered");
          Assert.AreEqual(lkgContents, codeToCompare, "Lkg does not match. To generate the file, set CodeGenTestBase.AutoFixLkgs temporarily to true.");
        }
      }
    }

    private void TraceEncountered(string contents, string title)
    {
      TestContext.WriteLine("*********************");
      TestContext.WriteLine("* " + title);
      TestContext.WriteLine("*********************");
      TestContext.WriteLine(contents);

      TestContext.WriteLine("*********************");
    }

    private string GetLkgPath(string lkgName)
    {
      var suffix = string.IsNullOrEmpty(lkgName) ? lkgName : "--" + lkgName;
      return Path.Combine("Lkg", TestContext.FullyQualifiedTestClassName + "--" + TestContext.TestName + suffix + ".lkg");
    }

    private string GetLkgSourceFile(string lkgName, [CallerFilePath] string file = "")
    {
      var sourceRoot = Path.GetDirectoryName(file);
      return Path.Combine(sourceRoot, "CodeGen", GetLkgPath(lkgName));
    }

    protected IPropertySymbol ParseProp(string csSnippet) => ParseMemberSymbol<IPropertySymbol>(csSnippet);

    protected TSymbol ParseMemberSymbol<TSymbol>(string csSnippet)
      where TSymbol : ISymbol
    {
      var (analyzer, type) = AnalyzeModuleFile(csSnippet);
      return type.GetMembers().OfType<TSymbol>().First();
    }
  }
}
