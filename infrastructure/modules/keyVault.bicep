param location string = resourceGroup().location

param organizationPrefix string
param applicationPrefix string
param environment string

param sharedResourceGroupName string

param logAnalyticsWorkspaceName string

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: logAnalyticsWorkspaceName
  scope: resourceGroup(sharedResourceGroupName)
}

resource keyvault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: 'kv-${organizationPrefix}-${applicationPrefix}-${environment}'
  location: location
  properties: {
    enableRbacAuthorization: true
    tenantId: tenant().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    enableSoftDelete: true
    enablePurgeProtection: true
    enabledForTemplateDeployment: true
  }
}

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'Audit Logs'
  scope: keyvault
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'AuditEvent'
        enabled: true
      }
    ]
  }
}

var apiKeySecretName = 'IdentitySettings-ApiKey'
resource apiKey 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyvault
  name: apiKeySecretName
  properties: {
    value: guid(apiKeySecretName)
  }
}

output keyvaultName string = keyvault.name
