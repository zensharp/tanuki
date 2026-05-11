### How to build
1. Use the provided Docker image.
1. `tanuki markdown [report.json] -o report.md`
1. `cat report.md >> zensical/docs/index.md`
1. `cd zensical`
1. `source .venv/bin/activate && pip install zensical`
1. `mv site ../public`
1. `cd ..`
