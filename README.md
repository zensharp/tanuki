# Tanuki
> GitLab artifact utilities

## File Transformations
### Transform between Report Formats
```shell
# Unity Project Auditor to Code Climate
tanuki transform report.json [--from unity.projectauditor] [-o codeclimate.json]

# Unity Test runner to GitLab Test reports
tanuki transform report.json --from unity.testing [-o gitlab.json]
```

## File Operations
### Edit a report
```shell
tanuki edit report.json [-o edited.json] [--location-prefix "src/Assets"] [--engine "Project Auditor"]
```

### Merge Code Climate reports
```shell
tanuki merge a.json b.json ... c.json [-o merged.json]
```

## Code Quality Report
### Transform a Code Climate report to HTML
```shell
tanuki html report.json [--base-url "https://docs.example.com"] [-o public] 
```

---

# Advanced Configuration
Tanuki will search for a configuration file in the following order:
1. `./.tanuki.yml`
2. `~/.config/tanuki.yml`

Engines listed in the configuration file will get properly linked in the generated HTML.

```
engines:
  - name: project_auditor
    url: https://docs.unity3d.com/Packages/com.unity.project-auditor@1.0/manual/index.html
  - name: rubocop
    url: https://docs.rubocop.org
```
