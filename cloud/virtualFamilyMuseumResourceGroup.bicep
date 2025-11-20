targetScope = 'resourceGroup'

param appConfigurationName string

resource familyConfiguration 'Microsoft.AppConfiguration/configurationStores@2025-06-01-preview' = {
  name: appConfigurationName
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  sku: {
    name: 'free'
  }
  properties: {
    createMode: 'Default'
    disableLocalAuth: true
    enablePurgeProtection: false
    softDeleteRetentionInDays: 0
    publicNetworkAccess: 'Enabled'
  }
}
