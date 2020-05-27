// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ReactNative.Managed.CodeGen
{
  class Program
  {
    static async Task<int> Main(string[] args)
    {
      using (var cancellationTokenSource = new CancellationTokenSource())
      {
        Console.CancelKeyPress += (_, e) =>
        {
          cancellationTokenSource.Cancel();
          e.Cancel = true;
        };

        var options = new Options();
        if (!options.TryParse(args))
        {
          return 1;
        }

        var app = new App(cancellationTokenSource.Token);
        return await app.Run(options);
      }
    }
  }
}
