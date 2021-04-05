# Nix Notes

To start azurite storage emulator:
`azurite --silent --location c:\azurite --debug c:\azurite\debug.log`

In the storage emulator, under local-1 storage account, I added a blob container nix01

Inside that, in a folder batch01, are 8 files ready for upload:

- OrgId-1-Employee-20210401-1631.csv
- OrgId-1-EmployeeGroups-20210401-1631.csv
- OrgId-1-EmployeeSkillRating-20210401-1631.csv
- OrgId-1-EmployeeSkills-20210401-1631.csv
- OrgId-1-SkillPlanDetails-20210401-1631.csv
- OrgId-1-SkillPlanFollowers-20210401-1631.csv
- OrgId-1-SkillPlans-20210401-1631.csv
- OrgId-1-SkillTaxonomy-20210401-1631.csv

For emulator:
`private const string ConnectionString = "UseDevelopmentStorage=true";`

To start, use 
Terminal--> Run Task...open-store --> 

Setting creds for storage account:

set AZURITE_ACCOUNTS="account1:key1"

where 

Account name: devstoreaccount1
Account key: Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==

i.e. 

`set AZURITE_ACCOUNTS="devstoreaccount1:Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="`

## Latest:
Able to run only using the old-style emulator.

Currently having difficulty passing a parameter in (I want to pass the batch01 folder name)

[Some help](https://damienbod.com/2020/07/06/using-external-inputs-in-azure-durable-functions/)

[JasonRoberts1](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-1-Overview)
[2](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-2-Creating-Your-First-Durable-Function)
[3](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-3-What-Is-Durability)
[12-4](http://dontcodetired.com/blog/?tag=durfuncseries)
[5](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-5-Getting-Results-from-Orchestrations)
