* Order by severity (like gitlab). Then by ID.

---

categories should look like:

```
<select id="category-filter">
	<option value="">All Categories</option>
	<option value="info">Info</option>
	<option value="minor">Minor</option>
	<option value="major">Major</option>
	<option value="critical">Critical</option>
	<option value="blocker">Blocker</option>
</select>
```

engine filter should look like:
```
<select id="engine-filter">
	<option value="">All Engines</option>
	<option value="project-auditor">Project Auditor</option>
	<option value="enforcer">Enforcer</option>
</select>
```

filter combinations:
```
.filter-category-CAT_i > li
.filter-engine-ENG_i > li
.filter-category-ENG_i > li
```
```
.filter-none > li {
	display: none;
}
.filter-category-info > li,
.filter-category-minor > li,
.filter-category-major > li,
.filter-category-critical > li,
.filter-category-blocker > li,
.filter-engine-enforcer > li,
.filter-engine-project-auditor > li,
.filter-category-all.filter-engine-enforcer > li[data-engine="enforcer"],
.filter-category-info.filter-engine-all > li[data-categories~="info"],
.filter-category-minor.filter-engine-all > li[data-categories~="minor"],
.filter-category-major.filter-engine-all > li[data-categories~="major"],
.filter-category-critical.filter-engine-all > li[data-categories~="critical"],
.filter-category-blocker.filter-engine-all > li[data-categories~="blocker"],
.filter-category-info.filter-engine-enforcer > li[data-categories~="info"][data-engine="enforcer"],
.filter-category-minor.filter-engine-enforcer > li[data-categories~="minor"][data-engine="enforcer"],
.filter-category-major.filter-engine-enforcer > li[data-categories~="major"][data-engine="enforcer"],
.filter-category-critical.filter-engine-enforcer > li[data-categories~="critical"][data-engine="enforcer"],
.filter-category-blocker.filter-engine-enforcer > li[data-categories~="blocker"][data-engine="enforcer"],
.filter-category-all.filter-engine-all > li {
	display: block;
}
```