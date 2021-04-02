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
