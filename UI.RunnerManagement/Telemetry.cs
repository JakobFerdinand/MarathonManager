﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace UI.RunnerManagement
{
    public static class Telemetry
    {
        private static TelemetryClient _telemetry;

        public static bool Enabled { get; set; } = true;

        public static void Initialize(string instrumentationKey)
        {
            if (_telemetry is null)
                _telemetry = GetAppInsightsClient(instrumentationKey);
        }

        private static TelemetryClient GetAppInsightsClient(string instrumentaionKey)
        {
            var config = new TelemetryConfiguration
            {
                InstrumentationKey = instrumentaionKey,
                TelemetryChannel = new Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel.ServerTelemetryChannel
                {
                    //config.TelemetryChannel = new Microsoft.ApplicationInsights.Channel.InMemoryChannel(); // Default channel
                    DeveloperMode = Debugger.IsAttached
                }
            };
#if DEBUG
            config.TelemetryChannel.DeveloperMode = true;
#endif
            var client = new TelemetryClient(config);
            client.Context.Component.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            client.Context.Session.Id = Guid.NewGuid().ToString();
            client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();
            client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            return client;
        }

        public static void SetUser(string user)
        {
            if (_telemetry != null)
                _telemetry.Context.User.AuthenticatedUserId = user;
        }

        public static void TrackEvent(string key)
            => TrackEvent(key, null);
        public static void TrackEvent(string key, IDictionary<string, string> properties)
        {
            if (Enabled)
                _telemetry?.TrackEvent(key, properties, metrics: null);
        }

        public static void TrackException(Exception ex)
        {
            if (ex != null && Enabled)
            {
                var telex = new Microsoft.ApplicationInsights.DataContracts.ExceptionTelemetry(ex);
                _telemetry?.TrackException(telex);
                Flush();
            }
        }

        public static void Flush()
            => _telemetry?.Flush();
    }
}
