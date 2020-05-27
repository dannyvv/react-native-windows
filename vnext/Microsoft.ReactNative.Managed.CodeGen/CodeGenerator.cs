// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.ReactNative.Managed.CodeGen.Model;
using System.Collections.Generic;
using System.Diagnostics.ContractsLight;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.ReactNative.Managed.CodeGen.SyntaxHelpers;

namespace Microsoft.ReactNative.Managed.CodeGen
{
  public partial class CodeGenerator
  {
    private ReactTypes m_reactTypes;

    private string m_rootNamespace;

    public CodeGenerator(ReactTypes reactTypes, string rootNamespace)
    {
      Contract.Requires(!string.IsNullOrEmpty(rootNamespace));

      m_reactTypes = reactTypes;
      m_rootNamespace = rootNamespace;
    }

    public CSharpSyntaxNode Generate(ReactAssembly assembly)
    {
      var separateRegistrationCalls = new List<StatementSyntax>();
      var classMembers = new List<MemberDeclarationSyntax>();

      if (assembly.ViewManagers.Any())
      {
        separateRegistrationCalls.Add(InvocationStatement(ReactNativeNames.CreateViewManagers, IdentifierName(ReactNativeNames.PackageBuilderId)));
        classMembers.Add(CreateViewManagers(assembly.ViewManagers));
      }

      if (assembly.Modules.Any())
      {
        separateRegistrationCalls.Add(InvocationStatement(ReactNativeNames.CreateModules, IdentifierName((ReactNativeNames.PackageBuilderId))));
        classMembers.Add(CreateModules(assembly.Modules));
      }

      if (assembly.SerializableTypes.Any())
      {
        separateRegistrationCalls.Add(InvocationStatement(ReactNativeNames.CreateSerializers));
        classMembers.AddRange(CreateSerializers(assembly.SerializableTypes.Keys));
      }

      if (assembly.JSReaderFunctions.Any())
      {
        separateRegistrationCalls.Add(InvocationStatement(ReactNativeNames.RegisterExtensionReaders));
        classMembers.Add(RegisterExtensionReaders(assembly.JSReaderFunctions));
      }

      if (assembly.JSWriterFunctions.Any())
      {
        separateRegistrationCalls.Add(InvocationStatement(ReactNativeNames.RegisterExtensionWriter));
        classMembers.Add(RegisterExtensionWriter(assembly.JSWriterFunctions));
      }

      // Generates:
      //  public void CreatePackage(IPackageBuilder packageBuilder)
      //  {
      //    ... calls to register the components
      //  }
      var createPackageMethod = MethodDeclaration(
          PredefinedType(Token(SyntaxKind.VoidKeyword)),
          ReactNativeNames.CreatePackage)
        .AddModifiers(
          Token(SyntaxKind.PublicKeyword))
        .AddParameterListParameters(
          GetPackageBuilderArgument())
        .WithBody(
          Block(
            separateRegistrationCalls
          ));
      classMembers.Insert(0, createPackageMethod);

      // Generates:
      //    namespace "MyNS"
      //    {
      //      public sealed partial ReactPackageProvider : IReactPackageProvider
      //      {
      //        internal void CreateViewManager(IPackageBuilder packageBuilder)
      //        {
      //           ... registrationCalls (ses above)
      //        }
      //      }
      //    }
      var ns =
        NamespaceDeclaration(ParseName(m_rootNamespace))
          .AddMembers(
            ClassDeclaration("ReactPackageProvider")
              .AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.SealedKeyword),
                Token(SyntaxKind.PartialKeyword))
              .AddBaseListTypes(
                SimpleBaseType(
                  m_reactTypes.IReactPackageProvider.ToTypeSyntax()))
              .WithMembers(
                new SyntaxList<MemberDeclarationSyntax>(classMembers))
          );

      return ns;
    }
  }
}
