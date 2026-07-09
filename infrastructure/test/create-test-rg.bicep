targetScope = 'subscription'

param location string = 'centralus'
param name string = 'rg-virtual-family-museum-test'

resource rg 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: name
  location: location
}
