### How to build
1. Use the provided Docker image.
1. `tanuki markdown [report.json]`
1. `cat summary.md >> zensical/docs/index.md`
1. `cd zensical && uv run zensical build`
1. `mv site ..`
1. `cd ..`
