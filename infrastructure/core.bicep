param location string = resourceGroup().location

param environment string

param organizationPrefix string
param sharedResourcesAbbreviation string
param applicationPrefix string

param sharedResourceGroupName string

var isPrd = environment == 'prd'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: 'log-${organizationPrefix}-${sharedResourcesAbbreviation}-${environment}'
  scope: resourceGroup(sharedResourceGroupName)
}

resource appservicePlan 'Microsoft.Web/serverfarms@2022-09-01' existing = {
  name: 'asp-${organizationPrefix}-${sharedResourcesAbbreviation}-${environment}'
  scope: resourceGroup(sharedResourceGroupName)
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: 'appi-${organizationPrefix}-${sharedResourcesAbbreviation}-${environment}'
  scope: resourceGroup(sharedResourceGroupName)
}

resource actiongroup 'Microsoft.Insights/actionGroups@2023-01-01' existing = {
  name: 'ag-${organizationPrefix}-${sharedResourcesAbbreviation}-${environment}'
  scope: resourceGroup(sharedResourceGroupName)
}

var envSettings = isPrd ? loadJsonContent('prd.settings.json') : loadJsonContent('dev.settings.json')
var appSettings = array(envSettings.webapp.appSettings)
var dockerRegistry = envSettings.webapp.dockerRegistry
var keyVaultReferences = array(envSettings.webapp.keyVaultReferences)

module webapp 'modules/webapp.bicep' = {
  name: '${deployment().name}-${applicationPrefix}-webapp'
  params: {
    organizationPrefix: organizationPrefix
    applicationPrefix: applicationPrefix
    environment: environment
    location: location
    appservicePlanName: appservicePlan.name
    sharedResourceGroupName: sharedResourceGroupName
    applicationInsightsName: applicationInsights.name
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.id
    keyvaultName: keyvaultModule.outputs.keyvaultName
    appSettings: appSettings
    dockerRegistry: dockerRegistry
    keyVaultReferences: keyVaultReferences
    sqlServerConnectionString: sqlDb.outputs.sqlDatabaseConnectionString
    enableAlerts: envSettings.webapp.alerts.resource == null ? true : envSettings.webapp.alerts.resource
    actionGroupId: actiongroup.id
  }
}

module sqlDb 'modules/sqldatabase.bicep' = {
  name: '${deployment().name}-${applicationPrefix}-sqldb'
  params: {
    organizationPrefix: organizationPrefix
    applicationPrefix: applicationPrefix
    environment: environment
    location: location
    sharedResourceGroupName: sharedResourceGroupName
    logAnalyticsWorkspaceName: logAnalyticsWorkspace.name
    skuCapacity: envSettings.sqlServer.skuCapacity
    skuName: envSettings.sqlServer.skuName
    skuTier: envSettings.sqlServer.skuTier
    actionGroupId: actiongroup.id
    enableAlerts: envSettings.sqlServer.alerts == null ? true : envSettings.sqlServer.alerts
  }
}

module keyvaultModule 'modules/keyVault.bicep' = {
  name: '${deployment().name}-${applicationPrefix}-kv'
  params: {
    organizationPrefix: organizationPrefix
    applicationPrefix: applicationPrefix
    environment: environment
    location: location
    sharedResourceGroupName: sharedResourceGroupName
    logAnalyticsWorkspaceName: logAnalyticsWorkspace.name
  }
}

@description('Built-in Key Vault Secrets User role')
resource keyvaultSecretUserRole 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: '4633458b-17de-408a-b874-0445c86b69e6'
}

module webappKeyvaultRoleAssignment 'modules/keyvaultRoleassignment.bicep' = {
  name: '${deployment().name}-${applicationPrefix}-rbac-kvwebapp'
  params: {
    keyvaultName: keyvaultModule.outputs.keyvaultName
    roleDefinitionId: keyvaultSecretUserRole.id
    principalId: webapp.outputs.webappPrincipalId
  }
}

resource keyvault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyvaultModule.outputs.keyvaultName
}

var enablePingAlerts = envSettings.webapp.alerts.ping == null ? true : envSettings.webapp.alerts.ping
module availabilityTestPing 'modules/urlPingTest.bicep' = {
  name: '${deployment().name}-${applicationPrefix}-pingtest'
  scope: resourceGroup(sharedResourceGroupName)
  params: {
    testName: '${applicationPrefix}-ping'
    url: 'https://${webapp.outputs.webappCustomDomainNameFqdn}/api/v2/health/ping'
    apiKeyQueryParam: 'x-api-key'
    apiKey: keyvault.getSecret('IdentitySettings-ApiKey')
    applicationInsightsName: applicationInsights.name
    actionGroupId: actiongroup.id
    enableAlerts: enablePingAlerts
  }
}

var enableHealthCheckAlerts = envSettings.webapp.alerts.healthcheck == null
  ? true
  : envSettings.webapp.alerts.healthcheck
module availabilityTestHealth 'modules/urlPingTest.bicep' = {
  name: '${deployment().name}-${applicationPrefix}-healthtest'
  scope: resourceGroup(sharedResourceGroupName)
  params: {
    testName: '${applicationPrefix}-healthcheck'
    url: 'https://${webapp.outputs.webappCustomDomainNameFqdn}/api/v2/health/check'
    apiKeyQueryParam: 'x-api-key'
    apiKey: keyvault.getSecret('IdentitySettings-ApiKey')
    applicationInsightsName: applicationInsights.name
    actionGroupId: actiongroup.id
    enableAlerts: enableHealthCheckAlerts
  }
}
