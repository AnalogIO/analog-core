@allowed(['dev', 'prd'])
param environment string
param location string

param organizationPrefix string
param sharedResourcesAbbreviation string
param hostingPlanId string

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: 'func-${organizationPrefix}-${sharedResourcesAbbreviation}-${environment}'
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hostingPlanId
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}
