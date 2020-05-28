// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ReactNative.Managed.CodeGen.UnitTests
{
  public abstract class AnalysysTestBase
  {
    public TestContext TestContext { get; set; }

    protected CancellationTokenSource TokenSource;

    public AnalysysTestBase()
    {
      TokenSource = new CancellationTokenSource();
    }

    protected (CodeAnalyzer, INamedTypeSymbol) AnalyzeModuleFile(string csSnippet)
    {
      var csCode = @"
using System;
using Microsoft.ReactNative.Managed;

[ReactModule]
public class TestClass
{
" + csSnippet + @"
}
";

      return AnalyzeFileAsync(csCode).GetAwaiter().GetResult();
    }



    protected (CodeAnalyzer, INamedTypeSymbol) AnalyzeFile(string csCode)
    {
      return AnalyzeFileAsync(csCode).GetAwaiter().GetResult();
    }

    protected async Task<(CodeAnalyzer, INamedTypeSymbol)> AnalyzeFileAsync(string csCode)
    {
      var codeAnalyzer = new CodeAnalyzer(TokenSource.Token);

      codeAnalyzer.AddSyntaxTree(
        CSharpSyntaxTree.ParseText(csCode, path: "test.cs", options: new CSharpParseOptions()));

      var references = new List<string>();
      references.AddRange(Directory.EnumerateFiles(@"C:\Users\dannyvv\.nuget\packages\microsoft.netcore.universalwindowsplatform\6.2.8\ref\uap10.0.15138", "*.dll"));
      references.Add(@"Z:\src\r1\vnext\target\X64\Debug\Microsoft.ReactNative.Managed\Microsoft.ReactNative.Managed\Microsoft.ReactNative.Managed.dll");
      references.Add(@"Z:\src\r1\vnext\target\X64\Debug\Microsoft.ReactNative\Microsoft.ReactNative.winmd");

      var win10SdkFolder = @"C:\Program Files (x86)\Windows Kits\10";
      var win10SdkVersion = "10.0.18362.0";
      
      references.AddRange(new [] {
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.AI.MachineLearning.MachineLearningContract\2.0.0.0\Windows.AI.MachineLearning.MachineLearningContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.AI.MachineLearning.Preview.MachineLearningPreviewContract\2.0.0.0\Windows.AI.MachineLearning.Preview.MachineLearningPreviewContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.ApplicationModel.Calls.Background.CallsBackgroundContract\2.0.0.0\Windows.ApplicationModel.Calls.Background.CallsBackgroundContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.ApplicationModel.Calls.CallsPhoneContract\5.0.0.0\Windows.ApplicationModel.Calls.CallsPhoneContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.ApplicationModel.Calls.CallsVoipContract\4.0.0.0\Windows.ApplicationModel.Calls.CallsVoipContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.ApplicationModel.CommunicationBlocking.CommunicationBlockingContract\2.0.0.0\Windows.ApplicationModel.CommunicationBlocking.CommunicationBlockingContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.ApplicationModel.SocialInfo.SocialInfoContract\2.0.0.0\Windows.ApplicationModel.SocialInfo.SocialInfoContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.ApplicationModel.StartupTaskContract\3.0.0.0\Windows.ApplicationModel.StartupTaskContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Devices.Custom.CustomDeviceContract\1.0.0.0\Windows.Devices.Custom.CustomDeviceContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Devices.DevicesLowLevelContract\3.0.0.0\Windows.Devices.DevicesLowLevelContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Devices.Printers.PrintersContract\1.0.0.0\Windows.Devices.Printers.PrintersContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Devices.SmartCards.SmartCardBackgroundTriggerContract\3.0.0.0\Windows.Devices.SmartCards.SmartCardBackgroundTriggerContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Devices.SmartCards.SmartCardEmulatorContract\6.0.0.0\Windows.Devices.SmartCards.SmartCardEmulatorContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Foundation.FoundationContract\3.0.0.0\Windows.Foundation.FoundationContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Foundation.UniversalApiContract\8.0.0.0\Windows.Foundation.UniversalApiContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Gaming.XboxLive.StorageApiContract\1.0.0.0\Windows.Gaming.XboxLive.StorageApiContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Graphics.Printing3D.Printing3DContract\4.0.0.0\Windows.Graphics.Printing3D.Printing3DContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Networking.Connectivity.WwanContract\2.0.0.0\Windows.Networking.Connectivity.WwanContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Networking.Sockets.ControlChannelTriggerContract\3.0.0.0\Windows.Networking.Sockets.ControlChannelTriggerContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Services.Maps.GuidanceContract\3.0.0.0\Windows.Services.Maps.GuidanceContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Services.Maps.LocalSearchContract\4.0.0.0\Windows.Services.Maps.LocalSearchContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Services.Store.StoreContract\4.0.0.0\Windows.Services.Store.StoreContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.Services.TargetedContent.TargetedContentContract\1.0.0.0\Windows.Services.TargetedContent.TargetedContentContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.System.Profile.ProfileHardwareTokenContract\1.0.0.0\Windows.System.Profile.ProfileHardwareTokenContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.System.Profile.ProfileSharedModeContract\2.0.0.0\Windows.System.Profile.ProfileSharedModeContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.System.Profile.SystemManufacturers.SystemManufacturersContract\3.0.0.0\Windows.System.Profile.SystemManufacturers.SystemManufacturersContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.System.SystemManagementContract\6.0.0.0\Windows.System.SystemManagementContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.UI.ViewManagement.ViewManagementViewScalingContract\1.0.0.0\Windows.UI.ViewManagement.ViewManagementViewScalingContract.winmd",
        $@"{win10SdkFolder}\References\{win10SdkVersion}\Windows.UI.Xaml.Core.Direct.XamlDirectContract\2.0.0.0\Windows.UI.Xaml.Core.Direct.XamlDirectContract.winmd",
        $@"{win10SdkFolder}\UnionMetadata\{win10SdkVersion}\Facade\Windows.winmd"
      });

      await codeAnalyzer.LoadMetadataReferencesAsync(references);

      var success = codeAnalyzer.TryCompileAndCheckForErrors();
      if (!success)
      {
        TestContext.WriteLine(string.Join(Environment.NewLine, codeAnalyzer.Errors));
        Assert.Fail("Errors encountered");
      }

      var errorCount = codeAnalyzer.Errors.Count(diag => diag.Severity == DiagnosticSeverity.Error);
      Assert.AreEqual(0, errorCount);

      var type = codeAnalyzer.GetAllTypes().FirstOrDefault(tp => tp.Name == "TestClass");

      return (codeAnalyzer, type);
    }

    protected void ExpectSingleError(CodeAnalyzer analyzer, DiagnosticDescriptor descriptor)
    {
      Assert.AreEqual(1, analyzer.Errors.Count());
      var error = analyzer.Errors.First();
      Assert.AreEqual(descriptor, error.Descriptor);
      Assert.AreNotEqual(Location.None, error.Location);
    }
  }
}
