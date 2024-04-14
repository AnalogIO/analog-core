param testName string
param url string
param httpMethod string = 'GET'
param expectedHttpStatusCode int = 200

@secure()
param apiKey string = ''
param apiKeyQueryParam string = ''

param applicationInsightsName string
param actionGroupId string
param enableAlerts bool = true

param location string = resourceGroup().location

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
}

var testUrlFormatted = apiKeyQueryParam == '' ? url : '${url}?${apiKeyQueryParam}=${apiKey}'

var id = guid(applicationInsights.name)
var guidId = guid(applicationInsights.name)
var timeoutSeconds = 90
var frequencySeconds = 300
var version = '1.1'
var followRedirects = 'True'
var recordResults = 'True'
var cache = 'False'
var parseDependentRequests = 'False'
var ignoreHttpStatusCode = 'False'

resource urlPingTest 'Microsoft.Insights/webtests@2015-05-01' = {
  name: testName
  location: location
  tags: {
    'hidden-link:/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/microsoft.insights/components/${applicationInsights.name}': 'Resource'
  }
  kind: 'ping'
  properties: {
    Configuration: {
      WebTest: '<WebTest xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010" Name="${testName}" Id="${id}" Enabled="True" CssProjectStructure="" CssIteration="" Timeout="${timeoutSeconds}" WorkItemIds="" Description="" CredentialUserName="" CredentialPassword="" PreAuthenticate="True" Proxy="default" StopOnError="False" RecordedResultFile="" ResultsLocale=""> <Items> <Request Method="${httpMethod}" Guid="${guidId}" Version="${version}" Url="${testUrlFormatted}" ThinkTime="0" Timeout="${timeoutSeconds}" ParseDependentRequests="${parseDependentRequests}" FollowRedirects="${followRedirects}" RecordResult="${recordResults}" Cache="${cache}" ResponseTimeGoal="0" Encoding="utf-8" ExpectedHttpStatusCode="${expectedHttpStatusCode}" ExpectedResponseUrl="" ReportingName="" IgnoreHttpStatusCode="${ignoreHttpStatusCode}" /> </Items> </WebTest>'
    }
    Description: 'Runs a classic URL ping test'
    Enabled: true
    Frequency: frequencySeconds
    Kind: 'ping'
    Locations: [
      {
        Id: 'emea-nl-ams-azr'
      }
      {
        Id: 'emea-ru-msa-edge'
      }
      {
        Id: 'emea-gb-db3-azr'
      }
      {
        Id: 'emea-ch-zrh-edge'
      }
      {
        Id: 'apac-hk-hkn-azr'
      }
      {
        Id: 'latam-br-gru-edge'
      }
      {
        Id: 'us-va-ash-azr'
      }
    ]
    Name: testName
    RetryEnabled: true
    SyntheticMonitorId: testName
    Timeout: timeoutSeconds
  }
}

resource metricalert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-${testName}'
  location: 'Global'
  tags: {
    'hidden-link:/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/microsoft.insights/components/${applicationInsights.name}': 'Resource'
    'hidden-link:/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/microsoft.insights/webtests/${testName}': 'Resource'
  }
  properties: {
    description: 'Availability test ${testName} is failing'
    severity: 1
    enabled: enableAlerts
    scopes: [
      urlPingTest.id
      applicationInsights.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      webTestId: urlPingTest.id
      componentId: applicationInsights.id
      failedLocationCount: 3
      'odata.type': 'Microsoft.Azure.Monitor.WebtestLocationAvailabilityCriteria'
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}
