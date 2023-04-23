# Azure infrastructure

## Resouce naming convention

Per Azure Cloud Adaptation Framework (CAF), we follow a defined resource naming convention to have a uniform way of naming resources. All resources will follow the below structure

```txt
<resource-type>-<organization>-<application>-<environment>
```

- `<resource-type>` An abbreviation that represents the type of Azure resource. See [Recommended abbreviations for Azure resource types](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations).
- `organization` Top-level name of the organization to ensure the uniqueness of resources that have a global resource name. Use `aio`.
- `<application>` Application, or service that the resource is a part of. For shared resources, `shr` is used.
- `<environment>` The stage of the development lifecycle for the workload that the resource supports. Use `dev` or `prd`.
