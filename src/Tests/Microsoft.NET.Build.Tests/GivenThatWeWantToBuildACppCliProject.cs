﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.NET.TestFramework;
using Microsoft.NET.TestFramework.Assertions;
using Microsoft.NET.TestFramework.Commands;
using NuGet.Frameworks;
using NuGet.ProjectModel;
using Xunit.Abstractions;

namespace Microsoft.NET.Build.Tests
{
    public class GivenThatWeWantToBuildACppCliProject : SdkTest
    {
        public GivenThatWeWantToBuildACppCliProject(ITestOutputHelper log) : base(log)
        {
        }

        [FullMSBuildOnlyFact]
        public void It_builds_and_runs()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("NetCoreCsharpAppReferenceCppCliLib")
                .WithSource()
                .Restore(Log, "NETCoreCppCliTest.sln");

            // build projects separately with BuildProjectReferences=false to simulate VS build behavior
            new BuildCommand(Log, Path.Combine(testAsset.TestRoot, "NETCoreCppCliTest"))
                .Execute("-p:Platform=x64")
                .Should()
                .Pass();

            new BuildCommand(Log, Path.Combine(testAsset.TestRoot, "CSConsoleApp"))
                .Execute(new string[] { "-p:Platform=x64", "-p:BuildProjectReferences=false" })
                .Should()
                .Pass();

            // There is a bug in MSVC in CI's old VS image.
            // Once https://github.com/dotnet/core-eng/issues/7409/ is done
            // we should directly run the app to test.
            var expectedIjwhost = Path.Combine(
                //find the platform directory
                new DirectoryInfo(Path.Combine(testAsset.TestRoot, "CSConsoleApp", "bin")).GetDirectories().Single().FullName,
                "Debug",
                "netcoreapp3.0",
                "Ijwhost.dll");

            File.Exists(expectedIjwhost).Should().BeTrue();
        }

        [FullMSBuildOnlyFact]
        public void Given_no_restore_It_builds_cpp_project()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("NetCoreCsharpAppReferenceCppCliLib")
                .WithSource();

            // build projects separately with BuildProjectReferences=false to simulate VS build behavior
            new BuildCommand(Log, Path.Combine(testAsset.TestRoot, "NETCoreCppCliTest"))
                .Execute("-p:Platform=x64")
                .Should()
                .Pass();
        }
    }
}
