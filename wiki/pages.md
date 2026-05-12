### How to build
Use the provided Docker image, run the following

```bash
# Generate markdown reports
tanuki markdown [report.json] [-o report/]

# Copy to output
for f in report/*.md; do
	BASENAME=$(basename "$f")
	cat "$f" >> "zensical/docs/$BASENAME"
done

# Build Zensical site
cd zensical
source .venv/bin/activate && pip install zensical
mv site ../public
cd ..
```
