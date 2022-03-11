﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DocumentFormat.OpenXml.Generator.NamespaceGeneration;
using System.Threading.Tasks;
using Xunit;

using VerifyCS = DocumentFormat.OpenXml.Generator.Tests.Verifiers.OpenXmlIncrementalSourceGeneratorVerifier<
    DocumentFormat.OpenXml.Generator.NamespaceGeneration.NamespaceGenerator>;

namespace DocumentFormat.OpenXml.Generator.Tests
{
    public class NamespaceGeneratorTests
    {
        [Fact]
        public async Task Empty()
        {
            await new VerifyCS.Test().RunAsync();
        }

        [Fact]
        public async Task PropertySetTrue()
        {
            const string Source = @"
namespace DocumentFormat.OpenXml.Features;

public enum FileFormatVersions
{
    None = 0,
    Office2007 = 0x1,
    Office2010 = 0x2,
    Office2013 = 0x4,
    Office2016 = 0x8,
    Office2019 = 0x10,
    Office2021 = 0x20,
    Microsoft365 = 0x40000000,
}
";
            const string NamespacesFile = @"[
  {
    ""Prefix"": """",
    ""Uri"": """"
  },
  {
    ""Prefix"": ""a"",
    ""Uri"": ""http://schemas.openxmlformats.org/drawingml/2006/main""
  },
  {
    ""Prefix"": ""a14"",
    ""Uri"": ""http://schemas.microsoft.com/office/drawing/2010/main"",
    ""Version"": ""Office2010""
  }
]";
            const string Options = @"
is_global = true

build_property.DocumentFormat_OpenXml_Generator_NamespaceLookup = true

[/namespaces.json]
build_metadata.AdditionalFiles.OpenXml = Namespace";

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { Source },
                    AnalyzerConfigFiles =
                    {
                        ("/.editorconfig", Options),
                    },
                    AdditionalFiles =
                    {
                        ("/namespaces.json", NamespacesFile),
                    },
                    GeneratedSources =
                    {
                        (typeof(NamespaceGenerator), "Namespaces.cs", ExpectedNamespaces),
                    },
                },
            }.RunAsync();
        }

        private const string ExpectedNamespaces = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Collections.Generic;

namespace DocumentFormat.OpenXml.Features;

internal partial class OpenXmlNamespaceResolver
{
    private readonly Dictionary<string, string> _urlToPrefix = new()
    {
        { string.Empty, string.Empty },
        { ""http://schemas.openxmlformats.org/drawingml/2006/main"", ""a"" },
        { ""http://schemas.microsoft.com/office/drawing/2010/main"", ""a14"" },
    };

    private readonly Dictionary<string, string> _prefixToUrl = new()
    {
        { string.Empty, string.Empty },
        { ""a"", ""http://schemas.openxmlformats.org/drawingml/2006/main"" },
        { ""a14"", ""http://schemas.microsoft.com/office/drawing/2010/main"" },
    };

    private readonly Dictionary<string, FileFormatVersions> _prefixToVersion = new()
    {
        { string.Empty, FileFormatVersions.Office2007 },
        { ""a"", FileFormatVersions.Office2007 },
        { ""a14"", FileFormatVersions.Office2010 },
    };
}
";
    }
}
