// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "For simplified usage of the library which is also deployed as a nuget, I want all classes in a single namespace.", Scope = "namespace", Target = "~N:OpenSubtitlesSharp")]
