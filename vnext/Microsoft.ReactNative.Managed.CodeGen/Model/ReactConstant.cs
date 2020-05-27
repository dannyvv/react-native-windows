// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics.ContractsLight;
using Microsoft.CodeAnalysis;

namespace Microsoft.ReactNative.Managed.CodeGen.Model
{
  public class ReactConstant
  {
    public ISymbol Symbol { get; }

    public string Name { get; }

    public ReactConstant(ISymbol symbol, string name)
    {
      Contract.Assert(symbol is IPropertySymbol || symbol is IFieldSymbol, "Should only be called for properties and fields");

      Symbol = symbol;
      Name = name;
    }
  }
}
