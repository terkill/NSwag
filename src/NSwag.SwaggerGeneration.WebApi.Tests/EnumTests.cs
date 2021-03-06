﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema.Annotations;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.SwaggerGeneration.WebApi.Tests
{
    [TestClass]
    public class EnumTests
    {
        public enum MetadataSchemaType
        {
            Foo,
            Bar
        }

        public class MetadataSchemaDetailViewItem
        {
            public string Id { get; set; }
            public List<MetadataSchemaType> Types { get; set; }
        }

        public class MetadataSchemaCreateRequest
        {
            public string Id { get; set; }
            public List<MetadataSchemaType> Types { get; set; }
        }

        public class MyController
        {
            public MetadataSchemaDetailViewItem GetItem(MetadataSchemaCreateRequest request)
            {
                return null;
            }
        }

        [TestMethod]
        public async Task When_enum_is_used_as_array_item_then_it_is_generated_only_once()
        {
            // Arrange
            var apiGenerator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var document = await apiGenerator.GenerateForControllerAsync<MyController>();
            var json = document.ToJson();

            // Assert
            Assert.IsTrue(json.Split(new[] { "x-enumNames" }, StringSplitOptions.None).Length == 2); // enum is defined only once
        }

        public class MyEnumResultController
        {
            public enum SetPasswordResult
            {
                A,
                B
            }

            public SetPasswordResult SetPassword(Guid accountId, Guid userId, [NotNull] String oldPassword,
                [NotNull] String password, String passwordRepetition)
            {
                return SetPasswordResult.B;
            }
        }

        [TestMethod]
        public async Task When_response_is_enum_then_it_is_referenced()
        {
            // Arrange
            var apiGenerator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var document = await apiGenerator.GenerateForControllerAsync<MyEnumResultController>();
            var json = document.ToJson();

            // Assert
            Assert.IsTrue(document.Operations.First().Operation.ActualResponses.First().Value.Schema.HasSchemaReference);
        }
    }
}