targetScope = 'resourceGroup'
param logContainerName string = 'logs'
param imageContainerName string = 'images'
param templateContainerName string = 'templates'
param dbName string = 'familytreedb'
param personContainerName string = 'person'
param familyDynamicContainerName string = 'familydynamic'
param myIpAddress string = '24.220.242.86'

resource familyTreeIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: 'familyTreeIdentity'
}

resource familyTreeVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: 'familyTreeVault'
}

resource acrCredentials 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: familyTreeVault
  name: 'ContainerRegistryCredentials'
  properties: {
    contentType: 'application/json'
    value: '{"username":"${familyTreeRegistry.listCredentials().username}","password":"${familyTreeRegistry.listCredentials().passwords[0].value}"}'
  }
}

resource familyTreeConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: 'familyTreeConfiguration'
}

resource acrName 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' = {
  parent: familyTreeConfiguration
  name: 'ContainerRegistry:Name'
  properties: {
    value: familyTreeRegistry.name
    contentType: 'text/plain'
  }
}

resource acrLoginServer 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' = {
  parent: familyTreeConfiguration
  name: 'ContainerRegistry:LoginServer'
  properties: {
    value: familyTreeRegistry.properties.loginServer
    contentType: 'text/plain'
  }
}

resource familyTreeInsights 'Microsoft.Insights/components@2018-05-01-preview' existing = {
  name: 'familyTreeInsights'
}

resource familyTreeStaticStorage 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: 'familytreestaticstorage'
}

resource familyTreeBlobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' existing = {
  parent: familyTreeStaticStorage
  name: 'default'
}

resource logContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' existing = {
  parent: familyTreeBlobService
  name: logContainerName
}

resource imageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' existing = {
  parent: familyTreeBlobService
  name: imageContainerName
}

resource templateContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' existing = {
  parent: familyTreeBlobService
  name: templateContainerName
}

resource lifecyclePolicy 'Microsoft.Storage/storageAccounts/managementPolicies@2021-09-01' existing = {
  parent: familyTreeStaticStorage
  name: 'default'
}

resource appInsightsDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' existing = {
  name: 'AppInsightsToStorageLogs'
  scope: familyTreeInsights
}

resource storageDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' existing = {
  name: 'StorageLogs'
  scope: familyTreeBlobService
}

resource familyTreeDocumentDBaccount 'Microsoft.DocumentDB/databaseAccounts@2024-12-01-preview' existing = {
  name: 'familytreedocumentdbaccount'
}

resource familyTreeDocumentDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-12-01-preview' existing = {
  parent: familyTreeDocumentDBaccount
  name: dbName
}

resource personContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-12-01-preview' existing = {
  parent: familyTreeDocumentDB
  name: personContainerName
}

resource familyDynamicContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-12-01-preview' existing = {
  parent: familyTreeDocumentDB
  name: familyDynamicContainerName
}

resource familyTreeRegistry 'Microsoft.ContainerRegistry/registries@2024-11-01-preview' = {
  location: resourceGroup().location
  name: 'familyTreeRegistry'
  properties: {
    adminUserEnabled: true
    publicNetworkAccess: 'Enabled'
  }
  sku: {
    name: 'Basic'
  }
}
