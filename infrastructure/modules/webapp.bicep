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

param enableAlerts bool
param actionGroupId string

var keyVaultReferencesFormatted = [
  for item in keyVaultReferences: {
    name: item.name
    value: '@Microsoft.KeyVault(VaultName=${keyvaultName};SecretName=${item.secretName})'
  }
]

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
      appSettings: union(
        [
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
        ],
        appSettings,
        keyVaultReferencesFormatted
      )
    }
    httpsOnly: true
    redundancyMode: 'None'
    keyVaultReferenceIdentity: 'SystemAssigned'
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
  name: '${deployment().name}-${applicationPrefix}-dns'
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
}

resource preventDeleteLock 'Microsoft.Authorization/locks@2020-05-01' = {
  name: 'PreventDeletion'
  scope: webapp
  properties: {
    level: 'CanNotDelete'
    notes: 'Prevent Deletion'
  }
}

resource serverErrorsAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-5xxerr-${webapp.name}'
  location: 'Global'
  properties: {
    description: 'High amout of Server Errors'
    severity: 2
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      webapp.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      allOf: [
        {
          alertSensitivity: 'Medium'
          name: 'ServerErrors'
          failingPeriods: {
            numberOfEvaluationPeriods: 4
            minFailingPeriodsToAlert: 4
          }
          metricNamespace: 'Microsoft.Web/sites'
          metricName: 'Http5xx'
          operator: 'GreaterThan'
          timeAggregation: 'Total'
          skipMetricValidation: false
          criterionType: 'DynamicThresholdCriterion'
        }
      ]
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

resource responseTimeAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-rsptime-${webapp.name}'
  location: 'Global'
  properties: {
    description: 'High Response Time'
    severity: 2
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      webapp.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT30M'
    criteria: {
      allOf: [
        {
          alertSensitivity: 'Medium'
          name: 'Response Time'
          failingPeriods: {
            numberOfEvaluationPeriods: 4
            minFailingPeriodsToAlert: 4
          }
          metricNamespace: 'Microsoft.Web/sites'
          metricName: 'HttpResponseTime'
          operator: 'GreaterThan'
          timeAggregation: 'Average'
          skipMetricValidation: false
          criterionType: 'DynamicThresholdCriterion'
        }
      ]
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

resource fourxxErrorsAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-4xx-${webapp.name}'
  location: 'Global'
  properties: {
    description: 'High amout of 4xx Errors'
    severity: 2
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      webapp.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      allOf: [
        {
          alertSensitivity: 'Medium'
          name: '4xx errors'
          failingPeriods: {
            numberOfEvaluationPeriods: 4
            minFailingPeriodsToAlert: 4
          }
          metricNamespace: 'Microsoft.Web/sites'
          metricName: 'Http4xx'
          operator: 'GreaterThan'
          timeAggregation: 'Total'
          skipMetricValidation: false
          criterionType: 'DynamicThresholdCriterion'
        }
      ]
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

resource connectionsAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-cnn-${webapp.name}'
  location: 'Global'
  properties: {
    description: 'High amout of connections'
    severity: 3
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      webapp.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      allOf: [
        {
          alertSensitivity: 'Medium'
          name: 'Connections'
          failingPeriods: {
            numberOfEvaluationPeriods: 4
            minFailingPeriodsToAlert: 4
          }
          metricNamespace: 'Microsoft.Web/sites'
          metricName: 'Requests'
          operator: 'GreaterThan'
          timeAggregation: 'Total'
          skipMetricValidation: false
          criterionType: 'DynamicThresholdCriterion'
        }
      ]
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

resource resourceHealth 'Microsoft.Insights/activityLogAlerts@2020-10-01' = {
  name: 'alert-resourcehealth-${webapp.name}'
  location: 'Global'
  properties: {
    description: 'Resource Health alert for App Services'
    enabled: enableAlerts
    scopes: [
      subscription().id
    ]
    condition: {
      allOf: [
        {
          field: 'category'
          equals: 'ResourceHealth'
        }
        {
          anyOf: [
            {
              field: 'resourceType'
              equals: 'Microsoft.Web/sites'
            }
          ]
        }
      ]
    }
    actions: {
      actionGroups: [
        {
          actionGroupId: actionGroupId
        }
      ]
    }
  }
}

output webappDefaultHostNameFqdn string = webapp.properties.defaultHostName
// Remove . (dot) from FQDN
output webappCustomDomainNameFqdn string = take(dns.outputs.customDomainFqdn, length(dns.outputs.customDomainFqdn) - 1)
