param environment string

param location string

param keyvaultName string

param organizationPrefix string
param applicationPrefix string

param sharedResourceGroupName string
param appservicePlanName string
param applicationInsightsName string
param logAnalyticsWorkspaceId string

param sqlServerConnectionString string

param appSettings array
param keyVaultReferences array
param dockerRegistry string

param enableCustomDomain bool = false

var keyVaultReferencesFormatted = [for item in keyVaultReferences: {
  name: item.name
  value: '@Microsoft.KeyVault(VaultName=${keyvaultName};SecretName=${item.secretName})'
}]

var fqdn = '${webapp.name}.analogio.dk'

resource appservicePlan 'Microsoft.Web/serverfarms@2022-03-01' existing = {
  name: appservicePlanName
  scope: resourceGroup(sharedResourceGroupName)
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
  scope: resourceGroup(sharedResourceGroupName)
}

resource webapp 'Microsoft.Web/sites@2022-03-01' = {
  name: 'app-${organizationPrefix}-${applicationPrefix}-${environment}'
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appservicePlan.id
    enabled: true
    reserved: true
    siteConfig: {
      numberOfWorkers: 1
      alwaysOn: true
      linuxFxVersion: 'DOCKER|${dockerRegistry}'
      http20Enabled: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      logsDirectorySizeLimit: 100 // MB
      appSettings: union([
          {
            name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
            value: reference(applicationInsights.id, '2015-05-01').InstrumentationKey
          }
          {
            name: 'APPLICATIONINSIGHTS__CONNECTIONSTRING'
            value: reference(applicationInsights.id, '2015-05-01').ConnectionString
          }
          {
            name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
            value: '~3'
          }
          {
            name: 'XDT_MicrosoftApplicationInsights_Mode'
            value: 'recommended'
          }
          {
            name: 'DatabaseSettings__ConnectionString'
            value: sqlServerConnectionString
          }
        ], appSettings, keyVaultReferencesFormatted)
    }
    httpsOnly: true
    redundancyMode: 'None'
    keyVaultReferenceIdentity: 'SystemAssigned'
  }
}

module webappManagedCertificate '../modules/webappManagedCertificate.bicep' = if (enableCustomDomain) {
  name: '${deployment().name}-${applicationPrefix}-ssl-${fqdn}'
  params: {
    location: location
    environment: environment
    appservicePlanName: appservicePlan.name
    webAppName: webapp.name
    sharedResourceGroupName: sharedResourceGroupName
  }
}

resource diagnosticSettingsWebApp 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'App Service Logs'
  scope: webapp
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'AppServiceConsoleLogs'
        enabled: true
      }
      {
        category: 'AppServiceAppLogs'
        enabled: true
      }
      {
        category: 'AppServicePlatformLogs'
        enabled: true
      }
    ]
  }
}

output webappPrincipalId string = webapp.identity.principalId

module dns 'webappDns.bicep' = {
  name: '${deployment().name}-core-dns'
  scope: resourceGroup(sharedResourceGroupName)
  params: {
    environment: environment
    webappVerificationIdValue: webapp.properties.customDomainVerificationId
    webappAzureGeneratedFqdn: webapp.properties.defaultHostName
  }
}

module certificate 'webappManagedCertificate.bicep' = {
  name: '${deployment().name}-core-certificate'
  params: {
    location: location
    environment: environment
    appservicePlanName: appservicePlan.name
    webAppName: webapp.name
    sharedResourceGroupName: sharedResourceGroupName
  }

  dependsOn: [
    dns
  ]
}

module bindCertificate 'bindCustomDomainCertificate.bicep' = {
  name: '${deployment().name}-core-bind-certificate'
  params: {
    webAppName: webapp.name
    certificateThumbprint: certificate.outputs.certificateThumbprint
    customDomainFqdn: certificate.outputs.customDomainFqdn
  }

  dependsOn: [
    certificate
  ]
}
