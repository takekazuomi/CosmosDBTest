# 
# psake Setup  -properties @{"UseEmulator"=$false}


Properties {
    # Set the Azure resource group name and location
    $ResourceGroupName = "kinmugicdb01"
    $ResourceGroupLocation = "East US 2"
    $DBName = $resourceGroupName
    $UseEmulator = $true
    $Emurator = "C:\Program Files\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe"
    $UserSecretsId = "ba18f91b-e8a7-4aba-8c4a-888ca0202f40"
}

Task default -Depends Compile

Task Compile -Depends Init, Clean {
    "compile"
}

Task Init {
    "init"
}

Task Clean -Depends Init {
    "clean"
}

Task Setup -Depends StartCosmosDBEmulator, NewCosmosDB, UserSecrets {
}

Task StartCosmosDBEmulator -precondition { $UseEmulator } {
    if (Test-Path $Emurator) {
        &"$Emurator" /NoUI /NoExplorer
    }
    else {
        Write-Verbose "emuraltor not found."
    }
}

Task StopCosmosDBEmulator -precondition { $UseEmulator } {
    if (Test-Path $Emurator) {
        &"$Emurator" /Shutdown
    }
    else {
        Write-Verbose "emuraltor not found."
    }
}

Task GetCosmosDBEmulator -precondition { $UseEmulator } {
    if (Test-Path $Emurator) {
        &"$Emurator" /GetStatus
        $LastExitCode
    }
    else {
        Write-Verbose "emuraltor not found."
    }
}

Task NewCosmosDB -precondition { !$UseEmulator } {
    # Create the resource group
    New-AzResourceGroup -Name $resourceGroupName -Location $resourceGroupLocation -Force -ErrorAction SilentlyContinue

    # Write and read locations and priorities for the database
    $locations = @(
        @{
            locationName = "East US 2"
        }
    )

    # IP addresses that can access the database through the firewall
    $iprangefilter = [string](curl.exe -s 'https://api.ipify.org')

    # Consistency policy
    $consistencyPolicy = @{
        defaultConsistencyLevel = "Session"
    }

    # DB properties
    # Azure Portal IP address
    # https://docs.microsoft.com/ja-jp/azure/cosmos-db/firewall-support#connections-from-the-azure-portal
    $DBProperties = @{
        databaseAccountOfferType = "Standard"
        locations                = $locations
        consistencyPolicy        = $consistencyPolicy
        ipRangeFilter            = "$iprangefilter,104.42.195.92,40.76.54.131,52.176.6.30,52.169.50.45,52.187.184.26"
    }

    # Create the database
    New-AzResource -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
        -ApiVersion "2015-04-08" `
        -ResourceGroupName $resourceGroupName `
        -Location $resourceGroupLocation `
        -Name $DBName `
        -PropertyObject $DBProperties `
        -Force `
        -Confirm:$false

}

Task UserSecrets {
    if ($UseEmulator) {
        $endpoint ="https://localhost:8081/"
        $key ="C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
    }
    else {
        $endpoint = "https://$DBName.documents.azure.com:443/"
        $key = (Invoke-AzResourceAction -Action listKeys `
            -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
            -ApiVersion "2015-04-08" `
            -ResourceGroupName $resourceGroupName `
            -Name $DBName `
            -Force).primaryMasterKey
    }

    dotnet user-secrets --id $UserSecretsId set "CosmosDB:AccountEndpoint" $endpoint
    dotnet user-secrets --id $UserSecretsId set "CosmosDB:AccountKey" $key
}
