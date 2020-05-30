using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Tokens;
using SharedServices.FederatedAuth;
using SharedServices.FederatedAuth.Principal;
using SharedServices.FederatedAuth.SecurityToken;
using System;
using System.Collections.Generic;
using X509AsymmetricSecurityKey = System.IdentityModel.Tokens.X509AsymmetricSecurityKey;
using X509RawDataKeyIdentifierClause = System.IdentityModel.Tokens.X509RawDataKeyIdentifierClause;
using SecurityKeyIdentifier = System.IdentityModel.Tokens.SecurityKeyIdentifier;
using SecurityKeyIdentifierClause = System.IdentityModel.Tokens.SecurityKeyIdentifierClause;

using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace SharedServices.Extensions
{
    internal static class Saml2Extensions
    {
        private static string SerializeValue(this object value)
        {
            Type type = value.GetType();
            if (value is IEnumerable<string> list)
            {
                return string.Join(",", list);
            }
            else if (type.IsEnum)
            {
                return ((int)value).ToString();
            }
            return value.ToString();
        }
        private static object DeserializeValue(this string value, Type type)
        {
            if (type == typeof(string))
            {
                return value;
            }
            else if (type == typeof(short))
            {
                return short.Parse(value);
            }
            else if (type.IsEnum || type == typeof(int))
            {
                return int.Parse(value);
            }
            else if (type == typeof(double))
            {
                return double.Parse(value);
            }
            else if (type == typeof(float))
            {
                return float.Parse(value);
            }
            else if (type == typeof(decimal))
            {
                return decimal.Parse(value);
            }
            else if (type == typeof(DateTime))
            {
                return DateTime.Parse(value);
            }
            else if (type == typeof(DateTime?))
            {
                return string.IsNullOrWhiteSpace(value) ? (DateTime?)null : DateTime.Parse(value);
            }
            else if (type == typeof(Guid))
            {
                return Guid.Parse(value);
            }
            else if (type == typeof(Uri))
            {
                return new Uri(value);
            }
            else if (type == typeof(IEnumerable<string>))
            {
                return value.Split(',');
            }
            return value;
        }
        public static object GetValue(this IEnumerable<Saml2Attribute> attributes, PropertyInfo propertyInfo)
        => attributes.GetValue(propertyInfo.Name, propertyInfo.PropertyType);
        public static T GetValue<T>(this IEnumerable<Saml2Attribute> attributes, string name)
        => (T) attributes.GetValue(name, typeof(T));
        public static object GetValue(this IEnumerable<Saml2Attribute> attributes, string name, Type type)
        {
            if (attributes.FirstOrDefault(a => a.Name == name) is Saml2Attribute saml2Attribute) {
                return saml2Attribute != null && saml2Attribute.Values.Any() ?
                    saml2Attribute.Values.First().DeserializeValue(type) : default;
            }
            return default;
        }
        public static PropertyInfo GetProperty<T>(string name) => typeof(T).GetProperty(name);
        public static Saml2Attribute ToSaml2Attribute<T>(this PropertyInfo propInfo, T model)
            => ToSaml2Attribute(propInfo.Name, propInfo.GetValue(model));
        public static Saml2Attribute ToSaml2Attribute( this string name, object value)
        {
            string valueStr = SerializeValue(value);
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                return null;
            }
            return new Saml2Attribute(name, valueStr);
        }
        public static IEnumerable<Saml2Attribute> ResolveAttributes(this IEnumerable<Saml2Attribute> attributes) => attributes != null ? attributes : new Saml2Attribute[0];

        public static Saml2SecurityToken ToSaml2SecurityToken<TPrincipal>(this FederatedIPSecurityToken<TPrincipal> securityToken, IIntegratedSharedService application)
        where TPrincipal : FederatedIPUser
        {
            string SamlConfirmationMethod = securityToken.FederatedIPAuthentication.ConfirmationMethod;
            string SamlAttributeNamespace = application.ProjectName;//securityToken.FederatedIPAuthentication.AttributeNamespace;
            string SamlSubjectName = securityToken.FederatedIPAuthentication.SubjectName;
            Uri AudienceUri = securityToken.FederatedIPAuthentication.AudienceUri;

            IEnumerable<PropertyInfo> PricipalProperties = typeof(TPrincipal).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            //PropertyInfo RefreshDateProperty = typeof(FederatedIPSecurityToken<TPrincipal>).GetProperty("RefreshDate");
           // PropertyInfo SessionIdProperty = typeof(FederatedIPSecurityToken<TPrincipal>).GetProperty("SessionId");


            IEnumerable<Saml2Attribute> principalAttributes = PricipalProperties.Select(propInfo => propInfo.ToSaml2Attribute(securityToken.User)).Where(a => a != null);
            IEnumerable<Saml2Attribute> tokenAttributes = new Saml2Attribute[] { 
                "RefreshDate".ToSaml2Attribute(securityToken.RefreshDate)
            };
            Saml2SubjectConfirmationData confirmationData = new Saml2SubjectConfirmationData() { Address = SamlConfirmationMethod };
            Saml2SubjectConfirmation subjectConfirmations = new Saml2SubjectConfirmation(new Uri(SamlConfirmationMethod), confirmationData);
            Saml2AudienceRestriction[] audienceRestriction = new Saml2AudienceRestriction[1] { new Saml2AudienceRestriction(AudienceUri.ToString()) };

            Saml2Assertion assertion = new Saml2Assertion(new Saml2NameIdentifier(securityToken.Issuer))
            {
                Conditions = new Saml2Conditions(audienceRestriction)
                {
                    NotBefore = securityToken.IsValidTokenDate() ? securityToken.ValidFrom : DateTime.UtcNow.AddDays(-1).AddMinutes(-1),
                    NotOnOrAfter = securityToken.IsValidTokenDate() ? securityToken.ValidTo : DateTime.UtcNow.AddDays(-1)
                },
                InclusiveNamespacesPrefixList = SamlAttributeNamespace,
                Subject = new Saml2Subject(subjectConfirmations)
                {
                    NameId = new Saml2NameIdentifier(SamlConfirmationMethod)
                }
            };

            assertion.Statements.Add(new Saml2AttributeStatement(principalAttributes));
            assertion.Statements.Add(new Saml2AttributeStatement(tokenAttributes));
            //assertion.SignSaml2Assertion(securityToken.FederatedIPAuthentication);
            return new Saml2SecurityToken(assertion);
        }
        public static FederatedIPSecurityToken<TPrincipal> ToSecurityToken<TPrincipal>(this Saml2SecurityToken saml2Token, FederatedIPAuthentication federatedIPAuthentication,
            IIntegratedSharedService application)
        where TPrincipal : FederatedIPUser
        {
            DateTime now = DateTime.UtcNow;
            DateTime? issueDate = saml2Token.Assertion.Conditions.NotBefore;
            DateTime? expirationDate = saml2Token.Assertion.Conditions.NotOnOrAfter;
            bool isValidIssueDate = issueDate is DateTime  && issueDate <= now;
            bool isValidExpirationDate = expirationDate is DateTime && expirationDate > now;

            IEnumerable <Saml2Attribute> pricipalAttributes = saml2Token.Assertion.Statements.First() is Saml2AttributeStatement attributeStatement ? attributeStatement.Attributes : null;
            IEnumerable<Saml2Attribute> tokenAttributes = saml2Token.Assertion.Statements.ElementAt(1) is Saml2AttributeStatement tokenAttributeStatement ? tokenAttributeStatement.Attributes : null;
            string issuerName = saml2Token.Assertion.Issuer.Value;
            
            bool isValidToken = isValidIssueDate && isValidExpirationDate && federatedIPAuthentication.Issuer == issuerName;

            if (isValidToken)
            {
                return new FederatedIPSecurityToken<TPrincipal>(
                    (DateTime)issueDate,
                    (DateTime)expirationDate,
                    pricipalAttributes.ResolveAttributes(),
                    tokenAttributes.ResolveAttributes(),
                    federatedIPAuthentication, application);
            }
            return null;
        }

        private static Saml2SecurityTokenHandler SamlTokenHandler = new Saml2SecurityTokenHandler();
        
        public static string Serialize(this Saml2SecurityToken token)
        {
            var sw = new StringWriter();
            using (var xmlWriter = new XmlTextWriter(sw))
            {
                SamlTokenHandler.WriteToken(xmlWriter, token);
                return sw.ToString();
            }

        }
        public static Saml2SecurityToken Deserialize(this string tokenStr)
        =>  SamlTokenHandler.CanReadToken(tokenStr) ? SamlTokenHandler.ReadSaml2Token(tokenStr) : null;
        

        //private static void SignSaml2Assertion(this Saml2Assertion assertion, FederatedIPAuthentication federatedIPAuthentication)
        //{
        //    X509Certificate2 singingCertificate = FindCertificateByThumprint("7DEC1B0F28A0E0835361DBE39A41F3FEA7D42C53");
        //    //prepare to resign token assertions
        //    X509SecurityKey signingKey = new X509SecurityKey(singingCertificate);
        //    assertion.SigningCredentials = new SigningCredentials
        //        (
        //        signingKey,
        //        federatedIPAuthentication.SignitureAlgorithmUri.ToString(),
        //        federatedIPAuthentication.SignitureDigestUri.ToString()
        //        );

        //    create and sign modified token
        //    Saml2SecurityToken token = new Saml2SecurityToken(assertion, new ReadOnlyCollection<SecurityKey>(new List<SecurityKey>() { signingKey }), templateToken.IssuerToken);
        //}
        private static X509Certificate2 FindCertificateByThumprint(string certThumbPrint)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                // Try to open the store.
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = store.Certificates;
                // Find currently valid certificates.
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                // Find the certificate that matches the thumbprint.
                X509Certificate2Collection signingCertificates = certCollection.Find(
                    X509FindType.FindByThumbprint, certThumbPrint, false);

                if (signingCertificates.Count == 0)
                    return null;
                return signingCertificates[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
