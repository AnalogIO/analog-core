param location string = resourceGroup().location

param environment string

param organizationPrefix string
param sharedResourcesAbbreviation string
param applicationPrefix string

param sharedResourceGroupName string

param enableCustomDomain bool = false

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

var envSettings = isPrd ? loadJsonContent('prd.settings.json') : loadJsonContent('dev.settings.json')
var appSettings = array(envSettings.webapp.appSettings)
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
    enableCustomDomain: enableCustomDomain
    keyvaultName: keyvaultName
    appSettings: appSettings
    keyVaultReferences: keyVaultReferences
    sqlServerConnectionString: sqlDb.outputs.sqlDatabaseConnectionString
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

var keyvaultName = keyvaultModule.outputs.keyvaultName

@description('Built-in Key Vault Secrets User role. See https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#key-vault-secrets-user')
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
