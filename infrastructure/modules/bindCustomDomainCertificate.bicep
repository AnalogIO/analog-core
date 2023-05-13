param webAppName string

param customDomainFqdn string
param certificateThumbprint string

resource webapp 'Microsoft.Web/sites@2022-03-01' existing = {
  name: webAppName
}

resource bindCertToDomain 'Microsoft.web/sites/hostnameBindings@2019-08-01' = {
  parent: webapp
  name: customDomainFqdn
  properties: {
    siteName: webapp.name
    hostNameType: 'Verified'
    customHostNameDnsRecordType: 'CName'
    sslState: 'SniEnabled'
    thumbprint: certificateThumbprint
  }
}
