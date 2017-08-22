﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowEngine.Config;
using WorkflowEngine.Interfaces;
using WorkflowEngine.Tests.Mocks.Config;

namespace WorkflowEngine.Tests
{
    [TestClass]
    public class VariantConfigPluginConfigTests
    {
     
        [TestMethod]
        public void VariantConfig_NoVaraints()
        {
            const string key = "Key1";
            const string value = "Value1";

            var rawConfig = new MockPluginConfig(new Dictionary<string, string>
            {
                { key, value }
            });

            // Initialize with no flights
            var variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, new HashSet<string>());
            var configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual(value, configValue);

            // Initialize with baseline flight
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("baseline"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual(value, configValue);

            // Initialize with non-baseline flight
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual(value, configValue);

            // Initialize with multiple flights
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt2"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual(value, configValue);

        }

        [TestMethod]
        public void VariantConfig_SingleVariant_SingleConstraint()
        {
            const string key = "Key1";
            const string value = "BASELINE_VALUE||flt::flt1==FLT1_VALUE";

            var rawConfig = new MockPluginConfig(new Dictionary<string, string>
            {
                { key, value }
            });

            // Initialize with no flights
            var variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, new HashSet<string>());
            var configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with baseline flight
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("baseline"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with matching flight
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_VALUE", configValue);

            // Initialize with non matching flight
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt2"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with matching and non matching flight
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1","flt2"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_VALUE", configValue);

        }

        [TestMethod]
        public void VariantConfig_SingleVariant_MultipleConstraints()
        {
            const string key = "Key1";
            const string value = "BASELINE_VALUE||flt::flt1&&flt::flt2==FLT1_FLT2_VALUE";

            var rawConfig = new MockPluginConfig(new Dictionary<string, string>
            {
                { key, value }
            });

            // Initialize with no flights
            var variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, new HashSet<string>());
            var configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with baseline flight
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("baseline"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with subset of matching flights
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with all matching constraints
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1","flt2"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_FLT2_VALUE", configValue);

            // Initialize with all matching constraints and one more
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt2", "flt3"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_FLT2_VALUE", configValue);

        }


        [TestMethod]
        public void VariantConfig_MultipleVariant_MultipleConstraints()
        {
            const string key = "Key1";
            const string value = "BASELINE_VALUE||flt::flt1&&flt::flt2==FLT1_FLT2_VALUE||flt::flt1&&flt::flt3==FLT1_FLT3_VALUE";

            var rawConfig = new MockPluginConfig(new Dictionary<string, string>
            {
                { key, value }
            });

            // Initialize with no flights
            var variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, new HashSet<string>());
            var configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with baseline flight
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("baseline"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with subset of matching flights
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with all matching constraints
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt2"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_FLT2_VALUE", configValue);

            // Initialize with another set of all matching constraints
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt3"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_FLT3_VALUE", configValue);

            // Initialize with all matching constraints and overlap
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt2", "flt3"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_FLT2_VALUE", configValue);

            // Initialize with all matching constraints and one more
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt2", "flt4"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_FLT2_VALUE", configValue);

        }

        [TestMethod]
        public void VariantConfig_MultipleVariant_MultipleConstraints_DifferentCardinality()
        {
            const string key = "Key1";
            const string value = "BASELINE_VALUE||flt::flt1==FLT1_VALUE||flt::flt1&&flt::flt3==FLT1_FLT3_VALUE";

            var rawConfig = new MockPluginConfig(new Dictionary<string, string>
            {
                { key, value }
            });

            // Initialize with no flights
            var variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, new HashSet<string>());
            var configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with baseline flight
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("baseline"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("BASELINE_VALUE", configValue);

            // Initialize with subset of matching flights
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_VALUE", configValue);

            // Initialize with all matching constraints
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt2"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_VALUE", configValue);

            // Initialize with another set of all matching constraints
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt3"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_FLT3_VALUE", configValue);

            // Initialize with all matching constraints and overlap
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt2", "flt3"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_FLT3_VALUE", configValue);

            // Initialize with all matching constraints and one more
            variantConfigPluginConfig = GetVariantConfingPluginConfig(rawConfig, GetFlights("flt1", "flt2", "flt4"));
            configValue = variantConfigPluginConfig.Get(key);
            Assert.AreEqual("FLT1_VALUE", configValue);

        }

        private static ISet<string> GetFlights(params string[] flights)
        {
            return new HashSet<string>(flights);
        }

        private static IPluginConfig GetVariantConfingPluginConfig(IPluginConfig rawConfig, ISet<string> flights)
        {
            var config = new VariantConfigPluginConfig(rawConfig);
            config.Initialize(flights.Select(f => new VariantConstraint("flt", f)));
            return config;
        }
    }
}
