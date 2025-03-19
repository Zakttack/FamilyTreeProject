targetScope = 'resourceGroup'
param logContainerName string = 'logs'
param imageContainerName string = 'images'
param templateContainerName string = 'templates'
param dbName string = 'familytreedb'
param personContainerName string = 'person'
param partnershipContainerName string = 'partnership'

resource familyTreeIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: 'familyTreeIdentity'
}

resource familyTreeVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: 'familyTreeVault'
}

resource familyTreeConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: 'familyTreeConfiguration'
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

resource familyTreeDocumentDBaccount 'Microsoft.DocumentDB/databaseAccounts@2024-12-01-preview' = {
  kind: 'GlobalDocumentDB'
  location: resourceGroup().location
  name: 'familytreedocumentdbaccount'
  properties: {
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Eventual'
    }
    locations: [
      {
        locationName: resourceGroup().location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 1440
        backupRetentionIntervalInHours: 168
        backupStorageRedundancy: 'Local'
      }
    }
    publicNetworkAccess: 'Disabled'
    minimalTlsVersion: 'tls12'
  }
}

resource familyTreeDocumentDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-12-01-preview' = {
  location: resourceGroup().location
  parent: familyTreeDocumentDBaccount
  name: dbName
  properties: {
    resource: {
      id: dbName
    }
  }
}

resource personContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-12-01-preview' = {
  parent: familyTreeDocumentDB
  location: resourceGroup().location
  name: personContainerName
  properties: {
    options: {
      autoscaleSettings: {
        maxThroughput: 4000
      }
    }
    resource: {
      id: personContainerName
      partitionKey: {
        kind: 'Hash'
        paths: [
          '/birthName'
        ]
      }
    }
  }
}

resource partnershipContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-12-01-preview' = {
  parent: familyTreeDocumentDB
  location: resourceGroup().location
  name: partnershipContainerName
  properties: {
    options: {
      autoscaleSettings: {
        maxThroughput: 4000
      }
    }
    resource: {
      id: partnershipContainerName
      partitionKey: {
        kind: 'Hash'
        paths: [
          '/partnershipDate'
        ]
      }
    }
  }
}
