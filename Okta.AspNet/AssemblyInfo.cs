// <copyright file="AssemblyInfo.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Runtime.CompilerServices;

// InternalsVisibleTo without PublicKey since we removed strong naming from .NET Framework assemblies
// Strong name signing with PublicSign is not production-ready on .NET Framework per Microsoft
[assembly: InternalsVisibleTo("Okta.AspNet.Test")]
