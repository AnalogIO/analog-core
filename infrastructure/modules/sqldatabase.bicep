param location string = resourceGroup().location

param organizationPrefix string
param applicationPrefix string
param environment string

param sharedResourceGroupName string

param logAnalyticsWorkspaceName string

param skuCapacity int
param skuName string
param skuTier string

param actionGroupId string
param enableAlerts bool = true

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-${organizationPrefix}-${applicationPrefix}-${environment}'
  location: location
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      principalType: 'Group'
      login: 'analogio-admins'
      sid: 'e1fc4d3f-0369-4250-80ad-78fb6ed443b0'
      tenantId: tenant().tenantId
    }
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
    version: '12.0'
  }

  resource auditSettings 'auditingSettings@2021-11-01' = {
    name: 'default'
    properties: {
      state: 'Enabled'
      auditActionsAndGroups: [
        'SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP'
        'FAILED_DATABASE_AUTHENTICATION_GROUP'
        'BATCH_COMPLETED_GROUP'
      ]
      isAzureMonitorTargetEnabled: true
    }
  }

  resource devOpsAuditingSettings 'devOpsAuditingSettings@2021-11-01' = {
    name: 'default'
    properties: {
      state: 'Enabled'
      isAzureMonitorTargetEnabled: true
    }
  }

  resource firewallRules 'firewallRules@2021-11-01' = {
    name: 'default'
    properties: {
      endIpAddress: '0.0.0.0' // 0.0.0.0 allow Azure services to connect
      startIpAddress: '0.0.0.0'
    }
  }
}

resource sqlServerDiagnosticSettings 'Microsoft.Sql/servers/databases/providers/diagnosticSettings@2021-05-01-preview' = {
  name: '${sqlServer.name}/master/microsoft.insights/LogAnalytics'
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'SQLSecurityAuditEvents'
        enabled: true
      }
      {
        category: 'DevOpsOperationsAudit'
        enabled: true
      }
    ]
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-11-01' = {
  name: 'sqldb-${organizationPrefix}-${applicationPrefix}-${environment}'
  parent: sqlServer
  location: location
  sku: {
    capacity: skuCapacity
    name: skuName
    tier: skuTier
  }
  properties: {
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
    zoneRedundant: false
  }

  resource auditSettings 'auditingSettings@2021-11-01' = {
    name: 'default'
    properties: {
      state: 'Enabled'
      auditActionsAndGroups: [
        'SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP'
        'FAILED_DATABASE_AUTHENTICATION_GROUP'
        'BATCH_COMPLETED_GROUP'
      ]
      isAzureMonitorTargetEnabled: true
    }
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: logAnalyticsWorkspaceName
  scope: resourceGroup(sharedResourceGroupName)
}

resource diagnosticSettingsSqldb 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'Audit Logs'
  scope: sqlDatabase
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'SQLSecurityAuditEvents'
        enabled: true
      }
      {
        category: 'DevOpsOperationsAudit'
        enabled: true
      }
    ]
  }
}

resource preventDeleteLock 'Microsoft.Authorization/locks@2020-05-01' = {
  name: 'PreventDeletion'
  scope: sqlDatabase
  properties: {
    level: 'CanNotDelete'
    notes: 'Prevent Deletion'
  }
}

