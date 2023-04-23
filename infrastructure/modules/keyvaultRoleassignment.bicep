@description('Id of Role Definition to be applied')
param roleDefinitionId string
@description('Object Id of principal to be assigned role')
param principalId string
@description('Principal type')
param principalType string = 'ServicePrincipal'

param keyvaultName string

resource keyvault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyvaultName
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyvault
  name: guid(keyvault.id, principalId, roleDefinitionId)
  properties: {
    roleDefinitionId: roleDefinitionId
    principalId: principalId
    principalType: principalType
  }
}
