targetScope = 'resourceGroup'

param appConfigName string = 'app-config-virtual-family-museum-test'

resource familyConfigurationTest 'Microsoft.AppConfiguration/configurationStores@2024-06-01' = {
  name: appConfigName
  location: resourceGroup().location
  sku: {
    name: 'free'
  }
  properties: {
    createMode: 'Default'
    defaultKeyValueRevisionRetentionPeriodInSeconds: 604800
    disableLocalAuth: false
    publicNetworkAccess: 'Enabled'
  }
}

var appConfigurationDataOwnerRoleId = '5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b'

resource developerDataOwnerAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(familyConfigurationTest.id, '71528c84-639b-4d98-8895-a97b10ed3da7', appConfigurationDataOwnerRoleId)
  scope: familyConfigurationTest
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', appConfigurationDataOwnerRoleId)
    principalId: '71528c84-639b-4d98-8895-a97b10ed3da7'
    principalType: 'User'
  }
}
