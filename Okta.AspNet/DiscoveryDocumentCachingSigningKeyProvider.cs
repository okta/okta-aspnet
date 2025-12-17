// <copyright file="DiscoveryDocumentCachingSigningKeyProvider.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.IdentityModel.Tokens;
using Okta.AspNet.Abstractions;

namespace Okta.AspNet
{
    internal class DiscoveryDocumentCachingSigningKeyProvider
    {
        private readonly TimeSpan _refreshInterval = new TimeSpan(1, 0, 0, 0);
        private readonly IDiscoveryDocumentSigningKeyProvider _discoveryDocumentSigningKeyProvider;
        private DateTimeOffset _syncAfter = new DateTimeOffset(new DateTime(2001, 1, 1));
        private IEnumerable<SecurityKey> _keys;
        private Exception _lastRefreshException;

        public DiscoveryDocumentCachingSigningKeyProvider(IDiscoveryDocumentSigningKeyProvider provider)
        {
            _discoveryDocumentSigningKeyProvider = provider ?? throw new ArgumentNullException(nameof(provider), "The provider cannot be null.");
            RetrieveMetadata();
        }

        /// <summary>
        /// Gets all known security keys.
        /// </summary>
        /// <value>
        /// All known security keys.
        /// </value>
        public IEnumerable<SecurityKey> SigningKeys
        {
            get
            {
                RefreshMetadata();

                // If we have no keys and had a previous error, throw that error
                if (_keys == null && _lastRefreshException != null)
                {
                    throw _lastRefreshException;
                }

                return _keys;
            }
        }

        private void RefreshMetadata()
        {
            if (_syncAfter >= DateTimeOffset.UtcNow)
            {
                return;
            }

            // Queue a refresh, but discourage other threads from doing so.
            _syncAfter = DateTimeOffset.UtcNow + _refreshInterval;

            // The keys retrieval runs in background. The IssuerSigningKeyResolver will always use the last known value, while periodically updating up.
            // This helps to prevent deadlocks. See https://github.com/aspnet/AspNetKatana/issues/363.
            ThreadPool.UnsafeQueueUserWorkItem(
                state =>
                {
                    try
                    {
                        RetrieveMetadata();
                        _lastRefreshException = null; // Clear any previous error on success
                    }
                    catch (Exception ex)
                    {
                        // Store the exception for later reporting, but don't throw on background threads.
                        _lastRefreshException = ex;

                        // Log the error using Debug/Trace so it's visible during troubleshooting
                        Debug.WriteLine($"[Okta] Background OIDC discovery refresh failed: {ex.Message}");
                        Trace.TraceWarning($"[Okta] Background OIDC discovery refresh failed: {ex.Message}");
                    }
                },
                state: null);
        }

        private void RetrieveMetadata()
        {
            _syncAfter = DateTimeOffset.UtcNow + _refreshInterval;
            _keys = _discoveryDocumentSigningKeyProvider.GetSigningKeys();
        }
    }
}
