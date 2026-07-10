targetScope = 'resourceGroup'

param appConfigName string = 'app-config-virtual-family-museum'

resource familyConfiguration 'Microsoft.AppConfiguration/configurationStores@2024-06-01' = {
  name: appConfigName
  location: resourceGroup().location
  sku: {
    name: 'free'
  }
  properties: {
    createMode: 'Default'
    defaultKeyValueRevisionRetentionPeriodInSeconds: 604800
    disableLocalAuth: true
    publicNetworkAccess: 'Enabled'
  }
}
