// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;

namespace Microsoft.ReactNative.Managed.CodeGen.Model
{
  public class ReactInitializer
  {
    public IMethodSymbol Method { get; }
    
    public ReactInitializer(IMethodSymbol method)
    {
      Method = method;
    }
  }
}
