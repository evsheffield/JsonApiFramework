// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Diagnostics.Contracts;
using System.Net;

using JsonApiFramework.Properties;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApiFramework.JsonApi.Dom.Internal
{
    /// <summary>JSON.Net converter for DomLink nodes.</summary>
    internal class DomLinkConverter : DomNodeConverter
    {
        // PUBLIC CONSTRUCTORS //////////////////////////////////////////////
        #region Constructors
        public DomLinkConverter(DomJsonSerializerSettings domJsonSerializerSettings)
            : base(domJsonSerializerSettings)
        { }
        #endregion

        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region JsonConverter Overrides
        public override bool CanConvert(Type objectType)
        {
            Contract.Requires(objectType != null);

            var canConvert = objectType == typeof(IDomLink) || objectType == typeof(DomLink);
            return canConvert;
        }

        public override object ReadJson(JsonReader jsonReader, Type objectType, object existingValue, JsonSerializer jsonSerializer)
        {
            Contract.Requires(jsonReader != null);
            Contract.Requires(objectType != null);
            Contract.Requires(jsonSerializer != null);

            var domReadJsonContext = new DomReadJsonContext();
            var tokenType = jsonReader.TokenType;
            switch (tokenType)
            {
                case JsonToken.Null:
                    {
                        return null;
                    }

                case JsonToken.String:
                    {
                        var jValue = (JValue)JToken.Load(jsonReader);
                        var domLink = CreateDomLink(domReadJsonContext, jValue);
                        if (!domReadJsonContext.AnyErrors())
                            return domLink;
                    }
                    break;

                case JsonToken.StartObject:
                    {
                        var jObject = JObject.Load(jsonReader);
                        var domLink = CreateDomLink(domReadJsonContext, jObject);
                        if (!domReadJsonContext.AnyErrors())
                            return domLink;
                    }
                    break;

                default:
                    {
                        var title = CoreErrorStrings.JsonReadErrorTitle;
                        var detail = "Expected JSON null, JSON string, or JSON object when reading JSON representing a json:api link object.";
                        var source = ErrorSource.CreatePointer(jsonReader.Path);
                        var error = new Error(null, null, HttpStatusCode.BadRequest, null, title, detail, source, null);
                        domReadJsonContext.AddError(error);
                    }
                    break;
            }

            var errorsCollection = domReadJsonContext.ErrorsCollection;
            throw new ErrorsException(HttpStatusCode.BadRequest, errorsCollection);
        }
        #endregion
    }
}