// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Humanizer;
using System;
using System.Diagnostics;

namespace Microsoft.ReactNative.Managed.CodeGen
{
  public class ConsoleMeasurement : IDisposable
  {
    private string m_message;
    private Stopwatch m_stopWatch;

    public ConsoleMeasurement(string message)
    {
      m_message = message;
      Console.Write(message);
      m_stopWatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
      Console.WriteLine($" [{m_stopWatch.Elapsed.Humanize(2)}]");
    }

  }
}
