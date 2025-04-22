# Tanuki
> GitLab artifact utilities

## File Transformations
### Transform a Unity Project Auditor report to Code Climate 
```shell
tanuki transform report.project_auditor.json --from unity.projectauditor -o report.codeclimate.json
```

### Transform a Unity Tests report to GitLab Tests report
```shell
tanuki transform report.unity.json --from unity.testing -o report.gitlab.json
```

## File Operations
### Edit a report
```shell
tanuki edit report.json --engine "Enforcer" --base-url "BaseUrl"
```

### Merge Code Climate reports
```shell
tanuki merge -o merged.codeclimate.json a.codeclimate.json b.codeclimate.json
```

## Code Quality Report
### Transform a Code Climate report to HTML
```shell
tanuki html report.codeclimate.json -o public
```
