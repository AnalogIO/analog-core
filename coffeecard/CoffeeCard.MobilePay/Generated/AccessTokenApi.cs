//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v9.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."
#pragma warning disable 8073 // Disable "CS8073 The result of the expression is always 'false' since a value of type 'T' is never equal to 'null' of type 'T?'"
#pragma warning disable 3016 // Disable "CS3016 Arrays as attribute arguments is not CLS-compliant"
#pragma warning disable 8603 // Disable "CS8603 Possible null reference return"

namespace CoffeeCard.MobilePay.Generated.Api.AccessTokenApi
{
    using System = global::System;

    

    /// <summary>
    /// This _new_ accesstoken endpoint is used to get the JWT (JSON Web Token) that
    /// <br/>must be passed in every API request in the `Authorization` header.
    /// <br/>The access token is a base64-encoded string value that must be
    /// <br/>acquired first before making any Vipps MobilePay API calls.
    /// <br/>The access token is valid for 15 minutes both in the test environment
    /// <br/>and in the production environment.
    /// <br/>See: https://developer.vippsmobilepay.com/docs/APIs/access-token-api/
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v9.0.0.0))")]
    public partial class TokenResponse
    {
        /// <summary>
        /// The type for the access token.
        /// <br/>This will always be `Bearer`.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("token_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Token_type { get; set; }

        /// <summary>
        /// Token expiry time in seconds. The token is currently valid for 15 minutes.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("expires_in", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Expires_in { get; set; }

        /// <summary>
        /// The access token itself, typically 1000+ characters.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("access_token", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Access_token { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v9.0.0.0))")]
    public partial class AuthorizationTokenResponse
    {
        /// <summary>
        /// The type for the access token.
        /// <br/>This will always be `Bearer`.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("token_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Token_type { get; set; }

        /// <summary>
        /// Token expiry time in seconds.
        /// <br/>The access token is valid for 1 hour in the test environment
        /// <br/>and 24 hours in the production environment.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("expires_in", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Expires_in { get; set; }

        /// <summary>
        /// Extra time added to expiry time. Currently disabled.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("ext_expires_in", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Ext_expires_in { get; set; }

        /// <summary>
        /// Token expiry time in epoch time format.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("expires_on", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Expires_on { get; set; }

        /// <summary>
        /// Token creation time in epoch time format.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("not_before", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Not_before { get; set; }

        /// <summary>
        /// A common resource object.
        /// <br/>Not used in token validation.
        /// <br/>This can be disregarded.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("resource", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Resource { get; set; }

        /// <summary>
        /// The access token itself.
        /// <br/>It is a base64-encoded string, typically 1000+ characters.
        /// <br/>It can be decoded on https://jwt.io, and using standard libraries.
        /// <br/>See the documentation for details.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("access_token", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Access_token { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v9.0.0.0))")]
    public partial class Body
    {
        [Newtonsoft.Json.JsonProperty("grant_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public BodyGrant_type Grant_type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v9.0.0.0))")]
    public enum BodyGrant_type
    {

        [System.Runtime.Serialization.EnumMember(Value = @"client_credentials")]
        Client_credentials = 0,

    }



    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v9.0.0.0))")]
    public partial class ApiException : System.Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v9.0.0.0))")]
    public partial class ApiException<TResult> : ApiException
    {
        public TResult Result { get; private set; }

        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }

}

#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore  472
#pragma warning restore  114
#pragma warning restore  108
#pragma warning restore 3016
#pragma warning restore 8603