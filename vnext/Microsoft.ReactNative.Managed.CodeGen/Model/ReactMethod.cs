// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;

namespace Microsoft.ReactNative.Managed.CodeGen.Model
{
  public class ReactMethod
  {
    public IMethodSymbol Method { get; }

    public string Name { get; }

    public bool IsSynchroneous { get; }

    public MethodReturnType ReturnType { get; }

    public ReactMethod(IMethodSymbol method, string name, bool isSynchroneous = false)
    {
      Method = method;
      Name = name;
      IsSynchroneous = isSynchroneous;

    }

    public enum MethodReturnType
    {
      Void = 0,
      Callback = 1,
      TwoCallbacks = 2,
      Promise = 3
    }
  }
}
