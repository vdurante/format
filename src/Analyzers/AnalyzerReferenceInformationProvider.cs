﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Microsoft.CodeAnalysis.Tools.Analyzers
{
    internal class AnalyzerReferenceInformationProvider : IAnalyzerInformationProvider
    {
        public (ImmutableArray<DiagnosticAnalyzer> Analyzers, ImmutableArray<CodeFixProvider> Fixers) GetAnalyzersAndFixers(
            Solution solution,
            FormatOptions formatOptions,
            ILogger logger)
        {
            if (!formatOptions.FixAnalyzers)
            {
                return (ImmutableArray<DiagnosticAnalyzer>.Empty, ImmutableArray<CodeFixProvider>.Empty);
            }

            var assemblies = solution.Projects
                .SelectMany(project => project.AnalyzerReferences.Select(reference => reference.FullPath))
                .Distinct()
                .Select(path => Assembly.LoadFrom(path));

            return AnalyzerFinderHelpers.LoadAnalyzersAndFixers(assemblies);
        }

        public DiagnosticSeverity GetSeverity(FormatOptions formatOptions) => formatOptions.AnalyzerSeverity;
    }
}
