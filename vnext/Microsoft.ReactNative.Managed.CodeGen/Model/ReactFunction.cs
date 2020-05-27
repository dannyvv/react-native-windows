// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Microsoft.ReactNative.Managed.CodeGen.Model
{
  public class ReactFunction : ReactCallback
  {
    public ReactFunction(ISymbol symbol, ImmutableArray<IParameterSymbol> callbackParameters, string name, string emitterName)
      : base(symbol, callbackParameters, name, emitterName)
    {
    }
  }
}
