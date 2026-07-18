targetScope = 'resourceGroup'

param applicationInsightsName string = 'insights-virtual-family-museum'
param logAnalyticsWorkspaceName string = 'log-analytics-virtual-family-museum'

resource familyLogAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2025-07-01' = {
  location: resourceGroup().location
  name: logAnalyticsWorkspaceName
  properties: {
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    retentionInDays: 30
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource familyInsights 'Microsoft.Insights/components@2020-02-02' = {
  kind: 'web'
  location: resourceGroup().location
  name: applicationInsightsName
  properties: {
    Application_Type: 'web'
    DisableIpMasking: false
    DisableLocalAuth: false
    ForceCustomerStorageForProfiler: false
    ImmediatePurgeDataOn30Days: false
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    Request_Source: 'rest'
    RetentionInDays: 90
    SamplingPercentage: 100
    WorkspaceResourceId: familyLogAnalyticsWorkspace.id
  }
}
