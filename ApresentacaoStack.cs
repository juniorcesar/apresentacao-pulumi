using System.Threading.Tasks;
using Pulumi;
using Azure = Pulumi.Azure;
using Pulumi.Tls;


class ApresentacaoStack : Stack
{
    public ApresentacaoStack()
    {

        // Get Pulumi Config
        var config = new Pulumi.Config();
        var environment = config.Require("environment");
        var vmPassword = config.RequireSecret("vm-password");

        // Create an Azure Resource Group
        var resourceGroup = new Azure.Core.ResourceGroup("rg-apresentacao-"+environment, new  Azure.Core.ResourceGroupArgs {
            Name = "rg-apresentacao-"+environment
        });

        // Create an Azure Virtual Network
        var mainVirtualNetwork = new Azure.Network.VirtualNetwork("vnet-apresentacao-" + environment, new Azure.Network.VirtualNetworkArgs
        {
            AddressSpaces = 
            {
                "10.0.0.0/16",
            },
            Location = resourceGroup.Location,
            ResourceGroupName = resourceGroup.Name,
        });
        
        // Create an Azure Subnet
        var @internal = new Azure.Network.Subnet("internal", new Azure.Network.SubnetArgs
        {
            ResourceGroupName = resourceGroup.Name,
            VirtualNetworkName = mainVirtualNetwork.Name,
            AddressPrefixes = 
            {
                "10.0.2.0/24",
            },
        });

        // Create an Azure Network Interface
        var mainNetworkInterface = new Azure.Network.NetworkInterface("vm-ni" + environment, new Azure.Network.NetworkInterfaceArgs
        {
            Location = resourceGroup.Location,
            ResourceGroupName = resourceGroup.Name,
            IpConfigurations = 
            {
                new Azure.Network.Inputs.NetworkInterfaceIpConfigurationArgs
                {
                    Name = "testconfiguration1",
                    SubnetId = @internal.Id,
                    PrivateIpAddressAllocation = "Dynamic",
                },
            },
        });
        // Create SSH Key
        var sshPrivateKey = new PrivateKey("vm-ssh-key", new PrivateKeyArgs
        {
             Algorithm = "RSA",
             RsaBits = 4096,
         });

        // Create an Azure Virtual Machine
        var mainVirtualMachine = new Azure.Compute.VirtualMachine("vm-" + environment, new Azure.Compute.VirtualMachineArgs
        {
            Location = resourceGroup.Location,
            ResourceGroupName = resourceGroup.Name,
            NetworkInterfaceIds = 
            {
                mainNetworkInterface.Id,
            },
            VmSize = "Standard_A1_v2",
            StorageImageReference = new Azure.Compute.Inputs.VirtualMachineStorageImageReferenceArgs
            {
                Publisher = "Canonical",
                Offer = "UbuntuServer",
                Sku = "16.04-LTS",
                Version = "latest",
            },
            StorageOsDisk = new Azure.Compute.Inputs.VirtualMachineStorageOsDiskArgs
            {
                Name = "diskos-vm-" + environment,
                Caching = "ReadWrite",
                CreateOption = "FromImage",
                ManagedDiskType = "Standard_LRS",
            },
            OsProfile = new Azure.Compute.Inputs.VirtualMachineOsProfileArgs
            {
                ComputerName = "hostname",
                AdminUsername = "testadmin",
                AdminPassword = vmPassword,
            },
            OsProfileLinuxConfig = new Azure.Compute.Inputs.VirtualMachineOsProfileLinuxConfigArgs
            {
                DisablePasswordAuthentication = false,
            }
        });

        // Create an Azure resource (Storage Account)
        var exampleStorageAccount = new Azure.Storage.Account("sapulumi" + environment, new Azure.Storage.AccountArgs
        {
            Name = "storagepulumi" + environment,
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            AccountTier = "Standard",
            AccountReplicationType = "LRS",
            Tags = 
            {
                { "environment", environment },
            },
        });

        this.sshPrivateKeyPem = sshPrivateKey.PrivateKeyPem;

    }    
    [Output] public Output<string> sshPrivateKeyPem { get; set; }

}
