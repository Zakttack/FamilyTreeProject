targetScope = 'resourceGroup'
param logContainerName string = 'logs'
param imageContainerName string = 'images'
param templateContainerName string = 'templates'
param dbName string = 'familytreedb'
param personContainerName string = 'person'
param familyDynamicContainerName string = 'familydynamic'
param myIpAddress string = '24.220.242.86'
param fileShareName string = 'neo4jcontents'
param containerGroupName string = 'familytreecontainers'
param neo4jAuthValue string
param storageAccountKeyValue string

resource familyTreeIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: 'familyTreeIdentity'
}

resource familyTreeVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: 'familyTreeVault'
}

resource acrCredentials 'Microsoft.KeyVault/vaults/secrets@2023-07-01' existing = {
  parent: familyTreeVault
  name: 'ContainerRegistryCredentials'
}

resource familyTreeGraphCredentials 'Microsoft.KeyVault/vaults/secrets@2023-07-01' existing = {
  parent: familyTreeVault
  name: 'FamilyTreeGraphCredentials'
}

resource familyTreeStaticStorageAccountKey 'Microsoft.KeyVault/vaults/secrets@2023-07-01' existing = {
  parent: familyTreeVault
  name: 'StorageAccountKey'
}

resource familyTreeConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: 'familyTreeConfiguration'
}

resource acrName 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' existing = {
  parent: familyTreeConfiguration
  name: 'ContainerRegistry:Name'
}

resource acrLoginServer 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' existing = {
  parent: familyTreeConfiguration
  name: 'ContainerRegistry:LoginServer'
}

resource familyTreeInsights 'Microsoft.Insights/components@2018-05-01-preview' existing = {
  name: 'familyTreeInsights'
}

resource familyTreeStaticStorage 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: 'familytreestaticstorage'
}

resource familyTreeFileServices 'Microsoft.Storage/storageAccounts/fileServices@2022-09-01' existing = {
  parent: familyTreeStaticStorage
  name: 'default'
}

resource neo4jContent 'Microsoft.Storage/storageAccounts/fileServices/shares@2022-09-01' existing = {
  parent: familyTreeFileServices
  name: fileShareName
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

resource familyTreeRegistry 'Microsoft.ContainerRegistry/registries@2024-11-01-preview' existing = {
  name: 'familyTreeRegistry'
}

resource neo4jContainer 'Microsoft.ContainerInstance/containerGroups@2022-10-01-preview' = {
  name: containerGroupName
  location: resourceGroup().location
  properties: {
    osType: 'Linux'
    restartPolicy: 'OnFailure'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          port: 7474
          protocol: 'TCP'
        }
        {
          port: 7687
          protocol: 'TCP'
        }
      ]
    }
    containers: [
      {
        name: 'neo4j'
        properties: {
          image: '${familyTreeRegistry.properties.loginServer}/family-tree-graph:latest'
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 2
            }
          }
          ports: [
            {
              port: 7474
              protocol: 'TCP'
            }
            {
              port: 7687
              protocol: 'TCP'
            }
          ]
          environmentVariables: [
            {
              name: 'NEO4J_AUTH'
              secureValue: neo4jAuthValue
            }
          ]
          volumeMounts: [
            {
              name: 'neo4jfiles'
              mountPath: '/data'
            }
          ]
        }
      }
    ]
    volumes: [
      {
        name: 'neo4jfiles'
        azureFile: {
          shareName: fileShareName
          storageAccountName: familyTreeStaticStorage.name
          storageAccountKey: storageAccountKeyValue
        }
      }
    ]
    imageRegistryCredentials: [
      {
        server: familyTreeRegistry.properties.loginServer
        username: familyTreeRegistry.listCredentials().username
        password: familyTreeRegistry.listCredentials().passwords[0].value
      }
    ]
  }
}