resource cpuAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-cpu-${sqlDatabase.name}'
  location: 'Global'
  properties: {
    description: 'High CPU usage on SQL Database ${sqlDatabase.name}'
    severity: 2
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      sqlDatabase.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT15M'
    criteria: {
      allOf: [
        {
          threshold: 90
          name: 'Percentage CPU'
          metricNamespace: 'Microsoft.Sql/servers/databases'
          metricName: 'cpu_percent'
          operator: 'GreaterThan'
          timeAggregation: 'Average'
          skipMetricValidation: false
          criterionType: 'StaticThresholdCriterion'
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

resource workerAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-worker-${sqlDatabase.name}'
  location: 'Global'
  properties: {
    description: 'High worker usage on SQL Database ${sqlDatabase.name}'
    severity: 1
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      sqlDatabase.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      allOf: [
        {
          threshold: 60
          name: 'Percentage Workers'
          metricNamespace: 'Microsoft.Sql/servers/databases'
          metricName: 'workers_percent'
          operator: 'GreaterThan'
          timeAggregation: 'Minimum'
          skipMetricValidation: false
          criterionType: 'StaticThresholdCriterion'
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

resource ioAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-io-${sqlDatabase.name}'
  location: 'Global'
  properties: {
    description: 'High IO usage on SQL Database ${sqlDatabase.name}'
    severity: 3
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      sqlDatabase.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT15M'
    criteria: {
      allOf: [
        {
          threshold: 90
          name: 'Percentage IO'
          metricNamespace: 'Microsoft.Sql/servers/databases'
          metricName: 'physical_data_read_percent'
          operator: 'GreaterThan'
          timeAggregation: 'Average'
          skipMetricValidation: false
          criterionType: 'StaticThresholdCriterion'
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

resource storageAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-storage-${sqlDatabase.name}'
  location: 'Global'
  properties: {
    description: 'Low storage capacity on SQL Database ${sqlDatabase.name}'
    severity: 1
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      sqlDatabase.id
    ]
    evaluationFrequency: 'PT30M'
    windowSize: 'PT1H'
    criteria: {
      allOf: [
        {
          threshold: 80
          name: 'Percentage storage'
          metricNamespace: 'Microsoft.Sql/servers/databases'
          metricName: 'storage_percent'
          operator: 'GreaterThan'
          timeAggregation: 'Minimum'
          skipMetricValidation: false
          criterionType: 'StaticThresholdCriterion'
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

resource deadlockAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-deadlock-${sqlDatabase.name}'
  location: 'Global'
  properties: {
    description: 'High amout of deadlocks on SQL Database ${sqlDatabase.name}'
    severity: 2
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      sqlDatabase.id
    ]
    evaluationFrequency: 'PT30M'
    windowSize: 'PT1H'
    criteria: {
      allOf: [
        {
          alertSensitivity: 'Medium'
          name: 'Deadlocks'
          failingPeriods: {
            numberOfEvaluationPeriods: 4
            minFailingPeriodsToAlert: 4
          }
          metricNamespace: 'Microsoft.Sql/servers/databases'
          metricName: 'deadlock'
          operator: 'GreaterOrLessThan'
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

resource failedSystemConnectionsAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-stmconn-${sqlDatabase.name}'
  location: 'Global'
  properties: {
    description: 'High amout of Failed System connections on SQL Database ${sqlDatabase.name}'
    severity: 2
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      sqlDatabase.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      allOf: [
        {
          threshold: 10
          name: 'Count connections'
          metricNamespace: 'Microsoft.Sql/servers/databases'
          metricName: 'connection_failed'
          operator: 'GreaterThan'
          timeAggregation: 'Total'
          skipMetricValidation: false
          criterionType: 'StaticThresholdCriterion'
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

resource failedUserConnectionsAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-usrconn-${sqlDatabase.name}'
  location: 'Global'
  properties: {
    description: 'High amout of Failed User connections on SQL Database ${sqlDatabase.name}'
    severity: 2
    enabled: enableAlerts
    autoMitigate: true
    scopes: [
      sqlDatabase.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT15M'
    criteria: {
      allOf: [
        {
          alertSensitivity: 'Medium'
          name: 'UsrConnections'
          failingPeriods: {
            numberOfEvaluationPeriods: 4
            minFailingPeriodsToAlert: 4
          }
          metricNamespace: 'Microsoft.Sql/servers/databases'
          metricName: 'connection_failed_user_error'
          operator: 'GreaterOrLessThan'
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
  name: 'alert-resourcehealth-${sqlDatabase.name}'
  location: 'Global'
  properties: {
    description: 'Resource Health alert for SQL Database ${sqlDatabase.name}'
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
              equals: 'Microsoft.Sql/servers/databases'
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

output sqlDatabaseConnectionString string = 'Server=tcp:${sqlServer.name}${az.environment().suffixes.sqlServerHostname};Authentication=Active Directory Default; Database=${sqlDatabase.name};Encrypt=True;TrustServerCertificate=False;'
