targetScope = 'subscription'

@allowed([ 'dev', 'prd' ])
param environment string

var location = 'West Europe'

var organizationPrefix = 'aio'
var sharedResourcesAbbreviation = 'shr'

resource sharedRg 'Microsoft.Resources/resourceGroups@2022-09-01' existing = {
  name: 'rg-${organizationPrefix}-${sharedResourcesAbbreviation}-${environment}'
}

resource coreRg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${organizationPrefix}-app-core-${environment}'
  location: location
  tags: {
    app: 'Analog Core'
    env: environment
    git: 'github.com/AnalogIO/analog-core'
  }
}

module corewebapp 'core.bicep' = {
  name: '${deployment().name}-app-core'
  scope: coreRg
  params: {
    location: location
    organizationPrefix: organizationPrefix
    sharedResourcesAbbreviation: sharedResourcesAbbreviation
    applicationPrefix: 'core'
    environment: environment
    sharedResourceGroupName: sharedRg.name
  }
}
