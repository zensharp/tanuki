
## File Transformations
### Transform a Unity Project Auditor report to Code Climate 
```shell
tanuki transform -i report.project_auditor.json --from unity.projectauditor -o report.codeclimate.json
```

### Transform a Unity Tests report to GitLab Tests report
```shell
tanuki transform -i report.codeclimate.json --from unity.testing -o report.gitlab.json
```

## File Operations
### Merge Code Climate reports
```shell
tanuki merge -o merged.codeclimate.json a.codeclimate.json b.codeclimate.json
```

## Code Quality Report
### Transform a Code Climate report to HTML
```shell
tanuki build -i report.codeclimate.json -o public
```
